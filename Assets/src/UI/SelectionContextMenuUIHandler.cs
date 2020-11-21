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
        private GameObject textureMenuPanel;
        private Button deleteObjectButton;
        private Button hideMenuButton;
        private Button changeTextureObjectButton;
        private Button hideTextureMenuButton;
        private Button firstTextureButton;
        private Button secondTextureButton;
        private Material[] materials;

        public SelectionContextMenuUIHandler(IController controller)
        {
            this.controller = controller;
            menuPanel = GameObject.Find("CubeContextMenuPanel");
            textureMenuPanel = GameObject.Find("TextureContextMenuPanel");
            deleteObjectButton = GameObject.Find("DeleteObjectButton").GetComponent<Button>();
            changeTextureObjectButton = GameObject.Find("ChangeObjectTextureButton").GetComponent<Button>();
            hideMenuButton = GameObject.Find("HideMenuButton").GetComponent<Button>();
            hideTextureMenuButton = GameObject.Find("HideTextureMenuButton").GetComponent<Button>();

            // Issute - seems to return only 1 element,
            // as the button for the 2 texture doesn't work
            materials = Renderer.FindObjectsOfType<Material>();

            firstTextureButton = GameObject.Find("FirstTextureButton").GetComponent<Button>();
            secondTextureButton = GameObject.Find("SecondTextureButton").GetComponent<Button>();

            HideContextMenu();
            HideTextureContextMenu();
            RegisterCallbacks();
        }

        private void RegisterCallbacks()
        {
            deleteObjectButton.onClick.AddListener(this.OnDeleteObjectButtonClick);
            hideMenuButton.onClick.AddListener(this.OnHideMenuButtonClick);
            hideTextureMenuButton.onClick.AddListener(this.OnHideTextureMenuButtonClick);
            changeTextureObjectButton.onClick.AddListener(this.OnChangeTextureButtonClick);
            firstTextureButton.onClick.AddListener(this.OnFirstTextureButtonClick);
            secondTextureButton.onClick.AddListener(this.OnSecondTextureButtonClick);
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
            ShowTextureContextMenu();
        }

        private void OnHideTextureMenuButtonClick()
        {
            HideTextureContextMenu();
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

        private void ShowTextureContextMenu()
        {
            textureMenuPanel.SetActive(true);
        }

        public void HideContextMenu()
        {
            menuPanel.SetActive(false);
        }

        public void HideTextureContextMenu()
        {
            textureMenuPanel.SetActive(false);
        }

        private void OnFirstTextureButtonClick()
        {
            OnChangeObjectMaterial(materials[0]);
        }

        private void OnSecondTextureButtonClick()
        {
            OnChangeObjectMaterial(materials[1]);
        }

        private void OnChangeObjectMaterial(Material material)
        {
            controller.HandleMaterialChange(material);
            HideTextureContextMenu();
        }
    }
}
