using UnityEngine;
using System;
using System.Collections.Generic;
public class InSurvivalState : IGameSequenceState
{
    private BattleFlowController context;
    private List<CubieFaceInfo> cubieFaceInfos;
    private CubeData cubeData;
    private Field field;
    public InSurvivalState(BattleFlowController context,CubeData cubeData,List<CubieFaceInfo> cubieFaceInfos)
    {
        this.context = context;
        this.cubieFaceInfos = cubieFaceInfos;
        this.cubeData = cubeData;
        this.field = context.GetField();    
    }
    public void Enter()
    {
        var positions = cubieFaceInfos.ConvertAll(info => info.Position);
        Vector3 center = Utils.GetCentroid(positions);
        Vector3 flipped = new Vector3(-center.x * 10, 0.5f * 10 , -center.z * 10);

        var fieldfata = new FieldData()
        {
            position = flipped,
            faceinfos = cubieFaceInfos,
            size = cubeData.size
        };
        field.Initialize(fieldfata);    
    }
    public void Update()
    {

    }
    public void Exit() { }


}
