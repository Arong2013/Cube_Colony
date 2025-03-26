using System.Collections.Generic;
using UnityEngine;

public static class UnitConditionHelper
{
    /// <summary>
    /// 특정 범위 내에서 우선순위에 따라 적 유닛 감지
    /// </summary>
    public static List<FaceUnit> GetEnemiesInRange(FaceUnit faceUnit,bool isAttackable)
    {
        List<FaceUnit> detectedEnemies = new List<FaceUnit>();
        List<UnitType> priorityList = UnitPriorityHelper.GetPriorityList(faceUnit.PropertyType);

        var range = isAttackable ? faceUnit.AttackRange : faceUnit.Range;   
        Collider[] colliders = Physics.OverlapSphere(faceUnit.transform.position, range);

        foreach (var col in colliders)
        {
            FaceUnit targetUnit = col.GetComponent<FaceUnit>();

            if (targetUnit != null && targetUnit.CubeFaceType == faceUnit.CubeFaceType)
            {
                if (priorityList.Contains(targetUnit.UnitType))
                {
                    detectedEnemies.Add(targetUnit);
                }
            }
        }
        detectedEnemies.Sort((unit1, unit2) =>
        {
            int priority1 = priorityList.IndexOf(unit1.UnitType);
            int priority2 = priorityList.IndexOf(unit2.UnitType);

            // 우선순위가 낮은 값이 먼저 오도록 정렬
            return priority1.CompareTo(priority2);
        });

        return detectedEnemies;
    }
    public static List<ExitGateObject> GetExitGateObjects(FaceUnit faceUnit)
    {
        List<ExitGateObject> exitGateObjects = new List<ExitGateObject>();
        Collider[] colliders = Physics.OverlapSphere(faceUnit.transform.position,300f);
        foreach (var col in colliders)
        {
            ExitGateObject targetUnit = col.GetComponent<ExitGateObject>();
            if (targetUnit != null)
            {
                exitGateObjects.Add(targetUnit);
            }
        }
        return exitGateObjects; 
    }

}
