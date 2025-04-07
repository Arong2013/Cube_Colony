using UnityEngine;

public abstract class ScriptableInteractionStrategy : ScriptableObject
{
    public abstract IInteractionStrategy CreateStrategy();
}