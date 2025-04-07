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

public enum CubieFaceSkillType
{
    Monster,
    Gold,
    Mine,
    Plant,
}
public enum UnitType
{
    Player,      // 플레이어 유닛
    Enemy,       // 적 유닛
    Ally,        // 아군 유닛
    AllyTower,   // 아군 타워 (같은 팀의 타워)
    NPC,         // Non-Player Character (NPC)
    Boss         // 보스 유닛
}
public enum PriorityNameType
{
    AttackEnemy,
    DefEnemy,
    AttackAlly,
    DefAlly
}

public enum BehaviorDataType
{
    TargetList
}


public enum EntityAnimTrigger
{
    AttackTrigger,
    HitTrigger,
    DieTrigger,
    DashTrigger
}
public enum EntityAnimBool
{
    IsMoving
}
public enum EntityAnimFloat
{
    Speed,
    MoveX,
    MoveY
}


public enum EntityStatName
{
    HP, MaxHP,
    SP, MaxSP,
    ATK,
    DEF,
    SPD,
}
public enum InteractionType
{
    Attack,
    Chop,
    Mine,
    Talk
}