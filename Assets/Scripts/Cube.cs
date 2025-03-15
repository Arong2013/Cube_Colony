using UnityEngine;

public class Cube : MonoBehaviour
{
    CubeRotater cubeRotater;
    PiceGridHandler piceGridHandler;
    [SerializeField] int size;
    public int Size => size;


    public GameObject Pice;

    private void Start()
    {
        piceGridHandler = new PiceGridHandler(this);
        cubeRotater = new CubeRotater(this);
        Init(); 
    }
    public void Init()
    {
        cubeRotater.GenerateCube();
    }

    void Update()
    {
 
    }
}
