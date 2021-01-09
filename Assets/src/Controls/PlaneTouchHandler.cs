using UnityEngine.XR.ARFoundation;
using VirtualObjects;

namespace Controls
{
    public class PlaneTouchHandler
    {
        private readonly VirtualObjectsManager virtualObjectsManager;
        private readonly IController controller;

        public PlaneTouchHandler(VirtualObjectsManager virtualObjectsManager, IController controller)
        {
            this.virtualObjectsManager = virtualObjectsManager;
            this.controller = controller;
        }

        public void OnPlaneTouch(ARRaycastHit hit)
        {
            controller.HandleNonUITouch();
            if (!virtualObjectsManager.HandleNewObject(hit.pose))
            {
                controller.HandleObjectNotCreated();
            }
            else
                controller.HandleObjectCreation();
        }

        public void OnPlaneHold()
        {
            controller.HandlePlaneHold();
        }
    }
}
