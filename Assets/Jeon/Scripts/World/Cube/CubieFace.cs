using Sirenix.OdinInspector;
using UnityEngine;

public class CubieFace : MonoBehaviour
{
    public Cubie cubie { get; private set; }
    public CubeFaceType face { get; private set; }
     public CubieFaceSkillType SkillType => cubieFaceInfo.Type;
    public CubieFaceInfo FaceInfo => cubieFaceInfo;

    [SerializeField] private Renderer targetRenderer,outLineRenderer;
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
    public void SetVisual()
    {
        var count = CubeGridHandler.Instance.GetSameTypeAdjacentCount(this);
        cubieFaceInfo.SetLevel(count);
        ApplyVisual(DataCenter.Instance.GetFaceData(cubieFaceInfo.Type));
    }
    public void SetSkillType(CubieFaceSkillType type)
    {
        cubieFaceInfo.Initialize(transform.position,type); // 상태 초기화
        ApplyVisual(DataCenter.Instance.GetFaceData(type));
    }
    public void ApplyVisual(CubieFaceVisualData data)
    {
        int level = Mathf.Clamp(cubieFaceInfo.Level, 0, data.CubieFaceMaterials.Count);
        targetRenderer.material = data.CubieFaceMaterials[level];
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
