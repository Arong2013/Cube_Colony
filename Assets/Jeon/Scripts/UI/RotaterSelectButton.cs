using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class RotaterSelectButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    Action RotaterAction;
    bool isPointerEneter = false;
    public void SetUp(Action action)
    {
        RotaterAction = action;
        isPointerEneter = false;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerEneter = true;
        Debug.Log("Enter");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerEneter = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isPointerEneter)
        {
            RotaterAction?.Invoke();
        }
    }
}
