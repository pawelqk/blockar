using Controller.UI;
using Controls;
using UnityEngine;

namespace UI
{
    public class UIControls : IUIControls
    {
        private IController controller;
        private SelectionContextMenuUIHandler selectionContextMenuUIHandler;
        private MainUIHandler mainUIHandler;

        public UIControls()
        {
            this.selectionContextMenuUIHandler = new SelectionContextMenuUIHandler(controller);
            this.mainUIHandler = new MainUIHandler(controller);
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
            this.mainUIHandler.Controller = controller;
        }

        public void HandlePlaneHold()
        {
            mainUIHandler.ShowPlaneContextMenu();
        }

        public void HandleTextureMenu()
        {
            mainUIHandler.ShowTextureContextMenu();
        }
    }
}
