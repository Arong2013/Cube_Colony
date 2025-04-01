using System;
using UnityEngine;

public class PCController : IEntityController
{
    private readonly Action<Vector3> _onMoveInput;

    public PCController(Action<Vector3> onMoveInput)
    {
        _onMoveInput = onMoveInput;
    }

    private float _interactionRadius = 2f;
    private IInteractable _currentTarget = null;

    public void Update(Entity entity)
    {
        // 1. 키보드 입력 (XZ 평면 기준)
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        bool hasInput = input != Vector2.zero;

        if (hasInput)
        {
            _onMoveInput?.Invoke(new Vector3(input.x, 0f, input.y).normalized);
            _currentTarget = null; // 수동 조작 시 자동 이동 취소
        }

        // 2. 좌클릭 → 가까우면 즉시 상호작용
        if (Input.GetMouseButtonDown(0))
        {
            TryInstantInteract(entity);
        }

        // 3. 우클릭 → 자동 이동 후 상호작용
        if (Input.GetMouseButtonDown(1))
        {

            _currentTarget = FindClosestInteractable(entity);
        }

        // 4. 자동 이동 중이면 계속 이동 → 도착 시 상호작용
        if (!hasInput && _currentTarget != null)
        {

            Vector3 origin = entity.transform.position;
            Vector3 targetPos = (_currentTarget as MonoBehaviour).transform.position;
            Vector3 offset = targetPos - origin;

            Vector2 xzOffset = new Vector2(offset.x, offset.z);
            float dist = xzOffset.magnitude;

            if (dist > 0.1f)
            {
                Vector3 moveDir = new Vector3(xzOffset.x, 0f, xzOffset.y).normalized;
                _onMoveInput?.Invoke(moveDir);
            }
            else
            {
                if (_currentTarget.CanInteract(entity))
                {
                    _currentTarget.Interact(entity);
                }
                _currentTarget = null;
            }
        }
    }

    private void TryInstantInteract(Entity entity)
    {
        Vector3 origin = entity.transform.position;
        Collider[] colliders = Physics.OverlapSphere(origin, _interactionRadius);

        IInteractable closest = null;
        float closestDist = float.MaxValue;

        foreach (var col in colliders)
        {
            var interactable = col.GetComponent<IInteractable>();
            if (interactable == null) continue;
            if (!interactable.CanInteract(entity)) continue;

            Vector3 offset = col.transform.position - origin;
            float dist = new Vector2(offset.x, offset.z).magnitude;

            if (dist < closestDist)
            {
                closestDist = dist;
                closest = interactable;
            }
        }

        if (closest != null)
        {
            closest.Interact(entity);
            Debug.Log($"[Click] {closest.GetInteractionLabel()} 즉시 상호작용 실행됨");
        }
        else
        {
            Debug.Log("즉시 상호작용 가능한 대상이 근처에 없음");
        }
    }

    private IInteractable FindClosestInteractable(Entity entity)
    {
        Vector3 origin = entity.transform.position;
        Collider[] colliders = Physics.OverlapSphere(origin, _interactionRadius);

        IInteractable closest = null;
        float closestDist = float.MaxValue;
        Debug.Log("아아");

        foreach (var col in colliders)
        {
            var interactable = col.GetComponent<IInteractable>();
            if (interactable == null) continue;
            if (!interactable.CanInteract(entity)) continue;

            Vector3 offset = col.transform.position - origin;
            float dist = new Vector2(offset.x, offset.z).magnitude;

            if (dist < closestDist)
            {
                closestDist = dist;
                closest = interactable;
            }
        }

        if (closest != null)
        {
            Debug.Log($"[AutoTarget] {closest.GetInteractionLabel()}이(가) 자동 타겟으로 설정됨");
        }

        return closest;
    }
}
