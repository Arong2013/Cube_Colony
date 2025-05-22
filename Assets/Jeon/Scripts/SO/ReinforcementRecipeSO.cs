using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Linq;

[CreateAssetMenu(fileName = "ReinforcementRecipe", menuName = "Data/ReinforcementRecipe")]
public class ReinforcementRecipeSO : ScriptableObject
{
    [TitleGroup("기본 정보")]
    [LabelText("조합법 ID"), ReadOnly]
    public int ID;

    [TitleGroup("기본 정보")]
    [LabelText("조합법 이름")]
    public string recipeName;

    [TitleGroup("강화 재료")]
    [LabelText("필요 아이템 ID 목록")]
    [InfoBox("강화에 필요한 아이템들의 ID")]
    public List<int> requiredItemIDs = new List<int>();

    [TitleGroup("강화 재료")]
    [LabelText("필요 아이템 개수 목록")]
    [InfoBox("각 아이템별 필요 개수 (위 ID 목록과 1:1 대응)")]
    public List<int> requiredItemCounts = new List<int>();
    
    [TitleGroup("강화 설정")]
    [LabelText("추가 골드 비용"), MinValue(0)]
    [Tooltip("강화에 필요한 추가 골드 비용 (옵션)")]
    public int additionalGoldCost = 0;
    
    [TitleGroup("강화 설정")]
    [LabelText("기본 성공률 (%)"), Range(0, 100)]
    [Tooltip("강화 성공 확률 (%)")]
    public float baseSuccessRate = 70f;

    [TitleGroup("디버그 정보")]
    [ShowInInspector, ReadOnly]
    [LabelText("재료 개수")]
    public int MaterialCount => requiredItemIDs?.Count ?? 0;

    [TitleGroup("디버그 정보")]
    [ShowInInspector, ReadOnly]
    [LabelText("총 재료 수량")]
    public int TotalMaterialCount
    {
        get
        {
            if (requiredItemCounts == null) return 0;
            int total = 0;
            foreach (int count in requiredItemCounts)
            {
                total += count;
            }
            return total;
        }
    }

    [TitleGroup("디버그 정보")]
    [Button("재료 정보 출력")]
    public void PrintMaterialInfo()
    {
        Debug.Log($"=== {recipeName} (ID: {ID}) 재료 정보 ===");
        
        if (requiredItemIDs == null || requiredItemCounts == null)
        {
            Debug.LogWarning("재료 데이터가 없습니다.");
            return;
        }

        if (requiredItemIDs.Count != requiredItemCounts.Count)
        {
            Debug.LogError($"❌ 재료 ID와 개수 목록의 크기가 다릅니다! ID: {requiredItemIDs.Count}, Count: {requiredItemCounts.Count}");
            return;
        }

        for (int i = 0; i < requiredItemIDs.Count; i++)
        {
            Debug.Log($"  재료 {i + 1}: 아이템 ID {requiredItemIDs[i]} x {requiredItemCounts[i]}개");
        }

        if (additionalGoldCost > 0)
        {
            Debug.Log($"  추가 골드: {additionalGoldCost}");
        }

        Debug.Log($"  성공률: {baseSuccessRate}%");
    }

    [TitleGroup("유틸리티")]
    [Button("재료 데이터 검증")]
    public void ValidateRecipeData()
    {
        bool isValid = true;

        // 기본 데이터 검증
        if (string.IsNullOrEmpty(recipeName))
        {
            Debug.LogError("❌ 조합법 이름이 비어있습니다.");
            isValid = false;
        }

        if (requiredItemIDs == null || requiredItemIDs.Count == 0)
        {
            Debug.LogError("❌ 필요 아이템 ID 목록이 비어있습니다.");
            isValid = false;
        }

        if (requiredItemCounts == null || requiredItemCounts.Count == 0)
        {
            Debug.LogError("❌ 필요 아이템 개수 목록이 비어있습니다.");
            isValid = false;
        }

        // 목록 크기 일치 확인
        if (requiredItemIDs != null && requiredItemCounts != null)
        {
            if (requiredItemIDs.Count != requiredItemCounts.Count)
            {
                Debug.LogError($"❌ 아이템 ID 목록과 개수 목록의 크기가 다릅니다! ID: {requiredItemIDs.Count}, Count: {requiredItemCounts.Count}");
                isValid = false;
            }
        }

        // 개수 유효성 검증
        if (requiredItemCounts != null)
        {
            for (int i = 0; i < requiredItemCounts.Count; i++)
            {
                if (requiredItemCounts[i] <= 0)
                {
                    Debug.LogError($"❌ 재료 {i + 1}의 개수가 0 이하입니다: {requiredItemCounts[i]}");
                    isValid = false;
                }
            }
        }

        // 성공률 유효성 검증
        if (baseSuccessRate < 0f || baseSuccessRate > 100f)
        {
            Debug.LogError($"❌ 성공률이 유효하지 않습니다: {baseSuccessRate}% (0-100 범위여야 함)");
            isValid = false;
        }

        // 중복 아이템 ID 검증
        if (requiredItemIDs != null)
        {
            var duplicates = requiredItemIDs.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key);
            foreach (var duplicate in duplicates)
            {
                Debug.LogWarning($"⚠️ 중복된 아이템 ID 발견: {duplicate}");
            }
        }

        if (isValid)
        {
            Debug.Log($"✅ {recipeName} 조합법 데이터가 유효합니다!");
        }
    }

    [TitleGroup("유틸리티")]
    [Button("재료 목록 정렬")]
    public void SortMaterialsByID()
    {
        if (requiredItemIDs == null || requiredItemCounts == null || 
            requiredItemIDs.Count != requiredItemCounts.Count)
        {
            Debug.LogError("재료 데이터가 유효하지 않습니다.");
            return;
        }

        // ID와 개수를 쌍으로 묶어서 정렬
        var pairs = new List<(int id, int count)>();
        for (int i = 0; i < requiredItemIDs.Count; i++)
        {
            pairs.Add((requiredItemIDs[i], requiredItemCounts[i]));
        }

        // ID 기준으로 정렬
        pairs.Sort((a, b) => a.id.CompareTo(b.id));

        // 정렬된 결과를 다시 리스트에 적용
        requiredItemIDs.Clear();
        requiredItemCounts.Clear();

        foreach (var pair in pairs)
        {
            requiredItemIDs.Add(pair.id);
            requiredItemCounts.Add(pair.count);
        }

        Debug.Log($"✅ {recipeName} 재료 목록이 ID 순으로 정렬되었습니다.");
    }

    /// <summary>
    /// 유니티 에디터에서 값 변경 시 자동 검증
    /// </summary>
    private void OnValidate()
    {
        // 음수 방지
        if (additionalGoldCost < 0)
            additionalGoldCost = 0;

        // 성공률 범위 제한
        baseSuccessRate = Mathf.Clamp(baseSuccessRate, 0f, 100f);

        // 개수 목록의 음수 방지
        if (requiredItemCounts != null)
        {
            for (int i = 0; i < requiredItemCounts.Count; i++)
            {
                if (requiredItemCounts[i] < 0)
                    requiredItemCounts[i] = 1;
            }
        }

        // 목록 크기 자동 조정 (ID 목록 크기에 맞춤)
        if (requiredItemIDs != null && requiredItemCounts != null)
        {
            while (requiredItemCounts.Count < requiredItemIDs.Count)
            {
                requiredItemCounts.Add(1);
            }
            while (requiredItemCounts.Count > requiredItemIDs.Count)
            {
                requiredItemCounts.RemoveAt(requiredItemCounts.Count - 1);
            }
        }
    }
}