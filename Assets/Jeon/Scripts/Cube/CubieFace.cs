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
            CubieFaceIndex.Front or CubieFaceIndex.Back => CubeAxisType.Z,  // 아이소메트릭에서 X 대신 Z
            CubieFaceIndex.Left or CubieFaceIndex.Right => CubeAxisType.X,  // 아이소메트릭에서 Z 대신 X
            CubieFaceIndex.Top or CubieFaceIndex.Bottom => CubeAxisType.Y,  // Y축은 동일
            _ => CubeAxisType.Y // 기본값 (예외 방지)
        };
    }
}
