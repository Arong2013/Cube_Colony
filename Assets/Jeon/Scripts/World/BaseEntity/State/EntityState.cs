public abstract class EntityState
{
    protected Entity _entity;

    public EntityState(Entity _entity)
    {
        this._entity = _entity;
    }
    public virtual void Enter() { }
    public virtual void Execute() { }
    public virtual void Exit() { }
}