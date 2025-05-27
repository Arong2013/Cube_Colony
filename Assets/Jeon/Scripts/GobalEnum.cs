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
    RMonster = 0,
    AMonster = 1,
    Mine = 2,
    Plant = 3,
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


public enum EntityActionType
{
    Idle = 0,
    Move = 1,
    Attack = 2,
    Return = 3
}
public enum EntityAnimInt
{
    ActionType
}
public enum EntityAnimTrigger
{
    HitTrigger,
    DieTrigger,
    DashTrigger,
}
public enum EntityAnimBool
{
    AttackRight
}
public enum EntityAnimFloat
{
    Speed,
    MoveX,
    MoveY,
}

public enum EntityStatName
{
    HP,             // 현재 체력
    MaxHP,          // 최대 체력
    O2,             // 현재 산소량
    MaxOxygen,      // 최대 산소량 (MaxO2를 MaxOxygen으로 변경)
    Eng,            // 현재 에너지
    MaxEnergy,      // 최대 에너지량 (MaxEng를 MaxEnergy로 변경)
    Attack,         // 공격력 (ATK를 Attack으로 변경)
    Defense,        // 방어력 (DEF를 Defense로 변경)
    SPD,            // 이동 속도

    // 새로 추가된 필드들
    MaxO2 = MaxOxygen,       // 이전 코드와의 호환성을 위해 별칭 제공
    MaxEng = MaxEnergy,      // 이전 코드와의 호환성을 위해 별칭 제공
    ATK = Attack,            // 이전 코드와의 호환성을 위해 별칭 제공
    DEF = Defense            // 이전 코드와의 호환성을 위해 별칭 제공
}
public enum InteractionType
{
    Attack,
    Chop,
    Mine,
    Talk
}