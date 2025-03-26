using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class FaceUnit : MonoBehaviour
{
    [SerializeField] private UnitType unitType;
    [SerializeField] private PriorityNameType propertyType;
    private CubieFace parentFace;
    private Action onDeath;
    private float range = 100f;
    [SerializeField] private float attackRange = 5f;

    private Dictionary<IBehaviorDatable, object> behaviorData = new Dictionary<IBehaviorDatable, object>();

    protected Vector3 moveDirection = Vector3.zero; // 이동 방향

    public CubeFaceType CubeFaceType => parentFace.face;
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

    public TData GetUnitData<TData, Tkey>()
        where Tkey : IBehaviorDatable
    {
        var key = default(Tkey);
        if (behaviorData.TryGetValue(key, out object value))
        {
            return (TData)value;
        }
        return default;
    }

    public void SetData<Tdata, Tkey>(Tdata data)
        where Tkey : IBehaviorDatable
    {
        var key = default(Tkey);
        behaviorData[key] = data;
    }

    public List<CubieFace> GetAstarList()
    {
        var unitlist = GetUnitData<List<FaceUnit>, DetectEnemyCondition>();
        var faceList = new List<CubieFace>();

        foreach (var unit in unitlist)
        {
            faceList = parentFace.GetAstarList(unit.ParentFace);
            if (faceList.Count > 1)
            {
                return faceList;
            }
        }
        return faceList;
    }

    private void OnTriggerEnter(Collider other)
    {
        var cubieFace = other.GetComponent<CubieFace>();

        if (cubieFace != null)
        {
            parentFace = cubieFace;
            moveDirection = Vector3.zero; 
        }
    }
    public void SetMoveDirection(Vector3 direction) => moveDirection = direction;
    public void MoveToNextFace()
    {
        var astarList = GetUnitData<List<CubieFace>, ChessTargetAcion>();

        if (astarList != null && astarList.Count > 0)
        {
            var nextFace = astarList[0];

            // 이동 방향 설정 (현재 위치 → 다음 목표 위치)
            moveDirection = (nextFace.transform.position - transform.position).normalized;

            // 목표 도달 후 리스트에서 제거
            astarList.RemoveAt(0);
            SetData<List<CubieFace>, ChessTargetAcion>(astarList);

            // 부모 Face 업데이트 (이동이 완료된 후)
            parentFace = nextFace;
        }
    }
    public Vector3Int GetMovementDirection(CubieFace currentFace, CubieFace targetFace) => UnitMovementHelper.GetMovementDirection(currentFace, targetFace);    
}
