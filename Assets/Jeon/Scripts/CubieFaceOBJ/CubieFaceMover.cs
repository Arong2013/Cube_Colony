using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CubieFaceMover : MonoBehaviour
{
    private List<CubieFace> path;  // 이동할 CubieFace 리스트
    private float moveSpeed = 2f;  // 이동 속도
    private int currentTargetIndex = 0;  // 현재 목표 인덱스
    private bool isMoving = false;  // 이동 상태

    [SerializeField] CubieFace startFace;  // 시작 CubieFace
    [SerializeField] CubieFace goalFace;   // 목표 CubieFace

    public Cube cube;

    void Update()
    {
        // 1. startFace와 goalFace가 존재하는지 확인
        if (startFace != null && goalFace != null)
        {
            if (!isMoving)
            {
                // 2. 두 CubieFace가 모두 설정되어 있으면 이동 시작
                StartMovement(startFace, goalFace);
            }
        }
    }

    // 이동 시작 함수
    public void StartMovement(CubieFace start, CubieFace goal)
    {
        this.startFace = start;
        this.goalFace = goal;

        isMoving = true;

        path = cube.TestAstar(startFace, goalFace);  // A* 알고리즘을 사용하여 경로 찾기  

        if (path != null && path.Count > 0)
        {
            StartCoroutine(MoveAlongPath());
        }
        else
        {
            Debug.Log("No path found.");
        }
    }

    // 경로를 따라 이동하는 코루틴
    private IEnumerator MoveAlongPath()
    {
        Vector3 startPosition = path[currentTargetIndex].transform.position;
        Vector3 endPosition = path[currentTargetIndex + 1].transform.position;

        // 대각선이 아닌 포물선 경로로 이동
        float journeyLength = Vector3.Distance(startPosition, endPosition);
        float startTime = Time.time;

        while (currentTargetIndex < path.Count - 1)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            if (fractionOfJourney < 1)
            {
                // 이동할 위치 계산
                Vector3 newPosition;

                // X -> Z 또는 Y -> Z로 직각 이동 여부 판단
                if (Mathf.Abs(startPosition.z - endPosition.z) > 0.3f)
                {
                    // X -> Z로 이동 시 직각 이동
                    newPosition = Vector3.Lerp(startPosition, new Vector3(startPosition.x, startPosition.y, endPosition.z), fractionOfJourney);
                    transform.rotation = Quaternion.Euler(0, 90, 0);  // 직각 회전
                }
                else if (Mathf.Abs(startPosition.x - endPosition.x) > 0.3f || Mathf.Abs(startPosition.y - endPosition.y) > 0.3f)
                {
                    // X -> Y 또는 Y -> Z 이동 시 직각 이동
                    newPosition = Vector3.Lerp(startPosition, new Vector3(endPosition.x, startPosition.y, startPosition.z), fractionOfJourney);
                    transform.rotation = Quaternion.Euler(0, 90, 0);  // 직각 회전
                }
                else
                {
                    // 기본적으로 직선 이동
                    newPosition = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);
                    transform.rotation = Quaternion.identity;  // 기본 회전 (직선)
                }

                // 오브젝트를 새로운 위치로 이동
                transform.position = newPosition;

                yield return null; // 계속해서 이동
            }
            else
            {
                // 목표를 완료한 경우, 다음 목표로 이동
                currentTargetIndex++;
                if (currentTargetIndex < path.Count - 1)
                {
                    startPosition = path[currentTargetIndex].transform.position;
                    endPosition = path[currentTargetIndex + 1].transform.position;
                    startTime = Time.time;  // 새로운 목표점에 대한 시간 재설정
                }
            }
        }

        // 모든 목표를 완료하면 이동을 멈춤
        isMoving = false;
    }

    // 포물선 계산 함수 (y = ax^2 + bx + c 형태)
    private float CalculateParabola(float x)
    {
        // 포물선 함수 계산: y = ax^2 + bx + c
        float a = -1f;  // a는 위로/아래로 굽어지는 정도
        float b = 0f;   // b는 평행 이동 정도
        float c = 0f;   // c는 y축 기준 이동

        return a * Mathf.Pow(x - 0.5f, 2) + c; // x가 0.5일 때 가장 높은 점에 도달
    }
}
