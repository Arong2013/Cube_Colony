using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private EntityComponentHandler _componentHandler;
    private EntityAnimatorHandler _animatorHandler;
    private IEntityController _controller;
    private Animator _animator; 
    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _animatorHandler = new EntityAnimatorHandler(_animator);
        _componentHandler = new EntityComponentHandler(this);
    }
    public void SetController(IEntityController controller)
    {
        _controller = controller;
    }
    public void AddEntityComponent<T>(T component) where T : IEntityComponent
        => _componentHandler.Add(component);
    public bool HasEntityComponent<T>() where T : class, IEntityComponent
        => _componentHandler.Has<T>();

    public T GetEntityComponent<T>() where T : class, IEntityComponent
        => _componentHandler.Get<T>();

    public void RemoveEntityComponent<T>() where T : class, IEntityComponent
        => _componentHandler.Remove<T>();

    private void Update()
    {
        _controller?.Update(this);
        _componentHandler.UpdateAll();
    }
    private void OnDestroy()
    {
        _componentHandler.ExitAll();
    }
    public void SetAnimatorValue<T>(T type, object value) where T : Enum { _animatorHandler.SetAnimatorValue(type, value); }
    public TResult GetAnimatorValue<T, TResult>(T type) where T : Enum { return _animatorHandler.GetAnimatorValue<T, TResult>(type); }
}

