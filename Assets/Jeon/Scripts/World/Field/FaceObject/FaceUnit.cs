using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class FaceUnit : MonoBehaviour
{
    [SerializeField] private UnitType unitType;
    [SerializeField] private PriorityNameType propertyType;
    private CubieFace parentFace;
    private Action onDeath;
    private float range = 100f;

    private Dictionary<IBehaviorDatable, object> behaviorData = new Dictionary<IBehaviorDatable, object>();
    public CubeFaceType CubeFaceType => parentFace.face;
    public UnitType UnitType => unitType;
    public PriorityNameType PropertyType => propertyType;
    public CubieFace ParentFace => parentFace;
    public float Range => range;

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

    public TData GetUnitData<TData,Tkey>()
        where Tkey : IBehaviorDatable
    {
        var key = default(Tkey);    
        if (behaviorData.TryGetValue(key, out object value))
        {
            return (TData)value;
        }
        return default;
    }
    public void SetData<Tdata,Tkey>(Tdata data)
        where Tkey : IBehaviorDatable
    {
        var key = default(Tkey);
        behaviorData[key] = data;
    }
    public List<CubieFace> GetAstarList()
    {
        var unitlist =  GetUnitData<List<FaceUnit>,DetectEnemyCondition>();  
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
            // 충돌한 CubieFace가 있을 때 부모 Face를 변경
            parentFace = cubieFace;
        }
    }
}
