using UnityEngine;
using Spine.Unity;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 20f;
    private Vector3 moveDirection;
    private Rigidbody rb;
    public SkeletonAnimation skeletonAnimation;
    public string walkAnimationName = "walk";
    public string idleAnimationName = "idle"; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        skeletonAnimation = GetComponent<SkeletonAnimation>();

        rb.freezeRotation = true;
        rb.useGravity = true; // 중력 사용 활성화
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // 이동 방향 설정
        moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // 애니메이션 변경
        if (moveDirection != Vector3.zero)
        {
            skeletonAnimation.AnimationName = walkAnimationName;
        }
        else
        {
            skeletonAnimation.AnimationName = idleAnimationName;
        }

        // 좌우 반전 처리
        if (moveX > 0)
        {
            // 오른쪽 이동 시 → 반전 (x = -1)
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (moveX < 0)
        {
            // 왼쪽 이동 시 → 원래 방향 (x = 1)
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void FixedUpdate()
    {
        Vector3 newPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(new Vector3(newPosition.x, rb.position.y, newPosition.z)); // Y는 유지
    }
}