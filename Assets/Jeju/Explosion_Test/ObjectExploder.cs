using UnityEngine;
using RayFire;

public class ObjectExploder : MonoBehaviour
{
    public GameObject objectToExplode;
    public float explosionForce = 250f;
    public float explosionRadius = 0.5f;
    public int fragmentAmount = 10;
    public int maxHP = 5;
    private int currentHP;
    private bool isDestroyed = false;

    void Start()
    {
        currentHP = maxHP;
        if (objectToExplode == null)
        {
            // objectToExplode가 설정되지 않은 경우, 이 GameObject가 폭발할 대상이라고 가정합니다.
            objectToExplode = gameObject;
        }
        StartCoroutine(DealPeriodicDamage()); // 주기적인 데미지 코루틴 시작
    }

    System.Collections.IEnumerator DealPeriodicDamage()
    {
        while (!isDestroyed)
        {
            yield return new WaitForSeconds(1f); // 1초 대기
            if (!isDestroyed) // 대기 중에 파괴되었는지 다시 확인
            {
                TakeDamage(1);
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDestroyed) return;

        currentHP -= damageAmount;
        Debug.Log(objectToExplode.name + " took " + damageAmount + " damage. Current HP: " + currentHP);

        if (currentHP <= 0)
        {
            ExplodeObject();
            isDestroyed = true;
        }
    }

    void ExplodeObject()
    {
        if (objectToExplode == null)
        {
            Debug.LogError("Object to explode is not assigned or is null!");
            return;
        }
        
        Debug.Log(objectToExplode.name + " HP is 0. Exploding!");

        // 1. RayfireShatter 추가 및 설정
        RayfireShatter shatter = objectToExplode.GetComponent<RayfireShatter>();
        if (shatter == null) // 컴포넌트가 없으면 추가합니다.
        {
            shatter = objectToExplode.AddComponent<RayfireShatter>();
        }
        shatter.type = FragType.Voronoi; // 또는 다른 조각화 유형
        shatter.voronoi.amount = fragmentAmount;
        shatter.mode = FragmentMode.Runtime; // 런타임에 실행되도록 보장

        // 2. 오브젝트 조각화
        shatter.Fragment();

        // 조각이 생성되었는지 확인
        if (shatter.fragmentsLast == null || shatter.fragmentsLast.Count == 0)
        {
            Debug.LogError("오브젝트 조각화 실패: " + objectToExplode.name);
            // 조각화 실패 시 오브젝트를 직접 파괴 시도
            // Destroy(objectToExplode); 
            return;
        }

        // 3. 조각에 RayfireRigid를 추가하고 폭발력 적용
        foreach (GameObject fragment in shatter.fragmentsLast)
        {
            if (fragment != null)
            {
                RayfireRigid rigid = fragment.GetComponent<RayfireRigid>();
                if (rigid == null)
                {
                    rigid = fragment.AddComponent<RayfireRigid>();
                }

                // RayfireRigid가 아직 초기화되지 않았다면 초기화
                if (!rigid.initialized)
                {
                    rigid.Initialize();
                }

                // 폭발을 위한 rigid 속성 설정
                rigid.simulationType = SimType.Dynamic;
                rigid.objectType = ObjectType.Mesh;
                rigid.demolitionType = DemolitionType.Runtime; // 런타임 파괴 허용
                
                // Rigidbody가 없으면 추가 (RayfireRigid가 보통 추가하지만, 확실히 하기 위함)
                Rigidbody rb = fragment.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = fragment.AddComponent<Rigidbody>();
                }
                // 원본 오브젝트 위치 기준으로 폭발력 적용
                rb.AddExplosionForce(explosionForce, objectToExplode.transform.position, explosionRadius);
            }
        }

        // 원본 오브젝트의 시각적/물리적 컴포넌트 비활성화 또는 파괴
        Renderer originalRenderer = objectToExplode.GetComponent<Renderer>();
        if (originalRenderer != null)
        {
            originalRenderer.enabled = false;
        }
        Collider originalCollider = objectToExplode.GetComponent<Collider>();
        if (originalCollider != null)
        {
            originalCollider.enabled = false;
        }
        // 선택 사항: 원본 오브젝트에 RayfireRigid가 있다면 비활성화하여
        // 조각화 후 간섭하거나 오류를 일으키는 것을 방지합니다.
        RayfireRigid originalRigid = objectToExplode.GetComponent<RayfireRigid>();
        if (originalRigid != null)
        {
            originalRigid.enabled = false;
        }
        
        // 지연 후 또는 원본 오브젝트를 완전히 제거해야 하는 경우:
        // Destroy(objectToExplode, 5f); // 예시: 5초 후 파괴

        Debug.Log(objectToExplode.name + " 조각화 및 폭발력 적용됨.");
    }
}
