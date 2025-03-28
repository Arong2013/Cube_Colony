using UnityEngine;
using UnityEngine.EventSystems;
public static class UnitMovementHelper
{
    private const float TOLERANCE = 0.2f;

    public static Vector3Int GetNextMoveDirection(Vector3 start, Vector3 end)
    {
        Vector3 diff = end - start;

        // 0.2 이하 차이는 0으로 간주
        Vector3Int filteredDiff = new Vector3Int(
            Mathf.Abs(diff.x) > TOLERANCE ? (int)Mathf.Sign(diff.x) : 0,
            Mathf.Abs(diff.y) > TOLERANCE ? (int)Mathf.Sign(diff.y) : 0,
            Mathf.Abs(diff.z) > TOLERANCE ? (int)Mathf.Sign(diff.z) : 0
        );

        int axisCount = CountNonZero(filteredDiff);

        if (axisCount == 1)
        {
            // 한 개의 축만 차이가 있다면 그대로 이동
            return filteredDiff;
        }
        else
        {
            // 두 개 이상의 축이 다르면 한 축씩 이동 (우선순위: X -> Y -> Z)
            if (filteredDiff.x != 0) return new Vector3Int((int)Mathf.Sign(filteredDiff.x), 0, 0);
            if (filteredDiff.y != 0) return new Vector3Int(0, (int)Mathf.Sign(filteredDiff.y), 0);
            if (filteredDiff.z != 0) return new Vector3Int(0, 0, (int)Mathf.Sign(filteredDiff.z));
        }

        return Vector3Int.zero; // 이미 도착
    }

    private static int CountNonZero(Vector3Int v)
    {
        int count = 0;
        if (v.x != 0) count++;
        if (v.y != 0) count++;
        if (v.z != 0) count++;
        return count;
    }
    public static void Move(FaceUnit faceUnit, Vector3Int dir)
    {
        if (dir == Vector3.zero) return; // 방향 벡터가 0일 경우 회전 방지

        faceUnit.transform.position = Vector3.MoveTowards(faceUnit.transform.position, faceUnit.transform.position + dir, 1f * Time.deltaTime);
        Quaternion targetRotation = GetRotation(faceUnit.CubeFaceType, dir);
        faceUnit.transform.rotation = targetRotation;
    }

    private static Quaternion GetRotation(CubeFaceType faceType, Vector3 dir)
    {
        switch (faceType)
        {
            case CubeFaceType.Top: // X축 이동
                if (dir.x <= 0) return Quaternion.Euler(-15f, -135f, 0f); // 왼쪽
                if (dir.x > 0) return Quaternion.Euler(15f, 45f, 0f);  // 오른쪽
                break;

            case CubeFaceType.Bottom: // X축 이동
                if (dir.x <= 0) return Quaternion.Euler(15f, -45f, 0f); // 왼쪽
                if (dir.x > 0) return Quaternion.Euler(-15f, 135f, 0f); // 오른쪽
                break;

            case CubeFaceType.Front: // Y축 이동
                if (dir.y >= 0) return Quaternion.Euler(-15f, -90f, 0f); // 위쪽
                if (dir.y < 0) return Quaternion.Euler(15f, 90f, 0f);  // 아래쪽
                break;

            case CubeFaceType.Back: // Y축 이동
                if (dir.y >= 0) return Quaternion.Euler(-15f, 90f, 0f); // 위쪽
                if (dir.y < 0) return Quaternion.Euler(15f, -90f, 0f); // 아래쪽
                break;

            case CubeFaceType.Left: // Y축 이동
                if (dir.y >= 0) return Quaternion.Euler(15f, 0f, 0f); // 위쪽
                if (dir.y < 0) return Quaternion.Euler(-15f, 180f, 0f); // 아래쪽
                break;

            case CubeFaceType.Right: // Y축 이동
                if (dir.y >= 0) return Quaternion.Euler(-15f, 180f, 0f); // 위쪽
                if (dir.y < 0) return Quaternion.Euler(15f, 0f, 0f); // 아래쪽
                break;
        }

        return Quaternion.identity;
    }
    public static CubeFaceType CalculateCurrentFace(Transform transform, int size)
    {
        Vector3 pos = transform.position;
        float halfSize = size / 2f; // 중심에서의 절반 크기

        // ✅ 가장 큰 절대값을 가진 축 찾기
        float absX = Mathf.Abs(pos.x);
        float absY = Mathf.Abs(pos.y);
        float absZ = Mathf.Abs(pos.z);
        
        if (absX > absY && absX > absZ)
            return (pos.x > 0) ? CubeFaceType.Right : CubeFaceType.Left;

        if (absY > absX && absY > absZ)
            return (pos.y > 0) ? CubeFaceType.Top : CubeFaceType.Bottom;

        return (pos.z > 0) ? CubeFaceType.Front : CubeFaceType.Back;
    }


}
