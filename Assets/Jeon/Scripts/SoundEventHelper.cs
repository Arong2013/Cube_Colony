using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;

public enum SFXType
{
    Footstep,
    Attack,
    Hit,
    CubeTurn,
    TileEnhance1,
    TileEnhance2,
    TileEnhance3,
    Portal
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
    public void PlayCubeTurnSound()
    {
        PlaySFXByType(SFXType.CubeTurn);
    }
    public void PlayTileEnhanceSound1()
    {
        PlaySFXByType(SFXType.TileEnhance1);
    }
public void PlayTileEnhanceSound2()
    {
        PlaySFXByType(SFXType.TileEnhance2);
    }
public void PlayTileEnhanceSound3()
    {
        PlaySFXByType(SFXType.TileEnhance3);
    }
    public void PlayPortalSound()
    {
        PlaySFXByType(SFXType.Portal);
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