using UnityEngine;
using UnityEngine.SceneManagement;

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

        public override void SetActive(bool active)
        {
            levelFailedScreen.SetActive(active);
        }

    }
}
