using System;
using UnityEngine;
public class EntityAnimatorHandler
{
    private Animator animator;
    public EntityAnimatorHandler(Animator animator)
    {
        this.animator = animator;
    }
    public void SetAnimatorValue<T>(T type, object value) where T : Enum
    {
        string parameterName = type.ToString();

        if (value is bool boolValue)
        {
            animator.SetBool(parameterName, boolValue);
        }
        else if (value is float floatValue)
        {
            animator.SetFloat(parameterName, floatValue);
        }
        else if (value is int intValue)
        {
            animator.SetInteger(parameterName, intValue);
        }
        else if(value is null)
        {
            animator.SetTrigger(parameterName); 
        }  
    }
    public TResult GetAnimatorValue<T, TResult>(T type) where T : Enum
    {
        string parameterName = type.ToString();

        if (typeof(TResult) == typeof(bool))
        {
            return (TResult)(object)animator.GetBool(parameterName);
        }
        else if (typeof(TResult) == typeof(float))
        {
            return (TResult)(object)animator.GetFloat(parameterName);
        }
        else if (typeof(TResult) == typeof(int))
        {
            return (TResult)(object)animator.GetInteger(parameterName);
        }
        else
        {
            Debug.LogError($"Unsupported type for parameter: {parameterName}");
            return default;
        }
    }
}