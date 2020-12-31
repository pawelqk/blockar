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
            virtualObjectsManager.HandleNewObject(hit.pose);
        }

        public void OnPlaneHold()
        {
            controller.HandlePlaneHold();
        }
    }
}
