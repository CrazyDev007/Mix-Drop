// IPauseManager.cs
// Contract for pause functionality

using System;

public interface IPauseManager
{
    bool IsPaused { get; }
    void PauseGame();
    void ResumeGame();
    void RestartLevel();
    void GoToHome();
    event Action<bool> OnPauseStateChanged;
}