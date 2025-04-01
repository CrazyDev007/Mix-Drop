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

        public override void SetActive(bool active)
        {
            gameplayScreen.SetActive(active);
        }
    }
}