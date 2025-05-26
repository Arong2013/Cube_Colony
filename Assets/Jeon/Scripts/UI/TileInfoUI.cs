using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Unity.VisualScripting;

public class TileInfoUI : SerializedMonoBehaviour
{
    [TitleGroup("큐브 정보 UI")]
    [LabelText("큐브 아이콘 이미지"), Required]
    [SerializeField] private Image cubeIconImage;

    [TitleGroup("큐브 정보 UI")]
    [LabelText("큐브 이름 텍스트"), Required]
    [SerializeField] private TextMeshProUGUI cubeNameText; 

    [TitleGroup("큐브 정보 UI")]
    [LabelText("큐브 설명 텍스트"), Required]
    [SerializeField] private TextMeshProUGUI cubeDescriptionText;

    [TitleGroup("출현 정보")]
    [LabelText("출현 몬스터 이미지 부모"), Required]
    [SerializeField] private Transform monsterImageContainer;

    [TitleGroup("출현 정보")]
    [LabelText("출현 아이템 이미지 부모"), Required]
    [SerializeField] private Transform itemImageContainer;

    [TitleGroup("프리팹")]
    [LabelText("몬스터 이미지 프리팹"), Required]
    [SerializeField] private GameObject monsterImagePrefab;

    [TitleGroup("프리팹")]
    [LabelText("아이템 이미지 프리팹"), Required]
    [SerializeField] private GameObject itemImagePrefab;

    [TitleGroup("세부 설정")]
    [LabelText("이미지 크기"), Range(50, 200)]
    [SerializeField] private float imageSize = 100f;

    private void Awake()
    {
        // 초기에는 UI 비활성화
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 큐브 정보를 표시하는 메서드
    /// </summary>
    public void ShowTileInfo(CubieFace selectedFace)
    {
        if (selectedFace == null) return;

        // 큐브 정보 설정
        SetCubeInfo(selectedFace);

        // 출현 정보 표시
        ShowSpawnInformation(selectedFace);

        // UI 활성화
        gameObject.SetActive(true);
    }

    private void SetCubeInfo(CubieFace selectedFace)
    {
        int currentStageLevel = BattleFlowController.Instance?.CurrentStage ?? 1;

        // 필드 타일 데이터 찾기
        var fieldTileData = new FieldTileData().FindFieldTileData(selectedFace.FaceInfo, currentStageLevel);

        cubeIconImage.sprite = DataCenter.Instance.GetFieldTileDataSO(fieldTileData.ID).tileIcon;
        cubeNameText.text = DataCenter.Instance.GetFieldTileDataSO(fieldTileData.ID).tileIconName;
        cubeDescriptionText.text = DataCenter.Instance.GetFieldTileDataSO(fieldTileData.ID).description;
    }

    private void ShowSpawnInformation(CubieFace selectedFace)
    {
        // 기존 아이템, 몬스터 이미지 제거
        ClearContainers();

             int currentStageLevel = BattleFlowController.Instance?.CurrentStage ?? 1;

        // 필드 타일 데이터 찾기
        var fieldTileData = new FieldTileData().FindFieldTileData(selectedFace.FaceInfo, currentStageLevel);
        var fieldTileDataSO = DataCenter.Instance.GetFieldTileDataSO(fieldTileData.ID);

        if (fieldTileData != null)
        {
            // 몬스터 표시
            SpawnMonsters(fieldTileDataSO);
            SpawnItems(fieldTileDataSO);
        }
    }

    private void SpawnMonsters(FieldTileDataSO tileData)
    {
        if (tileData.ObjectID == null) return;

        for (int i = 0; i < tileData.ObjectID.Count; i++)
        {
            // 몬스터 ID와 확률 확인
            int monsterId = tileData.ObjectID[i];

            GameObject monsterPrefab = DataCenter.Instance.GetEntity(monsterId);
            if (monsterPrefab != null)
            {
                // 몬스터 이미지 생성
                GameObject monsterImageObj = Instantiate(monsterImagePrefab, monsterImageContainer);
                Image monsterImage = monsterImageObj.GetComponent<Image>();
                monsterImage.sprite = Resources.Load<Sprite>($"Sprites/Object/{monsterPrefab.name}");
            }
        }
    }

    private void SpawnItems(FieldTileDataSO tileData)
    {
        if (tileData.ItemObjID == null) return;

        for (int i = 0; i < tileData.ItemObjID.Count; i++)
        {
            // 몬스터 ID와 확률 확인
            int itemId = tileData.ItemObjID[i];

            var itemSO = DataCenter.Instance.GetEquipableItemSO(itemId);
            if (itemSO != null)
            {
                // 몬스터 이미지 생성
                GameObject monsterImageObj = Instantiate(monsterImagePrefab, monsterImageContainer);
                Image monsterImage = monsterImageObj.GetComponent<Image>();
                monsterImage.sprite = itemSO.itemIcon;
            }
        }
    }

    private void ClearContainers()
    {
        // 기존 아이템, 몬스터 이미지 제거
        foreach (Transform child in monsterImageContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in itemImageContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private string GetCubeNameBySkillType(CubieFaceSkillType skillType)
    {
        return skillType switch
        {
            CubieFaceSkillType.RMonster => "적대적 몬스터 타일",
            CubieFaceSkillType.AMonster => "우호적 몬스터 타일",
            CubieFaceSkillType.Mine => "광산 타일",
            CubieFaceSkillType.Plant => "식물 타일",
            _ => "알 수 없는 타일"
        };
    }

    private string GetCubeDescriptionBySkillType(CubieFaceSkillType skillType)
    {
        return skillType switch
        {
            CubieFaceSkillType.RMonster => "적대적인 몬스터가 출현할 수 있는 위험한 타일입니다.",
            CubieFaceSkillType.AMonster => "우호적인 몬스터나 보상이 있을 수 있는 타일입니다.",
            CubieFaceSkillType.Mine => "광물이나 자원을 얻을 수 있는 채굴 타일입니다.",
            CubieFaceSkillType.Plant => "식물이나 농작물을 발견할 수 있는 타일입니다.",
            _ => "특별한 정보가 없는 타일입니다."
        };
    }

    // 닫기 버튼 핸들러
    public void OnCloseButtonClicked()
    {
        gameObject.SetActive(false);
    }
}