using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq; // Linq ����� ���� �߰�
using TMPro;

public class ResolutionManager : MonoBehaviour
{
    // ������ �ػ� ���
    private readonly (int width, int height)[] resolutions = {
        (1920, 1080),
        (1600, 900),
        (1280, 720)
        // �ʿ��ϴٸ� �� ���� �ػ� �߰�
    };

    // PlayerPrefs Ű (�ػ� �����)
    private const string ResWidthKey = "ScreenResWidth";
    private const string ResHeightKey = "ScreenResHeight";

    // --- UI ������ (������) ---
    public TMP_Dropdown resolutionDropdown;
  
    void Start()
    {
        InitializeUI();
        LoadAndApplyResolution(); // ����� �ػ� �ҷ����� (ȭ�� ���� �ǵ帮�� ����)
    }

    /// <summary>
    /// UI Dropdown���� ���õ� �ε����� �ش��ϴ� �ػ󵵸� �����մϴ�.
    /// </summary>
    public void SetResolutionByIndex(int index)
    {
        if (index >= 0 && index < resolutions.Length)
        {
            (int width, int height) = resolutions[index];
            ApplyAndSaveResolution(width, height);
        }
        else
        {
            Debug.LogError($"Invalid resolution index: {index}");
        }
    }

    /// <summary>
    /// �־��� �ػ󵵸� �����ϰ� PlayerPrefs�� �����մϴ�.
    /// ȭ�� ���� ���� ���¸� �����մϴ�.
    /// </summary>
    private void ApplyAndSaveResolution(int width, int height)
    {
        // ���� ȭ�� ��带 �����ͼ� �ػ� ���� �� ����
        FullScreenMode currentMode = Screen.fullScreenMode;
        Screen.SetResolution(width, height, currentMode);
        Debug.Log($"Resolution applied: {width}x{height} (Mode: {currentMode})");

        // PlayerPrefs�� �ػ� ����
        PlayerPrefs.SetInt(ResWidthKey, width);
        PlayerPrefs.SetInt(ResHeightKey, height);
        PlayerPrefs.Save();
        Debug.Log("Resolution saved.");

        UpdateUISelection(); // UI ������Ʈ
    }

    /// <summary>
    /// ���� ���� �� ����� �ػ󵵸� �ҷ��� �����մϴ�.
    /// ȭ�� ���� �������� �ʰ� ���� ���¸� ����մϴ�.
    /// </summary>
    private void LoadAndApplyResolution()
    {
        // ����� �ػ� �ҷ����� (������ ���� �ػ� ���)
        int savedWidth = PlayerPrefs.GetInt(ResWidthKey, Screen.width);
        int savedHeight = PlayerPrefs.GetInt(ResHeightKey, Screen.height);

        // ���� ȭ�� ���� �״�� �����ϸ鼭 �ҷ��� �ػ� ����
        // ScreenModeManager�� ���� ����Ǿ� ��带 �������� �� �����Ƿ� ���� ��带 �о��
        FullScreenMode currentMode = Screen.fullScreenMode;
        Screen.SetResolution(savedWidth, savedHeight, currentMode);
        Debug.Log($"Loaded resolution: {savedWidth}x{savedHeight} (Current Mode: {currentMode})");

        UpdateUISelection(); // UI ������Ʈ
    }

    // --- UI �ʱ�ȭ �� ������Ʈ ---

    private void InitializeUI()
    {
        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
            List<string> options = resolutions.Select(res => $"{res.width} x {res.height}").ToList();
            resolutionDropdown.AddOptions(options);

            resolutionDropdown.onValueChanged.RemoveAllListeners();
            resolutionDropdown.onValueChanged.AddListener(SetResolutionByIndex);
        }
    }

    private void UpdateUISelection()
    {
        if (resolutionDropdown != null)
        {
            int currentWidth = Screen.width;
            int currentHeight = Screen.height;
            int selectedIndex = -1;

            for (int i = 0; i < resolutions.Length; i++)
            {
                if (resolutions[i].width == currentWidth && resolutions[i].height == currentHeight)
                {
                    selectedIndex = i;
                    break;
                }
            }

            if (selectedIndex != -1)
            {
                resolutionDropdown.onValueChanged.RemoveListener(SetResolutionByIndex); // �ӽ� ������ ����
                resolutionDropdown.value = selectedIndex;
                resolutionDropdown.RefreshShownValue();
                resolutionDropdown.onValueChanged.AddListener(SetResolutionByIndex); // �ٽ� ������ �߰�
            }
            else
            {
                // ���� �ػ󵵰� ��Ͽ� ���� ��� ó�� (��: Dropdown ��Ȱ��ȭ �Ǵ� �⺻�� ǥ��)
                // resolutionDropdown.value = -1; // �Ǵ� �⺻�� �ε���
                // resolutionDropdown.RefreshShownValue();
                Debug.LogWarning($"Current resolution {currentWidth}x{currentHeight} not found in the options list.");
            }
        }
    }
}