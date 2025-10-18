using ScreenFlow;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI
{
    public class PauseScreen : ScreenUI
    {
        [SerializeField] private GameObject pauseScreen;

        public void OnClickBtnResume()
        {
            //uiManager.ChangeScreen(ScreenType.Gameplay);
        }

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
            // Initialize UI elements here
        }
    }
}