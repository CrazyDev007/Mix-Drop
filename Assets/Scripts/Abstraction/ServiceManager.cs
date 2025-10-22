using System;
using UnityEngine;

/// <summary>
/// Manages all abstraction services and provides a centralized point of access
/// </summary>
public class ServiceManager : MonoBehaviour
{
    private static ServiceManager _instance;
    
    private IUnityService _unityService;
    private IUIService _uiService;
    private IAudioService _audioService;
    private IInputService _inputService;
    private IDataService _dataService;
    private IDataContext _dataContext;
    
    /// <summary>
    /// Gets the singleton instance of the ServiceManager
    /// </summary>
    public static ServiceManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("ServiceManager");
                _instance = go.AddComponent<ServiceManager>();
                DontDestroyOnLoad(go);
            }
            
            return _instance;
        }
    }
    
    /// <summary>
    /// Gets the Unity service
    /// </summary>
    public IUnityService UnityService
    {
        get
        {
            if (_unityService == null)
            {
                _unityService = GetComponent<IUnityService>();
                if (_unityService == null)
                {
                    _unityService = gameObject.AddComponent<UnityService>();
                }
            }
            
            return _unityService;
        }
    }
    
    /// <summary>
    /// Gets the UI service
    /// </summary>
    public IUIService UIService
    {
        get
        {
            if (_uiService == null)
            {
                _uiService = GetComponent<IUIService>();
                if (_uiService == null)
                {
                    _uiService = gameObject.AddComponent<UIService>();
                }
            }
            
            return _uiService;
        }
    }
    
    /// <summary>
    /// Gets the audio service
    /// </summary>
    public IAudioService AudioService
    {
        get
        {
            if (_audioService == null)
            {
                _audioService = GetComponent<IAudioService>();
                if (_audioService == null)
                {
                    _audioService = gameObject.AddComponent<AudioService>();
                }
            }
            
            return _audioService;
        }
    }
    
    /// <summary>
    /// Gets the input service
    /// </summary>
    public IInputService InputService
    {
        get
        {
            if (_inputService == null)
            {
                _inputService = GetComponent<IInputService>();
                if (_inputService == null)
                {
                    _inputService = gameObject.AddComponent<InputService>();
                }
            }

            return _inputService;
        }
    }

    /// <summary>
    /// Gets the data service
    /// </summary>
    public IDataService DataService
    {
        get
        {
            if (_dataService == null)
            {
                var levelRepository = new LevelRepository();
                var progressRepository = new ProgressRepository();
                _dataService = new DataService(levelRepository, progressRepository);
            }

            return _dataService;
        }
    }

    /// <summary>
    /// Gets the data context
    /// </summary>
    public IDataContext DataContext
    {
        get
        {
            if (_dataContext == null)
            {
                _dataContext = new DataContext(DataService);
            }

            return _dataContext;
        }
    }
    
    /// <summary>
    /// Awake is called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeServices();
    }
    
    /// <summary>
    /// Initializes all services
    /// </summary>
    private void InitializeServices()
    {
        // Ensure all services are initialized
        _ = UnityService;
        _ = UIService;
        _ = AudioService;
        _ = InputService;
    }
    
    /// <summary>
    /// Registers all services with the dependency container
    /// </summary>
    /// <param name="container">The dependency container to register with</param>
    public void RegisterServices(IDependencyContainer container)
    {
        if (container == null)
        {
            throw new ArgumentNullException(nameof(container));
        }
        
        // Register services as singletons
        container.Register<IUnityService>(container => UnityService, ServiceLifetime.Singleton);
        container.Register<IUIService>(container => UIService, ServiceLifetime.Singleton);
        container.Register<IAudioService>(container => AudioService, ServiceLifetime.Singleton);
        container.Register<IInputService>(container => InputService, ServiceLifetime.Singleton);

        // Register data services
        container.Register<IDataService>(container => DataService, ServiceLifetime.Singleton);
        container.Register<IDataContext>(container => DataContext, ServiceLifetime.Singleton);
        container.Register<ILevelRepository>(container => DataService.LevelRepository, ServiceLifetime.Singleton);
        container.Register<IProgressRepository>(container => DataService.ProgressRepository, ServiceLifetime.Singleton);
    }
    
    /// <summary>
    /// Sets a custom Unity service implementation
    /// </summary>
    /// <param name="service">The custom Unity service implementation</param>
    public void SetUnityService(IUnityService service)
    {
        _unityService = service ?? throw new ArgumentNullException(nameof(service));
    }
    
    /// <summary>
    /// Sets a custom UI service implementation
    /// </summary>
    /// <param name="service">The custom UI service implementation</param>
    public void SetUIService(IUIService service)
    {
        _uiService = service ?? throw new ArgumentNullException(nameof(service));
    }
    
    /// <summary>
    /// Sets a custom audio service implementation
    /// </summary>
    /// <param name="service">The custom audio service implementation</param>
    public void SetAudioService(IAudioService service)
    {
        _audioService = service ?? throw new ArgumentNullException(nameof(service));
    }
    
    /// <summary>
    /// Sets a custom input service implementation
    /// </summary>
    /// <param name="service">The custom input service implementation</param>
    public void SetInputService(IInputService service)
    {
        _inputService = service ?? throw new ArgumentNullException(nameof(service));
    }
}