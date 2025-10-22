using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

/// <summary>
/// Interface for input-specific Unity services that need to be abstracted
/// </summary>
public interface IInputService
{
    /// <summary>
    /// Event that is triggered when a button is pressed
    /// </summary>
    event Action<string> ButtonPressed;
    
    /// <summary>
    /// Event that is triggered when a button is released
    /// </summary>
    event Action<string> ButtonReleased;
    
    /// <summary>
    /// Event that is triggered when the mouse is moved
    /// </summary>
    event Action<Vector2> MouseMoved;
    
    /// <summary>
    /// Event that is triggered when the mouse is clicked
    /// </summary>
    event Action<Vector2> MouseClicked;
    
    /// <summary>
    /// Event that is triggered when the touch screen is touched
    /// </summary>
    event Action<Vector2> TouchStarted;
    
    /// <summary>
    /// Event that is triggered when the touch screen is moved
    /// </summary>
    event Action<Vector2> TouchMoved;
    
    /// <summary>
    /// Event that is triggered when the touch screen is released
    /// </summary>
    event Action<Vector2> TouchEnded;
    
    /// <summary>
    /// Gets the current mouse position
    /// </summary>
    Vector2 MousePosition { get; }
    
    /// <summary>
    /// Gets the current touch position
    /// </summary>
    Vector2 TouchPosition { get; }
    
    /// <summary>
    /// Gets whether a button is currently pressed
    /// </summary>
    /// <param name="buttonName">The name of the button</param>
    /// <returns>True if the button is pressed</returns>
    bool IsButtonPressed(string buttonName);
    
    /// <summary>
    /// Gets whether a button was just pressed this frame
    /// </summary>
    /// <param name="buttonName">The name of the button</param>
    /// <returns>True if the button was just pressed</returns>
    bool IsButtonJustPressed(string buttonName);
    
    /// <summary>
    /// Gets whether a button was just released this frame
    /// </summary>
    /// <param name="buttonName">The name of the button</param>
    /// <returns>True if the button was just released</returns>
    bool IsButtonJustReleased(string buttonName);
    
    /// <summary>
    /// Gets whether the mouse button is currently pressed
    /// </summary>
    /// <param name="button">The mouse button to check</param>
    /// <returns>True if the mouse button is pressed</returns>
    bool IsMouseButtonPressed(int button);
    
    /// <summary>
    /// Gets whether the mouse button was just pressed this frame
    /// </summary>
    /// <param name="button">The mouse button to check</param>
    /// <returns>True if the mouse button was just pressed</returns>
    bool IsMouseButtonJustPressed(int button);
    
    /// <summary>
    /// Gets whether the mouse button was just released this frame
    /// </summary>
    /// <param name="button">The mouse button to check</param>
    /// <returns>True if the mouse button was just released</returns>
    bool IsMouseButtonJustReleased(int button);
    
    /// <summary>
    /// Gets whether the screen is currently being touched
    /// </summary>
    /// <returns>True if the screen is being touched</returns>
    bool IsTouching();
    
    /// <summary>
    /// Gets whether the screen was just touched this frame
    /// </summary>
    /// <returns>True if the screen was just touched</returns>
    bool IsTouchJustStarted();
    
    /// <summary>
    /// Gets whether the screen touch just ended this frame
    /// </summary>
    /// <returns>True if the screen touch just ended</returns>
    bool IsTouchJustEnded();
    
    /// <summary>
    /// Gets the value of an axis
    /// </summary>
    /// <param name="axisName">The name of the axis</param>
    /// <returns>The value of the axis</returns>
    float GetAxis(string axisName);
    
    /// <summary>
    /// Gets the raw value of an axis
    /// </summary>
    /// <param name="axisName">The name of the axis</param>
    /// <returns>The raw value of the axis</returns>
    float GetAxisRaw(string axisName);
    
    /// <summary>
    /// Enables or disables input processing
    /// </summary>
    /// <param name="enabled">Whether to enable input processing</param>
    void SetInputEnabled(bool enabled);
    
    /// <summary>
    /// Gets whether input processing is enabled
    /// </summary>
    /// <returns>True if input processing is enabled</returns>
    bool IsInputEnabled();
    
    /// <summary>
    /// Enables or disables a specific input action
    /// </summary>
    /// <param name="actionName">The name of the input action</param>
    /// <param name="enabled">Whether to enable the input action</param>
    void SetInputActionEnabled(string actionName, bool enabled);
    
    /// <summary>
    /// Gets whether a specific input action is enabled
    /// </summary>
    /// <param name="actionName">The name of the input action</param>
    /// <returns>True if the input action is enabled</returns>
    bool IsInputActionEnabled(string actionName);
    
    /// <summary>
    /// Registers a callback for an input action
    /// </summary>
    /// <param name="actionName">The name of the input action</param>
    /// <param name="callback">The callback to register</param>
    void RegisterInputActionCallback(string actionName, Action<InputAction.CallbackContext> callback);
    
    /// <summary>
    /// Unregisters a callback for an input action
    /// </summary>
    /// <param name="actionName">The name of the input action</param>
    /// <param name="callback">The callback to unregister</param>
    void UnregisterInputActionCallback(string actionName, Action<InputAction.CallbackContext> callback);
}