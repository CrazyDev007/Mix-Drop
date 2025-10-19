using ScreenFlow;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI
{
    public class LevelFailedScreenUI : ScreenUI
    {
        [SerializeField] private GameObject levelFailedScreen;

        public void OnClickBtnRestart()
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }

        public void OnClickBtnHome()
        {
            SceneManager.LoadSceneAsync("Main");
        }

        protected override void SetupScreen(VisualElement screen)
        {
            // Additional setup can be done here if needed
        }
    }
}
