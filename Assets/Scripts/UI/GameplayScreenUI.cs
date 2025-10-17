using ScreenFlow;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class GameplayScreenUI : ScreenUI
    {
        [SerializeField] private GameObject gameplayScreen;

        public void OnClickBtnAddTube()
        {
            TubeManager.Instance.AddEmptyTube();
        }

        public void OnClickBtnPause()
        {
            //uiManager.ChangeScreen(ScreenType.Pause);
        }
        public void OnLevelFailed()
        {
            //uiManager.ChangeScreen(ScreenType.LevelFailed);
        }

        protected override void SetupScreen(VisualElement screen)
        {
            // Setup your screen elements here
        }
    }
}