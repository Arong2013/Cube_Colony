using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CubieFaceMover : MonoBehaviour
{
    private List<CubieFace> path;  // �̵��� CubieFace ����Ʈ
    private float moveSpeed = 2f;  // �̵� �ӵ�
    private int currentTargetIndex = 0;  // ���� ��ǥ �ε���
    private bool isMoving = false;  // �̵� ����

    [SerializeField] CubieFace startFace;  // ���� CubieFace
    [SerializeField] CubieFace goalFace;   // ��ǥ CubieFace

    public Cube cube;

    void Update()
    {
        // 1. startFace�� goalFace�� �����ϴ��� Ȯ��
        if (startFace != null && goalFace != null)
        {
            if (!isMoving)
            {
                // 2. �� CubieFace�� ��� �����Ǿ� ������ �̵� ����
                StartMovement(startFace, goalFace);
            }
        }
    }

    // �̵� ���� �Լ�
    public void StartMovement(CubieFace start, CubieFace goal)
    {
        this.startFace = start;
        this.goalFace = goal;

        isMoving = true;

        path = cube.TestAstar(startFace, goalFace);  // A* �˰����� ����Ͽ� ��� ã��  

        if (path != null && path.Count > 0)
        {
            StartCoroutine(MoveAlongPath());
        }
        else
        {
            Debug.Log("No path found.");
        }
    }

    // ��θ� ���� �̵��ϴ� �ڷ�ƾ
    private IEnumerator MoveAlongPath()
    {
        Vector3 startPosition = path[currentTargetIndex].transform.position;
        Vector3 endPosition = path[currentTargetIndex + 1].transform.position;

        // �밢���� �ƴ� ������ ��η� �̵�
        float journeyLength = Vector3.Distance(startPosition, endPosition);
        float startTime = Time.time;

        while (currentTargetIndex < path.Count - 1)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            if (fractionOfJourney < 1)
            {
                // �̵��� ��ġ ���
                Vector3 newPosition;

                // X -> Z �Ǵ� Y -> Z�� ���� �̵� ���� �Ǵ�
                if (Mathf.Abs(startPosition.z - endPosition.z) > 0.3f)
                {
                    // X -> Z�� �̵� �� ���� �̵�
                    newPosition = Vector3.Lerp(startPosition, new Vector3(startPosition.x, startPosition.y, endPosition.z), fractionOfJourney);
                    transform.rotation = Quaternion.Euler(0, 90, 0);  // ���� ȸ��
                }
                else if (Mathf.Abs(startPosition.x - endPosition.x) > 0.3f || Mathf.Abs(startPosition.y - endPosition.y) > 0.3f)
                {
                    // X -> Y �Ǵ� Y -> Z �̵� �� ���� �̵�
                    newPosition = Vector3.Lerp(startPosition, new Vector3(endPosition.x, startPosition.y, startPosition.z), fractionOfJourney);
                    transform.rotation = Quaternion.Euler(0, 90, 0);  // ���� ȸ��
                }
                else
                {
                    // �⺻������ ���� �̵�
                    newPosition = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);
                    transform.rotation = Quaternion.identity;  // �⺻ ȸ�� (����)
                }

                // ������Ʈ�� ���ο� ��ġ�� �̵�
                transform.position = newPosition;

                yield return null; // ����ؼ� �̵�
            }
            else
            {
                // ��ǥ�� �Ϸ��� ���, ���� ��ǥ�� �̵�
                currentTargetIndex++;
                if (currentTargetIndex < path.Count - 1)
                {
                    startPosition = path[currentTargetIndex].transform.position;
                    endPosition = path[currentTargetIndex + 1].transform.position;
                    startTime = Time.time;  // ���ο� ��ǥ���� ���� �ð� �缳��
                }
            }
        }

        // ��� ��ǥ�� �Ϸ��ϸ� �̵��� ����
        isMoving = false;
    }

    // ������ ��� �Լ� (y = ax^2 + bx + c ����)
    private float CalculateParabola(float x)
    {
        // ������ �Լ� ���: y = ax^2 + bx + c
        float a = -1f;  // a�� ����/�Ʒ��� �������� ����
        float b = 0f;   // b�� ���� �̵� ����
        float c = 0f;   // c�� y�� ���� �̵�

        return a * Mathf.Pow(x - 0.5f, 2) + c; // x�� 0.5�� �� ���� ���� ���� ����
    }
}
