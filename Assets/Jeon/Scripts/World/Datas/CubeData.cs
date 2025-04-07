using System.Collections.Generic;

[System.Serializable]
public class CubeData
{
    public int size;  // 큐브 한 변 길이
    public int requiredMatches;  // 해당 스테이지에서 클리어를 위해 필요한 줄 완성 횟수
    public Dictionary<CubieFaceSkillType, float> skillProbabilities = new Dictionary<CubieFaceSkillType, float>(); // 스킬 타입별 확률

    //public CubeData(int size, int requiredMatches, Dictionary<CubieFaceSkillType, float> skillProbabilities)
    //{
    //    this.size = size;
    //    this.requiredMatches = requiredMatches;
    //    this.skillProbabilities = skillProbabilities;
    //}
}
