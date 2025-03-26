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

    private Dictionary<Type, object> behaviorData = new Dictionary<Type, object>();

    public Vector3Int moveDirection = Vector3Int.zero; // 이동 방향 (정수 좌표)

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
            // A* 경로를 찾는 과정
            var foundFaces = parentFace.GetAstarList(unit.ParentFace);

            // A* 경로가 있다면
            if (foundFaces.Count > 1)
            {
                faceList = foundFaces;

                // 모든 CubieFace들을 로그로 출력
                foreach (var face in faceList)
                {
                    Debug.Log($"CubieFace: {face.name}", face.gameObject); // face.gameObject를 넘겨주면 하이라키에서 선택 가능

                }

                return faceList;
            }
        }

        return faceList;
    }

    // 목표 위치를 기반으로 이동 방향 설정
    public void SetMoveDirection(Vector3 target)
    {
        moveDirection = UnitMovementHelper.GetNextMoveDirection(parentFace.transform.position, target);
    }
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
