using UnityEngine;

public class CubieFace : MonoBehaviour
{
    [SerializeField] GameObject tower;
    public Cubie cubie;
    public CubieFaceIndex face { get; private set; }
    public void Init(CubieFaceIndex face, Cubie cubie)
    {
        this.face = face;
        this.cubie = cubie;
    }
    public void SetFace(CubieFaceIndex face)
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
            CubieFaceIndex.Front or CubieFaceIndex.Back => CubeAxisType.Z,  
            CubieFaceIndex.Left or CubieFaceIndex.Right => CubeAxisType.X,  
            CubieFaceIndex.Top or CubieFaceIndex.Bottom => CubeAxisType.Y, 
            _ => CubeAxisType.Y 
        };
    }
}
