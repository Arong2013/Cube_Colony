using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "GameScene";
    [SerializeField] private Slider loadingBar;

    private void Start()
    {
        StartCoroutine(LoadGameSceneAsync());
    }

    private IEnumerator LoadGameSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (loadingBar != null)
                loadingBar.value = progress;

            // 로딩이 90% 이상 진행되면 (씬 준비 완료)
            if (operation.progress >= 0.9f)
            {
                // 바로 넘기기: 애니메이션, 키 입력 등 넣고 싶다면 이 지점에서 대기 가능
                yield return new WaitForSeconds(0.5f); // 짧은 연출 타임
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
