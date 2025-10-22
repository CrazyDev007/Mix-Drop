using System.Collections.Generic;
using UnityEngine.UIElements;
using MixDrop.Data;

/// <summary>
/// Interface for the Level Select Screen UI controller
/// </summary>
public interface ILevelSelectScreen
{
    /// <summary>
    /// Initializes the level select screen with required data
    /// </summary>
    /// <param name="progressManager">Progress manager instance</param>
    /// <param name="screenData">Screen configuration data</param>
    /// <param name="transitionManager">Screen transition manager</param>
    /// <param name="navigationController">Navigation controller</param>
    void Initialize(
        IProgressManager progressManager,
        LevelSelectScreenData screenData,
        IScreenTransitionManager transitionManager,
        INavigationController navigationController);
    
    /// <summary>
    /// Shows the level select screen
    /// </summary>
    void Show();
    
    /// <summary>
    /// Hides the level select screen
    /// </summary>
    void Hide();
    
    /// <summary>
    /// Refreshes the level grid with updated data
    /// </summary>
    void RefreshLevelGrid();
    
    /// <summary>
    /// Handles level button click event
    /// </summary>
    /// <param name="levelId">ID of the clicked level</param>
    void OnLevelButtonClicked(int levelId);
    
    /// <summary>
    /// Handles back button click event
    /// </summary>
    void OnBackButtonClicked();
    
    /// <summary>
    /// Gets the root visual element of the screen
    /// </summary>
    /// <returns>Root visual element</returns>
    VisualElement GetRootVisualElement();
    
    /// <summary>
    /// Sets up keyboard navigation for accessibility
    /// </summary>
    void SetupKeyboardNavigation();
    
    /// <summary>
    /// Updates the screen's visual state based on current data
    /// </summary>
    void UpdateVisualState();
}