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
        Vector3 offset = targetPos - origin;
        float dist = new Vector2(offset.x, offset.z).magnitude;

        float interactDist = _target.GetInteractionDistance();

        if (dist > interactDist)
        {
            Vector3 dir = new Vector3(offset.x, 0f, offset.z).normalized;
            moveCallback?.Invoke(dir);
            Debug.Log($"[InteractTask] {entity.name}가 {_target.GetInteractionLabel()}에 접근합니다. 거리: {dist}"); 
        }
        else
        {
            if (_target.CanInteract(entity))
            {
                _target.Interact(entity);
            }

            moveCallback?.Invoke(Vector3.zero);
            _isComplete = true;
        }
    }
}
