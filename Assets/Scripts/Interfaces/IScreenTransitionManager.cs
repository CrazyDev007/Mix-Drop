using System;
using UnityEngine;

/// <summary>
/// Interface for managing screen transition animations
/// </summary>
public interface IScreenTransitionManager
{
    /// <summary>
    /// Transitions to the specified level scene
    /// </summary>
    /// <param name="sceneName">Name of the scene to transition to</param>
    /// <param name="transitionDuration">Duration of the transition animation</param>
    /// <param name="onTransitionComplete">Callback when transition is complete</param>
    void TransitionToLevel(string sceneName, float transitionDuration, Action onTransitionComplete = null);
    
    /// <summary>
    /// Transitions back to the main menu
    /// </summary>
    /// <param name="transitionDuration">Duration of the transition animation</param>
    /// <param name="onTransitionComplete">Callback when transition is complete</param>
    void TransitionToMainMenu(float transitionDuration, Action onTransitionComplete = null);
    
    /// <summary>
    /// Plays a fade transition animation
    /// </summary>
    /// <param name="fadeIn">True to fade in, false to fade out</param>
    /// <param name="duration">Duration of the fade animation</param>
    /// <param name="onComplete">Callback when animation is complete</param>
    void PlayFadeTransition(bool fadeIn, float duration, Action onComplete = null);
    
    /// <summary>
    /// Plays a slide transition animation
    /// </summary>
    /// <param name="direction">Direction of the slide</param>
    /// <param name="duration">Duration of the slide animation</param>
    /// <param name="onComplete">Callback when animation is complete</param>
    void PlaySlideTransition(SlideDirection direction, float duration, Action onComplete = null);
    
    /// <summary>
    /// Checks if a transition is currently in progress
    /// </summary>
    /// <returns>True if a transition is in progress</returns>
    bool IsTransitionInProgress();
    
    /// <summary>
    /// Cancels any ongoing transition
    /// </summary>
    void CancelTransition();
}

/// <summary>
/// Enumeration for slide transition directions
/// </summary>
public enum SlideDirection
{
    Left,
    Right,
    Up,
    Down
}