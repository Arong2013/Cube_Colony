using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerWeaonController : SerializedMonoBehaviour
{
    [TitleGroup("�⺻ ����", order: 0)]
    [LabelText("�÷��̾� ��ƼƼ"), Required, Tooltip("���� ������")]
    [SerializeField] PlayerEntity playerEntity;

    [TitleGroup("���� ����", order: 1)]
    [LabelText("������ ����"), Range(0.1f, 5f), Tooltip("�⺻ ���ݷ¿� �������� ����")]
    [SerializeField] private float damageMultiplier = 1.0f;

    [TitleGroup("���� ����")]
    [LabelText("���� Ÿ��"), Tooltip("������ ��� ���")]
    [SerializeField] private AttackType attackType = AttackType.Normal;

    [TitleGroup("����� ����", order: 2)]
    [LabelText("����� ���"), ToggleLeft, Tooltip("�浹 �� ������ ������ �α׿� ���")]
    [SerializeField] private bool debugMode = true;

    [TitleGroup("����� ����")]
    [LabelText("������ ����"), ColorUsage(true), ShowIf("debugMode"), Tooltip("����� �ؽ�Ʈ ����")]
    [SerializeField] private Color debugColor = Color.red;

    // ���� Ÿ�� ������
    public enum AttackType
    {
        [LabelText("�Ϲ� ����")] Normal,
        [LabelText("ġ��Ÿ ����")] CriticalHit,
        [LabelText("���� ����")] AreaOfEffect
    }

    [FoldoutGroup("�浹 ����", false, order: 99)]
    [ReadOnly, ShowInInspector]
    private int hitCount = 0;

    [FoldoutGroup("�浹 ����")]
    [ReadOnly, ShowInInspector]
    private float lastDamage = 0f;

    [FoldoutGroup("�浹 ����")]
    [ReadOnly, ShowInInspector]
    private string lastHitEntity = "����";

    private void OnTriggerEnter(Collider other)
    {
        // Entity ������Ʈ�� �ִ��� Ȯ��
        if (other.TryGetComponent<Entity>(out Entity targetEntity))
        {
            // �ڱ� �ڽ��� ����
            if (targetEntity == playerEntity)
                return;

            // �÷��̾��� ���ݷ� ���
            float playerAttack = CalculateDamage();

            // ������ ����
            targetEntity.TakeDamage(playerAttack);

            // ����� ���� ������Ʈ
            hitCount++;
            lastDamage = playerAttack;
            lastHitEntity = targetEntity.name;

            // ����� �α�
            if (debugMode)
            {
                Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColor)}>[���� �浹] {playerEntity.name}�� {targetEntity.name}���� {playerAttack} �������� �������ϴ�.</color>");
            }
        }
    }

    [Button("������ ��� �׽�Ʈ", ButtonSizes.Medium), GUIColor(0.3f, 0.7f, 0.9f)]
    private float CalculateDamage()
    {
        float baseAttack = playerEntity != null ? playerEntity.GetEntityStat(EntityStatName.ATK) : 10f;
        float finalDamage = baseAttack * damageMultiplier;

        // ���� Ÿ�Ժ� �߰� ���
        switch (attackType)
        {
            case AttackType.CriticalHit:
                // 20% Ȯ���� ġ��Ÿ (2�� ������)
                if (UnityEngine.Random.value < 0.2f)
                {
                    finalDamage *= 2f;
                    if (debugMode) Debug.Log("<color=yellow>ġ��Ÿ!</color>");
                }
                break;

            case AttackType.AreaOfEffect:
                // ���� ������ �������� 75%�� ����
                finalDamage *= 0.75f;
                break;
        }

        return finalDamage;
    }

    [FoldoutGroup("�浹 ����")]
    [Button("�浹 ���� �ʱ�ȭ", ButtonSizes.Small), GUIColor(0.9f, 0.3f, 0.3f)]
    private void ResetHitInfo()
    {
        hitCount = 0;
        lastDamage = 0f;
        lastHitEntity = "����";
    }
}