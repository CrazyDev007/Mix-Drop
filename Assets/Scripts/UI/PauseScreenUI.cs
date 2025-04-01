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
            // Restart the current level/game
            // Reset timescale in case it was paused
            Time.timeScale = 1f;

            // You might want to use a scene manager or game manager to handle the restart
            // For example: GameManager.Instance.RestartGame();

            // For now, we can reload the current scene
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex);
        }

        public void OnClickBtnHome()
        {
            // Return to the home/main menu
            // Reset timescale in case it was paused
            Time.timeScale = 1f;

            // You might want to load the main menu scene
            // For example: SceneManager.LoadScene("MainMenu");

            // Or use a game manager to handle the transition
            // For example: GameManager.Instance.ReturnToMainMenu();
        }

        public override void SetActive(bool active)
        {
            pauseScreen.SetActive(active);
        }
    }
}