using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq; // Linq 사용을 위해 추가
using TMPro;

public class ResolutionManager : MonoBehaviour
{
    // 지원할 해상도 목록
    private readonly (int width, int height)[] resolutions = {
        (1920, 1080),
        (1600, 900),
        (1280, 720)
        // 필요하다면 더 많은 해상도 추가
    };

    // PlayerPrefs 키 (해상도 저장용)
    private const string ResWidthKey = "ScreenResWidth";
    private const string ResHeightKey = "ScreenResHeight";

    // --- UI 연동용 (선택적) ---
    public TMP_Dropdown resolutionDropdown;
  
    void Start()
    {
        InitializeUI();
        LoadAndApplyResolution(); // 저장된 해상도 불러오기 (화면 모드는 건드리지 않음)
    }

    /// <summary>
    /// UI Dropdown에서 선택된 인덱스에 해당하는 해상도를 적용합니다.
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
    /// 주어진 해상도를 적용하고 PlayerPrefs에 저장합니다.
    /// 화면 모드는 현재 상태를 유지합니다.
    /// </summary>
    private void ApplyAndSaveResolution(int width, int height)
    {
        // 현재 화면 모드를 가져와서 해상도 변경 시 유지
        FullScreenMode currentMode = Screen.fullScreenMode;
        Screen.SetResolution(width, height, currentMode);
        Debug.Log($"Resolution applied: {width}x{height} (Mode: {currentMode})");

        // PlayerPrefs에 해상도 저장
        PlayerPrefs.SetInt(ResWidthKey, width);
        PlayerPrefs.SetInt(ResHeightKey, height);
        PlayerPrefs.Save();
        Debug.Log("Resolution saved.");

        UpdateUISelection(); // UI 업데이트
    }

    /// <summary>
    /// 게임 시작 시 저장된 해상도를 불러와 적용합니다.
    /// 화면 모드는 변경하지 않고 현재 상태를 사용합니다.
    /// </summary>
    private void LoadAndApplyResolution()
    {
        // 저장된 해상도 불러오기 (없으면 현재 해상도 사용)
        int savedWidth = PlayerPrefs.GetInt(ResWidthKey, Screen.width);
        int savedHeight = PlayerPrefs.GetInt(ResHeightKey, Screen.height);

        // 현재 화면 모드는 그대로 유지하면서 불러온 해상도 적용
        // ScreenModeManager가 먼저 실행되어 모드를 설정했을 수 있으므로 현재 모드를 읽어옴
        FullScreenMode currentMode = Screen.fullScreenMode;
        Screen.SetResolution(savedWidth, savedHeight, currentMode);
        Debug.Log($"Loaded resolution: {savedWidth}x{savedHeight} (Current Mode: {currentMode})");

        UpdateUISelection(); // UI 업데이트
    }

    // --- UI 초기화 및 업데이트 ---

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
                resolutionDropdown.onValueChanged.RemoveListener(SetResolutionByIndex); // 임시 리스너 제거
                resolutionDropdown.value = selectedIndex;
                resolutionDropdown.RefreshShownValue();
                resolutionDropdown.onValueChanged.AddListener(SetResolutionByIndex); // 다시 리스너 추가
            }
            else
            {
                // 현재 해상도가 목록에 없는 경우 처리 (예: Dropdown 비활성화 또는 기본값 표시)
                // resolutionDropdown.value = -1; // 또는 기본값 인덱스
                // resolutionDropdown.RefreshShownValue();
                Debug.LogWarning($"Current resolution {currentWidth}x{currentHeight} not found in the options list.");
            }
        }
    }
}