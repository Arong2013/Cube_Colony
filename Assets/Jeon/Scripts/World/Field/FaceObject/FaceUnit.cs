using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class FaceUnit : MonoBehaviour
{
    private UnitType unitType;
    private PriorityNameType propertyType;
    private CubieFace parentFace;
    private Action onDeath;
    private float range = 100f;

    public CubeFaceType CubeFaceType => parentFace.face;  
    public UnitType UnitType => unitType;
    public PriorityNameType PropertyType => propertyType;   
    public CubieFace ParentFace => parentFace;  
    public float Range => range;

    private Dictionary<Type, object> unitData = new Dictionary<Type, object>();

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
        Destroy(gameObject);
    }


    public TData GetUnitData<TData>(Type type)
    {
        if (unitData.ContainsKey(type))
        {
            return (TData)unitData[type]; // 저장된 데이터 반환
        }
        return default(TData); // 해당 유닛 타입에 대한 데이터가 없으면 기본값 반환
    }

    // 유닛 타입에 맞는 데이터를 저장하기
    public void SetUnitData<TData>(Type type, TData data)
    {
        if (unitData.ContainsKey(type))
        {
            unitData[type] = data; // 이미 존재하면 업데이트
        }
        else
        {
            unitData.Add(type, data); // 없으면 추가
        }
    }
}