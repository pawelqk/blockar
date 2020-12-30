using System.Collections.Generic;
using UnityEngine;

namespace VirtualObjects
{
    public class VirtualObjectsManager
    {
        private readonly GameObject gameObjectToInstantiate;
        private readonly Dictionary<int, GameObject> objects;
        private readonly Logger logger;
        private GameObject currentlySelected;

        public VirtualObjectsManager(GameObject gameObjectToInstantiate, Logger logger)
        {
            this.gameObjectToInstantiate = gameObjectToInstantiate;
            this.objects = new Dictionary<int, GameObject>();
            this.logger = logger;
            this.currentlySelected = null;
        }

        public void HandleNewObject(Pose hitPose)
        {
            var newObject = Object.Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation);
            objects[newObject.GetInstanceID()] = newObject;
        }

        public void HandleNewObject(RaycastHit hit)
        {
            var hitObject = hit.collider.gameObject;
            // oversimplification - assumes that object has the same dimmensions,
            // for objects other than cube something smart must be figured out
            var scalingFactor = hitObject.transform.localScale.x;
            var pos = hitObject.transform.position + scalingFactor * hit.normal;
            var hitPose = new Pose(pos, hit.transform.rotation);
            var newObject = Object.Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation);
            objects[newObject.GetInstanceID()] = newObject;
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
    }
}
