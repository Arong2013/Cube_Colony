using System;
using UnityEngine;

public enum CubieFaceIndex
{
    Front,  // 정면
    Back,   // 후면
    Left,   // 왼쪽
    Right,  // 오른쪽
    Top,    // 위쪽
    Bottom  // 아래쪽
}
public class Cubie : MonoBehaviour
{
    [SerializeField] private CubieFace[] faces = new CubieFace[6];

    private void Awake()
    {
        for (int i = 0; i < faces.Length; i++)
        {
            faces[i].SetFace((CubieFaceIndex)i,this);   
        }
    }

    public void RotateCubie(CubeAxisType axis, bool clockwise)
    {
        switch (axis)
        {
            case CubeAxisType.X:
                RotateX(clockwise);
                break;
            case CubeAxisType.Y:
                RotateY(clockwise);
                break;
            case CubeAxisType.Z:
                RotateZ(clockwise);
                break;
        }
    }

    private void RotateX(bool clockwise)
    {
        int[] indices = clockwise ?
            new[] { (int)CubieFaceIndex.Back, (int)CubieFaceIndex.Top, (int)CubieFaceIndex.Front, (int)CubieFaceIndex.Bottom }
            : new[] { (int)CubieFaceIndex.Bottom, (int)CubieFaceIndex.Front, (int)CubieFaceIndex.Top, (int)CubieFaceIndex.Back };
        RotateFaces(indices);
    }

    private void RotateY(bool clockwise)
    {
        int[] indices = clockwise ?
            new[] { (int)CubieFaceIndex.Left, (int)CubieFaceIndex.Front, (int)CubieFaceIndex.Right, (int)CubieFaceIndex.Back }
            : new[] { (int)CubieFaceIndex.Back, (int)CubieFaceIndex.Right, (int)CubieFaceIndex.Front, (int)CubieFaceIndex.Left };
        RotateFaces(indices);
    }

    private void RotateZ(bool clockwise)
    {
        int[] indices = clockwise ?
            new[] { (int)CubieFaceIndex.Left, (int)CubieFaceIndex.Top, (int)CubieFaceIndex.Right, (int)CubieFaceIndex.Bottom }
            : new[] { (int)CubieFaceIndex.Bottom, (int)CubieFaceIndex.Right, (int)CubieFaceIndex.Top, (int)CubieFaceIndex.Left };
        RotateFaces(indices);
    }

    private void RotateFaces(int[] indices)
    {
        CubieFace temp = faces[indices[0]];
        faces[indices[0]] = faces[indices[1]];
        faces[indices[1]] = faces[indices[2]];
        faces[indices[2]] = faces[indices[3]];
        faces[indices[3]] = temp;

        for (int i = 0; i < indices.Length; i++)
        {
            faces[indices[i]].SetFace((CubieFaceIndex)indices[i], this);
        }
    }
}
