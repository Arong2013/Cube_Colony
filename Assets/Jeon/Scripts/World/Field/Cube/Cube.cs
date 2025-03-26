using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Cube : MonoBehaviour
{
    [SerializeField] PlayerUIController cubeUIController;
    [SerializeField] int size;
    [SerializeField] Cubie cubie;

    CubeRotater cubeRotater;
    public CubeGridHandler cubeGridHandler;

    public void Init()
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
    public void SpawnSpawner(EnemySpawnSequence seq, Action onMonsterDeath,CubeFaceType cubeFaceType)
    {
        var face = cubeGridHandler.GetCenterFace(cubeFaceType);
        var obj = face.SpawnObject(SpawnerFactory.Instance.GetPrefab(seq.spawnerId));
        obj.GetComponent<MonsterSpawner>().Init(seq, onMonsterDeath, face);
    }

    public void SpawnExitGate(Action onEnemyExit,CubeFaceType cubeFaceType)
    {
        var face = cubeGridHandler.GetCenterFace(cubeFaceType);
        var obj = face.SpawnObject(ExitGateFactory.Instance.GetPrefab(0));
        obj.GetComponent<ExitGateObject>().Init(onEnemyExit,face);
    }
  }
