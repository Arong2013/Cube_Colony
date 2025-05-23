using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FieldTileSlot : MonoBehaviour
{
    [SerializeField] private Image tileIcon;
    [SerializeField] private TextMeshProUGUI tileNameText;

    public void Initialize(FieldTileDataSO fieldTileSO)
    {
        tileIcon.sprite = fieldTileSO.tileIcon;
        tileNameText.text = $"Stage {fieldTileSO.StageLevel} - 필드 {fieldTileSO.ID}";
    }
}