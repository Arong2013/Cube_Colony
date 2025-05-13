using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CubieFaceVisualData
{
    public Sprite icon; // 필요 시 UI용
    public string displayName;
    public Color glowColor; // 필요 시 쉐이더 효과 등
    public List<GameObject> FieldMesh;
    public List<Material> CubieFaceMaterials;
}
