using ScreenFlow;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI
{
    public class PauseScreen : ScreenUI
    {

        public void OnClickBtnResume()
        {
            ScreenManager.Instance.ShowScreen("gameplay-screen");
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
            screen.Q<Button>("ResumeButton").clicked += OnClickBtnResume;
            screen.Q<Button>("RestartButton").clicked += OnClickBtnRestart;
            screen.Q<Button>("HomeButton").clicked += OnClickBtnHome;
        }
    }
}