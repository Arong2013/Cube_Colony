using System;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class CubeUIController 
{
    public float dragThreshold;
    private Vector2 initialMousePosition;
    private bool isAxisConfirmed = false;
    private bool isAxisLocked = false;
    private CubieFace selectedCubie;
    private CubeAxisType singleAxis;
    private float accumulatedRotation = 0f;
    private Action<Cubie, CubeAxisType, int> rotateCube;

    [SerializeField] Transform parent;

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

        if (Mathf.Abs(accumulatedRotation) >= dragThreshold)
        {
            int finalRotation = (int)(Mathf.Sign(accumulatedRotation) * 90);
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

        CubeAxisType cantAxis = selectedCubie.GetNotRotationAxis();

        switch (cantAxis)
        {
            case CubeAxisType.Z:
                singleAxis = Mathf.Abs(dragVector.x) < Mathf.Abs(dragVector.y) ? CubeAxisType.X : CubeAxisType.Y;
                break;
            case CubeAxisType.Y:
                float angle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;
                singleAxis = (angle >= 0 && angle < 90) || (angle >= 180 && angle < 270) ? CubeAxisType.Z : CubeAxisType.X;
                Debug.Log(angle);
                break;
            case CubeAxisType.X:
                singleAxis = Mathf.Abs(dragVector.x) > Mathf.Abs(dragVector.y) ? CubeAxisType.Y : CubeAxisType.Z;
                break;
        }

        Debug.Log($"{singleAxis} {selectedCubie.face} {selectedCubie.cubie.name}");
    }

    private void DetectRotation(Vector2 dragVector)
    {
        // 부모 오브젝트의 회전값을 가져옵니다.
        Quaternion parentRotation = parent.transform.parent.rotation;

        float rotationDirectionX = Mathf.Sign(parentRotation.eulerAngles.x);
        float rotationDirectionY = Mathf.Sign(parentRotation.eulerAngles.y);
        float rotationDirectionZ = Mathf.Sign(parentRotation.eulerAngles.z);

        // 회전값을 업데이트합니다. 부모의 회전 방향에 맞게 부호를 반전시킵니다.
        accumulatedRotation += singleAxis switch
        {
            CubeAxisType.X => dragVector.y * rotationDirectionX * Time.deltaTime,
            CubeAxisType.Y => -dragVector.x * rotationDirectionY * Time.deltaTime,
            CubeAxisType.Z => -dragVector.y * rotationDirectionZ * Time.deltaTime,
            _ => 0
        };
    }
    private void DetectSelectedCubie(PointerEventData eventData)
    {
        if (Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

        RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
        if (hits.Length == 0) return;

        CubieFace closestCubieFace = null;
        float minDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent(out CubieFace cubieFace))
            {
                float hitDistance = Vector3.Distance(ray.origin, hit.point);
                if (hitDistance < minDistance)
                {
                    closestCubieFace = cubieFace;
                    minDistance = hitDistance;
                }
            }
        }

        if (closestCubieFace != null)
        {
            selectedCubie = closestCubieFace;
        }
    }
}