using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public enum CubeAxisType
{
    X,       // X축 회전만 가능
    Y,       // Y축 회전만 가능
    Z        // Z축 회전만 가능
}

public class Cube : MonoBehaviour
{
    [SerializeField] PlayerUIController cubeUIController;
    [SerializeField] int size;
    [SerializeField] Cubie cubie;

    CubeRotater cubeRotater;
   public CubeGridHandler cubeGridHandler;

    private void Start()
    {
        cubeRotater = new CubeRotater(this.transform);
        cubeGridHandler = new CubeGridHandler(size, cubie,this.transform);

       cubeUIController.SetRotateAction(RotateCube);
    }
    private void Update()
    {
        if (cubeRotater.IsRotating) return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            InputRotateCube(KeyCode.W);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            InputRotateCube(KeyCode.S);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            InputRotateCube(KeyCode.A);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            InputRotateCube(KeyCode.D);
        }
    }

    public void InputRotateCube(KeyCode keyCode)
    {
        bool clockwise = keyCode == KeyCode.W || keyCode == KeyCode.A;
        CubeAxisType axis = (keyCode == KeyCode.W || keyCode == KeyCode.S) ? CubeAxisType.X : CubeAxisType.Y;

        int angle = clockwise ? 90 : -90;

        StartCoroutine(cubeRotater.RotateCubesSmooth(cubeGridHandler.GetAllCubies(), axis, angle));
        cubeGridHandler.RotateWholeCube(clockwise, axis);
    }
    private void RotateCube(Cubie selectedCubie, CubeAxisType axis, int rotationAmount)
    {
        if (cubeRotater.IsRotating) return;
        List<Cubie> targetLayer = cubeGridHandler.GetCubiesOnSameLayer(selectedCubie, axis);
        StartCoroutine(cubeRotater.RotateCubesSmooth(targetLayer, axis, rotationAmount));
        cubeGridHandler.RotateSingleLayer(selectedCubie, axis, rotationAmount);
    }

    public void SpawnSpawner(EnemySpawnSequence seq, Action onMonsterDeath)
    {
        var spawnerGO = SpawnerFactory.Create(seq, this.transform);
        spawnerGO.Init(seq, onMonsterDeath);
    }

    public GameObject SpawnMonster(int monsterId, CubeFaceType face, Action onDeath)
    {
        GameObject prefab = MonsterFactory.Instance.GetPrefab(monsterId);
        Vector3 spawnPos = GetFaceWorldPosition(face);
        GameObject enemyGO = Instantiate(prefab, spawnPos, Quaternion.identity, transform);

        // 계층 구조: Cube → Cubie → CubieFace → FaceObject (그 하위)
        // 필요 시 FaceObject.Find(face).Attach(enemyGO); 처럼 가능

        return enemyGO;
    }
    public Vector3 GetFaceWorldPosition(CubeFaceType face)
    {
        // face 기준으로 실제 월드 좌표 계산
        // (ex: forward, back 등)
        return transform.position + face switch
        {
            CubeFaceType.Front => Vector3.forward * 5f,
            CubeFaceType.Back => Vector3.back * 5f,
            CubeFaceType.Left => Vector3.left * 5f,
            CubeFaceType.Right => Vector3.right * 5f,
            CubeFaceType.Top => Vector3.up * 5f,
            CubeFaceType.Bottom => Vector3.down * 5f,
            _ => Vector3.zero
        };
    }
}
