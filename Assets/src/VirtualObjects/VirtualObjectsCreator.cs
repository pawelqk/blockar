using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace VirtualObjects
{
    public class ObjectWithAnchor
    {
        private GameObject obj;
        private ARAnchor anchor;
        public GameObject Obj { get => obj; set => obj = value; }
        public ARAnchor Anchor { get => anchor; set => anchor = value; }
    }

    public class VirtualObjectsCreator
    {
        private readonly ARAnchorManager anchorManager;
        private readonly GameObject gameObjectToInstantiate;
        private readonly LayerMask layerMask;
        private readonly Logger logger;

        private static readonly float collisionMargin = 0.001f;
        private Vector3 DEFAULT_CUBE_SIZE;


        public VirtualObjectsCreator(ARAnchorManager anchorManager, GameObject gameObjectToInstantiate, LayerMask layerMask, Logger logger)
        {
            this.anchorManager = anchorManager;
            this.gameObjectToInstantiate = gameObjectToInstantiate;
            this.DEFAULT_CUBE_SIZE = gameObjectToInstantiate.transform.localScale;
            this.layerMask = layerMask;
            this.logger = logger;
        }

        public void setCubeSize(int size)
        {
            var newCubeSize = DEFAULT_CUBE_SIZE * size;
            gameObjectToInstantiate.transform.localScale = newCubeSize;
        }

        public ObjectWithAnchor HandleNewObject(Transform transform)
        {
            var hitPose = new Pose(transform.position, transform.rotation);
            if (IsPoseIntersectingAnyObject(hitPose))
            {
                logger.Log("Object will not be created as it overlaps existing object");
                return null;
            }

            var anchor = anchorManager.AddAnchor(hitPose);
            var newObject = Object.Instantiate(gameObjectToInstantiate, transform.position, transform.rotation);
            newObject.transform.SetParent(anchor.transform);
            return new ObjectWithAnchor { Obj = newObject, Anchor = anchor };
        }

        public ObjectWithAnchor HandleNewObject(Pose hitPose)
        {
            if (IsPoseIntersectingAnyObject(hitPose))
            {
                logger.Log("Object will not be created as it overlaps existing object");
                return null;
            }

            var anchor = anchorManager.AddAnchor(hitPose);
            var newObject = Object.Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation);
            newObject.transform.SetParent(anchor.transform);
            return new ObjectWithAnchor { Obj = newObject, Anchor = anchor };
        }

        public ObjectWithAnchor HandleNewObject(RaycastHit hit)
        {
            var hitObject = hit.collider.gameObject;
            // oversimplification - assumes that object has the same dimmensions, for objects other than cube something smart must be figured out
            var gameObjectScaleFactor = gameObjectToInstantiate.transform.localScale.x;
            var scalingFactor = hitObject.transform.localScale.x;
            scalingFactor = scalingFactor/2 + gameObjectScaleFactor/2;
            var pos = hitObject.transform.position + scalingFactor * hit.normal;
            var hitPose = new Pose(pos, hit.transform.rotation);

            if (IsPoseIntersectingAnyObject(hitPose))
            {
                logger.Log("Object will not be created as it overlaps existing object");
                return null;
            }

            var newObject = Object.Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation);
            logger.Log("HandleNewObject(RaycastHit)", $"hit [position: {hitPose.position}, rot: {hitPose.rotation}]");
            logger.Log("HandleNewObject(RaycastHit)", $"collider: [position: {hitObject.transform.position}, rot: {hitObject.transform.rotation}], parent: [position: {hitObject.transform.parent.position}, rot: {hitObject.transform.parent.rotation}]");
            newObject.transform.SetParent(hit.collider.gameObject.transform.parent);
            logger.Log("HandleNewObject(RaycastHit)", $"newObj [pos: {newObject.transform.position}, rot: {newObject.transform.rotation}], parent: [pos: {newObject.transform.parent?.position}, rot: {newObject.transform.parent?.rotation}]");
            return new ObjectWithAnchor { Obj = newObject, Anchor = null };
        }

        private bool IsPoseIntersectingAnyObject(Pose pose)
        {
            var scaledObjectExtents = Vector3.Scale(gameObjectToInstantiate.GetComponent<BoxCollider>().size / 2.0f, gameObjectToInstantiate.transform.localScale);
            var objectExtentsWithMargin = scaledObjectExtents - Vector3.one * collisionMargin;

            var hitColliders = Physics.OverlapBox(pose.position, objectExtentsWithMargin, pose.rotation, layerMask);
            return hitColliders.Length > 0;
        }

        public void CreateFromObjectData(VirtualObjectData objectData)
        {
            logger.Log("CreateFromObjectData()", $"objectData={objectData}");
            var anchor = anchorManager.AddAnchor(new Pose(objectData.AnchorTransform.position, objectData.AnchorTransform.rotation));
            var newObject = Object.Instantiate(gameObjectToInstantiate, anchor.transform.position, anchor.transform.rotation);
            newObject.transform.SetParent(anchor.transform);
            newObject.transform.localPosition = objectData.ParentRelPosition;
            newObject.transform.localRotation = objectData.ParentRelRotation;
            newObject.transform.localScale = objectData.ParentRelScale;
            objectData.GameObject = newObject;
            objectData.Anchor = anchor;
        }
    }
}
