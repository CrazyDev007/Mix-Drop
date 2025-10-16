using UnityEngine;

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
            uiManager.ChangeScreen(ScreenType.Pause);
        }
        public void OnLevelFailed()
        {
            uiManager.ChangeScreen(ScreenType.LevelFailed);
        }
        public override void SetActive(bool active)
        {
            gameplayScreen.SetActive(active);
        }
    }
}