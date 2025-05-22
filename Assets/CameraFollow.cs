using UnityEngine;

public class CameraFollowConditional : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -7);
    public Vector3 defaultOffset = new Vector3(0, 5, -10);
    public float smoothSpeed = 5f;

    private bool followMode = false;

    public float maxYawAngle = 45f;
    private float initialYaw;

    public float rotationSmoothSpeed = 5f;

    private float currentYaw;

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("타겟이 설정되지 않았습니다.");
            return;
        }

        initialYaw = transform.eulerAngles.y;
        currentYaw = initialYaw;
    }

    void Update()
    {
        followMode = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) ||
                     Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = (followMode ? offset : defaultOffset) + target.position;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // 회전 처리
        Vector3 direction = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        float desiredYaw = ClampAngle(lookRotation.eulerAngles.y, initialYaw - maxYawAngle, initialYaw + maxYawAngle);

        // 회전을 부드럽게 보간
        currentYaw = Mathf.LerpAngle(currentYaw, desiredYaw, rotationSmoothSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(20f, currentYaw, 0f); // x=20: 고정된 약간 아래 시선
    }

    float ClampAngle(float angle, float min, float max)
    {
        angle = NormalizeAngle(angle);
        min = NormalizeAngle(min);
        max = NormalizeAngle(max);

        if (min < max)
            return Mathf.Clamp(angle, min, max);
        else
        {
            if (angle > max && angle < min)
            {
                float mid = (min + ((max + 360 - min) % 360) / 2f) % 360;
                return angle > mid ? min : max;
            }
            else return angle;
        }
    }

    float NormalizeAngle(float angle)
    {
        return (angle + 360) % 360;
    }
}
