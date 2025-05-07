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

        // Space키를 누르면 상호작용 시도
        if (Input.GetKey(KeyCode.Space))
        {
            if (_currentTask == null || _currentTask.IsComplete)
            {
                TryStartInteractTask(entity);
            }
        }

        if (_currentTask?.IsComplete == true)
        {
            _currentTask = null;
        }

        _currentTask?.Update(entity, _onMoveInput);

        // ✅ 좌클릭 시 공격 애니메이션 실행
        if (Input.GetMouseButtonDown(0))
        {
            entity.OnAttackAnime();
        }

        // 인벤토리 토글
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory(entity);
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
            _currentTask = null;
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
        }
    }

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
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = interactable;
            }
        }
        return closest;
    }

    private void ToggleInventory(Entity entity)
    {
        Utils.GetUI<InventoryUI>().ToggleInventoryUI();
    }
}
