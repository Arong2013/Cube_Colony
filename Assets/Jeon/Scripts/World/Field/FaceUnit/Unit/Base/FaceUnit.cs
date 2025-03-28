using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class FaceUnit : MonoBehaviour
{
    [SerializeField] private UnitType unitType;
    [SerializeField] private PriorityNameType propertyType;
    [ShowInInspector] private CubieFace parentFace;
    private Action onDeath;
    private float range = 1000f;
    [SerializeField] private float attackRange = 5f;
    private Dictionary<BehaviorDataType, object> behaviorData = new Dictionary<BehaviorDataType, object>();
    public CubeFaceType CubeFaceType => UnitMovementHelper.CalculateCurrentFace(this.transform, 3);
    public UnitType UnitType => unitType;
    public PriorityNameType PropertyType => propertyType;
    public CubieFace ParentFace => parentFace;
    public float Range => range;
    public float AttackRange => attackRange;

    public virtual void Init(CubieFace cubieFace)
    {
        parentFace = cubieFace;
    }

    public void AddOnDeathAction(Action action)
    {
        onDeath += action;
    }
    public void DestroySelf()
    {
        onDeath?.Invoke();
        behaviorData.Clear();
        Destroy(gameObject);
    }
    public TData GetUnitData<TData>(BehaviorDataType behaviorDataType)
    {
        if (behaviorData.TryGetValue(behaviorDataType, out object value))
        {
            return (TData)value;
        }
        return default;
    }
    public void SetData<Tdata>(BehaviorDataType behaviorDataType,Tdata data)
    {
        behaviorData[behaviorDataType] = data;
    }
    public List<CubieFace> GetAstarList()
    {
        var unitlist = GetUnitData<List<ExitGateObject>>(BehaviorDataType.TargetList);
        var faceList = new List<CubieFace>();
        foreach (var unit in unitlist)
        {
            var foundFaces = parentFace.GetAstarList(unit.ParentFace);
            if (foundFaces.Count > 1)
            {
                faceList = foundFaces;
                return faceList;
            }
        }
        return faceList;
    }
    public void Move(Vector3Int dir) => UnitMovementHelper.Move(this, dir);
    protected virtual void OnTriggerEnter(Collider other)
    {
        var cubieFace = other.GetComponent<CubieFace>();

        if (cubieFace != null)
        {
            // 현재 오브젝트를 cubieFace의 자식으로 설정
            transform.SetParent(cubieFace.transform);

            // 필요에 따라 추가 로직을 처리할 수 있습니다.
            parentFace = cubieFace;
        }
    }
}
