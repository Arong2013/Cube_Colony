using UnityEngine;
using UnityEngine.UI;
public class InSurvivalStateUI: MonoBehaviour
{
    [SerializeField] PlayerIonsAndBar bar;
    [SerializeField] Slider survivalStateUI;    
    private void OnEnable()
    {
        bar.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        bar.gameObject.SetActive(false);
        survivalStateUI.gameObject.SetActive(false);    
    }

    public void Initialize()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void EnterReturn()
    {
        survivalStateUI.gameObject.SetActive(true); 
    }
    public void ExitReturn()
    {
        survivalStateUI.gameObject.SetActive(false);
    }   
    public void UpdateReturn(float retrunTime,float curTime)
    {
        float progress = curTime / retrunTime;
        Debug.Log($"[UpdateReturn] cur: {curTime}, max: {retrunTime}, value: {progress}");
        survivalStateUI.value = progress;
    }       
}