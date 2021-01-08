using System;
using UnityEngine;

namespace VirtualObjects
{
    [Serializable]
    public class VirtualObjectDataDTO
    {
        [Serializable]
        public struct Position
        {
            public float x;
            public float y;
            public float z;
        }

        [Serializable]
        public struct Rotation
        {
            public float x;
            public float y;
            public float z;
            public float w;
        }

        public string guid;
        public string cloudAnchorId;
        public Position parentRelPosition;
        public Rotation parentRelRotation;
        public string[] materials;
        public string parentingObjectGuid;

        public VirtualObjectDataDTO()
        {
        }

        public VirtualObjectDataDTO(string guid, string cloudAnchorId, Position parentRelPosition, Rotation parentRelRotation,
            string[] materials, string parentingObjectGuid)
        {
            this.guid = guid;
            this.cloudAnchorId = cloudAnchorId;
            this.parentRelPosition = parentRelPosition;
            this.parentRelRotation = parentRelRotation;
            this.materials = materials;
            this.parentingObjectGuid = parentingObjectGuid;
        }

        public static VirtualObjectDataDTO FromVirtualObjectData(VirtualObjectData objectData)
        {
            var gameObjPos = objectData.ParentRelPosition;
            var gameObjRot = objectData.ParentRelRotation;

            return new VirtualObjectDataDTO
            {
                guid = objectData.Guid,
                cloudAnchorId = objectData.CloudAnchorId,
                parentRelPosition = new Position{ x = gameObjPos.x, y = gameObjPos.y, z = gameObjPos.z},
                parentRelRotation = new Rotation{ x = gameObjRot.x, y = gameObjRot.y, z = gameObjRot.z, w = gameObjRot.w},
                materials = objectData.Materials,
                parentingObjectGuid = objectData.ParentingObject?.Guid
            };
        }
    }
}
