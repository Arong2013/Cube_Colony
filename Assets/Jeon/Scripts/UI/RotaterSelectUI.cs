using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class RotaterSelectUI
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] List<GameObject> rotaterSelectButtons;
    public void EnableUI(CubieFace cubieFace, CubeAxisType cubeAxisType, bool isClocked)
    {
        if(cubieFace.face == CubieFaceIndex.Front || cubieFace.face == CubieFaceIndex.Back)
        {
            rectTransform.position = cubieFace.transform.position + new Vector3(0, 0.5f, 0);
            rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (cubieFace.face == CubieFaceIndex.Left || cubieFace.face == CubieFaceIndex.Right)
        {
            rectTransform.position = cubieFace.transform.position + new Vector3(0,0.5f,0);
            rectTransform.rotation = Quaternion.Euler(0, 90, 0);
        }
        if (cubieFace.face == CubieFaceIndex.Top || cubieFace.face == CubieFaceIndex.Back)
        {
            rectTransform.position = cubieFace.transform.position  + new Vector3(0.5f,0,0f);
            rectTransform.rotation = Quaternion.Euler(90, 0, -90);
        }

        SetAxis(cubeAxisType, isClocked);   
    }

    public void SetAxis(CubeAxisType cubeAxisType, bool isClocked)
    {
        // 모든 버튼을 비활성화합니다.
        foreach (var button in rotaterSelectButtons)
        {
            button.gameObject.SetActive(false);
        }

        // X 축 회전 처리
        if (cubeAxisType == CubeAxisType.X)
        {
            if (isClocked)
            {
                rotaterSelectButtons[2].gameObject.SetActive(true); // Right 방향
            }
            else
            {
                rotaterSelectButtons[3].gameObject.SetActive(true); // Left 방향
            }
        }
        else if (cubeAxisType == CubeAxisType.Y)
        {
            if (isClocked)
            {
                rotaterSelectButtons[1].gameObject.SetActive(true); // Top 방향
            }
            else
            {
                rotaterSelectButtons[0].gameObject.SetActive(true); // Down 방향
            }
        }
        // Z 축 회전 처리 (필요한 경우 추가적으로 적용)
        else if (cubeAxisType == CubeAxisType.Z)
        {
            if (isClocked)
            {
                rotaterSelectButtons[3].gameObject.SetActive(true); // Right 방향
            }
            else
            {
                rotaterSelectButtons[2].gameObject.SetActive(true); // Left 방향
            }
        }
    }



}
