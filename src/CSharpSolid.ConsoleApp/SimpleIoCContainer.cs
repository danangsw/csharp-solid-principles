using System.Collections.Concurrent;

namespace CSharpSolid.ConsoleApp;

/// <summary>
/// Simple Inversion of Control container demonstrating dependency injection principles
/// without using Microsoft.Extensions.DependencyInjection
/// </summary>
public class SimpleIoCContainer
{
    private readonly ConcurrentDictionary<Type, ServiceDescriptor> _services = new();

    /// <summary>
    /// Register a service with transient lifetime (new instance each time)
    /// </summary>
    public void AddTransient<TService, TImplementation>() where TImplementation : TService
    {
        _services[typeof(TService)] = new ServiceDescriptor(typeof(TImplementation), ServiceLifetime.Transient);
    }

    /// <summary>
    /// Register a service with transient lifetime using service type and implementation type
    /// </summary>
    public void AddTransient(Type serviceType, Type implementationType)
    {
        _services[serviceType] = new ServiceDescriptor(implementationType, ServiceLifetime.Transient);
    }

    /// <summary>
    /// Register a service with singleton lifetime (same instance always)
    /// </summary>
    public void AddSingleton<TService, TImplementation>() where TImplementation : TService
    {
        _services[typeof(TService)] = new ServiceDescriptor(typeof(TImplementation), ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Register a singleton instance directly
    /// </summary>
    public void AddSingleton<TService>(TService instance)
    {
        _services[typeof(TService)] = new ServiceDescriptor(instance, ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Register a service with singleton lifetime using service type and implementation type
    /// </summary>
    public void AddSingleton(Type serviceType, Type implementationType)
    {
        _services[serviceType] = new ServiceDescriptor(implementationType, ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Resolve a service instance
    /// </summary>
    public TService GetService<TService>()
    {
        return (TService)GetService(typeof(TService));
    }

    /// <summary>
    /// Resolve a service instance by type
    /// </summary>
    public object GetService(Type serviceType)
    {
        if (!_services.TryGetValue(serviceType, out var descriptor))
        {
            throw new InvalidOperationException($"Service {serviceType.Name} is not registered");
        }

        return descriptor.Lifetime switch
        {
            ServiceLifetime.Singleton => GetSingletonInstance(descriptor),
            ServiceLifetime.Transient => CreateInstance(descriptor.ServiceType),
            _ => throw new InvalidOperationException($"Unknown lifetime: {descriptor.Lifetime}")
        };
    }

    private object GetSingletonInstance(ServiceDescriptor descriptor)
    {
        if (descriptor.Instance == null)
        {
            descriptor.Instance = CreateInstance(descriptor.ServiceType);
        }
        return descriptor.Instance;
    }

    private object CreateInstance(Type type)
    {
        // Find the constructor with the most parameters (most specific)
        var constructor = type.GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();

        if (constructor == null)
        {
            throw new InvalidOperationException($"No public constructor found for {type.Name}");
        }

        var parameters = constructor.GetParameters();
        var parameterInstances = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var parameterType = parameters[i].ParameterType;
            parameterInstances[i] = GetService(parameterType);
        }

        return constructor.Invoke(parameterInstances);
    }

    private sealed class ServiceDescriptor
    {
        public Type ServiceType { get; }
        public ServiceLifetime Lifetime { get; }
        public object? Instance { get; set; }

        public ServiceDescriptor(Type serviceType, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            Lifetime = lifetime;
        }

        public ServiceDescriptor(object? instance, ServiceLifetime lifetime)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            ServiceType = instance.GetType();
            Lifetime = lifetime;
            Instance = instance;
        }
    }

    private enum ServiceLifetime
    {
        Transient,
        Singleton
    }
}