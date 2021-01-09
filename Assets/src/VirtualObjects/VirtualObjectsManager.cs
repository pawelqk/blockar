using System.Collections.Generic;
using UnityEngine;

namespace VirtualObjects
{
    public class VirtualObjectsManager
    {
        private readonly VirtualObjectsCreator virtualObjectsCreator;
        private readonly VirtualObjectsStore virtualObjectsStore;
        private readonly Logger logger;

        private GameObject currentlySelected;

        public VirtualObjectsManager(VirtualObjectsCreator virtualObjectsCreator, VirtualObjectsStore virtualObjectsStore, Logger logger)
        {
            this.virtualObjectsCreator = virtualObjectsCreator;
            this.virtualObjectsStore = virtualObjectsStore;
            this.logger = logger;
        }

        public bool HandleNewObject(Transform transform)
        {
            var newObjectWithAnchor = virtualObjectsCreator.HandleNewObject(transform);
            if (newObjectWithAnchor is null)
                return false;

            virtualObjectsStore.AddObjectToCollection(newObjectWithAnchor.Obj, newObjectWithAnchor.Anchor, null);
            return true;
        }

        public bool HandleNewObject(Pose hitPose)
        {
            var newObjectWithAnchor = virtualObjectsCreator.HandleNewObject(hitPose);
            if (newObjectWithAnchor is null)
                return false;

            virtualObjectsStore.AddObjectToCollection(newObjectWithAnchor.Obj, newObjectWithAnchor.Anchor, null);
            return true;
        }

        public bool HandleNewObject(RaycastHit hit)
        {
            var newObjectWithAnchor = virtualObjectsCreator.HandleNewObject(hit);
            if (newObjectWithAnchor is null)
                return false;

            virtualObjectsStore.AddObjectToCollection(newObjectWithAnchor.Obj, newObjectWithAnchor.Anchor, hit.collider.gameObject);
            return true;
        }
        public void RestoreObjects(IDictionary<string, VirtualObjectData> retrievedObjectsData)
        {
            logger.Log("RestoreObjects()", "");
            foreach (var objectData in retrievedObjectsData)
                virtualObjectsCreator.CreateFromObjectData(objectData.Value);

            virtualObjectsStore.AddObjectsDataToCollection(retrievedObjectsData);
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
            {
                logger.LogWarning("DeleteSelectedObject()", "currentlySelected object is null");
                return;
            }

            virtualObjectsStore.DeleteObject(currentlySelected);
            currentlySelected = null;
        }

        public void SetCubeSize(int size)
        {
            virtualObjectsCreator.setCubeSize(size);
        }
        

        public GameObject GetCurrentlySelected()
        {
            return currentlySelected;
        }

        public Dictionary<string, VirtualObjectData> GetGameObjectsData()
        {
            return virtualObjectsStore.GetAllObjectsData();
        }
    }
}