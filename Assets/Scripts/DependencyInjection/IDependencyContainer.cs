using System;

/// <summary>
/// Interface for the dependency injection container
/// </summary>
public interface IDependencyContainer
{
    /// <summary>
    /// Registers a type with its implementation
    /// </summary>
    /// <typeparam name="TInterface">Interface type</typeparam>
    /// <typeparam name="TImplementation">Implementation type</typeparam>
    /// <param name="lifetime">Lifetime of the registered service</param>
    void Register<TInterface, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TInterface : class
        where TImplementation : class, TInterface;
    
    /// <summary>
    /// Registers a type with a factory method
    /// </summary>
    /// <typeparam name="TInterface">Interface type</typeparam>
    /// <param name="factory">Factory method to create instances</param>
    /// <param name="lifetime">Lifetime of the registered service</param>
    void Register<TInterface>(Func<IDependencyContainer, TInterface> factory, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TInterface : class;
    
    /// <summary>
    /// Registers an instance as a singleton
    /// </summary>
    /// <typeparam name="TInterface">Interface type</typeparam>
    /// <param name="instance">Instance to register</param>
    void RegisterInstance<TInterface>(TInterface instance)
        where TInterface : class;
    
    /// <summary>
    /// Resolves an instance of the specified type
    /// </summary>
    /// <typeparam name="T">Type to resolve</typeparam>
    /// <returns>Resolved instance</returns>
    T Resolve<T>() where T : class;
    
    /// <summary>
    /// Resolves an instance of the specified type
    /// </summary>
    /// <param name="type">Type to resolve</param>
    /// <returns>Resolved instance</returns>
    object Resolve(Type type);
    
    /// <summary>
    /// Checks if a type is registered
    /// </summary>
    /// <typeparam name="T">Type to check</typeparam>
    /// <returns>True if the type is registered</returns>
    bool IsRegistered<T>() where T : class;
    
    /// <summary>
    /// Clears all registered services
    /// </summary>
    void Clear();
}

/// <summary>
/// Enumeration for service lifetime options
/// </summary>
public enum ServiceLifetime
{
    /// <summary>
    /// A single instance is created and shared for the lifetime of the container
    /// </summary>
    Singleton,
    
    /// <summary>
    /// A new instance is created each time the service is requested
    /// </summary>
    Transient
}