using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ScreenModeManager : MonoBehaviour
{
    // PlayerPrefs Ű (ȭ�� ��� �����)
    private const string ScreenModeKey = "ScreenMode";

    // --- UI ������ (������) ---
    public TMP_Dropdown screenModeDropdown; // ����: 0: â���, 1: ��üȭ��, 2: �׵θ� ���� â
    // �Ǵ� Toggle �׷� ���� ����� ���� �ֽ��ϴ�.

    void Start()
    {
        InitializeUI();
        LoadAndApplyScreenMode(); // ����� ȭ�� ��� �ҷ����� (�ػ󵵴� �ǵ帮�� ����)
    }

    // --- ���� �޼��� (UI ��ư/Dropdown � ����) ---

    public void SetWindowedMode()
    {
        ApplyAndSaveScreenMode(FullScreenMode.Windowed);
    }

    public void SetFullscreenMode()
    {
        ApplyAndSaveScreenMode(FullScreenMode.ExclusiveFullScreen);
    }

    public void SetBorderlessWindowedMode()
    {
        ApplyAndSaveScreenMode(FullScreenMode.FullScreenWindow);
    }

    /// <summary>
    /// UI Dropdown���� ���õ� �ε����� �ش��ϴ� ȭ�� ��带 �����մϴ�.
    /// 0: â���, 1: ��üȭ��, 2: �׵θ� ���� â
    /// </summary>
    public void SetScreenModeByIndex(int index)
    {
        FullScreenMode targetMode;
        switch (index)
        {
            case 0: targetMode = FullScreenMode.Windowed; break;
            case 1: targetMode = FullScreenMode.ExclusiveFullScreen; break;
            case 2: targetMode = FullScreenMode.FullScreenWindow; break;
            default:
                Debug.LogError($"Invalid screen mode index: {index}");
                return;
        }
        ApplyAndSaveScreenMode(targetMode);
    }

    // --- ���� ���� ---

    /// <summary>
    /// �־��� ȭ�� ��带 �����ϰ� PlayerPrefs�� �����մϴ�.
    /// �ػ󵵴� ���� ���¸� �����մϴ�.
    /// </summary>
    private void ApplyAndSaveScreenMode(FullScreenMode mode)
    {
        // ���� �ػ󵵸� �����ͼ� ȭ�� ��� ���� �� ����
        int currentWidth = Screen.width;
        int currentHeight = Screen.height;
        Screen.SetResolution(currentWidth, currentHeight, mode);
        Debug.Log($"Screen mode applied: {mode} (Resolution: {currentWidth}x{currentHeight})");

        // PlayerPrefs�� ȭ�� ��� ����
        PlayerPrefs.SetInt(ScreenModeKey, (int)mode);
        PlayerPrefs.Save();
        Debug.Log("Screen mode saved.");

        UpdateUISelection(); // UI ������Ʈ
    }

    /// <summary>
    /// ���� ���� �� ����� ȭ�� ��带 �ҷ��� �����մϴ�.
    /// �ػ󵵴� �������� �ʰ� ���� ���¸� ����մϴ�.
    /// </summary>
    private void LoadAndApplyScreenMode()
    {
        // ����� ȭ�� ��� �ҷ����� (������ �׵θ� ���� â �⺻��)
        FullScreenMode savedMode = (FullScreenMode)PlayerPrefs.GetInt(ScreenModeKey, (int)FullScreenMode.FullScreenWindow);

        // ���� �ػ󵵴� �״�� �����ϸ鼭 �ҷ��� ȭ�� ��� ����
        // ResolutionManager�� ���� ����Ǿ� �ػ󵵸� �������� �� �����Ƿ� ���� �ػ󵵸� �о��
        int currentWidth = Screen.width;
        int currentHeight = Screen.height;
        Screen.SetResolution(currentWidth, currentHeight, savedMode);
        Debug.Log($"Loaded screen mode: {savedMode} (Current Resolution: {currentWidth}x{currentHeight})");

        UpdateUISelection(); // UI ������Ʈ
    }

    // --- UI �ʱ�ȭ �� ������Ʈ ---

    private void InitializeUI()
    {
        if (screenModeDropdown != null)
        {
            screenModeDropdown.ClearOptions();
            screenModeDropdown.AddOptions(new List<string> { "â ���", "��ü ȭ��", "�׵θ� ���� â" });

            screenModeDropdown.onValueChanged.RemoveAllListeners();
            screenModeDropdown.onValueChanged.AddListener(SetScreenModeByIndex);
        }
        // Toggle �׷� ���� ����Ѵٸ� ���⼭ �ʱ�ȭ
    }

    private void UpdateUISelection()
    {
        if (screenModeDropdown != null)
        {
            int modeIndex = -1;
            switch (Screen.fullScreenMode)
            {
                case FullScreenMode.Windowed: modeIndex = 0; break;
                case FullScreenMode.ExclusiveFullScreen: modeIndex = 1; break;
                case FullScreenMode.FullScreenWindow:
                case FullScreenMode.MaximizedWindow: // Maximized�� �׵θ� ���� â���� ����
                    modeIndex = 2; break;
            }

            if (modeIndex != -1)
            {
                screenModeDropdown.onValueChanged.RemoveListener(SetScreenModeByIndex); // �ӽ� ������ ����
                screenModeDropdown.value = modeIndex;
                screenModeDropdown.RefreshShownValue();
                screenModeDropdown.onValueChanged.AddListener(SetScreenModeByIndex); // �ٽ� ������ �߰�
            }
        }
        // Toggle �׷� ���� ����Ѵٸ� ���⼭ ���� ���� �ݿ�
    }
}