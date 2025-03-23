using UnityEngine;

public class FaceObject : MonoBehaviour
{
    private CubieFace parentFace;
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}