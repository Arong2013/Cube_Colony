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
        rb.useGravity = true; // �߷� ��� Ȱ��ȭ
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // �̵� ���� ����
        moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // �ִϸ��̼� ����
        if (moveDirection != Vector3.zero)
        {
            skeletonAnimation.AnimationName = walkAnimationName;
        }
        else
        {
            skeletonAnimation.AnimationName = idleAnimationName;
        }

        // �¿� ���� ó��
        if (moveX > 0)
        {
            // ������ �̵� �� �� ���� (x = -1)
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (moveX < 0)
        {
            // ���� �̵� �� �� ���� ���� (x = 1)
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void FixedUpdate()
    {
        Vector3 newPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(new Vector3(newPosition.x, rb.position.y, newPosition.z)); // Y�� ����
    }
}