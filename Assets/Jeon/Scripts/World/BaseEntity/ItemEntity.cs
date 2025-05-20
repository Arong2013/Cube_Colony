using System;
using UnityEngine;

public class ItemEntity : Entity, IInteractable
{
    [SerializeField] private Item _item;
    [SerializeField] private SpriteRenderer itemSprite;
    [Header("Drop Physics")]
    [SerializeField] private Vector3 dropForce = new Vector3(1f, 5f, 0f);
    [SerializeField] private float stopAfter = 1.5f;

    private bool isCollectable = false;

    private Rigidbody _rb;

    public override void Init()
    {
        base.Init();
        _rb = GetComponent<Rigidbody>();
        ApplyDropPhysics();
    }

    public bool CanInteract(Entity interactor)
    {
        if (!isCollectable)
            return false;

        // 이제 PlayerEntity가 인벤토리를 갖고 있지 않음
        return interactor is PlayerEntity;
    }

    public float GetInteractionDistance() => 1f;

    public string GetInteractionLabel()
    {
        return "줍기";
    }

    public void Interact(Entity interactor)
    {
        // PlayerEntity에 직접 아이템 추가
        if (interactor is PlayerEntity playerEntity)
        {
            if (playerEntity.AddItem(_item))
            {
                Destroy(gameObject);
            }
        }
    }

    public override void OnDeath()
    {
        // 죽었을 때 처리
    }

    public override void OnHit(int dmg)
    {
        // 맞았을 때 처리
    }

    private void ApplyDropPhysics()
    {
        _rb.AddForce(dropForce, ForceMode.Impulse);
        Invoke(nameof(FreezeDrop), stopAfter);
    }

    private void FreezeDrop()
    {
        if (_rb == null) return;

        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.isKinematic = true;
        isCollectable = true;
    }

    public void SetItem(int itemId)
    {
        _item = DataCenter.Instance.GetCloneData<Item>(itemId);
        itemSprite.sprite = _item.ItemIcon;
        Init();
    }
}