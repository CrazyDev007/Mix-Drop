using UnityEngine;
using System;
using System.Reflection;
using CrazyDev007.LevelEditor.Abstraction;

/// <summary>
/// MonoBehaviour that initializes the dependency injection container
/// </summary>
public class DependencyInjectionBootstrap : MonoBehaviour
{
    [Tooltip("Whether to destroy this object on scene load")]
    [SerializeField]
    private bool _dontDestroyOnLoad = true;
    
    [Tooltip("Whether to initialize on awake")]
    [SerializeField]
    private bool _initializeOnAwake = true;

    /// <summary>
    /// Awake is called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        if (_dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        if (_initializeOnAwake)
        {
            Initialize();
        }
    }

    /// <summary>
    /// Initializes the dependency injection container
    /// </summary>
    public void Initialize()
    {
        var container = new DependencyContainer();
        ServiceLocator.Initialize(container);
        
        // Register core services here
        RegisterCoreServices(container);
        
        Debug.Log("Dependency injection container initialized successfully");
    }

    /// <summary>
    /// Registers core services with the dependency container
    /// </summary>
    /// <param name="container">The dependency container</param>
    private void RegisterCoreServices(IDependencyContainer container)
    {
        // Create and register the ServiceManager
        var serviceManagerGameObject = new GameObject("ServiceManager");
        
        // Get the ServiceManager type using reflection
        Type serviceManagerType = Type.GetType("ServiceManager, Assembly-CSharp");
        if (serviceManagerType == null)
        {
            // Try alternative type name
            serviceManagerType = Type.GetType("Abstraction.ServiceManager, Assembly-CSharp");
        }
        
        if (serviceManagerType != null)
        {
            var serviceManager = serviceManagerGameObject.AddComponent(serviceManagerType);
            
            // Get the RegisterServices method and invoke it
            MethodInfo registerServicesMethod = serviceManagerType.GetMethod("RegisterServices");
            if (registerServicesMethod != null)
            {
                registerServicesMethod.Invoke(serviceManager, new object[] { container });
            }
            
            // Register the ServiceManager instance
            MethodInfo registerInstanceMethod = container.GetType().GetMethod("RegisterInstance");
            MethodInfo genericRegisterInstanceMethod = registerInstanceMethod.MakeGenericMethod(serviceManagerType);
            genericRegisterInstanceMethod.Invoke(container, new object[] { serviceManager });
        }
        else
        {
            Debug.LogError("ServiceManager type not found. Abstraction services will not be registered.");
        }
        
        // Register localization service
        container.Register<ILocalizationService, LocalizationService>();

        // This will be populated with actual service registrations
        // when the implementations are created

        // Example:
        // container.Register<INavigationController, NavigationController>();
        // container.Register<IProgressManager, ProgressManager>();
        // container.Register<IScreenTransitionManager, ScreenTransitionManager>();
    }
}