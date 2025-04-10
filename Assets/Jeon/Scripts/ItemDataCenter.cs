using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class ItemDataCenter
{
    private static readonly Dictionary<Type, IDictionary> _typeToDict = new();
    public static void Register<T>(T obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("[ItemDataCenter] null 등록 시도됨");
            return;
        }

        Type type = typeof(T);

        // ID 필드 찾기
        var idField = type.GetField("ID", BindingFlags.Public | BindingFlags.Instance);
        if (idField == null || idField.FieldType != typeof(int))
        {
            Debug.LogError($"[ItemDataCenter] {type.Name}는 public int ID 필드가 필요합니다");
            return;
        }

        int id = (int)idField.GetValue(obj);

        // 딕셔너리 없으면 생성
        if (!_typeToDict.TryGetValue(type, out var dict))
        {
            var genericDictType = typeof(Dictionary<,>).MakeGenericType(typeof(int), type);
            dict = (IDictionary)Activator.CreateInstance(genericDictType);
            _typeToDict[type] = dict;
        }

        var typedDict = (IDictionary<int, T>)_typeToDict[type];

        if (typedDict.ContainsKey(id))
        {
            Debug.LogWarning($"[ItemDataCenter] 중복된 ID 등록 시도됨: {type.Name}[{id}]");
            return;
        }

        typedDict[id] = obj;
    }
    public static T Get<T>(int id)
    {
        Type type = typeof(T);
        if (_typeToDict.TryGetValue(type, out var dict))
        {
            var typedDict = (IDictionary<int, T>)dict;
            return typedDict.TryGetValue(id, out var value) ? value : default;
        }

        return default;
    }
    public static IEnumerable<T> GetAll<T>()
    {
        Type type = typeof(T);
        if (_typeToDict.TryGetValue(type, out var dict))
        {
            return ((IDictionary<int, T>)dict).Values;
        }

        return Array.Empty<T>();
    }
}
