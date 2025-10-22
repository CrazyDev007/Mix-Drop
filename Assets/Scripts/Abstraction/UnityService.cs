using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Implementation of the Unity service interface
/// </summary>
public class UnityService : MonoBehaviour, IUnityService
{
    /// <summary>
    /// Gets the time since the application started
    /// </summary>
    public float Time => UnityEngine.Time.time;
    
    /// <summary>
    /// Gets the time scale for the application
    /// </summary>
    public float TimeScale
    {
        get => UnityEngine.Time.timeScale;
        set => UnityEngine.Time.timeScale = value;
    }
    
    /// <summary>
    /// Gets the delta time for the current frame
    /// </summary>
    public float DeltaTime => UnityEngine.Time.deltaTime;
    
    /// <summary>
    /// Gets the unscaled delta time for the current frame
    /// </summary>
    public float UnscaledDeltaTime => UnityEngine.Time.unscaledDeltaTime;
    
    /// <summary>
    /// Gets the main camera
    /// </summary>
    public Camera MainCamera => Camera.main;
    
    /// <summary>
    /// Instantiates a new GameObject
    /// </summary>
    /// <param name="original">The original GameObject to instantiate</param>
    /// <returns>The instantiated GameObject</returns>
    public GameObject Instantiate(GameObject original)
    {
        return UnityEngine.Object.Instantiate(original);
    }
    
    /// <summary>
    /// Instantiates a new GameObject with a parent
    /// </summary>
    /// <param name="original">The original GameObject to instantiate</param>
    /// <param name="parent">The parent transform</param>
    /// <returns>The instantiated GameObject</returns>
    public GameObject Instantiate(GameObject original, Transform parent)
    {
        return UnityEngine.Object.Instantiate(original, parent);
    }
    
    /// <summary>
    /// Destroys a GameObject
    /// </summary>
    /// <param name="obj">The GameObject to destroy</param>
    public void Destroy(GameObject obj)
    {
        UnityEngine.Object.Destroy(obj);
    }
    
    /// <summary>
    /// Destroys a GameObject after a specified delay
    /// </summary>
    /// <param name="obj">The GameObject to destroy</param>
    /// <param name="delay">The delay in seconds</param>
    public void Destroy(GameObject obj, float delay)
    {
        UnityEngine.Object.Destroy(obj, delay);
    }
    
    /// <summary>
    /// Finds a GameObject by name
    /// </summary>
    /// <param name="name">The name of the GameObject to find</param>
    /// <returns>The found GameObject</returns>
    public GameObject Find(string name)
    {
        return GameObject.Find(name);
    }
    
    /// <summary>
    /// Loads a scene asynchronously
    /// </summary>
    /// <param name="sceneName">The name of the scene to load</param>
    /// <param name="callback">Callback to invoke when the scene is loaded</param>
    public void LoadSceneAsync(string sceneName, Action callback = null)
    {
        StartCoroutine(LoadSceneAsyncRoutine(sceneName, callback));
    }
    
    /// <summary>
    /// Loads a scene
    /// </summary>
    /// <param name="sceneName">The name of the scene to load</param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    /// <summary>
    /// Quits the application
    /// </summary>
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    /// <summary>
    /// Logs a message
    /// </summary>
    /// <param name="message">The message to log</param>
    public void Log(string message)
    {
        Debug.Log(message);
    }
    
    /// <summary>
    /// Logs a warning
    /// </summary>
    /// <param name="message">The warning to log</param>
    public void LogWarning(string message)
    {
        Debug.LogWarning(message);
    }
    
    /// <summary>
    /// Logs an error
    /// </summary>
    /// <param name="message">The error to log</param>
    public void LogError(string message)
    {
        Debug.LogError(message);
    }
    
    /// <summary>
    /// Coroutine for loading a scene asynchronously
    /// </summary>
    /// <param name="sceneName">The name of the scene to load</param>
    /// <param name="callback">Callback to invoke when the scene is loaded</param>
    /// <returns>IEnumerator for the coroutine</returns>
    private IEnumerator LoadSceneAsyncRoutine(string sceneName, Action callback)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        callback?.Invoke();
    }
}