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
    float GetInteractionDistance(); 
}

public interface IInteractionStrategy
{
    void Initialize(Entity self);
    bool CanInteract(Entity self, Entity interactor);
    void Interact(Entity self, Entity interactor);
    string GetLabel();
}

public interface ISubject
{
    void RegisterObserver(IObserver observer);
    void UnregisterObserver(IObserver observer);
    void NotifyObservers();
}
public interface IObserver
{
    void UpdateObserver();
}

public interface IPlayerUesableUI
{
    void Initialize(PlayerEntity playerEntity);
}