using UnityEngine;
using UnityEditor;
using Controls;
using UnityEngine.UI;

namespace UI
{
    public class MainUIHandler
    {
        private IController controller;
        private readonly Toggle edgesToggle;
        private readonly Toggle brickToggle;

        public MainUIHandler(IController controller)
        {
            this.controller = controller;
            this.edgesToggle = GameObject.Find("EdgesToggle").GetComponent<Toggle>();
            this.brickToggle = GameObject.Find("BrickToggle").GetComponent<Toggle>();
            RegisterCallbacks();
        }

        private void RegisterCallbacks()
        {
            edgesToggle.onValueChanged.AddListener(delegate
            {
                this.ToggleEdges(edgesToggle);
            });
            brickToggle.onValueChanged.AddListener(delegate
            {
                this.ToggleBrickTexture(brickToggle);
            });
        }

        private void ToggleEdges(Toggle edgesToggle)
        {
            controller.HandleEdgesToggle(edgesToggle.isOn);
        }

        private void ToggleBrickTexture(Toggle brickTexture)
        {
            controller.HandleBrickTexture(brickTexture.isOn);
        }

        public IController Controller { set => controller = value; }
    }
}