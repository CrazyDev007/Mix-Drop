using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour, IPauseManager
{
    public bool IsPaused { get; private set; }
    public event Action<bool> OnPauseStateChanged;

    private void Update()
    {
        // Handle pause input (Escape key)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (IsPaused) return;

        IsPaused = true;
        Time.timeScale = 0f;
        OnPauseStateChanged?.Invoke(true);
        // Show pause screen - handled by UI controller
    }

    public void ResumeGame()
    {
        if (!IsPaused) return;

        IsPaused = false;
        Time.timeScale = 1f;
        OnPauseStateChanged?.Invoke(false);
        // Hide pause screen - handled by UI controller
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // Reset time scale before scene change
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToHome()
    {
        Time.timeScale = 1f; // Reset time scale before scene change
        SceneManager.LoadScene("Home"); // Assuming "Home" scene exists
    }
}