using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

public class PCController : IEntityController
{
    private readonly Action<Vector3> _onMoveInput;
    private bool _wasMoving = false;

    public PCController(Action<Vector3> onMoveInput)
    {
        _onMoveInput = onMoveInput;
    }

    public void Update(Entity entity)
    {
        HandleMovementInput(entity);

        // 좌클릭 시 공격 애니메이션 실행
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                entity.OnAttackAnime(); // UI를 클릭한 게 아니라면 공격 수행
            }
        }

        // 우클릭 시 즉시 상호작용
        if (Input.GetMouseButtonDown(1))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                InteractWithClosestEntity(entity);
            }
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
        }
        else if (_wasMoving)
        {
            _onMoveInput?.Invoke(Vector3.zero);
            _wasMoving = false;
        }
    }

private void InteractWithClosestEntity(Entity entity)
{
    Vector3 origin = entity.transform.position;
    Collider[] colliders = Physics.OverlapSphere(origin, 10f);

    IInteractable closest = null;
    float closestDist = float.MaxValue;

    foreach (var col in colliders)
    {
        var interactable = col.GetComponent<IInteractable>();
        if (interactable == null || !interactable.CanInteract(entity))
            continue;

        // 상대방이 InteractableEntity이고 AttackComponent나 ChopComponent를 가진 경우 제외
        InteractableEntity interactableEntity = col.GetComponent<InteractableEntity>();
        if (interactableEntity != null && 
            (interactableEntity.HasEntityComponent<AttackComponent>() || 
             interactableEntity.HasEntityComponent<ChopComponent>()))
            continue;

        Vector3 offset = col.transform.position - origin;
        float dist = new Vector2(offset.x, offset.z).magnitude;
        if (dist < closestDist)
        {
            closestDist = dist;
            closest = interactable;
        }
    }

    // 가장 가까운 상호작용 대상을 즉시 상호작용
    if (closest != null && closestDist <= closest.GetInteractionDistance())
    {
        closest.Interact(entity);
    }
}
}