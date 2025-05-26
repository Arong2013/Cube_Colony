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
   // 랜덤 각도 생성
   float randomAngle = UnityEngine.Random.Range(0f, 360f);
   
   // 랜덤 방향 벡터 생성
   Vector3 randomDirection = Quaternion.Euler(0, randomAngle, 0) * dropForce;
   
   // 랜덤 회전 추가 (x, z 축)
   float randomXRotation = UnityEngine.Random.Range(-30f, 30f);
   float randomZRotation = UnityEngine.Random.Range(-30f, 30f);
   randomDirection = Quaternion.Euler(randomXRotation, 0, randomZRotation) * randomDirection;

   // 힘 적용
   _rb.AddForce(randomDirection, ForceMode.Impulse);
   
   // 회전 속도 추가 (optional)
   Vector3 randomTorque = new Vector3(
       UnityEngine.Random.Range(-5f, 5f),
       UnityEngine.Random.Range(-5f, 5f),
       UnityEngine.Random.Range(-5f, 5f)
   );
   _rb.AddTorque(randomTorque, ForceMode.Impulse);

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