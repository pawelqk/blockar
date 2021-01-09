using Audio;
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
                AudioManager.PlayAudioClip(AudioClips.PLACING_ERROR);
                controller.HandleObjectNotCreated();
            }
            else
                AudioManager.PlayAudioClip(AudioClips.KNOCK);
        }

        public void OnPlaneHold()
        {
            controller.HandlePlaneHold();
        }
    }
}
