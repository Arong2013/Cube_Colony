using UnityEngine;
using System;

public class InteractTask
{
    private readonly IInteractable _target;
    private readonly Transform _targetTransform;
    private bool _isComplete = false;

    public bool IsComplete => _isComplete;

    public InteractTask(IInteractable target)
    {
        _target = target;
        _targetTransform = (target as MonoBehaviour)?.transform;
    }

    public void Update(Entity entity, Action<Vector3> moveCallback)
    {
        if (_isComplete || _targetTransform == null)
        {
            _isComplete = true;
            return;
        }

        Vector3 origin = entity.transform.position;
        Vector3 targetPos = _targetTransform.position;
        float interactDist = _target.GetInteractionDistance();

        // 플레이어가 타겟보다 왼쪽에 있으면 오른쪽으로, 오른쪽에 있으면 왼쪽으로 위치 조정
        float xOffset = (origin.x < targetPos.x) ? -interactDist : interactDist;

        // Z는 타겟보다 살짝 아래쪽 (앞쪽)
        float zOffset = -0.2f;

        // 조정된 목표 위치
        Vector3 adjustedTargetPos = new Vector3(
            targetPos.x + xOffset,
            origin.y,
            targetPos.z + zOffset
        );

        Vector3 offset = adjustedTargetPos - origin;
        float dist = new Vector2(offset.x, offset.z).magnitude;

        if (dist > interactDist)
        {
            Vector3 dir = new Vector3(offset.x, 0f, offset.z).normalized;
            moveCallback?.Invoke(dir);
        }
        else
        {
            moveCallback?.Invoke(Vector3.zero);
            if (_target.CanInteract(entity))
            {
                _target.Interact(entity);
            }
            _isComplete = true;
        }
    }


}
