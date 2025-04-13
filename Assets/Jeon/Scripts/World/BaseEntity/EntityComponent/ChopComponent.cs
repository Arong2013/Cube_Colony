using UnityEngine;

public class ChopComponent : IEntityComponent
{
    public void Start(Entity entity) { }
    public void Update(Entity entity) { }
    public void Exit(Entity entity) { }
    public void Chop(Entity target)
    {
        float power = 5f; 
        target.TakeDamage(power);
        Debug.Log("[Chop] 나무를 쳤습니다!");
    }
}
