using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerWeaonController : SerializedMonoBehaviour
{
    [TitleGroup("기본 설정", order: 0)]
    [LabelText("플레이어 엔티티"), Required, Tooltip("무기 소유자")]
    [SerializeField] PlayerEntity playerEntity;

    [TitleGroup("공격 설정", order: 1)]
    [LabelText("데미지 배율"), Range(0.1f, 5f), Tooltip("기본 공격력에 곱해지는 배율")]
    [SerializeField] private float damageMultiplier = 1.0f;

    [TitleGroup("공격 설정")]
    [LabelText("공격 타입"), Tooltip("데미지 계산 방식")]
    [SerializeField] private AttackType attackType = AttackType.Normal;

    [TitleGroup("디버그 설정", order: 2)]
    [LabelText("디버그 모드"), ToggleLeft, Tooltip("충돌 및 데미지 정보를 로그에 출력")]
    [SerializeField] private bool debugMode = true;

    [TitleGroup("디버그 설정")]
    [LabelText("데미지 색상"), ColorUsage(true), ShowIf("debugMode"), Tooltip("디버그 텍스트 색상")]
    [SerializeField] private Color debugColor = Color.red;

    // 공격 타입 열거형
    public enum AttackType
    {
        [LabelText("일반 공격")] Normal,
        [LabelText("치명타 가능")] CriticalHit,
        [LabelText("범위 공격")] AreaOfEffect
    }

    [FoldoutGroup("충돌 정보", false, order: 99)]
    [ReadOnly, ShowInInspector]
    private int hitCount = 0;

    [FoldoutGroup("충돌 정보")]
    [ReadOnly, ShowInInspector]
    private float lastDamage = 0f;

    [FoldoutGroup("충돌 정보")]
    [ReadOnly, ShowInInspector]
    private string lastHitEntity = "없음";

    private void OnTriggerEnter(Collider other)
    {
        // Entity 컴포넌트가 있는지 확인
        if (other.TryGetComponent<Entity>(out Entity targetEntity))
        {
            // 자기 자신은 제외
            if (targetEntity == playerEntity)
                return;

            // 플레이어의 공격력 계산
            float playerAttack = CalculateDamage();

            // 데미지 적용
            targetEntity.TakeDamage(playerAttack);

            // 디버그 정보 업데이트
            hitCount++;
            lastDamage = playerAttack;
            lastHitEntity = targetEntity.name;

            // 디버그 로그
            if (debugMode)
            {
                Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColor)}>[무기 충돌] {playerEntity.name}가 {targetEntity.name}에게 {playerAttack} 데미지를 입혔습니다.</color>");
            }
        }
    }

    [Button("데미지 계산 테스트", ButtonSizes.Medium), GUIColor(0.3f, 0.7f, 0.9f)]
    private float CalculateDamage()
    {
        float baseAttack = playerEntity != null ? playerEntity.GetEntityStat(EntityStatName.ATK) : 10f;
        float finalDamage = baseAttack * damageMultiplier;

        // 공격 타입별 추가 계산
        switch (attackType)
        {
            case AttackType.CriticalHit:
                // 20% 확률로 치명타 (2배 데미지)
                if (UnityEngine.Random.value < 0.2f)
                {
                    finalDamage *= 2f;
                    if (debugMode) Debug.Log("<color=yellow>치명타!</color>");
                }
                break;

            case AttackType.AreaOfEffect:
                // 범위 공격은 데미지가 75%로 감소
                finalDamage *= 0.75f;
                break;
        }

        return finalDamage;
    }

    [FoldoutGroup("충돌 정보")]
    [Button("충돌 정보 초기화", ButtonSizes.Small), GUIColor(0.9f, 0.3f, 0.3f)]
    private void ResetHitInfo()
    {
        hitCount = 0;
        lastDamage = 0f;
        lastHitEntity = "없음";
    }
}