using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseScreenUI : ScreenUI
    {
        [SerializeField] private GameObject pauseScreen;

        public void OnClickBtnResume()
        {
            uiManager.ChangeScreen(ScreenType.Gameplay);
        }

        public void OnClickBtnRestart()
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }

        public void OnClickBtnHome()
        {
            SceneManager.LoadSceneAsync("Main");
        }

        public override void SetActive(bool active)
        {
            pauseScreen.SetActive(active);
        }
    }
}