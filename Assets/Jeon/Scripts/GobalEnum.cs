public enum BehaviorState
{
    SUCCESS,
    RUNNING,
    FAILURE, 
}
public enum BattlePhase
{
    Countdown,
    InBattle,
    Ended
}
public enum CubeAxisType
{
    X,       // X축 회전만 가능
    Y,       // Y축 회전만 가능
    Z        // Z축 회전만 가능
}
public enum CubeFaceType
{
    Front,  // 정면
    Back,   // 후면
    Left,   // 왼쪽
    Right,  // 오른쪽
    Top,    // 위쪽
    Bottom  // 아래쪽
}