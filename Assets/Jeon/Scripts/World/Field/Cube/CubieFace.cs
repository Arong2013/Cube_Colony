using UnityEngine;
using System;
using System.Collections.Generic;

public class CubieFace : MonoBehaviour
{
    [SerializeField] GameObject tower;
    public Cubie cubie { get; private set; }
    public CubeFaceType face { get; private set; }
    IAstarable astarable;
    public void Init(CubeFaceType face, Cubie cubie, IAstarable astarable)
    {
        this.face = face;
        this.cubie = cubie;
        this.astarable = astarable; 
    }
    public void SetFace(CubeFaceType face)
    {
        this.face = face;
    }
    public FaceUnit SpawnObject(GameObject prefab)
    {
        GameObject spawned = Instantiate(prefab, transform.position, transform.rotation, transform);
        var faceObj = spawned.GetComponent<FaceUnit>();
        faceObj.Init(this); 
        return faceObj; 
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

    public List<CubieFace> GetAstarList(CubieFace targetFace) => astarable.GetAstarPathFaces(this,targetFace);
}
