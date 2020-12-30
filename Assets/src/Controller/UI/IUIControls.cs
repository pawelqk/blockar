using Controls;
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
    }
}
