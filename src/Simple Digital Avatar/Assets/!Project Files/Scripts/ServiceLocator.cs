using System;
using System.Collections.Generic;

public class ServiceLocator
{
    public static ServiceLocator Instance => _instance ??= new ServiceLocator();
    private static ServiceLocator _instance;

    private readonly Dictionary<Type, object> _services = new();

    public void Register<T>(object service) => _services[typeof(T)] = service;

    public T Get<T>() => (T)_services[typeof(T)];

    public void Clear() => _services.Clear();

    public void Unregister<T>() => _services.Remove(typeof(T));

    public bool Contains<T>() => _services.ContainsKey(typeof(T));
}