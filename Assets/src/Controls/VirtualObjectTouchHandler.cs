using UnityEngine;
using VirtualObjects;

namespace Controls
{
    public class VirtualObjectTouchHandler
    {
        private readonly VirtualObjectsManager virtualObjectsManager;
        private readonly IController controller;

        public VirtualObjectTouchHandler(VirtualObjectsManager virtualObjectsManager, IController controller)
        {
            this.virtualObjectsManager = virtualObjectsManager;
            this.controller = controller;
        }

        public void OnVirtualObjectTap(RaycastHit hit)
        {
            controller.HandleNonUITouch();
            if (!virtualObjectsManager.HandleNewObject(hit))
            {
                controller.HandleObjectNotCreated();
            }
            else
                controller.HandleObjectCreation();
        }

        public void OnVirtualObjectHold(RaycastHit hit)
        {
            var gameObject = virtualObjectsManager.HandleSelection(hit);
            controller.HandleObjectSelection(gameObject);
        }
    }
}
