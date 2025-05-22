using UnityEngine;

public class GlowMaterialSetter : MonoBehaviour
{
    void Start()
    {
        Renderer rend = GetComponent<Renderer>();

        // ���ο� ��Ƽ���� ����
        Material glowMat = new Material(Shader.Find("Standard"));
        glowMat.EnableKeyword("_EMISSION");
        glowMat.SetColor("_EmissionColor", Color.cyan * 2.0f); // ���� * ����

        rend.material = glowMat;
    }
}