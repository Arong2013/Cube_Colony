using UnityEngine;

public class PlayerWeaonController : MonoBehaviour
{
    [SerializeField] PlayerEntity playerEntity;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<InteractableEntity>(out InteractableEntity component))
        {
            var atkCom =  component.GetEntityComponent<AttackComponent>();
            if(atkCom != null)
            {
                atkCom.Attack(playerEntity);    
            }
        }
    }
}
