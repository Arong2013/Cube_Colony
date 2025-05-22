using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    // �ν����� â���� �� Panel ������ ���� Panel ���� ������Ʈ�� �����ؾ� �մϴ�.
    public GameObject optionsPanel; // �ɼ� �г��� ������ ����
    public GameObject MainPanel;

    void Start()
    {
        // ���� ���� �� �ɼ� �г��� ��Ȱ��ȭ�Ǿ� �ֵ��� ���� (���� ����)
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Options Panel�� Main ��ũ��Ʈ�� �Ҵ���� �ʾҽ��ϴ�!");
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

    public void OnClickOption() // �޼ҵ� �̸��� onClickOption()���� OnClickOption()���� ���� (C# ���̹� ������)
    {
        if (optionsPanel != null)
        {
            // �г��� ���� Ȱ��ȭ ���¸� �����ͼ� �ݴ�� �����մϴ�.
            // ��, ���� ������ ����, ���� ������ �մϴ�.
            optionsPanel.SetActive(!optionsPanel.activeSelf);
            MainPanel.SetActive(!MainPanel.activeSelf);
        }
        else
        {
            Debug.LogError("Options Panel�� ã�� �� �����ϴ�. Main ��ũ��Ʈ�� �Ҵ�Ǿ����� Ȯ�����ּ���.");
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