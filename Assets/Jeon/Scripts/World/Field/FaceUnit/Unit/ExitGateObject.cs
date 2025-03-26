using System;
using UnityEngine;

public class ExitGateObject : FaceUnit
{
    private Action onEnemyExit;

    // Init 메서드: ExitGateObject를 초기화
    public void Init(Action onEnemyExit, CubieFace cubieFace)
    {
        base.Init(cubieFace);
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
