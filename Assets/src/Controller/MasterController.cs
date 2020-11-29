using Controller.UI;
using Materials;
using UnityEngine;
using VirtualObjects;

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
            var gameObject = virtualObjectsManager.GetCurrentlySelected();
            materialManager.ChangeGameObjectMaterial(gameObject);
        }

        // Dummy function for presentation purposes only. Will be replaced during implementation of new textures.
        public void HandleBrickTexture(bool brickTextureOn)
        {
            var materials = materialManager.GetMaterials();
            var materialBrick = materials["BrickTexture"];
            Debug.Log("##!! Founded: " + materialBrick.name);
            var materialCube = materials["CubeDemo"];
            Debug.Log("##!! Founded: " + materialCube.name);
            if (brickTextureOn)
                materialManager.ChangeSelectedMaterial(materialBrick);
            else
                materialManager.ChangeSelectedMaterial(materialCube);
        }

        public void HandleEdgesToggle(bool edgesOn)
        {
            var gameObjects = virtualObjectsManager.GetGameObjects();
            materialManager.SetEdgesVisibility(edgesOn, gameObjects);
        }
    }
}
