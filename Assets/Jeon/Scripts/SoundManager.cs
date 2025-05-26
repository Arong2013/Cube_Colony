using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static SoundManager Instance { get; private set; }


    [TitleGroup("볼륨 설정")]
    [LabelText("배경음악 볼륨"), Range(0f, 1f)]
    [OnValueChanged("UpdateBGMVolume")]
    [SerializeField] private float bgmVolume = 0.5f;

    [TitleGroup("볼륨 설정")]
    [LabelText("효과음 볼륨"), Range(0f, 1f)]
    [OnValueChanged("UpdateSFXVolume")]
    [SerializeField] private float sfxVolume = 0.7f;

    [TitleGroup("볼륨 설정")]
    [LabelText("음소거")]
    [OnValueChanged("UpdateMuteStatus")]
    [SerializeField] private bool isMuted = false;

    [TitleGroup("오디오 소스 설정")]
    [LabelText("최대 효과음 동시 재생 수"), Range(1, 20)]
    [SerializeField] private int maxSfxSources = 10;

    [TitleGroup("오디오 소스 설정")]
    [LabelText("3D 사운드 설정")]
    [SerializeField] private bool use3DSound = false;

    [TitleGroup("오디오 소스 설정")]
    [LabelText("사운드 풀 재사용"), Tooltip("오디오 소스를 재사용하여 메모리 사용량 최적화")]
    [SerializeField] private bool reuseAudioSources = true;

    [TitleGroup("오디오 소스 설정")]
    [LabelText("소스 이름 형식"), Tooltip("생성된 오디오 소스의 이름 지정 형식")]
    [SerializeField] private string sourceNameFormat = "AudioSource_{0}_{1}";

    [FoldoutGroup("사운드 데이터")]
    [LabelText("효과음 목록")]
    [TableList(ShowIndexLabels = true)]
    [SerializeField] private List<SoundEffect> soundEffects = new List<SoundEffect>();

    [FoldoutGroup("사운드 데이터")]
    [LabelText("배경음악 목록")]
    [TableList(ShowIndexLabels = true)]
    [SerializeField] private List<BackgroundMusic> backgroundMusics = new List<BackgroundMusic>();


[TitleGroup("도구")]
[Button("추가 오디오 소스 생성", ButtonSizes.Medium), GUIColor(0.3f, 0.7f, 0.9f)]
[InfoBox("이 버튼을 클릭하면 새로운 오디오 소스가 생성됩니다")]
private void CreateAdditionalAudioSource()
{
    int sourceCount = sfxSources.Count;
    GameObject sfxGO = new GameObject(string.Format(sourceNameFormat, "SFX", sourceCount));
    sfxGO.transform.SetParent(sfxSourcesParent);
    
    AudioSource sfxSource = sfxGO.AddComponent<AudioSource>();
    ConfigureSFXAudioSource(sfxSource);
    sfxSources.Add(sfxSource);
    
    Debug.Log($"SoundManager: 새 오디오 소스 생성됨 - {sfxGO.name}");
}
    // 오디오 소스 관리
    private AudioSource bgmSource;
    private List<AudioSource> sfxSources = new List<AudioSource>();
    private Transform sfxSourcesParent;

    // 빠른 검색을 위한 사운드 데이터 딕셔너리
    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>(System.StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, AudioClip> bgmDictionary = new Dictionary<string, AudioClip>(System.StringComparer.OrdinalIgnoreCase);

    // 대기 중인 페이드 코루틴
    private Coroutine bgmFadeCoroutine;

    private void Awake()
    {
        // 싱글톤 패턴 적용
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 오디오 소스 컨테이너 생성
        CreateAudioSourceContainers();
        
        // 오디오 소스 초기화
        InitializeAudioSources();
        
        // 사운드 데이터 딕셔너리 초기화
        InitializeSoundDictionaries();
    }

    private void CreateAudioSourceContainers()
    {
        // BGM 컨테이너
        Transform bgmContainer = new GameObject("BGM_Container").transform;
        bgmContainer.SetParent(transform);
        
        // SFX 컨테이너
        sfxSourcesParent = new GameObject("SFX_Container").transform;
        sfxSourcesParent.SetParent(transform);
    }

    private void InitializeAudioSources()
    {
        // BGM 오디오 소스 생성
        GameObject bgmGO = new GameObject(string.Format(sourceNameFormat, "BGM", 0));
        bgmGO.transform.SetParent(transform);
        bgmSource = bgmGO.AddComponent<AudioSource>();
        ConfigureBGMAudioSource(bgmSource);

        // SFX 오디오 소스 풀 생성
        for (int i = 0; i < maxSfxSources; i++)
        {
            GameObject sfxGO = new GameObject(string.Format(sourceNameFormat, "SFX", i));
            sfxGO.transform.SetParent(sfxSourcesParent);
            
            AudioSource sfxSource = sfxGO.AddComponent<AudioSource>();
            ConfigureSFXAudioSource(sfxSource);
            sfxSources.Add(sfxSource);
        }

        Debug.Log($"SoundManager: 오디오 소스 초기화 완료 (BGM: 1개, SFX: {maxSfxSources}개)");
    }

    private void ConfigureBGMAudioSource(AudioSource source)
    {
        source.loop = true;
        source.volume = bgmVolume;
        source.playOnAwake = false;
        source.priority = 0; // 최고 우선순위
        
    }

    private void ConfigureSFXAudioSource(AudioSource source)
    {
        source.loop = false;
        source.playOnAwake = false;
        source.volume = sfxVolume;
        source.priority = 128; // 중간 우선순위
        
        // 3D 사운드 설정
        if (use3DSound)
        {
            source.spatialBlend = 1.0f; // 3D 사운드 (0: 2D, 1: 3D)
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = 1f;
            source.maxDistance = 20f;
        }
        else
        {
            source.spatialBlend = 0f; // 2D 사운드
        }
        
    }

    private void InitializeSoundDictionaries()
    {
        // 효과음 딕셔너리 구성
        foreach (var sfx in soundEffects)
        {
            if (sfx.clip != null && !string.IsNullOrEmpty(sfx.name))
            {
                sfxDictionary[sfx.name] = sfx.clip;
            }
        }

        // 배경음악 딕셔너리 구성
        foreach (var bgm in backgroundMusics)
        {
            if (bgm.clip != null && !string.IsNullOrEmpty(bgm.name))
            {
                bgmDictionary[bgm.name] = bgm.clip;
            }
        }

        Debug.Log($"SoundManager: 사운드 데이터 초기화 완료 (효과음: {sfxDictionary.Count}개, 배경음악: {bgmDictionary.Count}개)");
    }

    #region Public API

    /// <summary>
    /// 효과음 재생
    /// </summary>
    /// <param name="soundName">효과음 이름</param>
    /// <param name="volume">볼륨 (0-1 사이, 기본값은 1)</param>
    /// <param name="pitch">피치 (0.5-2 사이, 기본값은 1)</param>
    /// <returns>효과음이 재생된 오디오 소스 (실패 시 null)</returns>
    public AudioSource PlaySFX(string soundName, float volume = 1f, float pitch = 1f)
    {
        if (isMuted) return null;
        
        if (!sfxDictionary.TryGetValue(soundName, out AudioClip clip))
        {
            Debug.LogWarning($"SoundManager: 효과음 '{soundName}'을(를) 찾을 수 없습니다.");
            return null;
        }

        // 사용 가능한 오디오 소스 찾기
        AudioSource source = GetAvailableSFXSource();
        if (source == null)
        {
            Debug.LogWarning("SoundManager: 사용 가능한 오디오 소스가 없습니다.");
            return null;
        }

        source.clip = clip;
        source.volume = volume * sfxVolume;
        source.pitch = pitch;
        source.Play();

        return source;
    }

    /// <summary>
    /// 배경음악 재생
    /// </summary>
    /// <param name="bgmName">배경음악 이름</param>
    /// <param name="fadeInDuration">페이드인 시간 (초)</param>
    public void PlayBGM(string bgmName, float fadeInDuration = 1f)
    {
        if (!bgmDictionary.TryGetValue(bgmName, out AudioClip clip))
        {
            Debug.LogWarning($"SoundManager: 배경음악 '{bgmName}'을(를) 찾을 수 없습니다.");
            return;
        }

        // 현재 재생 중인 BGM과 동일하면 무시
        if (bgmSource.clip == clip && bgmSource.isPlaying)
            return;

        // 이전 페이드 코루틴 중지
        if (bgmFadeCoroutine != null)
        {
            StopCoroutine(bgmFadeCoroutine);
        }

        // 페이드인 적용
        if (fadeInDuration > 0)
        {
            bgmFadeCoroutine = StartCoroutine(FadeBGM(clip, fadeInDuration));
        }
        else
        {
            bgmSource.clip = clip;
            bgmSource.volume = bgmVolume;
            bgmSource.Play();
        }
    }

    /// <summary>
    /// 배경음악 정지
    /// </summary>
    /// <param name="fadeOutDuration">페이드아웃 시간 (초)</param>
    public void StopBGM(float fadeOutDuration = 1f)
    {
        // 이전 페이드 코루틴 중지
        if (bgmFadeCoroutine != null)
        {
            StopCoroutine(bgmFadeCoroutine);
        }

        if (fadeOutDuration > 0)
        {
            bgmFadeCoroutine = StartCoroutine(FadeOutBGM(fadeOutDuration));
        }
        else
        {
            bgmSource.Stop();
        }
    }

    /// <summary>
    /// 모든 효과음 정지
    /// </summary>
    public void StopAllSFX()
    {
        foreach (var source in sfxSources)
        {
            source.Stop();
        }
    }

    /// <summary>
    /// 모든 사운드 정지 (배경음악 + 효과음)
    /// </summary>
    public void StopAllSounds()
    {
        StopBGM(0);
        StopAllSFX();
    }

    /// <summary>
    /// 배경음악 볼륨 설정
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;
        
        // 오디오 믹서 업데이트 (있는 경우)

    }

    /// <summary>
    /// 효과음 볼륨 설정
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        foreach (var source in sfxSources)
        {
            source.volume = sfxVolume;
        }
        
    }

    /// <summary>
    /// 음소거 설정
    /// </summary>
    public void SetMute(bool mute)
    {
        isMuted = mute;
        bgmSource.mute = mute;
        foreach (var source in sfxSources)
        {
            source.mute = mute;
        }
    }

    /// <summary>
    /// 지정된 위치에서 효과음 재생 (3D 사운드)
    /// </summary>
    public AudioSource PlaySFXAtPosition(string soundName, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (isMuted) return null;
        
        if (!sfxDictionary.TryGetValue(soundName, out AudioClip clip))
        {
            Debug.LogWarning($"SoundManager: 효과음 '{soundName}'을(를) 찾을 수 없습니다.");
            return null;
        }

        if (use3DSound)
        {
            // 3D 사운드 모드: 오디오 소스 위치 조정
            AudioSource source = GetAvailableSFXSource();
            if (source == null) return null;
            
            source.transform.position = position;
            source.clip = clip;
            source.volume = volume * sfxVolume;
            source.pitch = pitch;
            source.spatialBlend = 1f; // 3D 사운드
            source.Play();
            
            return source;
        }
        else
        {
            // 일반 모드: 간단히 위치에서 재생 (Unity 내장 함수)
            AudioSource.PlayClipAtPoint(clip, position, volume * sfxVolume);
            return null;
        }
    }

    /// <summary>
    /// 오디오 소스 동적 생성
    /// </summary>
    public AudioSource CreateAudioSource(string name, bool isMusic = false)
    {
        GameObject sourceGO = new GameObject($"DynamicAudioSource_{name}");
        
        if (isMusic)
        {
            sourceGO.transform.SetParent(transform);
        }
        else
        {
            sourceGO.transform.SetParent(sfxSourcesParent);
        }
        
        AudioSource source = sourceGO.AddComponent<AudioSource>();
        
        if (isMusic)
        {
            ConfigureBGMAudioSource(source);
        }
        else
        {
            ConfigureSFXAudioSource(source);
        }
        
        return source;
    }

    /// <summary>
    /// 오디오 소스에 사운드 할당 및 재생
    /// </summary>
    public void PlaySoundOnSource(AudioSource source, string soundName, float volume = 1f, float pitch = 1f, bool loop = false)
    {
        if (source == null || isMuted) return;
        
        AudioClip clip = null;
        
        // 효과음과 배경음악 모두 검색
        if (!sfxDictionary.TryGetValue(soundName, out clip) && !bgmDictionary.TryGetValue(soundName, out clip))
        {
            Debug.LogWarning($"SoundManager: 사운드 '{soundName}'을(를) 찾을 수 없습니다.");
            return;
        }
        
        source.clip = clip;
        source.volume = volume * (loop ? bgmVolume : sfxVolume);
        source.pitch = pitch;
        source.loop = loop;
        source.Play();
    }
    
    /// <summary>
    /// 효과음 등록
    /// </summary>
    public void RegisterSFX(string name, AudioClip clip)
    {
        if (clip != null && !string.IsNullOrEmpty(name))
        {
            sfxDictionary[name] = clip;
            
            // 목록에도 추가
            var existingSfx = soundEffects.Find(sfx => sfx.name == name);
            if (existingSfx != null)
            {
                existingSfx.clip = clip;
            }
            else
            {
                soundEffects.Add(new SoundEffect { name = name, clip = clip });
            }
        }
    }

    /// <summary>
    /// 배경음악 등록
    /// </summary>
    public void RegisterBGM(string name, AudioClip clip)
    {
        if (clip != null && !string.IsNullOrEmpty(name))
        {
            bgmDictionary[name] = clip;
            
            // 목록에도 추가
            var existingBgm = backgroundMusics.Find(bgm => bgm.name == name);
            if (existingBgm != null)
            {
                existingBgm.clip = clip;
            }
            else
            {
                backgroundMusics.Add(new BackgroundMusic { name = name, clip = clip });
            }
        }
    }

    /// <summary>
    /// 애니메이션 이벤트에서 사용할 메서드
    /// </summary>
    public void PlaySoundFromAnimation(string soundName)
    {
        PlaySFX(soundName);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 사용 가능한 SFX 오디오 소스 가져오기
    /// </summary>
    private AudioSource GetAvailableSFXSource()
    {
        // 재생 중이지 않은 소스 찾기
        foreach (var source in sfxSources)
        {
            if (!source.isPlaying)
                return source;
        }

        // 모든 소스가 사용 중일 때
        if (reuseAudioSources)
        {
            // 가장 오래된 소스 재사용
            return sfxSources[0];
        }
        else
        {
            // 새 소스 동적 생성
            AudioSource newSource = CreateAudioSource($"Dynamic_SFX_{sfxSources.Count}");
            sfxSources.Add(newSource);
            return newSource;
        }
    }

    /// <summary>
    /// 배경음악 페이드인
    /// </summary>
    private IEnumerator FadeBGM(AudioClip newClip, float duration)
    {
        float startVolume = bgmSource.volume;
        float timer = 0f;

        // 현재 BGM 페이드아웃
        if (bgmSource.isPlaying)
        {
            while (timer < duration * 0.5f)
            {
                timer += Time.unscaledDeltaTime;
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, timer / (duration * 0.5f));
                yield return null;
            }
        }

        // 새 BGM 설정 및 재생
        bgmSource.clip = newClip;
        bgmSource.volume = 0f;
        bgmSource.Play();

        // 새 BGM 페이드인
        timer = 0f;
        while (timer < duration * 0.5f)
        {
            timer += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(0f, bgmVolume, timer / (duration * 0.5f));
            yield return null;
        }

        bgmSource.volume = bgmVolume;
        bgmFadeCoroutine = null;
    }

    /// <summary>
    /// 배경음악 페이드아웃
    /// </summary>
    private IEnumerator FadeOutBGM(float duration)
    {
        float startVolume = bgmSource.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.volume = bgmVolume;
        bgmFadeCoroutine = null;
    }

    #endregion

    #region Editor Callbacks

    private void UpdateBGMVolume()
    {
        if (bgmSource != null)
        {
            bgmSource.volume = bgmVolume;
        }
    }

    private void UpdateSFXVolume()
    {
        foreach (var source in sfxSources)
        {
            if (source != null)
            {
                source.volume = sfxVolume;
            }
        }
    }

    private void UpdateMuteStatus()
    {
        if (bgmSource != null)
        {
            bgmSource.mute = isMuted;
        }

        foreach (var source in sfxSources)
        {
            if (source != null)
            {
                source.mute = isMuted;
            }
        }
    }

    #endregion
}

/// <summary>
/// 효과음 데이터 클래스
/// </summary>
[System.Serializable]
public class SoundEffect
{
    [LabelText("효과음 이름")]
    public string name;
    
    [LabelText("오디오 클립")]
    [PreviewField(60)]
    public AudioClip clip;
}

/// <summary>
/// 배경음악 데이터 클래스
/// </summary>
[System.Serializable]
public class BackgroundMusic
{
    [LabelText("배경음악 이름")]
    public string name;
    
    [LabelText("오디오 클립")]
    [PreviewField(60)]
    public AudioClip clip;
    
}