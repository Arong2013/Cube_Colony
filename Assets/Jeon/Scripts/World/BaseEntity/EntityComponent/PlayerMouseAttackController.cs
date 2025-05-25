using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerMouseAttackController : MonoBehaviour
{
    [TitleGroup("Attack Settings")]
    [SerializeField] private float attackBoxWidth = 1f;
    [SerializeField] private float attackBoxHeight = 1f;
    [SerializeField] private float attackBoxLength = 2f;
    [SerializeField] private float attackCooldown = 0.5f;

    [TitleGroup("References")]
    [SerializeField] private PlayerEntity playerEntity;
    [SerializeField] private Camera mainCamera;

    [TitleGroup("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private Color gizmoColor = Color.red;

    private float lastAttackTime;
    private Vector3 mouseWorldPosition;
    private Vector3 attackDirection;

    private void Start()
    {
        // 카메라가 설정되지 않았다면 메인 카메라 사용
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        // 마우스 위치 계산
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        
        if (groundPlane.Raycast(ray, out float distance))
        {
            mouseWorldPosition = ray.GetPoint(distance);
        }

        // 공격 방향 계산
        attackDirection = (mouseWorldPosition - transform.position).normalized;

        // 방향에 따른 이동 방향 설정
        UpdateMoveDirection();

        // 공격 입력 처리
        if (Input.GetMouseButtonDown(0) && CanAttack())
        {
            PerformAttack();
        }
    }

    private void UpdateMoveDirection()
    {
        // 공격 방향에 따라 이동 방향 조정
        Vector3 moveDirection = new Vector3(attackDirection.x, 0, attackDirection.z).normalized;
        playerEntity.SetDir(moveDirection);
    }

    private bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    private void PerformAttack()
    {
        lastAttackTime = Time.time;

        // 공격 박스 생성을 위한 위치 및 회전 계산
        Vector3 attackCenter = transform.position + attackDirection * (attackBoxLength / 2);
        Quaternion attackRotation = Quaternion.LookRotation(attackDirection);

        // 공격 범위 콜라이더 생성
        Collider[] hitColliders = Physics.OverlapBox(
            attackCenter, 
            new Vector3(attackBoxWidth / 2, attackBoxHeight / 2, attackBoxLength / 2), 
            attackRotation
        );

        // 각 충돌체에 대해 처리
        foreach (Collider hitCollider in hitColliders)
        {
            // 자기 자신은 제외
            if (hitCollider.gameObject == gameObject)
                continue;

            // 공격 컴포넌트 확인
            var attackComponent = playerEntity.GetEntityComponent<AttackComponent>();
            var chopComponent = playerEntity.GetEntityComponent<ChopComponent>();

            Entity targetEntity = hitCollider.GetComponent<Entity>();
            if (targetEntity != null)
            {
                if (attackComponent != null)
                {
                    attackComponent.Attack(targetEntity);
                }
                else if (chopComponent != null)
                {
                    chopComponent.Chop(targetEntity);
                }
            }
        }
    }

    // 디버그용 기즈모 그리기
    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        Gizmos.color = gizmoColor;
        
        // 공격 방향 표시
        Gizmos.DrawRay(transform.position, attackDirection * attackBoxLength);

        // 공격 박스 표시
        if (Application.isPlaying)
        {
            Vector3 attackCenter = transform.position + attackDirection * (attackBoxLength / 2);
            Quaternion attackRotation = Quaternion.LookRotation(attackDirection);
            
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(attackCenter, attackRotation, Vector3.one);
            
            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(attackBoxWidth, attackBoxHeight, attackBoxLength));
        }
    }

    [Button("Test Attack")]
    private void TestAttack()
    {
        PerformAttack();
    }
}