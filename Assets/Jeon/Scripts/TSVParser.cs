using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
public static class TSVParser
{
    public static List<T> Parse<T>(string tsv) where T : new()
    {
        var list = new List<T>();
        var lines = tsv.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 1) return list;

        string[] headers = lines[0].Trim().Split('\t');
        var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Trim().Split('\t');
            T instance = new T();

            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                string header = headers[j].Trim();
                string value = values[j].Trim();

                foreach (var field in fields)
                {


                    if (string.Equals(field.Name, header, StringComparison.OrdinalIgnoreCase))
                    {
                        
                        try
                        {
                            if (IsList(field.FieldType))
                            {

                                var listValue = ParseList(value, field.FieldType);
                                field.SetValue(instance, listValue);
                            }
                            else
                            {
                                object convertedValue = Convert.ChangeType(value, field.FieldType);
                                field.SetValue(instance, convertedValue);
                            }
                        }
                        catch
                        {
                            // 파싱 실패는 무시
                        }
                        break;
                    }
                }
            }
            
            list.Add(instance);
        }
        Debug.Log($"[TSVParser] {typeof(T).Name} 파싱 완료: {list.Count}개");
        return list;
    }
    private static bool IsList(Type type)
    {
        return typeof(IList).IsAssignableFrom(type) ||
         (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>));
    }

    private static object ParseList(string raw, Type listType)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;

        raw = raw.Trim('[', ']');
        string[] parts = raw.Split(',');

        var elementType = listType.GetGenericArguments()[0];
        var list = (IList)Activator.CreateInstance(listType);

        foreach (var part in parts)
        {
            var trimmed = part.Trim();
            var converted = Convert.ChangeType(trimmed, elementType);
            list.Add(converted);
        }

        return list;
    }
    public static List<itemAction> ParseItemAcion(string tsv)
    {
        var list = new List<itemAction>();
        var lines = tsv.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2) return list;

        string[] headers = lines[0].Trim().Split('\t');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Trim().Split('\t');
            if (values.Length != headers.Length) continue;

            string actionName = null;
            int id = -1;
            string dataValue = null;

            for (int j = 0; j < headers.Length; j++)
            {
                string header = headers[j].Trim();
                string value = values[j].Trim();

                if (header.Equals("ActionName", StringComparison.OrdinalIgnoreCase))
                {
                    actionName = value;
                }
                else if (header.Equals("ID", StringComparison.OrdinalIgnoreCase))
                {
                    int.TryParse(value, out id);
                }
                else if (header.Equals("Data", StringComparison.OrdinalIgnoreCase))
                {
                    dataValue = value;
                }
            }

            if (string.IsNullOrEmpty(actionName))
            {
                Debug.LogWarning($"[TSVParser] ActionName 누락 (줄 {i + 1})");
                continue;
            }

            Type actionType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t =>
                    t.Name == actionName &&
                    typeof(itemAction).IsAssignableFrom(t) &&
                    !t.IsAbstract);

            if (actionType == null)
            {
                Debug.LogWarning($"[TSVParser] 타입 '{actionName}'을 찾을 수 없거나 itemAction의 자식이 아님");
                continue;
            }

            itemAction tempInstance = Activator.CreateInstance(actionType) as itemAction;
            if (tempInstance == null)
            {
                Debug.LogError($"[TSVParser] 인스턴스 생성 실패: {actionType.Name}");
                continue;
            }

            // Data 값을 object[]로 파싱 (여기선 int 하나라고 가정)
            object[] args = new object[] { int.Parse(dataValue) };

            itemAction finalAction = tempInstance.CreatAction(args);
            finalAction.ID = id;

            list.Add(finalAction);
        }

        Debug.Log($"[TSVParser] itemAction 파싱 완료: {list.Count}개");
        return list;
    }
}
