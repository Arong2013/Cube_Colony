﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class InSurvivalState : IGameSequenceState
{
    private List<CubieFaceInfo> cubieFaceInfos;
    private CubeData cubeData;
    private Field field;
    private float nextStageTime;
    private bool isSpawnedNextStage = false;

    public InSurvivalState(BattleFlowController context, CubeData cubeData, List<CubieFaceInfo> cubieFaceInfos)
    {
        this.cubieFaceInfos = cubieFaceInfos;
        this.cubeData = cubeData;
        this.field = context.GetField();
        this.nextStageTime = context.stageTime;
    }

    public void Enter()
    {
        var positions = cubieFaceInfos.ConvertAll(info => info.Position);
        Vector3 center = Utils.GetCentroid(positions);
        Vector3 flipped = new Vector3(-center.x * 10, 0.5f * 10, -center.z * 10);

        var fieldData = new FieldData()
        {
            position = flipped,
            faceinfos = cubieFaceInfos,
            size = cubeData.size,
            currentStageLevel = BattleFlowController.Instance.CurrentStage
        };

        field.Initialize(fieldData, SetCountDownState, SetGameOverState);
        Utils.GetUI<InSurvivalStateUI>().Initialize();
    }

    public void Update()
    {
        nextStageTime -= nextStageTime > 0 ? Time.deltaTime : 0;
        if (nextStageTime <= 0 && !isSpawnedNextStage)
        {
            isSpawnedNextStage = true;
            nextStageTime = 0;
            SpawnNextStage();
        }
    }

    public void Exit()
    {
        field.OnDisableField();
        Utils.GetUI<InSurvivalStateUI>().Disable();
    }

    public void SpawnNextStage() => field.SpawnNextStage();

    public void SetCountDownState() => BattleFlowController.Instance.SetCountDownState();

    public void SetGameOverState() => BattleFlowController.Instance.SetCompleteState(true);
}