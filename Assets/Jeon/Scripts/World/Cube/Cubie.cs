using System;
using UnityEngine;


public class Cubie : MonoBehaviour
{
    public  CubieFace[] faces = new CubieFace[6];
    public void Init()
    {
        for (int i = 0; i < faces.Length; i++)
        {
            faces[i].Init((CubeFaceType)i, this);
        }
    }
    private void Awake()
    {

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
    public CubieFace GetFace(CubeFaceType type)
    {
        foreach (var face in GetComponentsInChildren<CubieFace>())
        {
            if (face.face == type)
                return face;
        }
        return null;
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
