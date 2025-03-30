using UnityEngine;

namespace UI
{
    public class SettingScreenUI : ScreenUI
    {
        [SerializeField] private GameObject settingScreen;

        public void OnClickBtnBack()
        {
            uiManager.ChangeScreen(ScreenType.Main);
        }

        public override void SetActive(bool active)
        {
            settingScreen.SetActive(active);
        }
    }
}