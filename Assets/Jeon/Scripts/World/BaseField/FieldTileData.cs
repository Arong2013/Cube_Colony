using System.Collections.Generic;

public class FieldTileData
{
    public int ID;
    public int StageLevel;
    public int minMonsterCount;  // 최소 몬스터 수
    public int maxMonsterCount;  // 최대 몬스터 수
    public List<int> ObjectID;
    public List<float> ObjectValue;
}