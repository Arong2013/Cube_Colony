using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleState
{
    void Enter();
    void Exit();
    void Update();
}
public interface IAstarable
{
    List<CubieFace> GetAstarPathFaces(CubieFace start, CubieFace target);
    CubieFace GetCubieFaceInPos(CubeFaceType cubeFaceType, Vector3 pos);
}
public interface IBehaviorDatable { }