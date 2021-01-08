using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace VirtualObjects
{
    public class VirtualObjectsStore
    {
        private readonly Dictionary<string, VirtualObjectData> objectsData;
        private readonly Dictionary<int, string> reversedGameObjectsToDataMapping;

        private readonly ARAnchorManager anchorManager;
        private readonly Logger logger;

        public VirtualObjectsStore(ARAnchorManager anchorManager, Logger logger)
        {
            this.objectsData = new Dictionary<string, VirtualObjectData>();
            this.reversedGameObjectsToDataMapping = new Dictionary<int, string>();
            this.anchorManager = anchorManager;
            this.logger = logger;
        }

        public Dictionary<string, VirtualObjectData> GetAllObjectsData()
        {
            return objectsData;
        }

        public void AddObjectToCollection(GameObject objectToStore, ARAnchor arAnchor, GameObject parentObject)
        {
            if(objectToStore is null)
            {
                logger.LogError("AddObjectToCollection()", "objectToStore is null, it never should be");
                return;
            }

            var guid = GetGuid();
            var anchor = parentObject is null ? arAnchor : objectsData[reversedGameObjectsToDataMapping[parentObject.GetInstanceID()]].Anchor;
            var objectData = new VirtualObjectData
            {
                Guid = guid,
                GameObject = objectToStore,
                Anchor = anchor,
                ParentingObject = parentObject is null ? null : objectsData[reversedGameObjectsToDataMapping[parentObject.GetInstanceID()]]
            };
            objectsData[guid] = objectData;
            reversedGameObjectsToDataMapping[objectToStore.GetInstanceID()] = guid;
            logger.Log("AddObjectToCollection()", $"obj: {objectData}");
        }

        public void AddObjectsDataToCollection(IDictionary<string, VirtualObjectData> retrievedObjectsData)
        {
            logger.Log("AddObjectsDataToCollection()", "");
            foreach (var objectData in retrievedObjectsData)
            {
                if (objectsData.ContainsKey(objectData.Key))
                {
                    logger.Log("AddObjectsDataToCollection()", $"obj with id={objectData.Key} already exists, it will be deleted");
                    DeleteObject(objectData.Key);
                }
            }

            foreach (var objectData in retrievedObjectsData)
            {
                objectsData[objectData.Key] = objectData.Value;
                reversedGameObjectsToDataMapping[objectData.Value.GameObject.GetInstanceID()] = objectData.Key;
                logger.Log("AddObjectsDataToCollection()", $"added obj to collection: {objectData.Value}");
            }
        }

        public void DeleteObject(GameObject gameObjectToDelete)
        {
            if (gameObjectToDelete is null)
            {
                logger.LogError("DeleteObject()", "gameObjectToDelete is null, it never should be");
                return;
            }

            var instanceId = gameObjectToDelete.GetInstanceID();
            if (!reversedGameObjectsToDataMapping.ContainsKey(instanceId))
            {
                logger.LogError("DeleteObject()", $"gameObj with ID={instanceId} is not present in reverse mapping");
                return;
            }

            DeleteObject(reversedGameObjectsToDataMapping[instanceId]);
        }

        public void DeleteObject(string objectToDeleteGuid)
        {
            var objectToDelete = objectsData[objectToDeleteGuid];
            if (objectToDelete is null)
            {
                logger.LogError("DeleteObject()", "objectToDelete is null, it never should be");
                return;
            }

            logger.Log("DeleteObject()", $"obj: {objectToDelete}");

            ForEachChild(objectToDeleteGuid, (child) => {
                logger.Log("DeleteObject()", $"child: {child}");
                child.ParentingObject = objectToDelete.ParentingObject;
                child.Anchor = objectToDelete.Anchor;
                child.GameObject.transform.SetParent(child.Anchor.transform);
                logger.Log("DeleteObject()", $"child after tuning: {child}");
            });

            var instanceId = objectToDelete.GameObject.GetInstanceID();
            if (reversedGameObjectsToDataMapping.ContainsKey(instanceId))
                reversedGameObjectsToDataMapping.Remove(instanceId);

            objectsData.Remove(objectToDeleteGuid);
            Object.Destroy(objectToDelete.GameObject);

            if (!IsAnchorStillInUse(objectToDelete.Anchor))
                anchorManager.RemoveAnchor(objectToDelete.Anchor);
        }

        private bool IsAnchorStillInUse(ARAnchor anchor)
        {
            foreach (var objectData in objectsData)
            {
                if (objectData.Value.Anchor == anchor)
                    return true;
            }
            return false;
        }

        private delegate void ActionOnChild(VirtualObjectData child);

        private void ForEachChild(string guid, ActionOnChild action)
        {
            foreach(var objectData in objectsData)
            {
                if (objectData.Value.ParentingObject is null)
                    continue;

                if (objectData.Value.ParentingObject.Guid == guid)
                {
                    action(objectData.Value);
                }
            }
        }

        private string GetGuid()
        {
            return System.Guid.NewGuid().ToString();
        }
    }
}
