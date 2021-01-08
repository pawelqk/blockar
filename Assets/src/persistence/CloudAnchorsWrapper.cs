using Google.XR.ARCoreExtensions;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Utils;

namespace VirtualObjects
{
    public class CloudAnchorsWrapper
    {
        private ARAnchorManager anchorManager;
        private Logger logger;

        private IDictionary<string, VirtualObjectData> ongoingHostingData;
        private bool hostingOK;
        private bool anchorsHostInProgress;
        private IDictionary<ARAnchor, ARCloudAnchor> cloudAnchors;
        private IDictionary<ARAnchor, IList<string>> anchorsBeingHostedToCorrespondingObjectsData;
        private uint completedHostingCount;

        private IDictionary<string, VirtualObjectData> ongoingResolvingData;
        private bool resolvingOK;
        private bool anchorsResolveInProgress;
        private IDictionary<string, ARCloudAnchor> anchorsBeingResolved;
        private IDictionary<string, IList<string>> anchorsBeingResolvedToCorrespondingObjectsData;
        private uint completedResolvingCount;
        private Stopwatch resolveStopwatch;
        private const long SAFE_TO_RESOLVE_TIME_MS = 10000;

        public bool IsHostingCompleted { get => !anchorsHostInProgress;}
        public bool IsHostingOK { get => hostingOK;}
        public bool IsResolvingCompleted { get => !anchorsResolveInProgress; }
        public bool IsResolvingOK { get => resolvingOK; }

        public CloudAnchorsWrapper(ARAnchorManager anchorManager, Logger logger)
        {
            this.anchorManager = anchorManager;
            this.logger = logger;
            
            this.hostingOK = true;
            this.anchorsHostInProgress = false;
            this.cloudAnchors = new Dictionary<ARAnchor, ARCloudAnchor>();
            this.anchorsBeingHostedToCorrespondingObjectsData = new Dictionary<ARAnchor, IList<string>>();
            this.completedHostingCount = 0;

            this.resolvingOK = true;
            this.anchorsResolveInProgress = false;
            this.anchorsBeingResolved = new Dictionary<string, ARCloudAnchor>();
            this.anchorsBeingResolvedToCorrespondingObjectsData = new Dictionary<string, IList<string>>();
            this.completedResolvingCount = 0;
            this.resolveStopwatch = new Stopwatch();
        }

        private void ResetHosting()
        {
            hostingOK = true;
            anchorsHostInProgress = false;
            cloudAnchors.Clear();
            anchorsBeingHostedToCorrespondingObjectsData.Clear();
            completedHostingCount = 0;
        }

        public void HostAnchors(IDictionary<string, VirtualObjectData> objectsData)
        {
            ResetHosting();
            ongoingHostingData = objectsData;
            foreach (var objectData in ongoingHostingData)
            {
                if (!HostAnchor(objectData.Value))
                    break;
            }
            if (hostingOK)
                anchorsHostInProgress = true;
        }

        private bool HostAnchor(VirtualObjectData objectData)
        {
            if (objectData.Anchor is null)
            {
                logger.Log("HostAnchor()", $"Anchor is not set for the object: {objectData.Guid}");
                return true;
            }

            if (anchorsBeingHostedToCorrespondingObjectsData.ContainsKey(objectData.Anchor))
            {
                logger.Log("HostAnchor()", $"Anchor is already being hosted for the object: {objectData.Guid}");
                anchorsBeingHostedToCorrespondingObjectsData[objectData.Anchor].Add(objectData.Guid);
                return true;
            }

            var cloudAnchor = anchorManager.HostCloudAnchor(objectData.Anchor /*, 365 */);

            if (cloudAnchor is null)
            {
                logger.Log("HostAnchor()", $"Unable to host cloud anchor for the object: {objectData.Guid}");
                hostingOK = false;
                return false;
            }

            cloudAnchors[objectData.Anchor] = cloudAnchor;
            anchorsBeingHostedToCorrespondingObjectsData[objectData.Anchor] = new List<string>{objectData.Guid};
            logger.Log("HostAnchor()", $"Hosting for the object: {objectData.Guid} began");

            return true;
        }

        public void UpdateHosting()
        {
            completedHostingCount = 0;
            foreach(var cloudAnchor in cloudAnchors)
            {
                if (!UpdateSingleAnchorHosting(cloudAnchor))
                    break;
            }

            if (!hostingOK)
                anchorsHostInProgress = false;

            if (completedHostingCount == cloudAnchors.Count)
                anchorsHostInProgress = false;
        }

        private bool UpdateSingleAnchorHosting(KeyValuePair<ARAnchor, ARCloudAnchor> anchorWithCloudAnchor)
        {
            var cloudAnchorState = anchorWithCloudAnchor.Value.cloudAnchorState;
            if (cloudAnchorState == CloudAnchorState.Success)
            {
                var anchorId = anchorWithCloudAnchor.Value.cloudAnchorId;

                foreach (var guid in anchorsBeingHostedToCorrespondingObjectsData[anchorWithCloudAnchor.Key])
                    ongoingHostingData[guid].CloudAnchorId = anchorId;

                completedHostingCount++;
                logger.Log("UpdateSingleAnchorHosting()", $"success, anchorId: {anchorId} for objects:" +
                    $" {PrintUtils.PrintCollection(anchorsBeingHostedToCorrespondingObjectsData[anchorWithCloudAnchor.Key])}");
            }
            else if (cloudAnchorState != CloudAnchorState.TaskInProgress)
            {
                logger.LogError("UpdateSingleAnchorHosting()", $"Error while hosting cloud anchor: {cloudAnchorState}");
                hostingOK = false;
                return false;
            }
            return true;
        }

        private void ResetResolving()
        {
            resolvingOK = true;
            anchorsResolveInProgress = false;
            anchorsBeingResolved.Clear();
            anchorsBeingResolvedToCorrespondingObjectsData.Clear();
            completedResolvingCount = 0;
            resolveStopwatch.Reset();
        }

        public void ResolveAnchors(IDictionary<string, VirtualObjectData> objectsData)
        {
            ResetResolving();
            ongoingResolvingData = objectsData;
            foreach(var objectData in ongoingResolvingData)
            {
                if (!ResolveAnchor(objectData.Value))
                    break;
            }

            if (resolvingOK)
            {
                resolveStopwatch.Start();
                anchorsResolveInProgress = true;
            }
        }

        private bool ResolveAnchor(VirtualObjectData objectData)
        {
            if(objectData.CloudAnchorId is null)
            {
                logger.Log("ResolveAnchor()", $"CloudAnchorId is not set for the object: {objectData.Guid}");
                return true;
            }

            if(anchorsBeingResolved.ContainsKey(objectData.CloudAnchorId))
            {
                logger.Log("ResolveAnchor()", $"CloudAnchorId={objectData.CloudAnchorId} is already being resolved for the object: {objectData.Guid}");
                anchorsBeingResolvedToCorrespondingObjectsData[objectData.CloudAnchorId].Add(objectData.Guid);
                return true;
            }

            var cloudAnchor = anchorManager.ResolveCloudAnchorId(objectData.CloudAnchorId);

            if(cloudAnchor is null)
            {
                logger.Log("ResolveAnchor()", $"Unable to resolve cloud anchor: {objectData.CloudAnchorId}");
                resolvingOK = false;
                return false;
            }

            anchorsBeingResolved[objectData.CloudAnchorId] = cloudAnchor;
            anchorsBeingResolvedToCorrespondingObjectsData[objectData.CloudAnchorId] = new List<string> {objectData.Guid};
            logger.Log("ResolveAnchor()", $"Resolving the cloud anchor: {objectData.CloudAnchorId} began");
            return true;
        }

        public void UpdateResolving()
        {
            resolveStopwatch.Stop();
            var elapsedMs = resolveStopwatch.ElapsedMilliseconds;

            if (elapsedMs < SAFE_TO_RESOLVE_TIME_MS)
            {
                resolveStopwatch.Start();
                return;
            }

            resolveStopwatch.Reset();
            resolveStopwatch.Start();
            completedResolvingCount = 0;
            foreach (var cloudAnchor in anchorsBeingResolved)
            {
                if (!UpdateSingleAnchorResolving(cloudAnchor))
                    break;
            }

            if (!resolvingOK)
                anchorsResolveInProgress = false;

            if (completedResolvingCount == anchorsBeingResolved.Count)
                anchorsResolveInProgress = false;
        }

        private bool UpdateSingleAnchorResolving(KeyValuePair<string, ARCloudAnchor> cloudAnchor)
        {
            var cloudAnchorState = cloudAnchor.Value.cloudAnchorState;
            if (cloudAnchorState == CloudAnchorState.Success)
            {
                logger.Log("UpdateSingleAnchorResolving()", $"cloudAnchorId={cloudAnchor.Key}, transformPos={cloudAnchor.Value.transform.position}, transformRot={cloudAnchor.Value.transform.rotation}");
                foreach(var guid in anchorsBeingResolvedToCorrespondingObjectsData[cloudAnchor.Key])
                    ongoingResolvingData[guid].AnchorTransform = cloudAnchor.Value.transform;

                completedResolvingCount++;
                logger.Log("UpdateSingleAnchorResolving()", $"success, anchorId: {cloudAnchor.Key} for objects:" +
                    $" {PrintUtils.PrintCollection(anchorsBeingResolvedToCorrespondingObjectsData[cloudAnchor.Key])}");
            }
            else if (cloudAnchorState != CloudAnchorState.TaskInProgress)
            {
                logger.LogError("UpdateSingleAnchorResolving()", $"Error while resolving cloud anchor: {cloudAnchor.Key}, status={cloudAnchorState}");
                resolvingOK = false;
                return false;
            }
            return true;
        }
    }
}
