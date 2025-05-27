using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    // 인스펙터 창에서 이 Panel 변수에 실제 Panel 게임 오브젝트를 연결해야 합니다.
    public GameObject optionsPanel; // 옵션 패널을 참조할 변수
    public GameObject MainPanel;

    void Start()
    {
        // 게임 시작 시 옵션 패널이 비활성화되어 있도록 설정 (선택 사항)
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Options Panel이 Main 스크립트에 할당되지 않았습니다!");
        }
        if (MainPanel != null)
        {
            MainPanel.SetActive(true);
        }
    }

    public void PlayBtn()
    {
        SceneManager.LoadScene("Loading");
    }

    public void OnClickOption() // 메소드 이름을 onClickOption()에서 OnClickOption()으로 변경 (C# 네이밍 컨벤션)
    {
        if (optionsPanel != null)
        {
            // 패널의 현재 활성화 상태를 가져와서 반대로 설정합니다.
            // 즉, 켜져 있으면 끄고, 꺼져 있으면 켭니다.
            optionsPanel.SetActive(!optionsPanel.activeSelf);
            MainPanel.SetActive(!MainPanel.activeSelf);
        }
        else
        {
            Debug.LogError("Options Panel을 찾을 수 없습니다. Main 스크립트에 할당되었는지 확인해주세요.");
        }
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}