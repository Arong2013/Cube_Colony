using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class FaceObject : MonoBehaviour
{
    private CubieFace parentFace;
    private Action onDeath;


    public virtual void Init(CubieFace cubieFace)
    {
        parentFace = cubieFace;
    }
    public void AddOnDeathAction(Action action)
    {
        onDeath += action;
    }
    public void DestroySelf()
    {
        onDeath?.Invoke();  
        Destroy(gameObject);
    }
}