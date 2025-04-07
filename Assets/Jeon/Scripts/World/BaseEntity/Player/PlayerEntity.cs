using UnityEngine;
using static UnityEngine.UI.ScrollRect;

public class PlayerEntity : Entity
{
    public bool CanWalk => (Mathf.Abs(CurrentDir.x) > 0.1f || Mathf.Abs(CurrentDir.z) > 0.1f);
    protected override void Awake()
    {
        base.Awake();
        SetController(new PCController(OnMoveInput));
        Initialize();
    }
    private void OnPlayerDamaged(int dmg)
    {
        Debug.Log($"[UI] Player took {dmg} damage!");
    }
    private void OnPlayerDeath()
    {
        Debug.Log("[UI] Player died!");
    }
    private void OnMoveInput(Vector3 direction) => SetDir(direction);

    public override void Initialize()
    {
        AddEntityComponent(new AttackComponent());  
    }

    protected override void Update()
    {
        base.Update();
        if (CanWalk)
            SetAnimatorValue(EntityAnimBool.IsMoving, true);
        else
            SetAnimatorValue(EntityAnimBool.IsMoving, false);
    }
}   

