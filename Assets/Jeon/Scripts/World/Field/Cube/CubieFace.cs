using UnityEngine;
using System;

public class CubieFace : MonoBehaviour
{
    [SerializeField] GameObject tower;
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
    public void SpawnObject(GameObject prefab, Action onDeath = null)
    {
        GameObject spawned = Instantiate(prefab, transform.position, transform.rotation, transform);
        if (spawned.TryGetComponent<FaceObject>(out var faceObject))
        {
            faceObject.AddOnDeathAction(onDeath);
        }
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
