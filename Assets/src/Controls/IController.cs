using UnityEngine;

namespace Controls
{
    public interface IController
    {
        void HandleObjectSelection(GameObject gameObject);
        void HandleSelectedObjectDeletion();
        void HandleNonUITouch();
    }
}
