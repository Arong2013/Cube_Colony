using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Reflection;
using ExcelDataReader;
using System.Collections.Generic;
using System.Data;

public static class ExcelLoaderEditor
{
    private const string excelFolder = "Assets/Excel";
    private const string outputFolder = "Assets/Resources/SO";

    [MenuItem("Tools/Data/Excel(All) → ScriptableObject 자동 등록")]
    public static void RegisterAllFromExcels()
    {
        if (!Directory.Exists(excelFolder))
        {
            Debug.LogError($"[ExcelLoader] Excel 폴더가 없습니다: {excelFolder}");
            return;
        }

        var excelFiles = Directory.GetFiles(excelFolder, "*.xlsx", SearchOption.TopDirectoryOnly);
        if (excelFiles.Length == 0)
        {
            Debug.LogWarning("[ExcelLoader] 처리할 엘셀 파일이 없습니다.");
            return;
        }

        foreach (var excelPath in excelFiles)
        {
            using var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            var dataset = reader.AsDataSet();

            foreach (DataTable table in dataset.Tables)
            {
                string sheetName = table.TableName.Trim();
                string className = sheetName + "SO";

                // 타입 찾기
                Type soType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t =>
                        t.Name.Equals(className, StringComparison.OrdinalIgnoreCase) &&
                        typeof(ScriptableObject).IsAssignableFrom(t) &&
                        !t.IsAbstract
                    );

                if (soType == null)
                {
                    Debug.LogWarning($"[ExcelLoader] 시트 '{sheetName}' 대응 타입 {className} 없음. 스킵.");
                    continue;
                }

                MethodInfo method = typeof(ExcelScriptableObjectGenerator)
                    .GetMethod("GenerateFromExcel", BindingFlags.Public | BindingFlags.Static)
                    .MakeGenericMethod(soType);

                method.Invoke(null, new object[] { table, outputFolder });
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[ExcelLoader] Excel → SO 전체 변환 완료!");
    }
}
