using UnityEngine;

public class GlowMaterialSetter : MonoBehaviour
{
    void Start()
    {
        Renderer rend = GetComponent<Renderer>();

        // 새로운 머티리얼 생성
        Material glowMat = new Material(Shader.Find("Standard"));
        glowMat.EnableKeyword("_EMISSION");
        glowMat.SetColor("_EmissionColor", Color.cyan * 2.0f); // 색상 * 강도

        rend.material = glowMat;
    }
}