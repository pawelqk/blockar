using UnityEngine;
using UnityEditor;
using Controls;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI
{
    public class MainUIHandler
    {
        private IController controller;
        private readonly Button enableEdgesButton;
        private readonly Button brickTextureButton;
        private readonly Button basicTextureButton;
        private readonly Button starsTextureButton;
        private readonly Button woodTextureButton;
        private readonly Button setDefaultTextureButton;
        private readonly Button hidePlaneContextMenuButton;
        private readonly Button hideTextureContextMenuButton;
        private GameObject textureContextMenuPanel;
        private GameObject planeContextMenuPanel;

        public MainUIHandler(IController controller)
        {
            this.controller = controller;
            this.enableEdgesButton = GameObject.Find("EnableEdgesButton").GetComponent<Button>();
            this.brickTextureButton = GameObject.Find("BrickTextureButton").GetComponent<Button>();
            this.basicTextureButton = GameObject.Find("BasicTextureButton").GetComponent<Button>();
            this.starsTextureButton = GameObject.Find("StarsTextureButton").GetComponent<Button>();
            this.woodTextureButton = GameObject.Find("WoodTextureButton").GetComponent<Button>();
            this.setDefaultTextureButton = GameObject.Find("SetDefaultTextureButton").GetComponent<Button>();
            this.hideTextureContextMenuButton = GameObject.Find("HideTextureContextButton").GetComponent<Button>();
            this.hidePlaneContextMenuButton = GameObject.Find("HidePlaneContextButton").GetComponent<Button>();
            textureContextMenuPanel = GameObject.Find("TextureContextMenuPanel");
            planeContextMenuPanel = GameObject.Find("PlaneContextMenuPanel");
            HidePlaneContextMenu();
            HideTextureContextMenu();
            RegisterCallbacks();
        }

        private void RegisterCallbacks()
        {
            enableEdgesButton.onClick.AddListener(this.OnEnableEdgesClick);
            brickTextureButton.onClick.AddListener(this.OnTextureChangeButtonClick);
            basicTextureButton.onClick.AddListener(this.OnTextureChangeButtonClick);
            starsTextureButton.onClick.AddListener(this.OnTextureChangeButtonClick);
            woodTextureButton.onClick.AddListener(this.OnTextureChangeButtonClick);
            setDefaultTextureButton.onClick.AddListener(this.ShowTextureContextMenu);
            hidePlaneContextMenuButton.onClick.AddListener(this.HidePlaneContextMenu);
            hideTextureContextMenuButton.onClick.AddListener(this.HideTextureContextMenu);
        }

        private void OnEnableEdgesClick(){
            controller.HandleEdgesClick();
        }

        private void OnTextureChangeButtonClick(){
            var buttonText = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text;
            controller.HandleTextureChange(buttonText);
            HideTextureContextMenu();
        }
        
        public void ShowPlaneContextMenu(){
            HideTextureContextMenu();
            planeContextMenuPanel.SetActive(true);
        }

        public void ShowTextureContextMenu(){
            HidePlaneContextMenu();
            textureContextMenuPanel.SetActive(true);
        }

        public void HidePlaneContextMenu(){
            planeContextMenuPanel.SetActive(false);
        }

        public void HideTextureContextMenu(){
            textureContextMenuPanel.SetActive(false);
        }

        public IController Controller { set => controller = value; }
    }
}