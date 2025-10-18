using System;

public interface ILevelFailedManager
{
    bool IsLevelFailed { get; }
    string FailureReason { get; }
    void TriggerLevelFailed(string reason = null);
    void RetryLevel();
    void GoToHome();
    event Action<string> OnLevelFailed;
}