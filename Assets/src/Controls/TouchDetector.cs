using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using VirtualObjects;

namespace Controls
{
    public class TouchDetector
    {
        private const string VIRTUAL_OBJECT_TAG = "InstantiatedVirtualObject";

        private readonly VirtualObjectsManager virtualObjectsManager;
        private readonly ARRaycastManager arRaycastmanager;
        private readonly Logger logger;
        private Touch? lastTouch;

        public TouchDetector(VirtualObjectsManager virtualObjectsManager, ARRaycastManager arRaycastmanager, Logger logger)
        {
            this.virtualObjectsManager = virtualObjectsManager;
            this.arRaycastmanager = arRaycastmanager;
            this.logger = logger;
            this.lastTouch = null;
        }

        public Touch? LastTouch { get => lastTouch; }

        public bool CheckForTouch()
        {
            if (Input.touchCount == 0)
            {
                lastTouch = null;
                return false;
            }

            var touch = Input.GetTouch(0);
            if(touch.phase != TouchPhase.Began)
            {
                lastTouch = null;
                return false;
            }

            lastTouch = touch;
            return true;
        }

        public bool ResolveLastTouch()
        {
            if (lastTouch == null)
                return false;

            if (TryToResolveVirtualObjectTouch())
                return true;

            if (TryToResolvePlaneTouch())
                return true;

            logger.LogWarning("ResolveLastTouch", String.Format("Unresolved touch: {0}", lastTouch));
            return false;
        }

        private bool TryToResolvePlaneTouch()
        {
            logger.Log("TryToResolvePlaneTouch", "");
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            var touch = (Touch)lastTouch;
            if (!arRaycastmanager.Raycast(touch.position, hits, TrackableType.Planes))
                return false;

            var hitPose = hits[0].pose;
            virtualObjectsManager.HandleNewObject(hitPose);
            logger.Log("TryToResolvePlaneTouch", "touch resolved");
            return true;
        }

        private bool TryToResolveVirtualObjectTouch()
        {
            logger.Log("TryToResolveVirtualObjectTouch", "");
            if (!GetVirtualObjectHit(out RaycastHit hit))
                return false;

            virtualObjectsManager.HandleNewObject(hit);
            logger.Log("TryToResolveVirtualObjectTouch", "touch resolved");
            return true;
        }

        private bool GetVirtualObjectHit(out RaycastHit hit)
        {
            var touch = (Touch)lastTouch;
            Vector3 pos = touch.position;
            Ray ray = Camera.main.ScreenPointToRay(pos);
            return Physics.Raycast(ray, out hit) && (hit.collider.tag.Equals(VIRTUAL_OBJECT_TAG));
        }
    }
}
