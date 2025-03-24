using System;
using UnityEngine;

public enum CubeFaceType
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
    public bool isWalkable = true;
    public  CubieFace[] faces = new CubieFace[6];

    private void Awake()
    {
        for (int i = 0; i < faces.Length; i++)
        {
            faces[i].Init((CubeFaceType)i,this);   
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
            new[] { (int)CubeFaceType.Back, (int)CubeFaceType.Top, (int)CubeFaceType.Front, (int)CubeFaceType.Bottom }
            : new[] { (int)CubeFaceType.Bottom, (int)CubeFaceType.Front, (int)CubeFaceType.Top, (int)CubeFaceType.Back };
        RotateFaces(indices);
    }

    private void RotateY(bool clockwise)
    {
        int[] indices = clockwise ?
            new[] { (int)CubeFaceType.Left, (int)CubeFaceType.Front, (int)CubeFaceType.Right, (int)CubeFaceType.Back }
            : new[] { (int)CubeFaceType.Back, (int)CubeFaceType.Right, (int)CubeFaceType.Front, (int)CubeFaceType.Left };
        RotateFaces(indices);
    }

    private void RotateZ(bool clockwise)
    {
        int[] indices = clockwise ?
            new[] { (int)CubeFaceType.Left, (int)CubeFaceType.Top, (int)CubeFaceType.Right, (int)CubeFaceType.Bottom }
            : new[] { (int)CubeFaceType.Bottom, (int)CubeFaceType.Right, (int)CubeFaceType.Top, (int)CubeFaceType.Left };
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
            faces[indices[i]].SetFace((CubeFaceType)indices[i]);
        }
    }
}
