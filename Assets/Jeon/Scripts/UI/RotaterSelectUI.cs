using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotaterSelectUI : MonoBehaviour
{
    [SerializeField] GameObject rotaterSelectUI;
    [SerializeField] List<RotaterSelectButton> rotaterSelectButtons;
    private Action<Cubie, CubeAxisType, int> rotateCube;

    public void SetRotateCubeUpdate(Action<Cubie, CubeAxisType, int> rotateCube)
    {
        this.rotateCube = rotateCube;
    }
    public void EnableUI(CubieFace cubieFace)
    {
        rotaterSelectUI.gameObject.SetActive(true);
        var rectTransform = rotaterSelectUI.GetComponent<RectTransform>();
        //rotaterSelectUI. transform.position = cubieFace.transform.position;

        if(cubieFace.face == CubieFaceIndex.Front || cubieFace.face == CubieFaceIndex.Back)
        {
            rectTransform.position = cubieFace.transform.position + new Vector3(0, 0.5f, 0);
            rectTransform.rotation = Quaternion.Euler(0, 0, 0);
            rotaterSelectButtons[0].SetUp(() => rotateCube(cubieFace.cubie, CubeAxisType.Y, -90));
        }
        if (cubieFace.face == CubieFaceIndex.Left || cubieFace.face == CubieFaceIndex.Right)
        {
            rectTransform.position = cubieFace.transform.position + new Vector3(0,0.5f,0);
            rectTransform.rotation = Quaternion.Euler(0, 90, 0);
        }
        if (cubieFace.face == CubieFaceIndex.Top || cubieFace.face == CubieFaceIndex.Back)
        {
            rectTransform.position = cubieFace.transform.position  + new Vector3(0,0,0.5f);
            rectTransform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }
}
