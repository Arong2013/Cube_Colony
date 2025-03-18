using UnityEngine;

public class CubieFace : MonoBehaviour
{
    [SerializeField] GameObject tower;
    public Cubie cubie;
    public CubieFaceIndex face;  // 큐브 면을 나타내는 인덱스

    // 큐브의 면과 해당 큐브에 대한 설정
    public void SetFace(CubieFaceIndex face, Cubie cubie)
    {
        this.face = face;
        this.cubie = cubie;
    }
    // 지정된 면에 객체를 생성하는 메서드
    public void SpawnObject()
    {
        GameObject spawnedObject = Instantiate(tower, transform.position, transform.rotation, transform);
    }


    public DualAxisType GetAllowedRotationAxis()
    {
        var dualAxis = DualAxisType.XY;
        switch (face)
        {
            case CubieFaceIndex.Front:
            case CubieFaceIndex.Back:
                dualAxis = DualAxisType.XY;  // Front/Back 면은 X, Y축 회전만 가능
                break;
            case CubieFaceIndex.Left:
            case CubieFaceIndex.Right:
                dualAxis = DualAxisType.YZ;  // Left/Right 면은 Y, Z축 회전만 가능
                break;
            case CubieFaceIndex.Top:
            case CubieFaceIndex.Bottom:
                dualAxis = DualAxisType.XZ;  // Top/Bottom 면은 X, Z축 회전만 가능
                break;
            default:
                dualAxis = DualAxisType.XY;  // 기본 설정 (기본적으로 XY축 회전 가능)
                break;
        }
        return dualAxis;
    }
}
