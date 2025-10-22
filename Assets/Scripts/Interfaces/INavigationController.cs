using System;

/// <summary>
/// Interface for managing navigation between screens
/// </summary>
public interface INavigationController
{
    /// <summary>
    /// Navigates to the specified screen
    /// </summary>
    /// <param name="screenName">Name of the screen to navigate to</param>
    /// <param name="transitionDuration">Duration of the transition animation</param>
    /// <param name="onNavigationComplete">Callback when navigation is complete</param>
    void NavigateToScreen(string screenName, float transitionDuration, Action onNavigationComplete = null);
    
    /// <summary>
    /// Navigates to the main menu screen
    /// </summary>
    /// <param name="transitionDuration">Duration of the transition animation</param>
    /// <param name="onNavigationComplete">Callback when navigation is complete</param>
    void NavigateToMainMenu(float transitionDuration, Action onNavigationComplete = null);
    
    /// <summary>
    /// Navigates to the specified level
    /// </summary>
    /// <param name="levelId">ID of the level to navigate to</param>
    /// <param name="transitionDuration">Duration of the transition animation</param>
    /// <param name="onNavigationComplete">Callback when navigation is complete</param>
    void NavigateToLevel(int levelId, float transitionDuration, Action onNavigationComplete = null);
    
    /// <summary>
    /// Navigates back to the previous screen
    /// </summary>
    /// <param name="transitionDuration">Duration of the transition animation</param>
    /// <param name="onNavigationComplete">Callback when navigation is complete</param>
    void NavigateBack(float transitionDuration, Action onNavigationComplete = null);
    
    /// <summary>
    /// Gets the name of the current screen
    /// </summary>
    /// <returns>Name of the current screen</returns>
    string GetCurrentScreen();
    
    /// <summary>
    /// Gets the name of the previous screen
    /// </summary>
    /// <returns>Name of the previous screen</returns>
    string GetPreviousScreen();
    
    /// <summary>
    /// Checks if navigation to a specific screen is possible
    /// </summary>
    /// <param name="screenName">Name of the screen to check</param>
    /// <returns>True if navigation is possible</returns>
    bool CanNavigateToScreen(string screenName);
    
    /// <summary>
    /// Checks if navigation back is possible
    /// </summary>
    /// <returns>True if navigation back is possible</returns>
    bool CanNavigateBack();
    
    /// <summary>
    /// Registers a screen with the navigation system
    /// </summary>
    /// <param name="screenName">Name of the screen</param>
    /// <param name="screenInstance">Instance of the screen</param>
    void RegisterScreen(string screenName, object screenInstance);
    
    /// <summary>
    /// Unregisters a screen from the navigation system
    /// </summary>
    /// <param name="screenName">Name of the screen</param>
    void UnregisterScreen(string screenName);
    
    /// <summary>
    /// Sets up keyboard navigation for accessibility
    /// </summary>
    void SetupKeyboardNavigation();
}