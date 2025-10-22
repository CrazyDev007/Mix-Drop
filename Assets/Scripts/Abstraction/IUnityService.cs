using System;
using UnityEngine;

/// <summary>
/// Interface for Unity-specific services that need to be abstracted
/// </summary>
public interface IUnityService
{
    /// <summary>
    /// Gets the time since the application started
    /// </summary>
    float Time { get; }
    
    /// <summary>
    /// Gets the time scale for the application
    /// </summary>
    float TimeScale { get; set; }
    
    /// <summary>
    /// Gets the delta time for the current frame
    /// </summary>
    float DeltaTime { get; }
    
    /// <summary>
    /// Gets the unscaled delta time for the current frame
    /// </summary>
    float UnscaledDeltaTime { get; }
    
    /// <summary>
    /// Gets the main camera
    /// </summary>
    Camera MainCamera { get; }
    
    /// <summary>
    /// Instantiates a new GameObject
    /// </summary>
    /// <param name="original">The original GameObject to instantiate</param>
    /// <returns>The instantiated GameObject</returns>
    GameObject Instantiate(GameObject original);
    
    /// <summary>
    /// Instantiates a new GameObject with a parent
    /// </summary>
    /// <param name="original">The original GameObject to instantiate</param>
    /// <param name="parent">The parent transform</param>
    /// <returns>The instantiated GameObject</returns>
    GameObject Instantiate(GameObject original, Transform parent);
    
    /// <summary>
    /// Destroys a GameObject
    /// </summary>
    /// <param name="obj">The GameObject to destroy</param>
    void Destroy(GameObject obj);
    
    /// <summary>
    /// Destroys a GameObject after a specified delay
    /// </summary>
    /// <param name="obj">The GameObject to destroy</param>
    /// <param name="delay">The delay in seconds</param>
    void Destroy(GameObject obj, float delay);
    
    /// <summary>
    /// Finds a GameObject by name
    /// </summary>
    /// <param name="name">The name of the GameObject to find</param>
    /// <returns>The found GameObject</returns>
    GameObject Find(string name);
    
    /// <summary>
    /// Loads a scene asynchronously
    /// </summary>
    /// <param name="sceneName">The name of the scene to load</param>
    /// <param name="callback">Callback to invoke when the scene is loaded</param>
    void LoadSceneAsync(string sceneName, Action callback = null);
    
    /// <summary>
    /// Loads a scene
    /// </summary>
    /// <param name="sceneName">The name of the scene to load</param>
    void LoadScene(string sceneName);
    
    /// <summary>
    /// Quits the application
    /// </summary>
    void Quit();
    
    /// <summary>
    /// Logs a message
    /// </summary>
    /// <param name="message">The message to log</param>
    void Log(string message);
    
    /// <summary>
    /// Logs a warning
    /// </summary>
    /// <param name="message">The warning to log</param>
    void LogWarning(string message);
    
    /// <summary>
    /// Logs an error
    /// </summary>
    /// <param name="message">The error to log</param>
    void LogError(string message);
}