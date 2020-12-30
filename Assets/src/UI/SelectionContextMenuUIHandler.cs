using Controls;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SelectionContextMenuUIHandler
    {
        private IController controller;
        private GameObject menuPanel;
        private Button changeTextureButton;
        private Button deleteObjectButton;
        private Button hideMenuButton;

        public SelectionContextMenuUIHandler(IController controller)
        {
            this.controller = controller;
            menuPanel = GameObject.Find("CubeContextMenuPanel");
            deleteObjectButton = GameObject.Find("DeleteObjectButton").GetComponent<Button>();
            hideMenuButton = GameObject.Find("HideMenuButton").GetComponent<Button>();
            changeTextureButton = GameObject.Find("ChangeTextureButton").GetComponent<Button>();
            HideContextMenu();
            RegisterCallbacks();
        }

        private void RegisterCallbacks()
        {
            changeTextureButton.onClick.AddListener(this.OnChangeTextureButtonClick);
            deleteObjectButton.onClick.AddListener(this.OnDeleteObjectButtonClick);
            hideMenuButton.onClick.AddListener(this.OnHideMenuButtonClick);
        }

        private void OnDeleteObjectButtonClick()
        {
            controller.HandleSelectedObjectDeletion();
            HideContextMenu();
        }

        private void OnHideMenuButtonClick()
        {
            HideContextMenu();
        }

        private void OnChangeTextureButtonClick()
        {
            HideContextMenu();
            controller.HandleSelectedObjectMaterialChange();
        }

        public IController Controller { set => controller = value; }

        public void HandleGameObjectSelection(GameObject selectedGameObject)
        {
            ShowContextMenu();
        }

        private void ShowContextMenu()
        {
            menuPanel.SetActive(true);
        }

        public void HideContextMenu()
        {
            menuPanel.SetActive(false);
        }
    }
}
