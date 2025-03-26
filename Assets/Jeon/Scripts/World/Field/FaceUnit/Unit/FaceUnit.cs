using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.Tilemaps;
using UnityEngine;

public interface IBehaviorDatables<T>
{
    
}
public abstract class FaceUnit : MonoBehaviour
{
    [SerializeField] private UnitType unitType;
    [SerializeField] private PriorityNameType propertyType;
    [ShowInInspector] private CubieFace parentFace;
    private Action onDeath;
    private float range = 1000f;
    [SerializeField] private float attackRange = 5f;

    private Dictionary<Type, object> behaviorData = new Dictionary<Type, object>();

    public Vector3 nextPos = Vector3.zero; // 이동 방향



    public CubeFaceType CubeFaceType => parentFace.face;
    public UnitType UnitType => unitType;
    public PriorityNameType PropertyType => propertyType;
    public CubieFace ParentFace => parentFace;
    public float Range => range;
    public float AttackRange => attackRange;
    public bool IsMoveable => false;


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
        var key = typeof(Tkey);
        if (behaviorData.TryGetValue(key, out object value))
        {
            return (TData)value;
        }
        return default;
    }
    public void SetData<Tdata, Tkey>(Tdata data)
        where Tkey : IBehaviorDatable
    {
        var key = typeof(Tkey);
        behaviorData[key] = data;
    }
    public List<CubieFace> GetAstarList()
    {
        var unitlist = GetUnitData<List<ExitGateObject>, DetectExitCondition>();
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
    protected virtual void OnTriggerEnter(Collider other)
    {
        var cubieFace = other.GetComponent<CubieFace>();

        if (cubieFace != null)
        {
            parentFace = cubieFace;
        }
    }

    public void SetMoveDirection(Vector3 _nextPos)
    {
        nextPos = _nextPos;
        if(moveCoroutine.name)
        moveCoroutine.
    }
}
