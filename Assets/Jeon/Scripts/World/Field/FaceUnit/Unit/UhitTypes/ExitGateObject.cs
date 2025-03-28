using System;
using UnityEngine;

public class ExitGateObject : FaceUnit
{
    private Action onEnemyExit;

    public void Init(Action onEnemyExit, IAstarable astarable)
    {
        base.Init(astarable);
        this.onEnemyExit = onEnemyExit;
    }
    private void OnTriggerEnter(Collider other)
    {
        var faceUnit = other.GetComponent<FaceUnit>();

        if (faceUnit != null && faceUnit.ParentFace == ParentFace)
        {
            onEnemyExit?.Invoke();
            faceUnit.DestroySelf(); 
        }
    }
}
