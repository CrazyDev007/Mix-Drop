using UnityEngine;

namespace UI
{
    public abstract class ScreenUI : MonoBehaviour
    {
        public ScreenType screenType;
        [HideInInspector] public UIManager uiManager;

        public void Setup(UIManager manager)
        {
            uiManager = manager;
            SetActive(false);
        }

        public abstract void SetActive(bool active);
    }
}