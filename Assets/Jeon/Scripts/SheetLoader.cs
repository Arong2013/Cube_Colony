using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SheetLoader : MonoBehaviour
{
    const string ConsumableItemURL = "https://docs.google.com/spreadsheets/d/1KDTCr-4-E_XC6CY_gD8N_BvpDukUzlhoTXrCtnLWdr4/export?format=tsv&range=A1:D&gid=0";
    const string EquipableItemURL = "https://docs.google.com/spreadsheets/d/1KDTCr-4-E_XC6CY_gD8N_BvpDukUzlhoTXrCtnLWdr4/export?format=tsv&range=A1:B&gid=584148317";
    const string ItemAcionURL = "https://docs.google.com/spreadsheets/d/1KDTCr-4-E_XC6CY_gD8N_BvpDukUzlhoTXrCtnLWdr4/export?format=tsv&range=A1:C&gid=972242930";
    const string FieldTileData = "https://docs.google.com/spreadsheets/d/1KDTCr-4-E_XC6CY_gD8N_BvpDukUzlhoTXrCtnLWdr4/export?format=tsv&range=A1:F&gid=197591956";


    [ShowInInspector] ConsumableItem testItem;
    [ShowInInspector] HealAction testAction;
    void Start()
    {
        StartCoroutine(LoadItemsFromSheet());
    }
    IEnumerator LoadItemsFromSheet()
    {
        UnityWebRequest www = UnityWebRequest.Get(ConsumableItemURL);
        yield return www.SendWebRequest();
        string tsv = www.downloadHandler.text;
        List<ConsumableItem> EquipableDetails = TSVParser.Parse<ConsumableItem>(tsv);
        foreach (var item in EquipableDetails)
        {
            ItemDataCenter.Register<ConsumableItem>(item);
        }
        www = UnityWebRequest.Get(EquipableItemURL);
        yield return www.SendWebRequest();
        tsv = www.downloadHandler.text;
        List<EquipableItem> ConsumeDetails = TSVParser.Parse<EquipableItem>(tsv);
        foreach (var item in ConsumeDetails)
        {
            ItemDataCenter.Register<EquipableItem>(item);
        }

        www = UnityWebRequest.Get(ItemAcionURL);
        yield return www.SendWebRequest();
        tsv = www.downloadHandler.text;
        List<itemAction> IitemActions = TSVParser.ParseItemAcion(tsv);
        foreach (var item in IitemActions)
        {
            ItemDataCenter.Register<itemAction>(item);
        }

        www = UnityWebRequest.Get(FieldTileData);
        yield return www.SendWebRequest();
        tsv = www.downloadHandler.text;
        List<FieldTileData> fieldTileDatas = TSVParser.Parse<FieldTileData>(tsv);
        foreach (var item in fieldTileDatas)
        {
            ItemDataCenter.Register<FieldTileData>(item);
        }

        testAction = ItemDataCenter.Get<itemAction>(1) as HealAction;
        testItem = ItemDataCenter.Get<ConsumableItem>(1);

    }
}
