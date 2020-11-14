using Controller.UI;
using Controls;
using UnityEngine;

namespace UI
{
    public class UIControls : IUIControls
    {
        private IController controller;
        private SelectionContextMenuUIHandler selectionContextMenuUIHandler;

        public UIControls()
        {
            this.selectionContextMenuUIHandler = new SelectionContextMenuUIHandler(controller);
        }

        public void HandleGameObjectSelection(GameObject selectedGameObject)
        {
            this.selectionContextMenuUIHandler.HandleGameObjectSelection(selectedGameObject);
        }

        public void HideContextMenus()
        {
            selectionContextMenuUIHandler.HideContextMenu();
        }

        public void SetController(IController controller)
        {
            this.controller = controller;
            this.selectionContextMenuUIHandler.Controller = controller;
        }
    }
}
