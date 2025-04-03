using UnityEngine;
using System;
using System.Collections.Generic;

public class CubieFace : MonoBehaviour
{
    public Cubie cubie { get; private set; }
    public CubeFaceType face { get; private set; }
    public void Init(CubeFaceType face, Cubie cubie)
    {
        this.face = face;
        this.cubie = cubie;   
    }
    public void SetFace(CubeFaceType face)
    {
        this.face = face;
    }
    public CubeAxisType GetNotRotationAxis()
    {
        return face switch
        {
            CubeFaceType.Front or CubeFaceType.Back => CubeAxisType.Z,  
            CubeFaceType.Left or CubeFaceType.Right => CubeAxisType.X,  
            CubeFaceType.Top or CubeFaceType.Bottom => CubeAxisType.Y, 
            _ => CubeAxisType.Y 
        };
    }
}
