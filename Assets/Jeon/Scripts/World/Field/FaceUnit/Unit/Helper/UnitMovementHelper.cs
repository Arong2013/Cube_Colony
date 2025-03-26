using UnityEngine;

public static class UnitMovementHelper
{
    public static Vector3Int GetMovementDirection(CubieFace currentFace, CubieFace targetFace)
    {
        if (currentFace == null || targetFace == null)
            return Vector3Int.zero;

        Vector3Int currentPos = new Vector3Int(
            Mathf.RoundToInt(currentFace.transform.position.x),
            Mathf.RoundToInt(currentFace.transform.position.y),
            Mathf.RoundToInt(currentFace.transform.position.z)
        );

        Vector3Int targetPos = new Vector3Int(
            Mathf.RoundToInt(targetFace.transform.position.x),
            Mathf.RoundToInt(targetFace.transform.position.y),
            Mathf.RoundToInt(targetFace.transform.position.z)
        );

        return targetPos - currentPos;
    }
}
