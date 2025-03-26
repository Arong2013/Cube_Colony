using UnityEngine;

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
}
