using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Collections;

public class PlayerHitEffectUI : SerializedMonoBehaviour
{
    public static PlayerHitEffectUI Instance { get; private set; }

    [TitleGroup("히트 효과 UI")]
    [LabelText("데미지 이미지"), Required, Tooltip("화면을 덮는 빨간색 이미지")]
    [SerializeField] private Image damageOverlay;

    [TitleGroup("히트 효과 설정")]
    [LabelText("데미지 색상"), ColorUsage(true), Tooltip("데미지를 입었을 때의 색상")]
    [SerializeField] private Color damageColor = new Color(1f, 0f, 0f, 0.3f); // 반투명 빨간색

    [TitleGroup("히트 효과 설정")]
    [LabelText("효과 지속 시간"), MinValue(0.1f), Tooltip("효과가 지속되는 시간(초)")]
    [SerializeField] private float effectDuration = 0.2f;

    [TitleGroup("히트 효과 설정")]
    [LabelText("페이드아웃 시간"), MinValue(0.1f), Tooltip("효과가 사라지는 시간(초)")]
    [SerializeField] private float fadeOutDuration = 0.3f;

    [TitleGroup("시간 효과")]
    [LabelText("시간 슬로우 효과"), Tooltip("데미지를 입었을 때 시간 슬로우 사용 여부")]
    [SerializeField] private bool useTimeSlowEffect = true;

    [TitleGroup("시간 효과")]
    [LabelText("슬로우 배율"), Range(0.1f, 1f), ShowIf("useTimeSlowEffect"), Tooltip("시간 슬로우 강도 (낮을수록 느려짐)")]
    [SerializeField] private float timeScale = 0.7f;

    [TitleGroup("카메라 효과")]
    [LabelText("카메라 흔들림 사용"), Tooltip("데미지를 입었을 때 카메라 흔들림 사용 여부")]
    [SerializeField] private bool useCameraShake = true;

    [TitleGroup("카메라 효과")]
    [LabelText("흔들림 강도"), Range(0.1f, 1f), ShowIf("useCameraShake"), Tooltip("카메라 흔들림 강도")]
    [SerializeField] private float shakeIntensity = 0.3f;

    private Transform playerCamera;
    private Vector3 originalCameraPosition;
    private Coroutine hitEffectCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 초기화: 이미지는 처음에 완전 투명하게
        if (damageOverlay != null)
        {
            damageOverlay.color = new Color(damageColor.r, damageColor.g, damageColor.b, 0f);
        }
    }

    private void OnDestroy()
    {
        // 시간 스케일 정상화
        Time.timeScale = 1f;
    }

    [Button("테스트 히트 효과"), GUIColor(0.9f, 0.3f, 0.3f)]
    public void TestHitEffect()
    {
        // 인스펙터에서 테스트용으로 사용
        PlayHitEffect(30f);
    }

    public void PlayHitEffect(float damage)
    {
        // 플레이어 카메라 찾기
        if (playerCamera == null)
        {
            // Utils로 플레이어 찾기
            PlayerEntity player = Utils.GetPlayer();
            if (player != null)
            {
                // 플레이어 하위의 카메라 찾기
                playerCamera = player.GetComponentInChildren<Camera>()?.transform;
                if (playerCamera == null)
                {
                    // 못 찾으면 메인 카메라 사용
                    playerCamera = Camera.main?.transform;
                }
            }
        }

        // 이미 효과가 재생 중이면 중단
        if (hitEffectCoroutine != null)
        {
            StopCoroutine(hitEffectCoroutine);
        }

        // 효과 재생
        hitEffectCoroutine = StartCoroutine(HitEffectCoroutine(damage));
    }

    private IEnumerator HitEffectCoroutine(float damage)
    {
        // 원래 카메라 위치 저장
        if (useCameraShake && playerCamera != null)
        {
            originalCameraPosition = playerCamera.localPosition;
        }

        // 데미지 크기에 따라 효과 강도 조절 (0-100 범위 가정)
        PlayerEntity player = Utils.GetPlayer();
        float maxHealth = player != null ? player.GetEntityStat(EntityStatName.MaxHP) : 100f;
        float intensity = Mathf.Clamp01(damage / maxHealth);

        // 1. 히트 효과 즉시 적용
        ApplyHitEffect(intensity);

        // 2. 일시적인 시간 슬로우
        if (useTimeSlowEffect)
        {
            float targetTimeScale = Mathf.Lerp(1f, timeScale, intensity);
            Time.timeScale = targetTimeScale;
            Debug.Log("11");
            Debug.Log(targetTimeScale);
        }

        // 3. 효과 지속
        yield return new WaitForSecondsRealtime(effectDuration);

        // 4. 페이드아웃
        float elapsed = 0f;

        Color startColor = damageOverlay.color;
        Color endColor = new Color(damageColor.r, damageColor.g, damageColor.b, 0f);
        float startTimeScale = Time.timeScale;

        while (elapsed < fadeOutDuration)
        {
            float t = elapsed / fadeOutDuration;

            // 이미지 페이드아웃
            damageOverlay.color = Color.Lerp(startColor, endColor, t);

            // 시간 정상화
            if (useTimeSlowEffect)
            {
                Time.timeScale = Mathf.Lerp(startTimeScale, 1f, t);
            }

            // 카메라 위치 복원
            if (useCameraShake && playerCamera != null)
            {
                playerCamera.localPosition = Vector3.Lerp(
                    playerCamera.localPosition,
                    originalCameraPosition,
                    t * 2f // 카메라는 좀 더 빠르게 원위치
                );
            }

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // 5. 완전히 원래 상태로 복원
        ResetEffects();

        hitEffectCoroutine = null;
    }

    private void ApplyHitEffect(float intensity)
    {
        // 데미지 이미지 표시
        if (damageOverlay != null)
        {
            damageOverlay.color = new Color(
                damageColor.r,
                damageColor.g,
                damageColor.b,
                damageColor.a * intensity
            );
        }

        // 카메라 흔들림 시작
        if (useCameraShake && playerCamera != null)
        {
            StartCoroutine(ShakeCameraCoroutine(intensity));
        }
    }

    private IEnumerator ShakeCameraCoroutine(float intensity)
    {
        float elapsed = 0f;
        float shakeDuration = effectDuration * 0.8f; // 효과 지속 시간보다 약간 짧게

        while (elapsed < shakeDuration)
        {
            float strength = Mathf.Lerp(shakeIntensity * intensity, 0f, elapsed / shakeDuration);

            // 랜덤한 방향으로 카메라 이동
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;

            playerCamera.localPosition = originalCameraPosition + new Vector3(x, y, 0f);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    private void ResetEffects()
    {
        // 이미지 투명하게
        if (damageOverlay != null)
        {
            damageOverlay.color = new Color(damageColor.r, damageColor.g, damageColor.b, 0f);
        }

        // 시간 원래대로
        Time.timeScale = 1f;

        // 카메라 원위치
        if (useCameraShake && playerCamera != null)
        {
            playerCamera.localPosition = originalCameraPosition;
        }
    }
}