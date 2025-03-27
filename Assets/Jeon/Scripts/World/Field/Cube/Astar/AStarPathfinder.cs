using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding
{
    private Dictionary<Vector2Int, CubieFace> v2CubieFaceMap;
    private int size;

    public AStarPathfinding(Dictionary<Vector2Int, CubieFace> v2CubieFaceMap, int size)
    {
        this.v2CubieFaceMap = v2CubieFaceMap;
        this.size = size;
    }

    // CubieFace를 입력받아 해당하는 시작점과 목표점의 인덱스를 구해서 경로 탐색을 실행
    public List<CubieFace> FindPath(CubieFace startFace, CubieFace goalFace)
    {
        if (startFace == goalFace)
        {
            // 시작과 목표가 동일 → 이동할 필요 없음
            return new List<CubieFace>();
        }

        Vector2Int start = GetPositionFromCubieFace(startFace); // CubieFace에서 시작 위치 얻기
        Vector2Int goal = GetPositionFromCubieFace(goalFace); // CubieFace에서 목표 위치 얻기

        // A* 알고리즘에 필요한 open list와 closed list
        List<Vector2Int> openList = new List<Vector2Int>();
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

        // 각 노드의 g, h, f 값
        Dictionary<Vector2Int, float> gScores = new Dictionary<Vector2Int, float>();
        Dictionary<Vector2Int, float> fScores = new Dictionary<Vector2Int, float>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        // 시작점과 목표점의 초기 설정
        openList.Add(start);
        gScores[start] = 0;
        fScores[start] = Heuristic(start, goal);

        // A* 알고리즘 실행
        while (openList.Count > 0)
        {
            // open list에서 f 값이 가장 작은 노드를 선택
            Vector2Int current = GetNodeWithLowestFScore(openList, fScores);

            // 목표에 도달하면 경로 반환
            if (current == goal)
            {
                List<CubieFace> path = ReconstructPath(cameFrom, current, goalFace);
                path.RemoveAt(0);   
                return path;
            }

            openList.Remove(current);
            closedList.Add(current);

            // 이웃 노드들에 대해 확장
            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (closedList.Contains(neighbor)) continue;

                float tentativeGScore = gScores[current] + 1;  // 이동 비용은 1로 설정 (상하좌우로 이동)

                if (!openList.Contains(neighbor))
                    openList.Add(neighbor);
                else if (tentativeGScore >= gScores[neighbor])
                    continue;

                // 더 좋은 경로를 찾았으면 점수 업데이트
                cameFrom[neighbor] = current;
                gScores[neighbor] = tentativeGScore;
                fScores[neighbor] = gScores[neighbor] + Heuristic(neighbor, goal);
            }
        }

        return null;  // 경로를 찾을 수 없는 경우 null 반환
    }
    private Vector2Int GetPositionFromCubieFace(CubieFace cubieFace)
    {
        // CubieFace 객체가 v2CubieFaceMap에 있을 때 해당하는 위치를 반환
        foreach (var pair in v2CubieFaceMap)
        {
            if (pair.Value == cubieFace)
            {
                return pair.Key;  // 해당 CubieFace가 있는 위치 반환
            }
        }

        // 만약 v2CubieFaceMap에 CubieFace가 없으면, 예외 처리 또는 디버그 메시지 출력
        Debug.LogWarning("CubieFace not found in v2CubieFaceMap.");
        return Vector2Int.zero;  // 찾을 수 없는 경우 (디폴트값 반환)
    }


    // 목표까지의 추정 비용 (맨해튼 거리)
    private float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // 맨해튼 거리
    }

    // f 값이 가장 작은 노드를 반환
    private Vector2Int GetNodeWithLowestFScore(List<Vector2Int> openList, Dictionary<Vector2Int, float> fScores)
    {
        Vector2Int bestNode = openList[0];
        foreach (var node in openList)
        {
            if (fScores.ContainsKey(node) && fScores[node] < fScores[bestNode])
                bestNode = node;
        }
        return bestNode;
    }

    // 주어진 노드의 인접 노드를 반환
    private List<Vector2Int> GetNeighbors(Vector2Int node)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // 상하좌우 방향으로 탐색
        Vector2Int[] directions = {
            new Vector2Int(0, 1),   // 위
            new Vector2Int(0, -1),  // 아래
            new Vector2Int(1, 0),   // 오른쪽
            new Vector2Int(-1, 0)   // 왼쪽
        };

        foreach (var direction in directions)
        {
            Vector2Int neighbor = node + direction;
            if (v2CubieFaceMap.ContainsKey(neighbor))  // 이동 가능한 타일인 경우
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    // 경로 재구성
    private List<CubieFace> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current, CubieFace goalFace)
    {
        List<CubieFace> path = new List<CubieFace>();

        // 경로를 재구성하면서 CubieFace 리스트에 삽입
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, v2CubieFaceMap[current]);  // 경로 순서를 올바르게 하여 삽입
        }
        path.Add(goalFace);

        return path;
    }
}
