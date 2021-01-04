using System.Collections.Generic;
using UnityEngine;

namespace VirtualObjects
{
    public class VirtualObjectsManager
    {
        private readonly GameObject gameObjectToInstantiate;
        private LayerMask layerMask;
        private readonly Logger logger;

        private readonly Dictionary<int, GameObject> objects;
        private GameObject currentlySelected;

        private static readonly float collisionMargin = 0.001f;

        public VirtualObjectsManager(GameObject gameObjectToInstantiate, LayerMask layerMask, Logger logger)
        {
            this.gameObjectToInstantiate = gameObjectToInstantiate;
            this.layerMask = layerMask;
            this.logger = logger;

            this.objects = new Dictionary<int, GameObject>();
            this.currentlySelected = null;
        }

        public bool HandleNewObject(Pose hitPose)
        {
            if (IsPoseIntersectingAnyObject(hitPose))
            {
                logger.Log("Object will not be created as it overlaps existing object");
                return false;
            }

            var newObject = Object.Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation);
            objects[newObject.GetInstanceID()] = newObject;
            return true;
        }

        public bool HandleNewObject(RaycastHit hit)
        {
            var hitObject = hit.collider.gameObject;
            // oversimplification - assumes that object has the same dimmensions,
            // for objects other than cube something smart must be figured out
            var scalingFactor = hitObject.transform.localScale.x;
            var pos = hitObject.transform.position + scalingFactor * hit.normal;
            var hitPose = new Pose(pos, hit.transform.rotation);
            if (IsPoseIntersectingAnyObject(hitPose))
            {
                logger.Log("Object will not be created as it overlaps existing object");
                return false;
            }

            var newObject = Object.Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation);
            objects[newObject.GetInstanceID()] = newObject;
            return true;
        }

        public GameObject HandleSelection(RaycastHit hit)
        {
            currentlySelected = hit.collider.gameObject;
            return currentlySelected;
        }

        public void InvalidateSelection()
        {
            currentlySelected = null;
        }

        public void DeleteSelectedObject()
        {
            if (currentlySelected is null)
                return;

            objects.Remove(currentlySelected.GetInstanceID());
            Object.Destroy(currentlySelected);
            currentlySelected = null;
        }

        public GameObject GetCurrentlySelected()
        {
            return currentlySelected;
        }

        public Dictionary<int, GameObject> GetGameObjects()
        {
            return objects;
        }

        private bool IsPoseIntersectingAnyObject(Pose pose)
        {
            var scaledObjectExtents = Vector3.Scale(gameObjectToInstantiate.GetComponent<BoxCollider>().extents, gameObjectToInstantiate.transform.localScale);
            var objectExtentsWithMargin = scaledObjectExtents - Vector3.one * collisionMargin;

            var hitColliders = Physics.OverlapBox(pose.position, objectExtentsWithMargin, pose.rotation, layerMask);
            return hitColliders.Length > 0;
        }
    }
}