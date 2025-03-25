using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class UnitPriorityHelper
{
    public static List<UnitType> GetPriorityList(PriorityNameType priorityName)
    {
        switch (priorityName)
        {
            case PriorityNameType.AttackEnemy:
                return new List<UnitType> { UnitType.NPC, UnitType.Enemy, UnitType.AllyTower };
            case PriorityNameType.DefEnemy:
                return new List<UnitType> { UnitType.Ally, UnitType.AllyTower, UnitType.NPC };
            case PriorityNameType.DefAlly:
                return new List<UnitType> { UnitType.Ally, UnitType.AllyTower, UnitType.NPC };
            case PriorityNameType.AttackAlly:
                return new List<UnitType> { UnitType.Enemy, UnitType.NPC, UnitType.AllyTower };
            default:
                return new List<UnitType>();
        }
    }
}
