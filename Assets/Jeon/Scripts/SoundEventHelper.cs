using UnityEngine;

/// <summary>
/// 애니메이션 이벤트에서 사운드를 재생하기 위한 헬퍼 클래스
/// </summary>
public class SoundEventHelper : MonoBehaviour
{
    /// <summary>
    /// 애니메이션 이벤트에서 호출할 메서드
    /// </summary>
    public void PlaySound(string soundName)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(soundName);
        }
    }
    
    /// <summary>
    /// 발소리 재생 메서드 (특수 케이스)
    /// </summary>
    public void PlayFootstep()
    {
        if (SoundManager.Instance != null)
        {
            // 랜덤 발소리 선택 (footstep1, footstep2, footstep3)
            SoundManager.Instance.PlaySFX("footstep");
        }
    }
    
    /// <summary>
    /// 공격 소리 재생 메서드
    /// </summary>
    public void PlayAttackSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX("attack");
        }
    }
    
    /// <summary>
    /// 피격 소리 재생 메서드
    /// </summary>
    public void PlayHitSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX("hit");
        }
    }
}