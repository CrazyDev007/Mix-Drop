using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implementation of a dependency injection container for Unity
/// </summary>
public class DependencyContainer : IDependencyContainer
{
    private readonly Dictionary<Type, ServiceRegistration> _registrations = new Dictionary<Type, ServiceRegistration>();
    private readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();

    /// <summary>
    /// Registers a type with its implementation
    /// </summary>
    /// <typeparam name="TInterface">Interface type</typeparam>
    /// <typeparam name="TImplementation">Implementation type</typeparam>
    /// <param name="lifetime">Lifetime of the registered service</param>
    public void Register<TInterface, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        var interfaceType = typeof(TInterface);
        var registration = new ServiceRegistration
        {
            ImplementationType = typeof(TImplementation),
            Lifetime = lifetime,
            Factory = null
        };

        _registrations[interfaceType] = registration;
    }

    /// <summary>
    /// Registers a type with a factory method
    /// </summary>
    /// <typeparam name="TInterface">Interface type</typeparam>
    /// <param name="factory">Factory method to create instances</param>
    /// <param name="lifetime">Lifetime of the registered service</param>
    public void Register<TInterface>(Func<IDependencyContainer, TInterface> factory, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TInterface : class
    {
        var interfaceType = typeof(TInterface);
        var registration = new ServiceRegistration
        {
            ImplementationType = null,
            Lifetime = lifetime,
            Factory = container => factory(container)
        };

        _registrations[interfaceType] = registration;
    }

    /// <summary>
    /// Registers an instance as a singleton
    /// </summary>
    /// <typeparam name="TInterface">Interface type</typeparam>
    /// <param name="instance">Instance to register</param>
    public void RegisterInstance<TInterface>(TInterface instance)
        where TInterface : class
    {
        var interfaceType = typeof(TInterface);
        _singletons[interfaceType] = instance;

        var registration = new ServiceRegistration
        {
            ImplementationType = null,
            Lifetime = ServiceLifetime.Singleton,
            Factory = null
        };

        _registrations[interfaceType] = registration;
    }

    /// <summary>
    /// Resolves an instance of the specified type
    /// </summary>
    /// <typeparam name="T">Type to resolve</typeparam>
    /// <returns>Resolved instance</returns>
    public T Resolve<T>() where T : class
    {
        return (T)Resolve(typeof(T));
    }

    /// <summary>
    /// Resolves an instance of the specified type
    /// </summary>
    /// <param name="type">Type to resolve</param>
    /// <returns>Resolved instance</returns>
    public object Resolve(Type type)
    {
        if (_singletons.TryGetValue(type, out var singletonInstance))
        {
            return singletonInstance;
        }

        if (!_registrations.TryGetValue(type, out var registration))
        {
            throw new InvalidOperationException($"Service of type {type.Name} is not registered");
        }

        if (registration.Lifetime == ServiceLifetime.Singleton && _singletons.TryGetValue(type, out singletonInstance))
        {
            return singletonInstance;
        }

        object instance;
        if (registration.Factory != null)
        {
            instance = registration.Factory(this);
        }
        else
        {
            instance = CreateInstance(registration.ImplementationType);
        }

        if (registration.Lifetime == ServiceLifetime.Singleton)
        {
            _singletons[type] = instance;
        }

        return instance;
    }

    /// <summary>
    /// Checks if a type is registered
    /// </summary>
    /// <typeparam name="T">Type to check</typeparam>
    /// <returns>True if the type is registered</returns>
    public bool IsRegistered<T>() where T : class
    {
        return _registrations.ContainsKey(typeof(T));
    }

    /// <summary>
    /// Clears all registered services
    /// </summary>
    public void Clear()
    {
        _registrations.Clear();
        _singletons.Clear();
    }

    /// <summary>
    /// Creates an instance of the specified type with dependency injection
    /// </summary>
    /// <param name="type">Type to create</param>
    /// <returns>Created instance</returns>
    private object CreateInstance(Type type)
    {
        var constructors = type.GetConstructors();
        if (constructors.Length == 0)
        {
            return Activator.CreateInstance(type);
        }

        // Use the constructor with the most parameters
        var constructor = constructors[0];
        var parameters = constructor.GetParameters();
        var parameterValues = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var parameterType = parameters[i].ParameterType;
            parameterValues[i] = Resolve(parameterType);
        }

        return constructor.Invoke(parameterValues);
    }

    /// <summary>
    /// Represents a service registration
    /// </summary>
    private class ServiceRegistration
    {
        public Type ImplementationType { get; set; }
        public ServiceLifetime Lifetime { get; set; }
        public Func<IDependencyContainer, object> Factory { get; set; }
    }
}