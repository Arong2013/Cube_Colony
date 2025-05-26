public class ChopComponent : IEntityComponent 
{
   private Entity _entity;
   private int maxChopCount = 3; // 고정된 최대 타격 횟수
   private float fixedDamage = 100f; // 고정 데미지

   public void Start(Entity entity) => _entity = entity;
   public void Update(Entity entity) { }
   public void Exit(Entity entity) { }

   public void Chop(Entity target)
   {
       if (target == null) return;
       target.TakeDamage(1);
   }
}