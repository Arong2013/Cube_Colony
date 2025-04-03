using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public interface IGameSequenceState
{
    void Enter();
    void Exit();
    void Update();
}
public interface IBehaviorDatable { }

public interface ICubeController
{
    void RotateCube();
}

public interface IEntityComponent
{
    void Start(Entity entity);
    void Update(Entity entity);
    void Exit(Entity entity);
}
public interface IEntityController
{
    void Update(Entity entity);
}
public interface IInteractable
{
    bool CanInteract(Entity interactor);
    void Interact(Entity interactor);
    string GetInteractionLabel();
}
public interface IitemAction
{
    bool IsInstant { get; }
    void Execute(PlayerEntity player, Item item);
}