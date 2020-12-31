using UnityEngine;

namespace Controls
{
    public interface IController
    {
        void HandleObjectSelection(GameObject gameObject);
        void HandleSelectedObjectDeletion();
        void HandleSelectedObjectMaterialChange();
        void HandleEdgesClick();
        void HandleTextureChange(string buttonText);
        void HandleNonUITouch();
        void HandlePlaneHold();
    }
}
