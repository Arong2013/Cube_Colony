using System;
using UnityEngine;
using UnityEngine.UI;

public class CountdownUI : MonoBehaviour
{
    [SerializeField] Button nextButton;
    public void OnEnableUI(Action nextButtonAction)
    {
        nextButton.onClick.RemoveAllListeners();    
        gameObject.SetActive(true); 
        nextButton.onClick.AddListener(() => nextButtonAction());   
    }
    public void OnDisableUI()
    {
        gameObject.SetActive(false);    
    }
}
