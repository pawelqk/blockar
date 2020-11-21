using Controller.UI;
using UnityEngine;
using VirtualObjects;

namespace Controls
{
    public class MasterController : IController
    {
        private readonly VirtualObjectsManager virtualObjectsManager;
        private readonly IUIControls uiControls;

        public MasterController(VirtualObjectsManager virtualObjectsManager, IUIControls uiControls)
        {
            this.virtualObjectsManager = virtualObjectsManager;
            this.uiControls = uiControls;
        }

        public void HandleNonUITouch()
        {
            uiControls.HideContextMenus();
            virtualObjectsManager.InvalidateSelection();
        }

        public void HandleObjectSelection(GameObject gameObject)
        {
            uiControls.HandleGameObjectSelection(gameObject);
        }

        public void HandleSelectedObjectDeletion()
        {
            virtualObjectsManager.DeleteSelectedObject();
        }

        public void HandleMaterialChange(Material material)
        {
            virtualObjectsManager.SetSelectedObjectMaterial(material);
        }
    }
}
