using System.Collections.Generic;
using UnityEngine;

namespace VirtualObjects
{
    public class VirtualObjectsManager
    {
        private readonly GameObject gameObjectToInstantiate;
        private readonly List<GameObject> objects;

        public VirtualObjectsManager(GameObject gameObjectToInstantiate)
        {
            this.gameObjectToInstantiate = gameObjectToInstantiate;
            this.objects = new List<GameObject>();
        }

        public void HandleNewObject(Pose hitPose)
        {
            var newObject = Object.Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation);
            objects.Add(newObject);
        }

        public void HandleNewObject(RaycastHit hit)
        {
            var hitObject = hit.collider.gameObject;
            // oversimplification - assumes that object has the same dimmensions, for objects other than cube something smart must be figured out
            var scalingFactor = hitObject.transform.localScale.x;
            var pos = hitObject.transform.position + scalingFactor * hit.normal;
            var hitPose = new Pose(pos, hit.transform.rotation);
            var newObject = Object.Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation);
            objects.Add(newObject);
        }
    }
}
