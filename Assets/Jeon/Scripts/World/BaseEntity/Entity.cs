using UnityEngine;

public class Entity : MonoBehaviour
{
    private ComponentHandler _componentHandler;
    private IEntityController _controller;
    protected virtual void Awake()
    {
        _componentHandler = new ComponentHandler(this);
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

}
