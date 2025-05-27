using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;

public enum SFXType 
{
    Footstep,
    Attack,
    Hit
}

public class SoundEventHelper : SerializedMonoBehaviour 
{
    [SerializeField]
    private Dictionary<SFXType, string> sfxNameMap = new Dictionary<SFXType, string>();

    public void PlaySound(string soundName)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(soundName);
        }
    }
    public void PlayFootstep()
    {
        PlaySFXByType(SFXType.Footstep);
    }

    public void PlayAttackSound()
    {
        PlaySFXByType(SFXType.Attack);
    }

    public void PlayHitSound()
    {
        PlaySFXByType(SFXType.Hit);
    }

    private void PlaySFXByType(SFXType type)
    {
        if (sfxNameMap.TryGetValue(type, out string soundName))
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySFX(soundName);
            }
        }
        else
        {
            Debug.LogWarning($"No sound name found for SFX type: {type}");
        }
    }
}