using UnityEngine;

public class CubieFace : MonoBehaviour
{
    [SerializeField] GameObject tower;
    public Cubie cubie { get; private set; }
    public CubeFaceType face;
    public void Init(CubeFaceType face, Cubie cubie)
    {
        this.face = face;
        this.cubie = cubie;
    }
    public void SetFace(CubeFaceType face)
    {
        this.face = face;
    }
    public void SpawnObject()
    {
        GameObject spawnedObject = Instantiate(tower, transform.position, transform.rotation, transform);
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
