using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainScreenUI : ScreenUI
    {
        [SerializeField] private GameObject mainScreen;

        public void OnClickBtnStartGame()
        {
            uiManager.ChangeScreen(ScreenType.Level);
        }

        public void OnClickBtnSetting()
        {
            uiManager.ChangeScreen(ScreenType.Setting);
        }

        public override void SetActive(bool active)
        {
            mainScreen.SetActive(active);
        }
    }
}