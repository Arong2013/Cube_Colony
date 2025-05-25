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
    private GameObject attackRangeVisualizer;
    private float visualDisplayTime = 0.3f; // 공격 범위 표시 시간
    
    [Header("공격 범위 시각화 설정")]
    [SerializeField] private Color attackRangeColor = new Color(1f, 0f, 0f, 0.3f);
    [SerializeField] private bool enablePulseEffect = true;
    [SerializeField] private bool enableWireframeMode = false;

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

        // 공격 범위 시각화
        CreateAttackRangeVisual(attackCenter, attackRotation);

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
        visualDisplayTime -= Time.deltaTime;
        
        // 일정 시간 후 시각화 제거
        if (visualDisplayTime <= 0 && attackRangeVisualizer != null)
        {
            Object.Destroy(attackRangeVisualizer);
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        // 상태 종료 시 시각화 오브젝트 제거
        if (attackRangeVisualizer != null)
        {
            Object.Destroy(attackRangeVisualizer);
        }
    }

    /// <summary>
    /// 런타임에서 공격 범위를 시각적으로 표시하는 메서드
    /// </summary>
    private void CreateAttackRangeVisual(Vector3 attackCenter, Quaternion attackRotation)
    {
        // 공격 범위 시각화용 GameObject 생성
        attackRangeVisualizer = new GameObject("AttackRangeVisualizer");
        attackRangeVisualizer.transform.position = attackCenter;
        attackRangeVisualizer.transform.rotation = attackRotation;

        // 반투명 큐브 메시 렌더러 추가
        MeshRenderer meshRenderer = attackRangeVisualizer.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = attackRangeVisualizer.AddComponent<MeshFilter>();

        // 기본 큐브 메시 사용
        meshFilter.mesh = CreateCubeMesh();

        // 반투명 머티리얼 생성
        Material attackRangeMaterial = new Material(Shader.Find("Standard"));
        attackRangeMaterial.color = attackRangeColor;
        attackRangeMaterial.SetFloat("_Mode", 3); // Transparent 모드
        attackRangeMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        attackRangeMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        attackRangeMaterial.SetInt("_ZWrite", 0);
        attackRangeMaterial.DisableKeyword("_ALPHATEST_ON");
        attackRangeMaterial.EnableKeyword("_ALPHABLEND_ON");
        attackRangeMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        attackRangeMaterial.renderQueue = 3000;

        meshRenderer.material = attackRangeMaterial;

        // 크기 조정
        attackRangeVisualizer.transform.localScale = new Vector3(attackBoxWidth, attackBoxHeight, attackBoxLength);

        // 와이어프레임 모드 설정
        if (enableWireframeMode)
        {
            // 와이어프레임용 라인 렌더러 추가
            CreateWireframeEffect();
        }
    }

    /// <summary>
    /// 큐브 메시 생성
    /// </summary>
    private Mesh CreateCubeMesh()
    {
        Mesh mesh = new Mesh();

        // 큐브의 정점
        Vector3[] vertices = new Vector3[]
        {
            // 앞면
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            // 뒷면
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f)
        };

        // 삼각형 인덱스
        int[] triangles = new int[]
        {
            // 앞면
            0, 2, 1, 0, 3, 2,
            // 뒷면
            5, 6, 4, 4, 6, 7,
            // 왼쪽면
            4, 7, 0, 0, 7, 3,
            // 오른쪽면
            1, 6, 5, 1, 2, 6,
            // 위쪽면
            3, 6, 2, 3, 7, 6,
            // 아래쪽면
            4, 1, 5, 4, 0, 1
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    /// <summary>
    /// 와이어프레임 효과 생성
    /// </summary>
    private void CreateWireframeEffect()
    {
        GameObject wireframe = new GameObject("AttackRangeWireframe");
        wireframe.transform.SetParent(attackRangeVisualizer.transform);
        wireframe.transform.localPosition = Vector3.zero;
        wireframe.transform.localRotation = Quaternion.identity;
        wireframe.transform.localScale = Vector3.one;

        LineRenderer lineRenderer = wireframe.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(attackRangeColor.r, attackRangeColor.g, attackRangeColor.b, 1f);
        lineRenderer.widthMultiplier = 0.05f;
        lineRenderer.useWorldSpace = false;

        // 큐브의 와이어프레임 라인 생성
        Vector3[] wireframePoints = new Vector3[]
        {
            // 아래면 사각형
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            // 위로 올라가는 선
            new Vector3(-0.5f, 0.5f, -0.5f),
            // 위면 사각형
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
        };

        lineRenderer.positionCount = wireframePoints.Length;
        lineRenderer.SetPositions(wireframePoints);
    }

}
