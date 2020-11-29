using UnityEngine;

namespace Controls
{
    public interface IController
    {
        void HandleObjectSelection(GameObject gameObject);
        void HandleSelectedObjectDeletion();
        void HandleSelectedObjectMaterialChange();
        void HandleEdgesToggle(bool edgesOn);
        void HandleBrickTexture(bool brickTextureOn);
        void HandleNonUITouch();
    }
}
