using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ScreenModeManager : MonoBehaviour
{
    // PlayerPrefs 키 (화면 모드 저장용)
    private const string ScreenModeKey = "ScreenMode";

    // --- UI 연동용 (선택적) ---
    public TMP_Dropdown screenModeDropdown; // 예시: 0: 창모드, 1: 전체화면, 2: 테두리 없는 창
    // 또는 Toggle 그룹 등을 사용할 수도 있습니다.

    void Start()
    {
        InitializeUI();
        LoadAndApplyScreenMode(); // 저장된 화면 모드 불러오기 (해상도는 건드리지 않음)
    }

    // --- 공개 메서드 (UI 버튼/Dropdown 등에 연결) ---

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
    /// UI Dropdown에서 선택된 인덱스에 해당하는 화면 모드를 적용합니다.
    /// 0: 창모드, 1: 전체화면, 2: 테두리 없는 창
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

    // --- 내부 로직 ---

    /// <summary>
    /// 주어진 화면 모드를 적용하고 PlayerPrefs에 저장합니다.
    /// 해상도는 현재 상태를 유지합니다.
    /// </summary>
    private void ApplyAndSaveScreenMode(FullScreenMode mode)
    {
        // 현재 해상도를 가져와서 화면 모드 변경 시 유지
        int currentWidth = Screen.width;
        int currentHeight = Screen.height;
        Screen.SetResolution(currentWidth, currentHeight, mode);
        Debug.Log($"Screen mode applied: {mode} (Resolution: {currentWidth}x{currentHeight})");

        // PlayerPrefs에 화면 모드 저장
        PlayerPrefs.SetInt(ScreenModeKey, (int)mode);
        PlayerPrefs.Save();
        Debug.Log("Screen mode saved.");

        UpdateUISelection(); // UI 업데이트
    }

    /// <summary>
    /// 게임 시작 시 저장된 화면 모드를 불러와 적용합니다.
    /// 해상도는 변경하지 않고 현재 상태를 사용합니다.
    /// </summary>
    private void LoadAndApplyScreenMode()
    {
        // 저장된 화면 모드 불러오기 (없으면 테두리 없는 창 기본값)
        FullScreenMode savedMode = (FullScreenMode)PlayerPrefs.GetInt(ScreenModeKey, (int)FullScreenMode.FullScreenWindow);

        // 현재 해상도는 그대로 유지하면서 불러온 화면 모드 적용
        // ResolutionManager가 먼저 실행되어 해상도를 설정했을 수 있으므로 현재 해상도를 읽어옴
        int currentWidth = Screen.width;
        int currentHeight = Screen.height;
        Screen.SetResolution(currentWidth, currentHeight, savedMode);
        Debug.Log($"Loaded screen mode: {savedMode} (Current Resolution: {currentWidth}x{currentHeight})");

        UpdateUISelection(); // UI 업데이트
    }

    // --- UI 초기화 및 업데이트 ---

    private void InitializeUI()
    {
        if (screenModeDropdown != null)
        {
            screenModeDropdown.ClearOptions();
            screenModeDropdown.AddOptions(new List<string> { "창 모드", "전체 화면", "테두리 없는 창" });

            screenModeDropdown.onValueChanged.RemoveAllListeners();
            screenModeDropdown.onValueChanged.AddListener(SetScreenModeByIndex);
        }
        // Toggle 그룹 등을 사용한다면 여기서 초기화
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
                case FullScreenMode.MaximizedWindow: // Maximized도 테두리 없는 창으로 간주
                    modeIndex = 2; break;
            }

            if (modeIndex != -1)
            {
                screenModeDropdown.onValueChanged.RemoveListener(SetScreenModeByIndex); // 임시 리스너 제거
                screenModeDropdown.value = modeIndex;
                screenModeDropdown.RefreshShownValue();
                screenModeDropdown.onValueChanged.AddListener(SetScreenModeByIndex); // 다시 리스너 추가
            }
        }
        // Toggle 그룹 등을 사용한다면 여기서 현재 상태 반영
    }
}