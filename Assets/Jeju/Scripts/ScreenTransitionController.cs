using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenTransitionController : MonoBehaviour
{
    [Header("트랜지션 설정")]
    public Image transitionImage; // 트랜지션에 사용할 이미지
    public float transitionDuration = 1.0f; // 전환 시간
    public float pixelationAmount = 5.0f; // 픽셀화 정도
    public float waitTime = 1.0f; // 페이드 인 후 대기 시간

    [Header("로딩 설정")]
    public Slider loadingBar; // 로딩 진행 표시 바
    public RectTransform loadingUI; // 로딩 UI 전체 (활성/비활성용)

    private Material transitionMaterial;
    private bool isTransitioning = false;

    private void Start()
    {
        // 트랜지션 이미지가 지정되지 않았다면 현재 게임오브젝트에서 찾기
        if (transitionImage == null)
            transitionImage = GetComponent<Image>();

        // 머티리얼 가져오기
        transitionMaterial = transitionImage.material;
        
        // 초기 설정 - 완전 투명
        transitionMaterial.SetFloat("_TransitionTime", 0);
        transitionMaterial.SetFloat("_Resolution", pixelationAmount);
        
        // 시작 시 이미지 비활성화
        transitionImage.enabled = false;
        
        // 로딩 UI가 있다면 시작 시 비활성화
        if (loadingUI != null)
            loadingUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// 페이드 인 효과 (투명 -> 검정)
    /// </summary>
    public void FadeIn()
    {
        if (!isTransitioning)
            StartCoroutine(FadeInRoutine());
    }
    
    /// <summary>
    /// 페이드 아웃 효과 (검정 -> 투명)
    /// </summary>
    public void FadeOut()
    {
        if (!isTransitioning)
            StartCoroutine(FadeOutRoutine());
    }

    /// <summary>
    /// 페이드 인 후 지정된 대기 시간 후 페이드 아웃 효과
    /// </summary>
    public void FadeInOutWithDelay()
    {
        if (!isTransitioning)
            StartCoroutine(FadeInOutWithDelayRoutine());
    }

    /// <summary>
    /// 단순 씬 전환 효과 (로딩 진행바 없음)
    /// </summary>
    /// <param name="sceneName">전환할 씬 이름</param>
    public void TransitionToScene(string sceneName)
    {
        if (!isTransitioning)
            StartCoroutine(SceneTransitionRoutine(sceneName));
    }

    /// <summary>
    /// 로딩바와 함께 씬 전환 (비동기 로딩)
    /// </summary>
    /// <param name="sceneName">전환할 씬 이름</param>
    public void LoadSceneWithProgress(string sceneName)
    {
        if (!isTransitioning)
            StartCoroutine(LoadSceneWithProgressRoutine(sceneName));
    }

    private IEnumerator FadeInRoutine()
    {
        isTransitioning = true;
        transitionImage.enabled = true;

        float elapsedTime = 0f;
        
        // 트랜지션 값을 0에서 1로 변경 (투명 -> 검정)
        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            transitionMaterial.SetFloat("_TransitionTime", t);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 완전히 검정으로 설정
        transitionMaterial.SetFloat("_TransitionTime", 1f);
        
        isTransitioning = false;
    }

    private IEnumerator FadeOutRoutine()
    {
        isTransitioning = true;
        transitionImage.enabled = true;

        float elapsedTime = 0f;
        
        // 트랜지션 값을 1에서 0으로 변경 (검정 -> 투명)
        while (elapsedTime < transitionDuration)
        {
            float t = 1f - (elapsedTime / transitionDuration);
            transitionMaterial.SetFloat("_TransitionTime", t);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 완전히 투명으로 설정
        transitionMaterial.SetFloat("_TransitionTime", 0f);
        transitionImage.enabled = false;
        
        isTransitioning = false;
    }
    
    private IEnumerator SceneTransitionRoutine(string sceneName)
    {
        // 페이드 인 (화면이 검게 됨)
        yield return StartCoroutine(FadeInRoutine());
        
        // 대기 시간
        yield return new WaitForSeconds(waitTime);
        
        // 씬 로드
        SceneManager.LoadScene(sceneName);
        
        // 다음 프레임 대기 (새 씬이 로드되도록)
        yield return null;
        
        // 페이드 아웃 (화면이 다시 나타남)
        yield return StartCoroutine(FadeOutRoutine());
    }
    
    /// <summary>
    /// 페이드 인 후 대기 시간 후 페이드 아웃 효과를 위한 코루틴
    /// </summary>
    private IEnumerator FadeInOutWithDelayRoutine()
    {
        // 페이드 인 (투명 -> 검정)
        yield return StartCoroutine(FadeInRoutine());
        
        // 대기
        yield return new WaitForSeconds(waitTime);
        
        // 페이드 아웃 (검정 -> 투명)
        yield return StartCoroutine(FadeOutRoutine());
    }
    
    /// <summary>
    /// 트랜지션 값을 직접 설정하는 메서드
    /// </summary>
    public void SetTransitionValue(float value)
    {
        transitionImage.enabled = value > 0;
        transitionMaterial.SetFloat("_TransitionTime", value);
    }
    
    /// <summary>
    /// 로딩 진행바와 함께 씬을 비동기로 로드하는 코루틴
    /// </summary>
    private IEnumerator LoadSceneWithProgressRoutine(string sceneName)
    {
        // 페이드 인
        yield return StartCoroutine(FadeInRoutine());
        
        // 로딩 UI 활성화
        if (loadingUI != null)
            loadingUI.gameObject.SetActive(true);
            
        // 로딩바 초기화
        if (loadingBar != null)
            loadingBar.value = 0f;
            
        // 비동기 씬 로드 시작
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        
        // 로딩 진행 상황 업데이트
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            
            // 로딩바 업데이트
            if (loadingBar != null)
                loadingBar.value = progress;
                
            // 로딩이 90% 이상 진행되면 (씬 준비 완료)
            if (operation.progress >= 0.9f)
            {
                // 짧은 대기 시간 (사용자가 로딩 상태를 확인할 시간)
                yield return new WaitForSeconds(0.5f);
                
                // 로딩 UI 비활성화
                if (loadingUI != null)
                    loadingUI.gameObject.SetActive(false);
                    
                // 씬 활성화 허용
                operation.allowSceneActivation = true;
            }
            
            yield return null;
        }
        
        // 다음 프레임 대기 (새 씬이 완전히 로드되도록)
        yield return null;
        
        // 페이드 아웃
        yield return StartCoroutine(FadeOutRoutine());
    }
}
