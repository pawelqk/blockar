using Audio;
using Controller.UI;
using Controls;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIControls : IUIControls
    {
        private IController controller;
        private SelectionContextMenuUIHandler selectionContextMenuUIHandler;
        private MainUIHandler mainUIHandler;
        private ShortTimeMsgHandler shortTimeMsgHandler;

        public UIControls()
        {
            this.selectionContextMenuUIHandler = new SelectionContextMenuUIHandler(controller);
            this.mainUIHandler = new MainUIHandler(controller);
            this.shortTimeMsgHandler = new ShortTimeMsgHandler();
        }

        public void Update()
        {
            shortTimeMsgHandler.Update();
        }

        public void HandleGameObjectSelection(GameObject selectedGameObject)
        {
            this.selectionContextMenuUIHandler.HandleGameObjectSelection(selectedGameObject);
        }

        public void PlayObjectCreationSound()
        {
            AudioManager.PlayAudioClip(AudioClips.KNOCK);
        }

        public void PlayObjectDeletionSound()
        {
            AudioManager.PlayAudioClip(AudioClips.CRACKLE);
        }

        public void PlayInvalidPlacementSound()
        {
            AudioManager.PlayAudioClip(AudioClips.PLACING_ERROR);
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

        public void ShowShortTimeMsg(string msg)
        {
            shortTimeMsgHandler.Show(msg);
        }

        public void PresentSessionsList(IList<string> sessions)
        {
            mainUIHandler.ShowSessionsList(sessions);
        }
    }
}
