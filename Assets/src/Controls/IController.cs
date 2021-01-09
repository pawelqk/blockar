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
        void HandleObjectNotCreated();
        void HandleCubeSizeChange(int size);
        void SaveSesson(string sessionName);
        void RestoreSession(string sessionName);
        void GetAvailableSessions();
    }
}
