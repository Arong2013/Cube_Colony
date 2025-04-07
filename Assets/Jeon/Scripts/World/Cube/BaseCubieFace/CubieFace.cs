using Sirenix.OdinInspector;
using UnityEngine;

public class CubieFace : MonoBehaviour
{
    public Cubie cubie { get; private set; }
    public CubeFaceType face { get; private set; }
     public CubieFaceSkillType SkillType => cubieFaceInfo.Type;
    public CubieFaceInfo FaceInfo => cubieFaceInfo;

    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private CubieFaceInfo cubieFaceInfo = new();
    public void Init(CubeFaceType face, Cubie cubie)
    {
        this.face = face;
        this.cubie = cubie;
    }

    public void SetFace(CubeFaceType face)
    {
        this.face = face;
    }

    public void SetSkillType(CubieFaceSkillType type)
    {
        cubieFaceInfo.Initialize(transform.position,type); // 상태 초기화
        ApplyVisual(DataCenter.Instance.GetFaceData(type));
    }

    public void ApplyVisual(CubieFaceVisualData data)
    {
        if (targetRenderer != null && data != null && data.material != null)
        {
            targetRenderer.material = data.material;
        }
    }

    public void LevelUp()
    {
        if (cubieFaceInfo.LevelUp())
        {
            Debug.Log($"{this.GetType().Name} leveled up to {cubieFaceInfo.Level}");
            OnLevelUp();
        }
        else
        {
            Debug.Log($"{this.GetType().Name} reached max level and is being reset.");
            ResetFace();
        }
    }

    protected virtual void OnLevelUp()
    {
        // Override 가능
    }

    protected virtual void ResetFace()
    {
        cubieFaceInfo.Reset();
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
