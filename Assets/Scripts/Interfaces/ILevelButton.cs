using UnityEngine.UIElements;
using MixDrop.Data;

/// <summary>
/// Interface for individual level button components
/// </summary>
public interface ILevelButton
{
    /// <summary>
    /// Initializes the level button with data
    /// </summary>
    /// <param name="buttonData">Data for the level button</param>
    /// <param name="screenData">Screen configuration data</param>
    void Initialize(LevelButtonData buttonData, LevelSelectScreenData screenData);
    
    /// <summary>
    /// Sets up the button's visual appearance based on its state
    /// </summary>
    void SetupVisualAppearance();
    
    /// <summary>
    /// Updates the button's state
    /// </summary>
    /// <param name="buttonData">Updated data for the level button</param>
    void UpdateState(LevelButtonData buttonData);
    
    /// <summary>
    /// Gets the root visual element of the button
    /// </summary>
    /// <returns>Root visual element</returns>
    VisualElement GetRootVisualElement();
    
    /// <summary>
    /// Sets the button's click callback
    /// </summary>
    /// <param name="callback">Callback to invoke when button is clicked</param>
    void SetClickCallback(System.Action<int> callback);
    
    /// <summary>
    /// Sets the button's hover state
    /// </summary>
    /// <param name="isHovered">True if button is being hovered</param>
    void SetHoverState(bool isHovered);
    
    /// <summary>
    /// Sets the button's pressed state
    /// </summary>
    /// <param name="isPressed">True if button is being pressed</param>
    void SetPressedState(bool isPressed);
    
    /// <summary>
    /// Animates the star ratings for the button
    /// </summary>
    /// <param name="delayBetweenStars">Delay between star animations in seconds</param>
    void AnimateStars(float delayBetweenStars = 0.1f);
    
    /// <summary>
    /// Shows or hides the unlock tooltip
    /// </summary>
    /// <param name="show">True to show tooltip, false to hide</param>
    /// <param name="unlockLevelId">ID of the level that must be completed to unlock this level</param>
    void ShowUnlockTooltip(bool show, int unlockLevelId = 0);
    
    /// <summary>
    /// Sets up accessibility attributes for the button
    /// </summary>
    void SetupAccessibility();
    
    /// <summary>
    /// Gets the level ID associated with this button
    /// </summary>
    /// <returns>Level ID</returns>
    int GetLevelId();
    
    /// <summary>
    /// Gets whether the level is unlocked
    /// </summary>
    /// <returns>True if the level is unlocked</returns>
    bool IsUnlocked();
    
    /// <summary>
    /// Gets whether the level is completed
    /// </summary>
    /// <returns>True if the level is completed</returns>
    bool IsCompleted();
    
    /// <summary>
    /// Gets the number of stars earned for the level
    /// </summary>
    /// <returns>Number of stars earned (0-3)</returns>
    int GetStarsEarned();
}