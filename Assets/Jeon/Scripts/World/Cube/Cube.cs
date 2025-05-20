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

    // ���� ť�갡 �ʱ�ȭ�Ǿ����� Ȯ��
    private bool isInitialized = false;

    public void Init(CubeData cubeData)
    {
        // ���� ť�갡 �ִٸ� ����
        if (isInitialized)
        {
            RemoveCube();
        }

        cubeRotater = new CubeRotater(this.transform);
        cubeGridHandler = new CubeGridHandler(cubiePrefab, this.transform, cubeData);

        cubeController = new CubeKeybordController(RotateAllCube);

        isInitialized = true;
        Debug.Log("ť�� �ʱ�ȭ �Ϸ�");
    }

    /// <summary>
    /// ���� ���̽����� ���� Ÿ�Ϸ� ����
    /// </summary>
    public void UpdateUsedFaces(List<Vector3> usedPositions)
    {
        if (!isInitialized || usedPositions == null || usedPositions.Count == 0)
        {
            return;
        }

        // ��� TOP ���̽��� ������
        var topFaces = cubeGridHandler.GetCubieFaces(CubeFaceType.Top);

        foreach (var faceInfo in topFaces)
        {
            // ���� ��ġ�� ��ġ�ϴ� ���̽��� ã�Ƽ� ���� Ÿ�Ϸ� ����
            foreach (var usedPos in usedPositions)
            {
                if (Vector3.Distance(faceInfo.Position, usedPos) < 0.1f)
                {
                    // �ش� ���̽��� ���� Ÿ�Ϸ� ����
                    var cubieFace = GetCubieFaceAtPosition(faceInfo.Position, CubeFaceType.Top);
                    if (cubieFace != null)
                    {
                        // ���� Ÿ�Ϸ� ��ų Ÿ�� ���� (RMonster �Ǵ� AMonster)
                        var newSkillType = UnityEngine.Random.value > 0.5f ?
                            CubieFaceSkillType.RMonster : CubieFaceSkillType.AMonster;

                        cubieFace.SetSkillType(newSkillType);
                        Debug.Log($"���̽� ��ġ {usedPos}�� {newSkillType}�� ����");
                    }
                    break;
                }
            }
        }

        // ���� �� ť�� �ð� ������Ʈ
        cubeGridHandler.UpDateCubieVisual();
    }

    /// <summary>
    /// Ư�� ��ġ�� CubieFace�� ã�� ���� �޼���
    /// </summary>
    private CubieFace GetCubieFaceAtPosition(Vector3 position, CubeFaceType faceType)
    {
        var allCubies = cubeGridHandler.GetAllCubies();

        foreach (var cubie in allCubies)
        {
            var cubiePosition = cubeGridHandler.GetCubieGridPosition(cubie);

            // ��ġ�� ��ġ�ϴ� ť�� ã������ �ش� ���̽� ��ȯ
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
            Debug.Log("ť�� ���ŵ�");
        }
    }
}