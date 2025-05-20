using UnityEditor;
using UnityEngine;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using ExcelDataReader;
using System.Reflection;
using System;

public class ExcelImporterWindow : EditorWindow
{
    private Vector2 scroll;
    private string excelFolder = "Assets/Excel";
    private string outputFolder = "Assets/Resources/SO";
    private List<string> excelPaths = new();
    private Dictionary<string, List<string>> sheetMap = new();
    private Dictionary<(string excelPath, string sheetName), bool> selectionMap = new();

    [MenuItem("Tools/Data/Excel → ScriptableObject 생성 툴")]
    public static void ShowWindow()
    {
        var window = GetWindow<ExcelImporterWindow>("Excel → ScriptableObject");
        window.RefreshExcelFiles();
    }

    private void RefreshExcelFiles()
    {
        excelPaths.Clear();
        sheetMap.Clear();
        selectionMap.Clear();

        var files = Directory.GetFiles(excelFolder, "*.xlsx", SearchOption.TopDirectoryOnly);
        foreach (var path in files)
        {
            excelPaths.Add(path);
            var sheets = new List<string>();

            using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var dataset = reader.AsDataSet();

            foreach (DataTable table in dataset.Tables)
            {
                var sheetName = table.TableName.Trim();
                sheets.Add(sheetName);
                selectionMap[(path, sheetName)] = false;
            }

            sheetMap[path] = sheets;
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("📄 Excel → ScriptableObject 생성", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("변환할 시트를 선택한 후 [변환 실행]을 누르세요.", MessageType.Info);

        if (GUILayout.Button("🔄 Excel 파일 새로고침"))
        {
            RefreshExcelFiles();
        }

        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach (var path in excelPaths)
        {
            EditorGUILayout.LabelField($"📁 {Path.GetFileName(path)}", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            foreach (var sheet in sheetMap[path])
            {
                var key = (path, sheet);
                selectionMap[key] = EditorGUILayout.ToggleLeft($"- {sheet}", selectionMap[key]);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
        if (GUILayout.Button("🚀 변환 실행"))
        {
            ConvertSelectedSheets();
        }
    }

    private void ConvertSelectedSheets()
    {
        foreach (var entry in selectionMap)
        {
            if (!entry.Value) continue;

            string path = entry.Key.excelPath;
            string sheetName = entry.Key.sheetName;
            string className = sheetName + "SO";

            using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var dataset = reader.AsDataSet();
            var table = dataset.Tables.Cast<DataTable>().FirstOrDefault(t => t.TableName.Trim() == sheetName);
            if (table == null)
            {
                Debug.LogWarning($"[ExcelImporter] 시트 '{sheetName}' 없음.");
                continue;
            }

            var soType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t =>
                    t.Name.Equals(className, System.StringComparison.OrdinalIgnoreCase) &&
                    typeof(ScriptableObject).IsAssignableFrom(t) &&
                    !t.IsAbstract);

            if (soType == null)
            {
                Debug.LogWarning($"[ExcelImporter] 시트 '{sheetName}' 대응 타입 {className} 없음. 스킵.");
                continue;
            }

            var method = typeof(ExcelScriptableObjectGenerator)
                .GetMethod("GenerateFromExcel", BindingFlags.Public | BindingFlags.Static)
                ?.MakeGenericMethod(soType);

            method?.Invoke(null, new object[] { table, outputFolder });
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[ExcelImporter] 선택된 시트 ScriptableObject 생성 완료!");
    }
}
