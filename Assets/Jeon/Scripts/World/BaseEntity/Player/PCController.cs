using System;
using UnityEngine;

public class PCController : IEntityController
{
    private readonly Action<Vector3> _onMoveInput;
    private IInteractable _currentTarget = null;
    private bool _wasMoving = false;
    private InteractTask _currentTask = null;

    public PCController(Action<Vector3> onMoveInput)
    {
        _onMoveInput = onMoveInput;
    }

    public void Update(Entity entity)
    {
        HandleMovementInput(entity);

        // Space키를 누르고 있는 동안, (반복적으로 상호작용 대상 탐색)
        if (Input.GetKey(KeyCode.Space))
        {
            if (_currentTask == null || _currentTask.IsComplete)
            {
                TryStartInteractTask(entity);
            }
        }

        _currentTask?.Update(entity, _onMoveInput);

        if (_currentTask?.IsComplete == true)
        {
            _currentTask = null;
        }
    }

    private void HandleMovementInput(Entity entity)
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool hasInput = input != Vector2.zero;

        if (hasInput)
        {
            _onMoveInput?.Invoke(new Vector3(input.x, 0f, input.y).normalized);
            _wasMoving = true;
            _currentTask = null; // 수동 조작 시 자동 상호작용 취소
        }
        else if (_wasMoving)
        {
            _onMoveInput?.Invoke(Vector3.zero);
            _wasMoving = false;
        }
    }

    private void TryStartInteractTask(Entity entity)
    {
        IInteractable target = FindClosestInteractable(entity, maxDistance: 10f);
        if (target != null)
        {
            _currentTask = new InteractTask(target);
           // Debug.Log($"[Space] {target.GetInteractionLabel()} 상호작용을 시도합니다.");
        }
        else
        {
            //Debug.Log("주변에 상호작용 가능한 대상이 없습니다.");
        }
    }

    /// <summary>
    /// 주변에서 상호작용 가능한 대상을 탐색만 합니다.
    /// 이 함수는 단순히 대상 선택(의사결정)을 위한 코드로, 직접 행동을 수행하지 않습니다.
    /// </summary>
    /// <param name="entity">탐색을 시작할 엔티티</param>
    /// <param name="maxDistance">탐색 최대 거리</param>
    /// <returns>조건에 맞는 가장 가까운 상호작용 대상</returns>
    private IInteractable FindClosestInteractable(Entity entity, float maxDistance)
    {
        Vector3 origin = entity.transform.position;
        Collider[] colliders = Physics.OverlapSphere(origin, maxDistance);

        IInteractable closest = null;
        float closestDist = float.MaxValue;

        foreach (var col in colliders)
        {
            var interactable = col.GetComponent<IInteractable>();
            if (interactable == null || !interactable.CanInteract(entity))
                continue;

            Vector3 offset = col.transform.position - origin;
            float dist = new Vector2(offset.x, offset.z).magnitude;

            // 대상이 자신의 상호작용 거리 내에 있고, 현재까지의 최단 거리보다 가까우면 선택
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = interactable;
            }
        }
        return closest;
    }
}
