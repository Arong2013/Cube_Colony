using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

public class Cube : MonoBehaviour
{
    [SerializeField] Cubie cubiePrefab;

    CubeRotater cubeRotater;
    CubeGridHandler cubeGridHandler;
    ICubeController cubeController;

    // 현재 큐브가 초기화되었는지 확인
    private bool isInitialized = false;

    public void Init(CubeData cubeData)
    {
        // 기존 큐브가 있다면 제거
        if (isInitialized)
        {
            RemoveCube();
        }

        cubeRotater = new CubeRotater(this.transform);
        cubeGridHandler = new CubeGridHandler(cubiePrefab, this.transform, cubeData);

        cubeController = new CubeKeybordController(RotateAllCube);

        isInitialized = true;
        Debug.Log("큐브 초기화 완료");
    }

    /// <summary>
    /// 사용된 페이스들을 몬스터 타일로 변경
    /// </summary>
    public void UpdateUsedFaces(List<Vector3> usedPositions)
    {
        if (!isInitialized || usedPositions == null || usedPositions.Count == 0)
        {
            return;
        }

        // 모든 TOP 페이스를 가져옴
        var topFaces = cubeGridHandler.GetCubieFaces(CubeFaceType.Top);

        foreach (var faceInfo in topFaces)
        {
            // 사용된 위치와 일치하는 페이스를 찾아서 몬스터 타일로 변경
            foreach (var usedPos in usedPositions)
            {
                if (Vector3.Distance(faceInfo.Position, usedPos) < 0.1f)
                {
                    // 해당 페이스를 몬스터 타일로 변경
                    var cubieFace = GetCubieFaceAtPosition(faceInfo.Position, CubeFaceType.Top);
                    if (cubieFace != null)
                    {
                        // 몬스터 타일로 스킬 타입 변경 (RMonster 또는 AMonster)
                        var newSkillType = UnityEngine.Random.value > 0.5f ?
                            CubieFaceSkillType.RMonster : CubieFaceSkillType.AMonster;

                        cubieFace.SetSkillType(newSkillType);
                        Debug.Log($"페이스 위치 {usedPos}를 {newSkillType}로 변경");
                    }
                    break;
                }
            }
        }

        // 변경 후 큐브 시각 업데이트
        cubeGridHandler.UpDateCubieVisual();
    }

    /// <summary>
    /// 특정 위치의 CubieFace를 찾는 헬퍼 메서드
    /// </summary>
    private CubieFace GetCubieFaceAtPosition(Vector3 position, CubeFaceType faceType)
    {
        var allCubies = cubeGridHandler.GetAllCubies();

        foreach (var cubie in allCubies)
        {
            var cubiePosition = cubeGridHandler.GetCubieGridPosition(cubie);

            // 위치가 일치하는 큐비를 찾았으면 해당 페이스 반환
            if (Vector3.Distance(position, cubiePosition) < 0.1f)
            {
                return cubie.GetFace(faceType);
            }
        }

        return null;
    }

    public List<CubieFaceInfo> GetTopCubieFace() => cubeGridHandler.GetCubieFaces(CubeFaceType.Top);

    private void Update()
    {
        if (!isInitialized || cubeRotater.IsRotating) return;
        cubeController?.RotateCube();
    }

    public void RotateCube(Cubie selectedCubie, CubeAxisType axis, bool isClock)
    {
        if (!isInitialized) return;
        StartCoroutine(RotateCubeCoroutine(selectedCubie, axis, isClock));
    }

    private void RotateAllCube(CubeAxisType cubeAxisType, bool isClock)
    {
        if (!isInitialized) return;
        StartCoroutine(RotateAllCubeCoroutine(cubeAxisType, isClock));
    }

    IEnumerator RotateAllCubeCoroutine(CubeAxisType cubeAxisType, bool isClock)
    {
        cubeGridHandler.RotateWholeCube(cubeAxisType, isClock);
        yield return StartCoroutine(cubeRotater.RotateCubesSmooth(cubeGridHandler.GetAllCubies(), cubeAxisType, isClock));
    }

    IEnumerator RotateCubeCoroutine(Cubie selectedCubie, CubeAxisType axis, bool isClock)
    {
        List<Cubie> targetLayer = cubeGridHandler.GetCubiesOnSameLayer(selectedCubie, axis);
        cubeGridHandler.RotateSingleLayer(selectedCubie, axis, isClock);
        yield return StartCoroutine(cubeRotater.RotateCubesSmooth(targetLayer, axis, isClock));
    }

    public void RemoveCube()
    {
        if (isInitialized)
        {
            cubeGridHandler.DestroyAllCubies();
            cubeGridHandler = null;
            cubeController = null;
            isInitialized = false;
            Debug.Log("큐브 제거됨");
        }
    }
}