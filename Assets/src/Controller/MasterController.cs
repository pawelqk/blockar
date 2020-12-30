using Controller.UI;
using Materials;
using UnityEngine;
using VirtualObjects;
using System.Collections.Generic;
using System;

namespace Controls
{
    public class MasterController : IController
    {
        private readonly VirtualObjectsManager virtualObjectsManager;
        private readonly MaterialManager materialManager;
        private readonly IUIControls uiControls;

        public MasterController(VirtualObjectsManager virtualObjectsManager, MaterialManager materialManager, 
            IUIControls uiControls)
        {
            this.virtualObjectsManager = virtualObjectsManager;
            this.materialManager = materialManager;
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

        public void HandleSelectedObjectMaterialChange()
        {
            // At this point the menu with textures should appear.
            // After the texture selection it will be applied to the object, that was selected
            uiControls.HandleTextureMenu();
        }

        public void HandleTextureChange(string buttonText)
        {

            string materialName = buttonText.ToLower().Replace(" ", "_");
            var materials = materialManager.GetMaterials();
            var material = materials[materialName];
            var gameObject = virtualObjectsManager.GetCurrentlySelected();
            if (gameObject is null){
                materialManager.ChangeSelectedMaterial(material);
                return;
            }
            materialManager.SetGameObjectMaterial(gameObject, material);
        }

        public void HandleEdgesClick()
        {
            var gameObjects = virtualObjectsManager.GetGameObjects();
            materialManager.SetEdgesVisibility(gameObjects);
        }

        public void HandlePlaneHold()
        {
            uiControls.HandlePlaneHold();
            
        }
    }
}
