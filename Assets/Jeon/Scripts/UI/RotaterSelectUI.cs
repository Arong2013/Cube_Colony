using System;
using UnityEngine;
using UnityEngine.UI;

public class RotaterSelectUI : MonoBehaviour
{
    [SerializeField] GameObject TBtns, XBtns;

    public void EnableUI(CubieFace cubieFace)
    {
        gameObject.SetActive(true);
        var cubieFaceIndex = cubieFace.face;
        if (cubieFaceIndex == CubieFaceIndex.Top || cubieFaceIndex == CubieFaceIndex.Bottom)
        {
            XBtns.SetActive(true);
            TBtns.SetActive(false);

            // CubieFace의 월드 위치를 화면 좌표로 변환
            Vector3 worldPosition = cubieFace.transform.position + new Vector3(0.03f,-0.05f,0.05f); // CubieFace의 위치 (월드 좌표)

            // 월드 좌표를 정확하게 화면 좌표로 변환
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition); // 화면 좌표로 변환

            // 카메라의 회전과 위치를 고려하여 변환
            //screenPosition = AdjustForCameraRotation(screenPosition);

            // XBtns의 RectTransform을 가져와서 위치를 설정
            RectTransform xBtnsRectTransform = XBtns.GetComponent<RectTransform>();
            if (xBtnsRectTransform != null)
            {
                xBtnsRectTransform.position = screenPosition;
            }
        }
        else
        {
            TBtns.SetActive(true);
            XBtns.SetActive(false);
        }
    }

    // 카메라의 회전과 위치를 고려하여 화면 좌표를 조정하는 함수
    private Vector3 AdjustForCameraRotation(Vector3 screenPosition)
    {
        Camera camera = Camera.main;

        // 카메라의 회전 값이 적용된 후 화면 좌표를 정확히 계산
        // 카메라 회전 적용 후 화면 좌표를 계산하여 정확한 위치를 반영
        Vector3 adjustedPosition = camera.WorldToScreenPoint(camera.transform.position + camera.transform.rotation * (screenPosition - camera.transform.position));
        return adjustedPosition;
    }
}
