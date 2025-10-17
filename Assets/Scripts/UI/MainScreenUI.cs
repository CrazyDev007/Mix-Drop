using ScreenFlow;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI
{
    public class MainScreenUI : ScreenUI
    {
        [SerializeField] private GameObject mainScreen;

        public void OnClickBtnStartGame()
        {
            //uiManager.ChangeScreen(ScreenType.Level);
        }

        public void OnClickBtnSetting()
        {
            //uiManager.ChangeScreen(ScreenType.Setting);
        }

        protected override void SetupScreen(VisualElement screen)
        {
            throw new System.NotImplementedException();
        }
    }
}