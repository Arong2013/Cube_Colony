using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using DG.Tweening;

public class InteractableEntity : Entity, IInteractable
{
[TitleGroup("타격 효과")]
[LabelText("떨림 강도")]
[Range(0f, 5f)]
[SerializeField] private float shakeIntensity = 0.3f;

[TitleGroup("타격 효과")]
[LabelText("떨림 지속 시간")]
[Range(0f, 2f)]
[SerializeField] private float shakeDuration = 0.2f;

[TitleGroup("타격 효과")]
[LabelText("진동 횟수")]
[Range(1, 30)]
[SerializeField] private int shakeVibrato = 15;

[TitleGroup("타격 효과")]
[LabelText("무작위성")]
[Range(0f, 360f)]
[SerializeField] private float shakeRandomness = 120f;

    private Tweener shakeTweener;

    [SerializeField] private List<BehaviorSequenceSO> behaviorSequencesSO;
    [SerializeField] private ScriptableInteractionStrategy strategyAsset;
    [SerializeField] private float interactionDistance;
    private IInteractionStrategy _strategy;
    private bool initavle;


    [SerializeField] int MinDropItem;
    [SerializeField] int MaxDropItem;

    [SerializeField] Dictionary<float, int> DropChances;
    public override void Init()
    {
        base.Init();
        _strategy = strategyAsset?.CreateStrategy();
        _strategy?.Initialize(this);
        if (behaviorSequencesSO != null)
        {
            SetController(new AIController(behaviorSequencesSO, this));
        }
        initavle = true;
    }
    protected override void Update()
    {
        if (!initavle) return;
        base.Update();
    }
    public bool CanInteract(Entity interactor) => _strategy?.CanInteract(this, interactor) ?? false;
    public void Interact(Entity interactor) => _strategy?.Interact(this, interactor);
    public string GetInteractionLabel() => _strategy?.GetLabel() ?? "상호작용";
  public void DropItems()
{
    Field currentField = FindAnyObjectByType<Field>();
    
    if (currentField == null)
    {
        Debug.LogWarning("현재 필드를 찾을 수 없습니다.");
        return;
    }

    int dropCount = Random.Range(MinDropItem, MaxDropItem + 1);

    for (int i = 0; i < dropCount; i++)
    {
        foreach (var pair in DropChances)
        {
            float dropChance = pair.Key;
            int itemId = pair.Value;

            if (Random.value <= dropChance)
            {
                var itemPre = Instantiate(
                    DataCenter.Instance.GetDropItemPrefab().gameObject, 
                    transform.position, 
                    Quaternion.identity, 
                    currentField.transform.Find("DisableField")
                );     
                itemPre.GetComponent<ItemEntity>().SetItem(itemId);
                break; // 한 번의 드롭에 하나의 아이템만 생성
            }
        }
    }
}




    public float GetInteractionDistance() => interactionDistance;

    public override void OnHit(int dmg)
    {
if (shakeTweener != null && shakeTweener.IsActive())
   {
       shakeTweener.Kill();
   }

   // 새로운 흔들림 트윈 시작
   shakeTweener = transform
       .DOShakePosition(
           duration: shakeDuration,
           strength: new Vector3(shakeIntensity, 0, shakeIntensity),
           vibrato: shakeVibrato,
           randomness: shakeRandomness
       )
       .SetEase(Ease.InOutQuad);


    }

    public override void OnDeath()
    {
        DropItems();
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        if (shakeTweener != null && shakeTweener.IsActive())
        {
            shakeTweener.Kill();
        }

    }
}
