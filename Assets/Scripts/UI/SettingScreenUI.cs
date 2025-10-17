using ScreenFlow;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class SettingScreenUI : ScreenUI
    {
        [SerializeField] private GameObject settingScreen;

        public void OnClickBtnBack()
        {
            //uiManager.ChangeScreen(ScreenType.Main);
        }

        protected override void SetupScreen(VisualElement screen)
        {
            throw new System.NotImplementedException();
        }
    }
}