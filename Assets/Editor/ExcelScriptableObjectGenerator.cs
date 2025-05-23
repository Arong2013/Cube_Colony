#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Data;
using ExcelDataReader;

/// <summary>
/// 통합된 ExcelScriptableObjectGenerator
/// 범용 리플렉션 기반 처리 + 특정 타입별 특화 처리를 모두 지원합니다.
/// </summary>
public static class ExcelScriptableObjectGenerator
{
    // === 메인 진입점 ===

    /// <summary>
    /// 범용 Excel → ScriptableObject 생성 메서드 (기존 메서드)
    /// </summary>
    public static void GenerateFromExcel<T>(DataTable sheet, string outputFolder) where T : ScriptableObject
    {
        Debug.Log($"\n=== {typeof(T).Name} 처리 시작 (범용 모드) ===");

        // 특화 메서드가 있는 타입인지 확인하고 있다면 특화 메서드 사용
        if (typeof(T) == typeof(EquipableItemSO))
        {
            GenerateEquipableItemFromExcel(sheet, outputFolder);
            return;
        }
        if (typeof(T) == typeof(ConsumableItemSO))
        {
            GenerateConsumableItemFromExcel(sheet, outputFolder);
            return;
        }
        if (typeof(T) == typeof(FieldTileDataSO))
        {
            GenerateFieldTileDataFromExcel(sheet, outputFolder);
            return;
        }
        if (typeof(T) == typeof(ItemActionSO))
        {
            GenerateItemActionFromExcel(sheet, outputFolder);
            return;
        }

        // 특화 메서드가 없는 타입은 범용 처리
        ProcessGenericType<T>(sheet, outputFolder);
    }

    // === 특화 타입별 메서드들 ===

    /// <summary>
    /// EquipableItemSO 전용 생성 메서드
    /// </summary>
    public static void GenerateEquipableItemFromExcel(DataTable sheet, string outputFolder)
    {
        Debug.Log($"\n=== EquipableItemSO 처리 시작 (특화 모드) ===");

        if (sheet.Rows.Count < 2)
        {
            Debug.LogWarning($"[ExcelParser] {sheet.TableName} 시트에 유효한 데이터가 없습니다.");
            return;
        }

        var headers = ExtractHeaders(sheet);
        string typeFolder = EnsureOutputFolderExists(outputFolder, typeof(EquipableItemSO));

        int createdCount = 0;
        int skippedCount = 0;

        for (int i = 1; i < sheet.Rows.Count; i++)
        {
            try
            {
                var values = sheet.Rows[i].ItemArray;

                // 빈 행 체크
                bool isEmptyRow = values.All(v => string.IsNullOrWhiteSpace(v?.ToString()));
                if (isEmptyRow)
                {
                    skippedCount++;
                    continue;
                }

                var so = ScriptableObject.CreateInstance<EquipableItemSO>();
                string nameValue = ProcessEquipableItemRow(so, headers, values, i);

                if (!string.IsNullOrEmpty(nameValue) && ValidateAndCreateAsset(so, nameValue, typeFolder, i))
                {
                    createdCount++;
                }
                else
                {
                    skippedCount++;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[ExcelParser] 행 {i + 1} 처리 중 오류: {e.Message}");
                skippedCount++;
            }
        }

        FinalizeAssetCreation(typeof(EquipableItemSO), createdCount, skippedCount);
        AutoRegisterToDataCenterIfExists();
    }

    /// <summary>
    /// ConsumableItemSO 전용 생성 메서드
    /// </summary>
    public static void GenerateConsumableItemFromExcel(DataTable sheet, string outputFolder)
    {
        Debug.Log($"\n=== ConsumableItemSO 처리 시작 (특화 모드) ===");

        if (sheet.Rows.Count < 2)
        {
            Debug.LogWarning($"[ExcelParser] {sheet.TableName} 시트에 유효한 데이터가 없습니다.");
            return;
        }

        var headers = ExtractHeaders(sheet);
        string typeFolder = EnsureOutputFolderExists(outputFolder, typeof(ConsumableItemSO));

        int createdCount = 0;
        int skippedCount = 0;

        for (int i = 1; i < sheet.Rows.Count; i++)
        {
            try
            {
                var values = sheet.Rows[i].ItemArray;

                // 빈 행 체크
                bool isEmptyRow = values.All(v => string.IsNullOrWhiteSpace(v?.ToString()));
                if (isEmptyRow)
                {
                    skippedCount++;
                    continue;
                }

                var so = ScriptableObject.CreateInstance<ConsumableItemSO>();
                string nameValue = ProcessConsumableItemRow(so, headers, values, i);

                if (!string.IsNullOrEmpty(nameValue) && ValidateAndCreateAsset(so, nameValue, typeFolder, i))
                {
                    createdCount++;
                }
                else
                {
                    skippedCount++;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[ExcelParser] 행 {i + 1} 처리 중 오류: {e.Message}");
                skippedCount++;
            }
        }

        FinalizeAssetCreation(typeof(ConsumableItemSO), createdCount, skippedCount);
        AutoRegisterToDataCenterIfExists();
    }

    /// <summary>
    /// FieldTileDataSO 전용 생성 메서드
    /// </summary>
    public static void GenerateFieldTileDataFromExcel(DataTable sheet, string outputFolder)
    {
        Debug.Log($"\n=== FieldTileDataSO 처리 시작 (특화 모드) ===");

        if (sheet.Rows.Count < 2)
        {
            Debug.LogWarning($"[ExcelParser] {sheet.TableName} 시트에 유효한 데이터가 없습니다.");
            return;
        }

        var headers = ExtractHeaders(sheet);
        string typeFolder = EnsureOutputFolderExists(outputFolder, typeof(FieldTileDataSO));

        int createdCount = 0;
        int skippedCount = 0;

        for (int i = 1; i < sheet.Rows.Count; i++)
        {
            try
            {
                var values = sheet.Rows[i].ItemArray;

                // 빈 행 체크
                bool isEmptyRow = values.All(v => string.IsNullOrWhiteSpace(v?.ToString()));
                if (isEmptyRow)
                {
                    skippedCount++;
                    continue;
                }

                var so = ScriptableObject.CreateInstance<FieldTileDataSO>();
                string nameValue = ProcessFieldTileDataRow(so, headers, values, i);

                if (!string.IsNullOrEmpty(nameValue) && ValidateAndCreateAsset(so, nameValue, typeFolder, i))
                {
                    createdCount++;
                }
                else
                {
                    skippedCount++;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[ExcelParser] 행 {i + 1} 처리 중 오류: {e.Message}");
                skippedCount++;
            }
        }

        FinalizeAssetCreation(typeof(FieldTileDataSO), createdCount, skippedCount);
        AutoRegisterToDataCenterIfExists();
    }

    /// <summary>
    /// ItemActionSO 전용 생성 메서드
    /// </summary>
    public static void GenerateItemActionFromExcel(DataTable sheet, string outputFolder)
    {
        Debug.Log($"\n=== ItemActionSO 처리 시작 (특화 모드) ===");

        if (sheet.Rows.Count < 2)
        {
            Debug.LogWarning($"[ExcelParser] {sheet.TableName} 시트에 유효한 데이터가 없습니다.");
            return;
        }

        var headers = ExtractHeaders(sheet);
        string typeFolder = EnsureOutputFolderExists(outputFolder, typeof(ItemActionSO));

        int createdCount = 0;
        int skippedCount = 0;

        for (int i = 1; i < sheet.Rows.Count; i++)
        {
            try
            {
                var values = sheet.Rows[i].ItemArray;

                // 빈 행 체크
                bool isEmptyRow = values.All(v => string.IsNullOrWhiteSpace(v?.ToString()));
                if (isEmptyRow)
                {
                    skippedCount++;
                    continue;
                }

                var so = ScriptableObject.CreateInstance<ItemActionSO>();
                string nameValue = ProcessItemActionRow(so, headers, values, i);

                if (!string.IsNullOrEmpty(nameValue) && ValidateAndCreateAsset(so, nameValue, typeFolder, i))
                {
                    createdCount++;
                }
                else
                {
                    skippedCount++;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[ExcelParser] 행 {i + 1} 처리 중 오류: {e.Message}");
                skippedCount++;
            }
        }

        FinalizeAssetCreation(typeof(ItemActionSO), createdCount, skippedCount);
        AutoRegisterToDataCenterIfExists();
    }

    // === 범용 처리 메서드 ===

    /// <summary>
    /// 범용 타입 처리 (기존 로직 유지)
    /// </summary>
    private static void ProcessGenericType<T>(DataTable sheet, string outputFolder) where T : ScriptableObject
    {
        if (sheet.Rows.Count < 2)
        {
            Debug.LogWarning($"[ExcelParser] {sheet.TableName} 시트에 유효한 데이터가 없습니다.");
            return;
        }

        var headers = ExtractHeaders(sheet);
        var fields = GetAllFieldsIncludingInherited(typeof(T));

        // 특별 디버깅: ItemActionSO와 FieldTileDataSO만
        if (typeof(T) == typeof(ItemActionSO) || typeof(T) == typeof(FieldTileDataSO))
        {
            Debug.Log($"[디버그] {typeof(T).Name} 상세 분석:");
            Debug.Log($"  시트명: {sheet.TableName}");
            Debug.Log($"  헤더 개수: {headers.Length}");
            Debug.Log($"  헤더 목록: [{string.Join(", ", headers.Select(h => $"'{h}'"))}]");
            Debug.Log($"  필드 개수: {fields.Length}");
            Debug.Log($"  필드 목록: [{string.Join(", ", fields.Select(f => f.Name))}]");

            // name 관련 필드 찾기
            var nameFields = fields.Where(f => IsNameField(f)).ToArray();
            Debug.Log($"  name 관련 필드: [{string.Join(", ", nameFields.Select(f => f.Name))}]");

            // name 관련 헤더 찾기
            var nameHeaders = headers.Where(h => IsNameHeader(h)).ToArray();
            Debug.Log($"  name 관련 헤더: [{string.Join(", ", nameHeaders.Select(h => $"'{h}'"))}]");
        }

        string typeFolder = EnsureOutputFolderExists(outputFolder, typeof(T));

        int createdCount = 0;
        int skippedCount = 0;

        for (int i = 1; i < sheet.Rows.Count; i++)
        {
            try
            {
                var values = sheet.Rows[i].ItemArray;

                // 빈 행 체크
                bool isEmptyRow = values.All(v => string.IsNullOrWhiteSpace(v?.ToString()));
                if (isEmptyRow)
                {
                    skippedCount++;
                    continue;
                }

                var so = ScriptableObject.CreateInstance<T>();
                string nameValue = ProcessRowDataWithReflection(so, headers, values, fields, i);

                if (nameValue != null && ValidateAndCreateAsset(so, nameValue, typeFolder, i))
                {
                    createdCount++;
                }
                else
                {
                    skippedCount++;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[ExcelParser] 행 {i + 1} 처리 중 오류: {e.Message}");
                Debug.LogError($"스택 트레이스: {e.StackTrace}");
                skippedCount++;
            }
        }

        FinalizeAssetCreation(typeof(T), createdCount, skippedCount);
        AutoRegisterToDataCenterIfExists();
    }

    // === 특화 타입별 행 처리 메서드들 ===

    /// <summary>
    /// EquipableItemSO 행 데이터 처리
    /// </summary>
    private static string ProcessEquipableItemRow(EquipableItemSO so, string[] headers, object[] values, int rowIndex)
    {
        string nameValue = null;

        for (int j = 0; j < headers.Length && j < values.Length; j++)
        {
            string header = headers[j].Trim();
            string rawValue = values[j]?.ToString()?.Trim();

            if (string.IsNullOrEmpty(header)) continue;

            try
            {
                switch (header.ToLower().Replace(" ", "").Replace("_", ""))
                {
                    case "id":
                        so.ID = ParseInt(rawValue);
                        break;
                    case "itemname":
                    case "name":
                        so.ItemName = rawValue ?? "";
                        nameValue = so.ItemName;
                        break;
                    case "equipmenttype":
                        so.equipmentType = ParseEquipmentType(rawValue);
                        break;
                    case "requiredlevel":
                        so.requiredLevel = ParseInt(rawValue);
                        break;
                    case "attackbonus":
                        so.attackBonus = ParseFloat(rawValue);
                        break;
                    case "defensebonus":
                        so.defenseBonus = ParseFloat(rawValue);
                        break;
                    case "healthbonus":
                        so.healthBonus = ParseFloat(rawValue);
                        break;
                    case "description":
                        so.description = rawValue ?? "";
                        break;
                    case "grade":
                        so.grade = ParseItemGrade(rawValue);
                        break;
                    case "reinforcementrecipeid":
                        so.reinforcementRecipeId = ParseInt(rawValue);
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ExcelParser] 필드 {header} 파싱 실패 (행 {rowIndex + 1}): {e.Message}");
            }
        }

        // name이 없다면 ID로 생성
        if (string.IsNullOrEmpty(nameValue))
        {
            nameValue = $"Equipment_{so.ID}";
            so.ItemName = nameValue;
        }

        return nameValue;
    }

    /// <summary>
    /// ConsumableItemSO 행 데이터 처리
    /// </summary>
    private static string ProcessConsumableItemRow(ConsumableItemSO so, string[] headers, object[] values, int rowIndex)
    {
        string nameValue = null;

        for (int j = 0; j < headers.Length && j < values.Length; j++)
        {
            string header = headers[j].Trim();
            string rawValue = values[j]?.ToString()?.Trim();

            if (string.IsNullOrEmpty(header)) continue;

            try
            {
                switch (header.ToLower().Replace(" ", "").Replace("_", ""))
                {
                    case "id":
                        so.ID = ParseInt(rawValue);
                        break;
                    case "itemname":
                    case "name":
                        so.ItemName = rawValue ?? "";
                        nameValue = so.ItemName;
                        break;
                    case "maxamount":
                        so.maxamount = ParseInt(rawValue);
                        break;
                    case "ids":
                    case "actionids":
                        so.ids = ParseIntList(rawValue);
                        break;
                    case "description":
                        so.description = rawValue ?? "";
                        break;
                    case "grade":
                        so.grade = ParseItemGrade(rawValue);
                        break;
                    case "acquirefields":
                        so.acquirableFieldIds = ParseIntList(rawValue);
                        break;

                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ExcelParser] 필드 {header} 파싱 실패 (행 {rowIndex + 1}): {e.Message}");
            }
        }

        // name이 없다면 ID로 생성
        if (string.IsNullOrEmpty(nameValue))
        {
            nameValue = $"Consumable_{so.ID}";
            so.ItemName = nameValue;
        }

        return nameValue;
    }

    /// <summary>
    /// FieldTileDataSO 행 데이터 처리
    /// </summary>
    private static string ProcessFieldTileDataRow(FieldTileDataSO so, string[] headers, object[] values, int rowIndex)
    {
        string nameValue = null;

        for (int j = 0; j < headers.Length && j < values.Length; j++)
        {
            string header = headers[j].Trim();
            string rawValue = values[j]?.ToString()?.Trim();

            if (string.IsNullOrEmpty(header)) continue;

            try
            {
                switch (header.ToLower().Replace(" ", "").Replace("_", ""))
                {
                    case "id":
                        so.ID = ParseInt(rawValue);
                        break;
                    case "stagelevel":
                        so.StageLevel = ParseInt(rawValue);
                        break;
                    case "mincount":
                        so.minCount = ParseInt(rawValue);
                        break;
                    case "maxcount":
                        so.maxCount = ParseInt(rawValue);
                        break;
                    case "objectid":
                        so.ObjectID = ParseIntList(rawValue);
                        break;
                    case "objectvalue":
                        so.ObjectValue = ParseFloatList(rawValue);
                        break;
                    case "description":
                        so.description = rawValue ?? "";
                        break;
                    case "iconName":
                        so.IconName = rawValue ?? "";
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ExcelParser] 필드 {header} 파싱 실패 (행 {rowIndex + 1}): {e.Message}");
            }
        }

        // name 생성
        nameValue = $"Stage{so.StageLevel}_Tile{so.ID}";

        return nameValue;
    }

    /// <summary>
    /// ItemActionSO 행 데이터 처리
    /// </summary>
    private static string ProcessItemActionRow(ItemActionSO so, string[] headers, object[] values, int rowIndex)
    {
        string nameValue = null;

        for (int j = 0; j < headers.Length && j < values.Length; j++)
        {
            string header = headers[j].Trim();
            string rawValue = values[j]?.ToString()?.Trim();

            if (string.IsNullOrEmpty(header)) continue;

            try
            {
                switch (header.ToLower().Replace(" ", "").Replace("_", ""))
                {
                    case "id":
                        so.ID = ParseInt(rawValue);
                        break;
                    case "actionname":
                        so.ActionName = rawValue ?? "";
                        break;
                    case "data":
                        so.Data = rawValue ?? "";
                        break;
                    case "description":
                        so.description = rawValue ?? "";
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ExcelParser] 필드 {header} 파싱 실패 (행 {rowIndex + 1}): {e.Message}");
            }
        }

        // name 생성
        if (!string.IsNullOrEmpty(so.ActionName))
        {
            nameValue = $"{so.ActionName}_{so.ID}";
        }
        else
        {
            nameValue = $"Action_{so.ID}";
        }

        return nameValue;
    }

    // === 범용 리플렉션 기반 처리 메서드 (기존 유지) ===

    /// <summary>
    /// 시트에서 헤더 추출 (디버깅 강화)
    /// </summary>
    private static string[] ExtractHeaders(DataTable sheet)
    {
        var headers = sheet.Rows[0].ItemArray
            .Select(h => h?.ToString()?.Trim() ?? string.Empty)
            .ToArray();

        Debug.Log($"[ExcelParser] 추출된 헤더들: [{string.Join(", ", headers.Select(h => $"'{h}'"))}]");
        return headers;
    }

    /// <summary>
    /// 상속된 클래스의 모든 필드를 재귀적으로 가져오기
    /// </summary>
    private static FieldInfo[] GetAllFieldsIncludingInherited(Type type)
    {
        var allFields = new List<FieldInfo>();

        while (type != null && type != typeof(ScriptableObject) && type != typeof(MonoBehaviour))
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            allFields.AddRange(fields);
            type = type.BaseType;
        }

        // 중복 제거 (같은 이름의 필드가 있을 경우 가장 파생된 클래스의 것을 사용)
        return allFields
            .GroupBy(f => f.Name)
            .Select(g => g.First())
            .ToArray();
    }

    /// <summary>
    /// 리플렉션으로 행 데이터 처리 (디버깅 강화)
    /// </summary>
    private static string ProcessRowDataWithReflection(object so, string[] headers, object[] values, FieldInfo[] fields, int rowIndex)
    {
        string nameValue = null;
        var processedFields = new HashSet<string>();
        var soType = so.GetType();

        // 특별 디버깅
        if (soType == typeof(ItemActionSO) || soType == typeof(FieldTileDataSO))
        {
            Debug.Log($"\n[디버그] 행 {rowIndex + 1} 처리 중 ({soType.Name}):");
            for (int k = 0; k < Math.Min(headers.Length, values.Length); k++)
            {
                Debug.Log($"  [{k}] '{headers[k]}' = '{values[k]}'");
            }
        }

        // 먼저 모든 값이 비어있는지 확인 (빈 행 스킵)
        bool isEmptyRow = values.All(v => string.IsNullOrWhiteSpace(v?.ToString()));
        if (isEmptyRow)
        {
            Debug.LogWarning($"[ExcelParser] 빈 행 발견됨 (행 {rowIndex + 1}). 스킵합니다.");
            return null;
        }

        for (int j = 0; j < headers.Length && j < values.Length; j++)
        {
            string header = headers[j];
            if (string.IsNullOrEmpty(header)) continue;

            // 대소문자 구분 없이 필드 찾기
            var matchingFields = FindMatchingFields(fields, header);

            if (matchingFields.Length == 0 && (soType == typeof(ItemActionSO) || soType == typeof(FieldTileDataSO)))
            {
                Debug.Log($"[디버그] 헤더 '{header}'에 매칭되는 필드 없음");
            }

            foreach (var field in matchingFields)
            {
                if (processedFields.Contains(field.Name)) continue;

                try
                {
                    string rawValue = values[j]?.ToString()?.Trim();

                    if (soType == typeof(ItemActionSO) || soType == typeof(FieldTileDataSO))
                    {
                        Debug.Log($"[디버그] 필드 '{field.Name}' <- 헤더 '{header}' = '{rawValue}'");
                    }

                    object parsedValue = ConvertValueUsingReflection(rawValue, field.FieldType, field.Name);
                    SetFieldValueSafely(so, field, parsedValue);
                    processedFields.Add(field.Name);

                    // name 필드 확인 (다양한 필드명 지원)
                    if (IsNameField(field))
                    {
                        nameValue = parsedValue?.ToString();
                        if (soType == typeof(ItemActionSO) || soType == typeof(FieldTileDataSO))
                        {
                            Debug.Log($"[디버그] Name 필드 발견! '{field.Name}' = '{nameValue}'");
                        }
                    }

                    break; // 첫 번째 매칭 필드만 처리
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[ExcelParser] 필드 {field.Name} 파싱 실패 (행 {rowIndex + 1}): {e.Message}");
                }
            }
        }

        // 특별 처리: name을 못찾았을 때
        if (string.IsNullOrEmpty(nameValue))
        {
            if (soType == typeof(ItemActionSO) || soType == typeof(FieldTileDataSO))
            {
                Debug.LogError($"[디버그] Name 값을 찾지 못했습니다!");
                Debug.LogError($"[디버그] 처리된 필드들: [{string.Join(", ", processedFields)}]");

                // 모든 필드 값 출력
                foreach (var field in fields)
                {
                    try
                    {
                        var value = field.GetValue(so);
                        Debug.LogError($"  {field.Name} = {value}");
                    }
                    catch { }
                }
            }

            // 특별 타입에 대한 강제 처리
            ForceSetNameForSpecialTypes(so, rowIndex);

            // name 재확인
            var nameField = soType.GetField("name", BindingFlags.Public | BindingFlags.Instance);
            if (nameField != null)
            {
                nameValue = nameField.GetValue(so)?.ToString();
            }

            // 여전히 없다면 자동 생성
            if (string.IsNullOrEmpty(nameValue))
            {
                nameValue = GenerateAutoName(so, rowIndex);
                Debug.LogWarning($"[ExcelParser] name 필드가 없어 자동 생성: '{nameValue}' (행 {rowIndex + 1})");
            }
        }

        return nameValue;
    }

    /// <summary>
    /// 헤더와 매칭되는 필드들 찾기
    /// </summary>
    private static FieldInfo[] FindMatchingFields(FieldInfo[] fields, string header)
    {
        var matches = fields.Where(f =>
            f.Name.Equals(header, StringComparison.OrdinalIgnoreCase) ||
            f.Name.Replace("_", "").Equals(header.Replace("_", ""), StringComparison.OrdinalIgnoreCase) ||
            f.Name.Replace(" ", "").Equals(header.Replace(" ", ""), StringComparison.OrdinalIgnoreCase)
        ).ToArray();

        return matches;
    }

    /// <summary>
    /// 이름 필드인지 확인 (더 많은 패턴 지원)
    /// </summary>
    private static bool IsNameField(FieldInfo field)
    {
        var nameFields = new[] {
           "name", "Name", "NAME",
           "itemName", "ItemName", "ITEMNAME",
           "displayName", "DisplayName", "DISPLAYNAME",
           "fileName", "FileName", "FILENAME",
           "assetName", "AssetName", "ASSETNAME",
           "title", "Title", "TITLE"
       };

        bool isNameField = nameFields.Any(n => field.Name.Equals(n, StringComparison.OrdinalIgnoreCase));
        return isNameField;
    }

    /// <summary>
    /// 헤더가 name 관련인지 확인
    /// </summary>
    private static bool IsNameHeader(string header)
    {
        if (string.IsNullOrEmpty(header)) return false;

        var nameHeaders = new[] {
           "name", "Name", "NAME",
           "itemName", "ItemName", "ITEMNAME",
           "displayName", "DisplayName", "DISPLAYNAME",
           "fileName", "FileName", "FILENAME",
           "assetName", "AssetName", "ASSETNAME"
       };

        return nameHeaders.Any(n => header.Equals(n, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 특정 타입에 대한 name 강제 설정
    /// </summary>
    private static void ForceSetNameForSpecialTypes(object so, int rowIndex)
    {
        var soType = so.GetType();

        // name 필드 찾기
        var nameField = soType.GetField("name", BindingFlags.Public | BindingFlags.Instance);
        if (nameField != null)
        {
            var currentName = nameField.GetValue(so)?.ToString();
            if (string.IsNullOrEmpty(currentName))
            {
                // 강제로 name 설정
                var generatedName = GenerateAutoName(so, rowIndex);
                nameField.SetValue(so, generatedName);
                Debug.LogWarning($"[ExcelParser] name 필드를 강제로 설정: '{generatedName}' ({soType.Name}, 행 {rowIndex + 1})");
            }
        }
    }

    /// <summary>
    /// 자동 name 생성
    /// </summary>
    private static string GenerateAutoName(object so, int rowIndex)
    {
        var soType = so.GetType();

        // ItemActionSO의 경우
        if (soType == typeof(ItemActionSO))
        {
            var actionNameField = soType.GetField("ActionName");
            var idField = soType.GetField("ID");

            if (actionNameField != null && idField != null)
            {
                var actionName = actionNameField.GetValue(so)?.ToString();
                var id = idField.GetValue(so);

                if (!string.IsNullOrEmpty(actionName) && id != null)
                {
                    return $"{actionName}_{id}";
                }
            }
        }

        // FieldTileDataSO의 경우
        if (soType == typeof(FieldTileDataSO))
        {
            var idField = soType.GetField("ID");
            var stageLevelField = soType.GetField("StageLevel");

            if (idField != null && stageLevelField != null)
            {
                var id = idField.GetValue(so);
                var stageLevel = stageLevelField.GetValue(so);

                if (id != null && stageLevel != null)
                {
                    return $"Stage{stageLevel}_Tile{id}";
                }
            }
        }

        // 기본 fallback - ID 필드 찾기
        var defaultIdField = soType.GetField("ID", BindingFlags.Public | BindingFlags.Instance);
        if (defaultIdField != null)
        {
            var idValue = defaultIdField.GetValue(so);
            if (idValue != null)
            {
                string typeName = soType.Name.Replace("SO", "");
                return $"{typeName}_{idValue}";
            }
        }

        // ID도 없으면 행 번호로
        return $"{soType.Name.Replace("SO", "")}_{rowIndex}";
    }

    // === 간단한 파싱 헬퍼 메서드들 (특화 처리용) ===

    /// <summary>
    /// 정수 파싱
    /// </summary>
    private static int ParseInt(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0;
        if (int.TryParse(value, out int result))
            return result;
        return 0;
    }

    /// <summary>
    /// 실수 파싱
    /// </summary>
    private static float ParseFloat(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0f;
        if (float.TryParse(value, out float result))
            return result;
        return 0f;
    }

    /// <summary>
    /// EquipmentType 파싱
    /// </summary>
    private static EquipmentType ParseEquipmentType(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return EquipmentType.None;

        // 숫자로 입력된 경우
        if (int.TryParse(value, out int intValue))
        {
            if (Enum.IsDefined(typeof(EquipmentType), intValue))
                return (EquipmentType)intValue;
        }

        // 문자열로 입력된 경우
        if (Enum.TryParse<EquipmentType>(value, true, out EquipmentType result))
            return result;

        return EquipmentType.None; // 기본값
    }

    /// <summary>
    /// ItemGrade 파싱
    /// </summary>
    private static ItemGrade ParseItemGrade(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return ItemGrade.Common;

        // 숫자로 입력된 경우
        if (int.TryParse(value, out int intValue))
        {
            if (Enum.IsDefined(typeof(ItemGrade), intValue))
                return (ItemGrade)intValue;
        }

        // 문자열로 입력된 경우
        if (Enum.TryParse<ItemGrade>(value, true, out ItemGrade result))
            return result;

        return ItemGrade.Common; // 기본값
    }

    /// <summary>
    /// 정수 리스트 파싱 (쉼표로 구분)
    /// </summary>
    private static List<int> ParseIntList(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new List<int>();

        try
        {
            var parts = value.Split(',', ';', '|');
            var result = new List<int>();

            for (int i = 0; i < parts.Length; i++)
            {
                int parsed = ParseInt(parts[i].Trim());
                if (parsed > 0 || parts[i].Trim() == "0") // 0도 유효값으로 처리
                {
                    result.Add(parsed);
                }
            }

            return result;
        }
        catch
        {
            return new List<int>();
        }
    }

    /// <summary>
    /// 실수 리스트 파싱 (쉼표로 구분)
    /// </summary>
    private static List<float> ParseFloatList(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new List<float>();

        try
        {
            var parts = value.Split(',', ';', '|');
            var result = new List<float>();

            for (int i = 0; i < parts.Length; i++)
            {
                float parsed = ParseFloat(parts[i].Trim());
                result.Add(parsed);
            }

            return result;
        }
        catch
        {
            return new List<float>();
        }
    }

    // === 고급 변환 메서드들 (범용 처리용, 기존 유지) ===

    /// <summary>
    /// 리플렉션을 사용한 고급 값 변환
    /// </summary>
    private static object ConvertValueUsingReflection(string value, Type targetType, string fieldName)
    {
        if (string.IsNullOrEmpty(value))
        {
            return GetDefaultValueForType(targetType);
        }

        // Nullable 타입 처리
        Type nullableUnderlyingType = Nullable.GetUnderlyingType(targetType);
        if (nullableUnderlyingType != null)
        {
            return ConvertValueUsingReflection(value, nullableUnderlyingType, fieldName);
        }

        // 특수 타입별 처리
        if (targetType == typeof(string))
            return value;

        if (targetType == typeof(bool))
            return ParseBooleanValue(value);

        if (targetType.IsEnum)
            return ParseEnumValue(value, targetType);

        if (IsListType(targetType))
            return ParseListValue(value, targetType);

        if (targetType.IsArray)
            return ParseArrayValue(value, targetType);

        if (targetType == typeof(Vector2))
            return ParseVector2Value(value);

        if (targetType == typeof(Vector3))
            return ParseVector3Value(value);

        if (targetType == typeof(Color))
            return ParseColorValue(value);

        if (targetType == typeof(Quaternion))
            return ParseQuaternionValue(value);

        // 기본 타입 변환
        try
        {
            return Convert.ChangeType(value, targetType);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[ExcelParser] 타입 변환 실패 {fieldName}: {value} → {targetType.Name} ({e.Message})");
            return GetDefaultValueForType(targetType);
        }
    }

    /// <summary>
    /// 타입의 기본값 가져오기
    /// </summary>
    private static object GetDefaultValueForType(Type type)
    {
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }
        else if (IsListType(type))
        {
            return Activator.CreateInstance(type);
        }
        return null;
    }

    /// <summary>
    /// Boolean 값 파싱 (여러 형태 지원)
    /// </summary>
    private static bool ParseBooleanValue(string value)
    {
        if (bool.TryParse(value, out bool result))
            return result;

        value = value.ToLower();
        return value == "1" || value == "yes" || value == "y" || value == "true" || value == "on";
    }

    /// <summary>
    /// Enum 값 파싱 (이름과 숫자 모두 지원)
    /// </summary>
    private static object ParseEnumValue(string value, Type enumType)
    {
        try
        {
            // 숫자로 파싱 시도
            if (int.TryParse(value, out int intValue))
            {
                return Enum.ToObject(enumType, intValue);
            }

            // 이름으로 파싱 시도
            return Enum.Parse(enumType, value, true);
        }
        catch
        {
            // 기본값 반환
            return Enum.GetValues(enumType).GetValue(0);
        }
    }

    /// <summary>
    /// List 값 파싱
    /// </summary>
    private static object ParseListValue(string value, Type listType)
    {
        var list = (IList)Activator.CreateInstance(listType);

        if (string.IsNullOrWhiteSpace(value))
            return list;

        var elementType = listType.GetGenericArguments()[0];
        var elements = ParseArrayString(value);

        foreach (var element in elements)
        {
            try
            {
                var convertedElement = ConvertValueUsingReflection(element.Trim(), elementType, "ListElement");
                list.Add(convertedElement);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ExcelParser] List 요소 변환 실패: {element} → {elementType.Name} ({e.Message})");
            }
        }

        return list;
    }

    /// <summary>
    /// Array 값 파싱
    /// </summary>
    private static object ParseArrayValue(string value, Type arrayType)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.CreateInstance(arrayType.GetElementType(), 0);

        var elementType = arrayType.GetElementType();
        var elements = ParseArrayString(value);
        var array = Array.CreateInstance(elementType, elements.Length);

        for (int i = 0; i < elements.Length; i++)
        {
            try
            {
                var convertedElement = ConvertValueUsingReflection(elements[i].Trim(), elementType, "ArrayElement");
                array.SetValue(convertedElement, i);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ExcelParser] Array 요소 변환 실패: {elements[i]} → {elementType.Name} ({e.Message})");
            }
        }

        return array;
    }

    /// <summary>
    /// 배열 문자열 파싱 (여러 구분자 지원)
    /// </summary>
    private static string[] ParseArrayString(string value)
    {
        // 다양한 형태 지원: [1,2,3], "1,2,3", 1|2|3, 1;2;3
        value = value.Trim('[', ']', '"', '\'');

        char[] separators = { ',', '|', ';', '\t' };
        return value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// Vector2 값 파싱
    /// </summary>
    private static Vector2 ParseVector2Value(string value)
    {
        try
        {
            var parts = ParseArrayString(value);
            if (parts.Length >= 2)
            {
                float x = float.Parse(parts[0]);
                float y = float.Parse(parts[1]);
                return new Vector2(x, y);
            }
        }
        catch { }
        return Vector2.zero;
    }

    /// <summary>
    /// Vector3 값 파싱
    /// </summary>
    private static Vector3 ParseVector3Value(string value)
    {
        try
        {
            var parts = ParseArrayString(value);
            if (parts.Length >= 3)
            {
                float x = float.Parse(parts[0]);
                float y = float.Parse(parts[1]);
                float z = float.Parse(parts[2]);
                return new Vector3(x, y, z);
            }
        }
        catch { }
        return Vector3.zero;
    }

    /// <summary>
    /// Color 값 파싱 (Hex, RGB, RGBA 지원)
    /// </summary>
    private static Color ParseColorValue(string value)
    {
        try
        {
            // Hex 색상 (#RRGGBB, #RRGGBBAA)
            if (value.StartsWith("#"))
            {
                if (ColorUtility.TryParseHtmlString(value, out Color color))
                    return color;
            }

            // RGB/RGBA 값
            var parts = ParseArrayString(value);
            if (parts.Length >= 3)
            {
                float r = float.Parse(parts[0]) / 255f;
                float g = float.Parse(parts[1]) / 255f;
                float b = float.Parse(parts[2]) / 255f;
                float a = parts.Length >= 4 ? float.Parse(parts[3]) / 255f : 1f;
                return new Color(r, g, b, a);
            }
        }
        catch { }
        return Color.white;
    }

    /// <summary>
    /// Quaternion 값 파싱
    /// </summary>
    private static Quaternion ParseQuaternionValue(string value)
    {
        try
        {
            var parts = ParseArrayString(value);
            if (parts.Length >= 4)
            {
                float x = float.Parse(parts[0]);
                float y = float.Parse(parts[1]);
                float z = float.Parse(parts[2]);
                float w = float.Parse(parts[3]);
                return new Quaternion(x, y, z, w);
            }
            else if (parts.Length == 3)
            {
                // Euler angles
                float x = float.Parse(parts[0]);
                float y = float.Parse(parts[1]);
                float z = float.Parse(parts[2]);
                return Quaternion.Euler(x, y, z);
            }
        }
        catch { }
        return Quaternion.identity;
    }

    /// <summary>
    /// 안전하게 필드 값 설정
    /// </summary>
    private static void SetFieldValueSafely(object obj, FieldInfo field, object value)
    {
        try
        {
            field.SetValue(obj, value);
        }
        catch (Exception e)
        {
            Debug.LogError($"[ExcelParser] 필드 설정 실패 {field.Name}: {e.Message}");
        }
    }

    /// <summary>
    /// List 타입인지 확인
    /// </summary>
    private static bool IsListType(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
    }

    // === 공통 유틸리티 메서드들 ===

    /// <summary>
    /// 출력 폴더 확인 및 생성
    /// </summary>
    private static string EnsureOutputFolderExists(string outputFolder, Type type)
    {
        string typeFolder = Path.Combine(outputFolder, type.Name);
        if (!Directory.Exists(typeFolder))
        {
            Directory.CreateDirectory(typeFolder);
        }
        return typeFolder;
    }

    /// <summary>
    /// ScriptableObject 검증 및 에셋 생성
    /// </summary>
    private static bool ValidateAndCreateAsset(ScriptableObject so, string nameValue, string typeFolder, int rowIndex)
    {
        if (string.IsNullOrEmpty(nameValue))
        {
            Debug.LogError($"[ExcelParser] name 필드가 비어있습니다. 행 {rowIndex + 1}");
            Debug.LogError($"[ExcelParser] 현재 SO 내용: {GetSODebugInfo(so)}");
            return false;
        }

        // 파일명에 사용할 수 없는 문자 제거
        string sanitizedName = SanitizeFileName(nameValue);
        string assetPath = Path.Combine(typeFolder, $"{sanitizedName}.asset").Replace("\\", "/");

        try
        {
            // 기존 에셋이 있다면 덮어쓰기
            AssetDatabase.CreateAsset(so, assetPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[ExcelParser] 에셋 생성 실패 (행 {rowIndex + 1}): {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// ScriptableObject의 현재 상태를 디버그용으로 출력
    /// </summary>
    private static string GetSODebugInfo(ScriptableObject so)
    {
        var sb = new System.Text.StringBuilder();
        var fields = so.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields)
        {
            var value = field.GetValue(so);
            sb.AppendLine($"  {field.Name}: {value ?? "null"}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// 파일명 검증 및 정리
    /// </summary>
    private static string SanitizeFileName(string fileName)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        foreach (char c in invalidChars)
        {
            fileName = fileName.Replace(c, '_');
        }
        return fileName;
    }

    /// <summary>
    /// 에셋 생성 완료 처리
    /// </summary>
    private static void FinalizeAssetCreation(Type type, int createdCount, int skippedCount = 0)
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[ExcelParser] {type.Name} 처리 완료!");
        Debug.Log($"  ✅ 생성됨: {createdCount}개");
        if (skippedCount > 0)
        {
            Debug.LogWarning($"  ⚠️ 스킵됨: {skippedCount}개");
        }
    }

    /// <summary>
    /// DataCenter가 존재하면 자동 등록
    /// </summary>
    private static void AutoRegisterToDataCenterIfExists()
    {
        EditorApplication.delayCall += () =>
        {
            try
            {
                ScriptableObjectAutoRegistrar.RegisterSOsToDataCenterPrefab();
                Debug.Log("🚀 [ExcelParser] DataCenter 자동 등록 완료!");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ExcelParser] DataCenter 자동 등록 실패: {e.Message}");
            }
        };
    }
}
#endif