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
    // 정확한 타입 매치
    if (_components.ContainsKey(typeof(T)))
        return true;
        
    // 상속 관계도 확인 (필요한 경우)
    foreach (var type in _components.Keys)
    {
        if (typeof(T).IsAssignableFrom(type))
            return true;
    }
    
    return false;
}

public T Get<T>() where T : class, IEntityComponent
{
    // 정확한 타입 매치
    if (_components.TryGetValue(typeof(T), out var comp))
        return comp as T;
        
    // 상속 관계도 확인 (필요한 경우)
    foreach (var kvp in _components)
    {
        if (typeof(T).IsAssignableFrom(kvp.Key))
            return kvp.Value as T;
    }
    
    return null;
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
