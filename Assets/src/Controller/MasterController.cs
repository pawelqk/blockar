using Controller.UI;
using Materials;
using VirtualObjects;
using UnityEngine;
using System.Linq;

namespace Controls
{
    public class MasterController : IController, IDatabaseObserver
    {
        private readonly VirtualObjectsManager virtualObjectsManager;
        private readonly MaterialManager materialManager;
        private readonly IUIControls uiControls;
        private readonly IDatabase databaseCtrl;
        private readonly Logger logger;

        public MasterController(VirtualObjectsManager virtualObjectsManager, MaterialManager materialManager,  IUIControls uiControls, IDatabase databaseCtrl, Logger logger)
        {
            this.virtualObjectsManager = virtualObjectsManager;
            this.materialManager = materialManager;
            this.uiControls = uiControls;
            this.databaseCtrl = databaseCtrl;
            this.logger = logger;
            this.databaseCtrl.AttachDbObserver(this);
        }

        public void HandleNonUITouch()
        {
            uiControls.HideContextMenus();
            virtualObjectsManager.InvalidateSelection();
        }

        public void HandleCubeSizeChange(int size)
        {
            virtualObjectsManager.SetCubeSize(size);
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
            var gameObjectsData = virtualObjectsManager.GetGameObjectsData();
            materialManager.SetEdgesVisibility(gameObjectsData);
        }

        public void HandlePlaneHold()
        {
            uiControls.HandlePlaneHold();
        }

        public void HandleObjectNotCreated()
        {
            uiControls.ShowShortTimeMsg("Object couldn't be created in that place");
        }

        public void Notify(DatabaseStatus status)
        {
            logger.Log("Notify()", $"status={status}");
            switch(status)
            {
                case DatabaseStatus.actionCurrentlyNotPossible:
                    uiControls.ShowShortTimeMsg("Action is currently not possible");
                    break;
                case DatabaseStatus.saveOngoing:
                    uiControls.ShowShortTimeMsg("Save ongoing");
                    break;
                case DatabaseStatus.saveDone:
                    uiControls.ShowShortTimeMsg("Save done");
                    break;
                case DatabaseStatus.saveError:
                    uiControls.ShowShortTimeMsg("Save error");
                    break;
                case DatabaseStatus.getOngoing:
                    uiControls.ShowShortTimeMsg("Get data ongoing");
                    break;
                case DatabaseStatus.getObjectsDataDone:
                    uiControls.ShowShortTimeMsg("Get objects data done");
                    RestoreObjects();
                    break;
                case DatabaseStatus.getSessionsListDone:
                    uiControls.ShowShortTimeMsg("Get sessions list done");
                    uiControls.PresentSessionsList(databaseCtrl.GetRetrievedSessionsList());
                    break;
                case DatabaseStatus.getError:
                    uiControls.ShowShortTimeMsg("Get data error");
                    break;
            }
        }

        private void RestoreObjects()
        {
            var objectsData = databaseCtrl.GetRetrievedObjectsData();
            var materialsToApply = objectsData.ToDictionary(item => item.Key, item => item.Value.Materials[1]);
            var materials = materialManager.GetMaterials();
            virtualObjectsManager.RestoreObjects(objectsData);
            foreach(var objectData in objectsData.Values)
            {
                var gameObj = objectData.GameObject;
                var mainMaterial = materials[materialsToApply[objectData.Guid]];
                logger.Log("RestoreObjects()", $"obj={objectData.Guid}, objMaterial={materialsToApply[objectData.Guid]}, mainMaterial={mainMaterial}");
                materialManager.SetGameObjectMaterial(gameObj, mainMaterial);
            }
        }

        public void SaveSesson(string sessionName)
        {
            databaseCtrl.SaveToDb(sessionName, virtualObjectsManager.GetGameObjectsData());
        }

        public void RestoreSession(string sessionName)
        {
            databaseCtrl.GetAllFromDb(sessionName);
        }

        public void GetAvailableSessions()
        {
            databaseCtrl.GetSessionsFromDb();
        }
    }
}
