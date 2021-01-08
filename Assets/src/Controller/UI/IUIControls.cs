using Controls;
using System.Collections.Generic;
using UnityEngine;

namespace Controller.UI
{
    public interface IUIControls
    {
        void HandleGameObjectSelection(GameObject selectedGameObject);
        void SetController(IController controller);
        void HideContextMenus();
        void HandlePlaneHold();
        void HandleTextureMenu();
        void ShowShortTimeMsg(string msg);
        void Update();
        void PresentSessionsList(IList<string> sessions);
    }
}
