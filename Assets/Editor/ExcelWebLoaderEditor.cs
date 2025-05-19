using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Reflection;
using ExcelDataReader;
using System.Collections.Generic;
using System.Data;
public static class ExcelWebLoaderEditor
{
    private const string outputFolder = "Assets/Resources/SO";

    [MenuItem("Tools/Data/Excel(웹) → ScriptableObject 자동 등록")]
    public static void RegisterFromWebExcels()
    {
        var source = AssetDatabase.LoadAssetAtPath<ExcelWebSource>("Assets/ExcelWebSource/ExcelWebSource.asset");
        if (source == null)
        {
            Debug.LogError("ExcelWebSource.asset 을 찾을 수 없습니다. 먼저 ScriptableObject를 생성해주세요.");
            return;
        }

        foreach (var entry in source.entries)
        {
            string url = entry.url;
            if (string.IsNullOrWhiteSpace(url)) continue;

            try
            {
                Debug.Log($"[ExcelWebLoader] 다운로드 중: {entry.name}");
                byte[] excelBytes = DownloadExcel(url);
                if (excelBytes == null)
                {
                    Debug.LogWarning($"[ExcelWebLoader] 다운로드 실패: {url}");
                    continue;
                }

                using var stream = new MemoryStream(excelBytes);
                using var reader = ExcelReaderFactory.CreateReader(stream);
                var dataset = reader.AsDataSet();

                foreach (DataTable table in dataset.Tables)
                {
                    string sheetName = table.TableName.Trim();
                    string className = sheetName + "SO";

                    Type soType = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .FirstOrDefault(t =>
                            t.Name.Equals(className, StringComparison.OrdinalIgnoreCase) &&
                            typeof(ScriptableObject).IsAssignableFrom(t) &&
                            !t.IsAbstract);

                    if (soType == null)
                    {
                        Debug.LogWarning($"[ExcelWebLoader] 시트 '{sheetName}' 대응 타입 {className} 없음. 스킵.");
                        continue;
                    }

                    MethodInfo method = typeof(ExcelScriptableObjectGenerator)
                        .GetMethod("GenerateFromExcel", BindingFlags.Public | BindingFlags.Static)
                        .MakeGenericMethod(soType);

                    method.Invoke(null, new object[] { table, outputFolder });
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[ExcelWebLoader] {entry.name} 처리 실패: {e.Message}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[ExcelWebLoader] ✅ 전체 웹 Excel 변환 완료!");
    }

    private static byte[] DownloadExcel(string url)
    {
        try
        {
            using var client = new System.Net.WebClient();
            return client.DownloadData(url);
        }
        catch (Exception e)
        {
            Debug.LogError($"[ExcelWebLoader] 다운로드 에러: {e.Message}");
            return null;
        }
    }
}
