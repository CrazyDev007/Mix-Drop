using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;

/// <summary>
/// Implementation of the input service interface
/// </summary>
public class InputService : MonoBehaviour, IInputService
{
    [SerializeField]
    private InputActionAsset _inputActions;
    
    private Dictionary<string, InputAction> _inputActionMap = new Dictionary<string, InputAction>();
    private Dictionary<string, List<Action<InputAction.CallbackContext>>> _inputActionCallbacks = new Dictionary<string, List<Action<InputAction.CallbackContext>>>();
    
    private bool _inputEnabled = true;
    private Mouse _mouse;
    private Touchscreen _touchscreen;
    
    /// <summary>
    /// Event that is triggered when a button is pressed
    /// </summary>
    public event Action<string> ButtonPressed;
    
    /// <summary>
    /// Event that is triggered when a button is released
    /// </summary>
    public event Action<string> ButtonReleased;
    
    /// <summary>
    /// Event that is triggered when the mouse is moved
    /// </summary>
    public event Action<Vector2> MouseMoved;
    
    /// <summary>
    /// Event that is triggered when the mouse is clicked
    /// </summary>
    public event Action<Vector2> MouseClicked;
    
    /// <summary>
    /// Event that is triggered when the touch screen is touched
    /// </summary>
    public event Action<Vector2> TouchStarted;
    
    /// <summary>
    /// Event that is triggered when the touch screen is moved
    /// </summary>
    public event Action<Vector2> TouchMoved;
    
    /// <summary>
    /// Event that is triggered when the touch screen is released
    /// </summary>
    public event Action<Vector2> TouchEnded;
    
    /// <summary>
    /// Gets the current mouse position
    /// </summary>
    public Vector2 MousePosition => _mouse != null ? _mouse.position.ReadValue() : Vector2.zero;
    
    /// <summary>
    /// Gets the current touch position
    /// </summary>
    public Vector2 TouchPosition => _touchscreen != null && _touchscreen.touches.Count > 0 ? _touchscreen.touches[0].position.ReadValue() : Vector2.zero;
    
    /// <summary>
    /// Awake is called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        InitializeInputSystem();
    }
    
    /// <summary>
    /// Initializes the input system
    /// </summary>
    private void InitializeInputSystem()
    {
        // Get input devices
        _mouse = Mouse.current;
        _touchscreen = Touchscreen.current;
        
        // Set up input actions
        if (_inputActions != null)
        {
            foreach (InputActionMap actionMap in _inputActions.actionMaps)
            {
                foreach (InputAction action in actionMap)
                {
                    string actionName = action.name;
                    _inputActionMap[actionName] = action;
                    _inputActionCallbacks[actionName] = new List<Action<InputAction.CallbackContext>>();
                    
                    // Add default callbacks
                    action.performed += context => OnInputActionPerformed(actionName, context);
                    action.canceled += context => OnInputActionCanceled(actionName, context);
                }
            }
            
            _inputActions.Enable();
        }
    }
    
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        if (!_inputEnabled)
        {
            return;
        }
        
        // Handle mouse movement
        if (_mouse != null && _mouse.delta.magnitude > 0)
        {
            MouseMoved?.Invoke(_mouse.position.ReadValue());
        }
        
        // Handle mouse clicks
        if (_mouse != null)
        {
            if (_mouse.leftButton.wasPressedThisFrame)
            {
                MouseClicked?.Invoke(_mouse.position.ReadValue());
            }
        }
        
        // Handle touch input
        if (_touchscreen != null && _touchscreen.touches.Count > 0)
        {
            TouchControl touch = _touchscreen.touches[0];
            
            if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
            {
                TouchStarted?.Invoke(touch.position.ReadValue());
            }
            else if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Ended || touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Canceled)
            {
                TouchEnded?.Invoke(touch.position.ReadValue());
            }
            else if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                TouchMoved?.Invoke(touch.position.ReadValue());
            }
        }
    }
    
    /// <summary>
    /// Called when an input action is performed
    /// </summary>
    /// <param name="actionName">The name of the input action</param>
    /// <param name="context">The callback context</param>
    private void OnInputActionPerformed(string actionName, InputAction.CallbackContext context)
    {
        if (!_inputEnabled)
        {
            return;
        }
        
        ButtonPressed?.Invoke(actionName);
        
        // Invoke registered callbacks
        if (_inputActionCallbacks.TryGetValue(actionName, out var callbacks))
        {
            foreach (var callback in callbacks)
            {
                callback?.Invoke(context);
            }
        }
    }
    
    /// <summary>
    /// Called when an input action is canceled
    /// </summary>
    /// <param name="actionName">The name of the input action</param>
    /// <param name="context">The callback context</param>
    private void OnInputActionCanceled(string actionName, InputAction.CallbackContext context)
    {
        if (!_inputEnabled)
        {
            return;
        }
        
        ButtonReleased?.Invoke(actionName);
    }
    
    /// <summary>
    /// Gets whether a button is currently pressed
    /// </summary>
    /// <param name="buttonName">The name of the button</param>
    /// <returns>True if the button is pressed</returns>
    public bool IsButtonPressed(string buttonName)
    {
        if (!_inputEnabled || !_inputActionMap.TryGetValue(buttonName, out var action))
        {
            return false;
        }
        
        return action.IsPressed();
    }
    
    /// <summary>
    /// Gets whether a button was just pressed this frame
    /// </summary>
    /// <param name="buttonName">The name of the button</param>
    /// <returns>True if the button was just pressed</returns>
    public bool IsButtonJustPressed(string buttonName)
    {
        if (!_inputEnabled || !_inputActionMap.TryGetValue(buttonName, out var action))
        {
            return false;
        }
        
        return action.triggered && action.ReadValue<float>() > 0;
    }
    
    /// <summary>
    /// Gets whether a button was just released this frame
    /// </summary>
    /// <param name="buttonName">The name of the button</param>
    /// <returns>True if the button was just released</returns>
    public bool IsButtonJustReleased(string buttonName)
    {
        if (!_inputEnabled || !_inputActionMap.TryGetValue(buttonName, out var action))
        {
            return false;
        }
        
        return action.triggered && action.ReadValue<float>() == 0;
    }
    
    /// <summary>
    /// Gets whether the mouse button is currently pressed
    /// </summary>
    /// <param name="button">The mouse button to check</param>
    /// <returns>True if the mouse button is pressed</returns>
    public bool IsMouseButtonPressed(int button)
    {
        if (!_inputEnabled || _mouse == null)
        {
            return false;
        }
        
        switch (button)
        {
            case 0: return _mouse.leftButton.isPressed;
            case 1: return _mouse.rightButton.isPressed;
            case 2: return _mouse.middleButton.isPressed;
            default: return false;
        }
    }
    
    /// <summary>
    /// Gets whether the mouse button was just pressed this frame
    /// </summary>
    /// <param name="button">The mouse button to check</param>
    /// <returns>True if the mouse button was just pressed</returns>
    public bool IsMouseButtonJustPressed(int button)
    {
        if (!_inputEnabled || _mouse == null)
        {
            return false;
        }
        
        switch (button)
        {
            case 0: return _mouse.leftButton.wasPressedThisFrame;
            case 1: return _mouse.rightButton.wasPressedThisFrame;
            case 2: return _mouse.middleButton.wasPressedThisFrame;
            default: return false;
        }
    }
    
    /// <summary>
    /// Gets whether the mouse button was just released this frame
    /// </summary>
    /// <param name="button">The mouse button to check</param>
    /// <returns>True if the mouse button was just released</returns>
    public bool IsMouseButtonJustReleased(int button)
    {
        if (!_inputEnabled || _mouse == null)
        {
            return false;
        }
        
        switch (button)
        {
            case 0: return _mouse.leftButton.wasReleasedThisFrame;
            case 1: return _mouse.rightButton.wasReleasedThisFrame;
            case 2: return _mouse.middleButton.wasReleasedThisFrame;
            default: return false;
        }
    }
    
    /// <summary>
    /// Gets whether the screen is currently being touched
    /// </summary>
    /// <returns>True if the screen is being touched</returns>
    public bool IsTouching()
    {
        if (!_inputEnabled || _touchscreen == null)
        {
            return false;
        }
        
        return _touchscreen.touches.Count > 0 && _touchscreen.touches[0].press.isPressed;
    }
    
    /// <summary>
    /// Gets whether the screen was just touched this frame
    /// </summary>
    /// <returns>True if the screen was just touched</returns>
    public bool IsTouchJustStarted()
    {
        if (!_inputEnabled || _touchscreen == null)
        {
            return false;
        }
        
        return _touchscreen.touches.Count > 0 && _touchscreen.touches[0].phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began;
    }
    
    /// <summary>
    /// Gets whether the screen touch just ended this frame
    /// </summary>
    /// <returns>True if the screen touch just ended</returns>
    public bool IsTouchJustEnded()
    {
        if (!_inputEnabled || _touchscreen == null)
        {
            return false;
        }
        
        return _touchscreen.touches.Count > 0 && (_touchscreen.touches[0].phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Ended || _touchscreen.touches[0].phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Canceled);
    }
    
    /// <summary>
    /// Gets the value of an axis
    /// </summary>
    /// <param name="axisName">The name of the axis</param>
    /// <returns>The value of the axis</returns>
    public float GetAxis(string axisName)
    {
        if (!_inputEnabled || !_inputActionMap.TryGetValue(axisName, out var action))
        {
            return 0f;
        }
        
        return action.ReadValue<float>();
    }
    
    /// <summary>
    /// Gets the raw value of an axis
    /// </summary>
    /// <param name="axisName">The name of the axis</param>
    /// <returns>The raw value of the axis</returns>
    public float GetAxisRaw(string axisName)
    {
        if (!_inputEnabled || !_inputActionMap.TryGetValue(axisName, out var action))
        {
            return 0f;
        }
        
        float value = action.ReadValue<float>();
        return value > 0f ? 1f : (value < 0f ? -1f : 0f);
    }
    
    /// <summary>
    /// Enables or disables input processing
    /// </summary>
    /// <param name="enabled">Whether to enable input processing</param>
    public void SetInputEnabled(bool enabled)
    {
        _inputEnabled = enabled;
        
        if (_inputActions != null)
        {
            if (enabled)
            {
                _inputActions.Enable();
            }
            else
            {
                _inputActions.Disable();
            }
        }
    }
    
    /// <summary>
    /// Gets whether input processing is enabled
    /// </summary>
    /// <returns>True if input processing is enabled</returns>
    public bool IsInputEnabled()
    {
        return _inputEnabled;
    }
    
    /// <summary>
    /// Enables or disables a specific input action
    /// </summary>
    /// <param name="actionName">The name of the input action</param>
    /// <param name="enabled">Whether to enable the input action</param>
    public void SetInputActionEnabled(string actionName, bool enabled)
    {
        if (_inputActionMap.TryGetValue(actionName, out var action))
        {
            if (enabled)
            {
                action.Enable();
            }
            else
            {
                action.Disable();
            }
        }
    }
    
    /// <summary>
    /// Gets whether a specific input action is enabled
    /// </summary>
    /// <param name="actionName">The name of the input action</param>
    /// <returns>True if the input action is enabled</returns>
    public bool IsInputActionEnabled(string actionName)
    {
        if (_inputActionMap.TryGetValue(actionName, out var action))
        {
            return action.enabled;
        }
        
        return false;
    }
    
    /// <summary>
    /// Registers a callback for an input action
    /// </summary>
    /// <param name="actionName">The name of the input action</param>
    /// <param name="callback">The callback to register</param>
    public void RegisterInputActionCallback(string actionName, Action<InputAction.CallbackContext> callback)
    {
        if (callback == null)
        {
            return;
        }
        
        if (!_inputActionCallbacks.TryGetValue(actionName, out var callbacks))
        {
            callbacks = new List<Action<InputAction.CallbackContext>>();
            _inputActionCallbacks[actionName] = callbacks;
        }
        
        callbacks.Add(callback);
    }
    
    /// <summary>
    /// Unregisters a callback for an input action
    /// </summary>
    /// <param name="actionName">The name of the input action</param>
    /// <param name="callback">The callback to unregister</param>
    public void UnregisterInputActionCallback(string actionName, Action<InputAction.CallbackContext> callback)
    {
        if (callback == null || !_inputActionCallbacks.TryGetValue(actionName, out var callbacks))
        {
            return;
        }
        
        callbacks.Remove(callback);
    }
}