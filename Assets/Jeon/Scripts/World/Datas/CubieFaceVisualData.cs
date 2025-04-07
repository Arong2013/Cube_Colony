using UnityEngine;

[System.Serializable]
public class CubieFaceVisualData
{
    public Material material;
    public Sprite icon; // 필요 시 UI용
    public string displayName;
    public Color glowColor; // 필요 시 쉐이더 효과 등
}
