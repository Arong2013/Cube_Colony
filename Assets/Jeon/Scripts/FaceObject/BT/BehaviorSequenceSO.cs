using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBehaviorSequence", menuName = "Behavior/Sequence")]
public class BehaviorSequenceSO : ScriptableObject
{
    [SerializeField] private List<BehaviorStepSO> stepSOs;

    public BehaviorSequence CreateBehaviorSequence(FaceObject faceObject)
    {
        return new BehaviorSequence(faceObject, stepSOs);
    }
}