using Sirenix.OdinInspector;
using UnityEngine;

public class CubieFace : MonoBehaviour
{
    public Cubie cubie { get; private set; }
    public CubeFaceType face { get; private set; }
    public CubieFaceSkillType SkillType => cubieFaceInfo.Type;
    public CubieFaceInfo FaceInfo => cubieFaceInfo;

    [SerializeField] private Renderer targetRenderer, outLineRenderer;
    [SerializeField] private CubieFaceInfo cubieFaceInfo = new();

    [SerializeField] Animator animator;

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
        //animator.SetTrigger(count);
    }

    public void SetSkillType(CubieFaceSkillType type)
    {
        // 현재 위치를 유지하면서 스킬 타입만 변경
        Vector3 currentPosition = cubieFaceInfo.Position;
        cubieFaceInfo.Initialize(currentPosition, type);

        // 즉시 시각 효과 적용
        ApplyVisual(DataCenter.Instance.GetFaceData(type));

        Debug.Log($"CubieFace [{gameObject.name}] 스킬 타입이 {type}으로 변경됨 (위치: {currentPosition})");
    }

    /// <summary>
    /// 스킬 타입을 강제로 변경 (위치 업데이트 없이)
    /// </summary>
    public void ForceChangeSkillType(CubieFaceSkillType newType)
    {
        Vector3 currentPos = cubieFaceInfo.Position;
        cubieFaceInfo.Initialize(currentPos, newType);
        ApplyVisual(DataCenter.Instance.GetFaceData(newType));

        Debug.Log($"[ForceChange] {gameObject.name}의 스킬이 {newType}으로 강제 변경됨");
    }

    public void ApplyVisual(CubieFaceVisualData data)
    {
        if (data == null)
        {
            Debug.LogWarning($"CubieFaceVisualData가 null입니다. 스킬 타입: {cubieFaceInfo.Type}");
            return;
        }

        int level = Mathf.Clamp(cubieFaceInfo.Level, 0, data.CubieFaceMaterials.Count - 1);

        if (level < data.CubieFaceMaterials.Count)
        {
            targetRenderer.material = data.CubieFaceMaterials[level];
        }
        else
        {
            Debug.LogWarning($"레벨 {level}에 해당하는 머티리얼이 없습니다. 스킬 타입: {cubieFaceInfo.Type}");
        }
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

    /// <summary>
    /// 몬스터 타일인지 확인하는 헬퍼 메서드
    /// </summary>
    public bool IsMonsterTile()
    {
        return SkillType == CubieFaceSkillType.RMonster || SkillType == CubieFaceSkillType.AMonster;
    }

    /// <summary>
    /// 자원 타일인지 확인하는 헬퍼 메서드
    /// </summary>
    public bool IsResourceTile()
    {
        return SkillType == CubieFaceSkillType.Mine || SkillType == CubieFaceSkillType.Plant;
    }
}