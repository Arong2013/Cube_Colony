using System;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class CubeUIController : IPointerUpHandler, IDragHandler
{
    public float dragThreshold;

    private Vector2 initialMousePosition;
    private bool isAxisConfirmed = false;
    private bool isAxisLocked = false;

    private CubieFace selectedCubie;
    private CubeAxisType singleAxis;
    private float accumulatedRotation = 0f;

    private Action<Cubie, CubeAxisType, int> rotateCube;

    public void SetRotateCubeUpdate(Action<Cubie, CubeAxisType, int> rotateCube)
    {
        this.rotateCube = rotateCube;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        selectedCubie = null;
        initialMousePosition = eventData.position;
        isAxisConfirmed = false;
        isAxisLocked = false;
        accumulatedRotation = 0f;
        DetectSelectedCubie(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (selectedCubie == null) return;

        bool isRotateConfirmed = Mathf.Abs(accumulatedRotation) >= dragThreshold;
        int finalRotation;

        if (isRotateConfirmed)
        {
            finalRotation = (int)(Mathf.Sign(accumulatedRotation) * 90);
            rotateCube?.Invoke(selectedCubie.cubie, singleAxis, finalRotation);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (selectedCubie == null) return;

        Vector2 dragVector = eventData.position - initialMousePosition;

        if (!isAxisLocked)
        {
            DetectAxis(dragVector);
            isAxisLocked = true;
        }

        DetectRotation(dragVector);

        
    }

    private void DetectAxis(Vector2 dragVector)
    {
        if (selectedCubie == null) return;

        // 선택된 면에서 가능한 회전 축을 가져옴
        CubeAxisType CantAxis = selectedCubie.GetNotRotationAxis();
        

        // 회전 가능한 축에 맞게 축을 결정
        if (CantAxis == CubeAxisType.Z)
        {

            // XY축 회전만 가능
            if (Mathf.Abs(dragVector.x) < Mathf.Abs(dragVector.y))
            {
                singleAxis = CubeAxisType.X;  // X축 기준 회전
            }
            else
            {
                singleAxis = CubeAxisType.Y;  // Y축 기준 회전
            }
        }
        else if (CantAxis == CubeAxisType.Y)
        {
            float angle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;
            if ((angle >= 0 && angle < 90) || (angle >= 180 && angle < 270))
            {
                singleAxis = CubeAxisType.Z; // 0-90, 180-270° → X축
            }
            else
            {
                singleAxis = CubeAxisType.X; // 90-180, 270-360° → Y축
            }
            Debug.Log(angle);
            //// XZ축 회전만 가능
            //if (Mathf.Abs(dragVector.x) > Mathf.Abs(dragVector.y))
            //{
            //    singleAxis = CubeAxisType.X;  // X축 기준 회전
            //}
            //else
            //{
            //    singleAxis = CubeAxisType.Z;  // Z축 기준 회전
            //}
        }
        else if (CantAxis == CubeAxisType.X)
        {
            // YZ축 회전만 가능
            if (Mathf.Abs(dragVector.x) > Mathf.Abs(dragVector.y))
            {
                singleAxis = CubeAxisType.Y;  // Y축 기준 회전
            }
            else
            {
                singleAxis = CubeAxisType.Z;  // Z축 기준 회전
            }
        }

        Debug.Log(singleAxis + selectedCubie.face.ToString() + selectedCubie.cubie.name);
    }


    private void DetectRotation(Vector2 dragVector)
    {
        if (singleAxis == CubeAxisType.X)
        {
            accumulatedRotation += dragVector.y* Time.deltaTime;  // X축
        }
        else if (singleAxis == CubeAxisType.Y)
        {
            accumulatedRotation += -dragVector.x * Time.deltaTime;  // Z축
        }
        else if (singleAxis == CubeAxisType.Z)
        {
            accumulatedRotation += -dragVector.y * Time.deltaTime;  // Y축
        }
    }

    private void DetectSelectedCubie(PointerEventData eventData)
    {
        if (Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

        // 2️⃣ RaycastAll을 사용하여 모든 충돌 감지
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f);

        if (hits.Length == 0) return; // 아무것도 감지되지 않으면 종료

        // 3️⃣ 가장 가까운 CubieFace 찾기
        RaycastHit closestHit = hits[0];
        float minDistance = Vector3.Distance(ray.origin, closestHit.point);

        foreach (var hit in hits)
        {
            float hitDistance = Vector3.Distance(ray.origin, hit.point);
            if (hitDistance < minDistance)
            {
                closestHit = hit;
                minDistance = hitDistance;
            }
        }

        // 4️⃣ CubieFace를 감지하여 선택
        CubieFace cubie = closestHit.collider.GetComponent<CubieFace>();
        if (cubie != null)
        {
            selectedCubie = cubie;
            Debug.Log($"[DetectSelectedCubie] Selected Cubie: {selectedCubie.cubie.name}, Face: {selectedCubie.face}");
        }
    }
}
