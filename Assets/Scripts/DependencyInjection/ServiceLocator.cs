using UnityEngine;

/// <summary>
/// Static class providing global access to the dependency container
/// </summary>
public static class ServiceLocator
{
    private static IDependencyContainer _container;

    /// <summary>
    /// Gets the current dependency container
    /// </summary>
    public static IDependencyContainer Container
    {
        get
        {
            if (_container == null)
            {
                Debug.LogError("Dependency container has not been initialized. Call Initialize() first.");
                return null;
            }
            return _container;
        }
    }

    /// <summary>
    /// Initializes the service locator with a dependency container
    /// </summary>
    /// <param name="container">The dependency container to use</param>
    public static void Initialize(IDependencyContainer container)
    {
        if (_container != null)
        {
            Debug.LogWarning("Dependency container is already initialized. Clearing existing registrations.");
            _container.Clear();
        }
        
        _container = container;
    }

    /// <summary>
    /// Resolves an instance of the specified type
    /// </summary>
    /// <typeparam name="T">Type to resolve</typeparam>
    /// <returns>Resolved instance</returns>
    public static T Resolve<T>() where T : class
    {
        return Container.Resolve<T>();
    }

    /// <summary>
    /// Checks if a type is registered
    /// </summary>
    /// <typeparam name="T">Type to check</typeparam>
    /// <returns>True if the type is registered</returns>
    public static bool IsRegistered<T>() where T : class
    {
        return Container.IsRegistered<T>();
    }

    /// <summary>
    /// Clears all registered services
    /// </summary>
    public static void Clear()
    {
        _container?.Clear();
    }
}