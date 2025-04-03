using System;
using System.Collections.Generic;

public class EntityComponentHandler
{
    private Dictionary<Type, IEntityComponent> _components = new();
    private Entity _owner;
    public EntityComponentHandler(Entity owner)
    {
        _owner = owner;
    }
    public void Add<T>(T component) where T : IEntityComponent
    {
        var type = typeof(T);

        if (_components.ContainsKey(type))
        {
            UnityEngine.Debug.LogWarning($"Component {type.Name} already exists on {_owner.name}");
            return;
        }
        _components[type] = component;
        component.Start(_owner);
    }

    public bool Has<T>() where T : class, IEntityComponent
    {
        return _components.ContainsKey(typeof(T));
    }

    public T Get<T>() where T : class, IEntityComponent
    {
        _components.TryGetValue(typeof(T), out var comp);
        return comp as T;
    }

    public void Remove<T>() where T : class, IEntityComponent
    {
        var type = typeof(T);
        if (_components.TryGetValue(type, out var comp))
        {
            comp.Exit(_owner);
            _components.Remove(type);
        }
    }
    public void UpdateAll()
    {
        foreach (var comp in _components.Values)
        {
            comp.Update(_owner);
        }
    }
    public void ExitAll()
    {
        foreach (var comp in _components.Values)
        {
            comp.Exit(_owner);
        }
    }
}
