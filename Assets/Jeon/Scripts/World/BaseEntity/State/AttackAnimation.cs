using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class AttackAnimation : StateMachineBehaviour
{
    private Entity _entity;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _entity ??= animator.GetComponent<Entity>();
        _entity.ChangePlayerState(new AttackState(_entity));
    }
}
public class AttackState : EntityState
{
    private float attackBoxWidth = 1f;
    private float attackBoxHeight = 1f;
    private float attackBoxLength = 2f;

    private Camera mainCamera;
    private Vector3 mouseWorldPosition;
    private Vector3 attackDirection;

    public AttackState(Entity _entity) : base(_entity)
    {
        mainCamera = Camera.main;
    }

    public override void Enter()
    {
        base.Enter();

        // 마우스 위치 계산
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, _entity.transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            mouseWorldPosition = ray.GetPoint(distance);
        }

        // 공격 방향 계산
        attackDirection = (mouseWorldPosition - _entity.transform.position).normalized;

        // 공격 박스 생성을 위한 위치 및 회전 계산
        Vector3 attackCenter = _entity.transform.position + attackDirection * (attackBoxLength / 2);
        Quaternion attackRotation = Quaternion.LookRotation(attackDirection);

        // 안전한 박스 크기 계산 (절대값 사용)
        Vector3 safeBoxSize = new Vector3(
            Mathf.Abs(attackBoxWidth / 2),
            Mathf.Abs(attackBoxHeight / 2),
            Mathf.Abs(attackBoxLength / 2)
        );

        Debug.Log($"[AttackState] Attack Box Debug Info:");
        Debug.Log($"Entity Position: {_entity.transform.position}");
        Debug.Log($"Mouse World Position: {mouseWorldPosition}");
        Debug.Log($"Attack Direction: {attackDirection}");
        Debug.Log($"Attack Center: {attackCenter}");
        Debug.Log($"Attack Rotation: {attackRotation.eulerAngles}");
        Debug.Log($"Safe Box Size: {safeBoxSize}");

        // 공격 범위 콜라이더 생성
        Collider[] hitColliders = Physics.OverlapBox(
            attackCenter,
            safeBoxSize,
            attackRotation
        );

        Debug.Log($"Detected Colliders: {hitColliders.Length}");

        List<Entity> targetEntities = new List<Entity>();

        foreach (Collider hitCollider in hitColliders)
        {
            // 자기 자신은 제외
            if (hitCollider.gameObject == _entity.gameObject)
                continue;

            Entity targetEntity = hitCollider.GetComponent<Entity>();
            if (targetEntity != null)
            {
                Debug.Log($"Target Entity Found: {targetEntity.name}");
                targetEntities.Add(targetEntity);
            }
        }

        // 공격 컴포넌트 확인
        var attackComponent = _entity.GetEntityComponent<AttackComponent>();
        var chopComponent = _entity.GetEntityComponent<ChopComponent>();

        if (attackComponent != null)
        {
            Debug.Log($"Attacking with AttackComponent: {targetEntities.Count} targets");
            attackComponent.Attack(targetEntities);
        }

        if (chopComponent != null)
        {
            Debug.Log($"Chopping with ChopComponent: {targetEntities.Count} targets");
            foreach (var target in targetEntities)
            {
                chopComponent.Chop(target);
            }

        }
    }

    public override void Execute()
    {
        // 공격 상태 유지 로직 (필요한 경우 추가)
    }

    public override void Exit()
    {
        base.Exit();
    }

    // 씬 뷰에서 공격 박스 시각화
    private void OnDrawGizmos()
    {
        if (_entity == null || mainCamera == null) return;

        Gizmos.color = Color.red;

        // 공격 방향 표시
        Gizmos.DrawRay(_entity.transform.position, attackDirection * attackBoxLength);

        // 공격 박스 표시
        Vector3 attackCenter = _entity.transform.position + attackDirection * (attackBoxLength / 2);
        Quaternion attackRotation = Quaternion.LookRotation(attackDirection);

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(attackCenter, attackRotation, Vector3.one);

        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(attackBoxWidth, attackBoxHeight, attackBoxLength));
    }
}