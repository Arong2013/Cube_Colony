using NUnit.Framework;
using System.Collections.Generic;

public interface IBattleState
{
    void Enter();
    void Exit();
    void Update();
}
public interface IAstarable
{
    List<CubieFace> GetAstarPathFaces(CubieFace start, CubieFace target);
}
public interface IBehaviorDatable { }