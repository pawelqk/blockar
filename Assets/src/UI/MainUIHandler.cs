using UnityEngine;
using Controls;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

namespace UI
{
    public class MainUIHandler
    {
        private IController controller;
        private readonly Button enableEdgesButton;
        private readonly Button saveSessionButton;
        private readonly Button showSessionsButton;
        private readonly Button brickTextureButton;
        private readonly Button basicTextureButton;
        private readonly Button starsTextureButton;
        private readonly Button woodTextureButton;
        private readonly Button setDefaultTextureButton;
        private readonly Button hidePlaneContextMenuButton;
        private readonly Button hideTextureContextMenuButton;
        private readonly Button hideCubeSizeMenuButton;
        private readonly Button setCubeSizeButton;
        private readonly Button defaultCubeSize;
        private readonly Button doubleCubeSize;
        private readonly Button tripleCubeSize;
        private GameObject textureContextMenuPanel;
        private GameObject planeContextMenuPanel;
        private GameObject sessionsListMenuPanel;
        private GameObject sessionEntityButtonPrefab;
        private InputField sessionNameInputField;
        private GameObject cubeSizeMenuPanel;

        public MainUIHandler(IController controller)
        {
            this.controller = controller;
            this.enableEdgesButton = GameObject.Find("EnableEdgesButton").GetComponent<Button>();
            this.saveSessionButton = GameObject.Find("SaveSessionButton").GetComponent<Button>();
            this.showSessionsButton = GameObject.Find("ShowSessionsButton").GetComponent<Button>();
            this.brickTextureButton = GameObject.Find("BrickTextureButton").GetComponent<Button>();
            this.basicTextureButton = GameObject.Find("BasicTextureButton").GetComponent<Button>();
            this.starsTextureButton = GameObject.Find("StarsTextureButton").GetComponent<Button>();
            this.woodTextureButton = GameObject.Find("WoodTextureButton").GetComponent<Button>();
            this.setCubeSizeButton = GameObject.Find("SetCubeSizeButton").GetComponent<Button>();
            this.defaultCubeSize = GameObject.Find("DefaultCubeSize").GetComponent<Button>();
            this.doubleCubeSize = GameObject.Find("DoubleCubeSize").GetComponent<Button>();
            this.tripleCubeSize = GameObject.Find("TripleCubeSize").GetComponent<Button>();
            this.setDefaultTextureButton = GameObject.Find("SetDefaultTextureButton").GetComponent<Button>();
            this.hideTextureContextMenuButton = GameObject.Find("HideTextureContextButton").GetComponent<Button>();
            this.hidePlaneContextMenuButton = GameObject.Find("HidePlaneContextButton").GetComponent<Button>();
            this.hideCubeSizeMenuButton = GameObject.Find("HideCubeSizeMenuButton").GetComponent<Button>();
            this.textureContextMenuPanel = GameObject.Find("TextureContextMenuPanel");
            this.planeContextMenuPanel = GameObject.Find("PlaneContextMenuPanel");
            this.sessionsListMenuPanel = GameObject.Find("SessionsListMenuPanel");
            this.cubeSizeMenuPanel = GameObject.Find("CubeSizeMenuPanel");
            this.sessionEntityButtonPrefab = Resources.Load("Prefabs/SessionEntityButton", typeof(GameObject)) as GameObject;
            this.sessionNameInputField = GameObject.Find("SessionNameInputField").GetComponent<InputField>();
            HidePlaneContextMenu();
            HideTextureContextMenu();
            HideSessionsListMenu();
            HideSessionNameInputField();
            HideCubeSizeMenuPanel();
            RegisterCallbacks();
        }

        public IController Controller { set => controller = value; }

        private void RegisterCallbacks()
        {
            enableEdgesButton.onClick.AddListener(this.OnEnableEdgesClick);
            saveSessionButton.onClick.AddListener(this.OnSaveSessionClick);
            showSessionsButton.onClick.AddListener(this.OnShowSessionsClick);
            brickTextureButton.onClick.AddListener(this.OnTextureChangeButtonClick);
            basicTextureButton.onClick.AddListener(this.OnTextureChangeButtonClick);
            starsTextureButton.onClick.AddListener(this.OnTextureChangeButtonClick);
            woodTextureButton.onClick.AddListener(this.OnTextureChangeButtonClick);
            defaultCubeSize.onClick.AddListener(this.OnCubeSizeChangeButtonClick);
            doubleCubeSize.onClick.AddListener(this.OnCubeSizeChangeButtonClick);
            tripleCubeSize.onClick.AddListener(this.OnCubeSizeChangeButtonClick);
            setDefaultTextureButton.onClick.AddListener(this.ShowTextureContextMenu);
            setCubeSizeButton.onClick.AddListener(this.ShowCubeSizeMenuPanel);
            hidePlaneContextMenuButton.onClick.AddListener(this.HidePlaneContextMenu);
            hideTextureContextMenuButton.onClick.AddListener(this.HideTextureContextMenu);
            hideCubeSizeMenuButton.onClick.AddListener(this.HideCubeSizeMenuPanel);
            sessionNameInputField.onEndEdit.AddListener(this.OnSessionNameProvided);
        }

        private void OnEnableEdgesClick(){
            controller.HandleEdgesClick();
        }

        private void OnSaveSessionClick()
        {
            HidePlaneContextMenu();
            ShowSessionNameInputField();
        }

        private void OnSessionNameProvided(string sessionName)
        {
            HideSessionNameInputField();
            if(string.IsNullOrEmpty(sessionName))
                return;

            controller.SaveSesson(sessionName);
        }

        private void OnShowSessionsClick()
        {
            controller.GetAvailableSessions();
            HidePlaneContextMenu();
        }

        private void OnCubeSizeChangeButtonClick(){
            var buttonText = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text;
            controller.HandleCubeSizeChange(Int32.Parse(buttonText));
            HideCubeSizeMenuPanel();
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

        private void ShowSessionNameInputField()
        {
            sessionNameInputField.gameObject.SetActive(true);
            sessionNameInputField.Select();
            sessionNameInputField.ActivateInputField();
        }

        public void ShowCubeSizeMenuPanel(){
            HidePlaneContextMenu();
            cubeSizeMenuPanel.SetActive(true);
        }

        public void HideCubeSizeMenuPanel(){
            cubeSizeMenuPanel.SetActive(false);
        }

        private void HideSessionNameInputField()
        {
            sessionNameInputField.DeactivateInputField();
            sessionNameInputField.gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
        }

        public void HidePlaneContextMenu(){
            planeContextMenuPanel.SetActive(false);
        }

        public void HideTextureContextMenu(){
            textureContextMenuPanel.SetActive(false);
        }

        public void HandleObjectNotCreated()
        {
            // TODO: implement
        }

        private void HideSessionsListMenu()
        {
            sessionsListMenuPanel.SetActive(false);
        }

        public void ShowSessionsList(IList<string> sessions)
        {
            DestroySessionEntitiesButtons();
            foreach(var sessionName in sessions)
            {
                AddButtonToSessionsListPanel(sessionName, () => OnSessionEntityButtonClick(sessionName));
            }
            AddButtonToSessionsListPanel("Hide menu", () => {
                DestroySessionEntitiesButtons();
                HideSessionsListMenu();
            });
            sessionsListMenuPanel.SetActive(true);
        }

        private void AddButtonToSessionsListPanel(string buttonText, UnityAction onClickCallback)
        {
            var newButton = UnityEngine.Object.Instantiate(sessionEntityButtonPrefab);
            newButton.transform.position = sessionsListMenuPanel.transform.position;
            newButton.GetComponent<RectTransform>().SetParent(sessionsListMenuPanel.transform);
            var buttonComponent = newButton.GetComponent<Button>();
            buttonComponent.GetComponentInChildren<Text>().text = buttonText;
            buttonComponent.onClick.AddListener(onClickCallback);
        }

        private void OnSessionEntityButtonClick(string sessionName)
        {
            controller.RestoreSession(sessionName);
            DestroySessionEntitiesButtons();
            HideSessionsListMenu();
        }

        private void DestroySessionEntitiesButtons()
        {
            EventSystem.current.SetSelectedGameObject(null);
            var currentButtons = sessionsListMenuPanel.GetComponentsInChildren<Button>();
            foreach (var button in currentButtons)
                UnityEngine.Object.Destroy(button.gameObject);
        }
    }
}
