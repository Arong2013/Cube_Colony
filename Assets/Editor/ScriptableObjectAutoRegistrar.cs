#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;

public static class ScriptableObjectAutoRegistrar
{
    private const string SO_ROOT_PATH = "Assets/Resources/SO";

    [MenuItem("Tools/Data/Resources의 DataCenter Prefab에 SO 자동 등록")]
    public static void RegisterSOsToDataCenterPrefab()
    {
        // Resources 폴더에서 DataCenter 컴포넌트를 가진 Prefab 찾기
        GameObject dataCenterPrefab = FindComponentInResources<DataCenter>();

        if (dataCenterPrefab == null)
        {
            Debug.LogError("❌ Resources 폴더에서 DataCenter 컴포넌트를 가진 Prefab을 찾을 수 없습니다!");
            return;
        }

        DataCenter dataCenter = dataCenterPrefab.GetComponent<DataCenter>();
        Debug.Log($"🎯 DataCenter Prefab 발견: {AssetDatabase.GetAssetPath(dataCenterPrefab)}");

        // 리플렉션으로 모든 ScriptableObject 타입과 등록 메서드 자동 발견
        RegisterAllSOsUsingReflection(dataCenter);

        // Prefab을 dirty로 마킹하여 변경사항 저장
        EditorUtility.SetDirty(dataCenterPrefab);
        EditorUtility.SetDirty(dataCenter);
        AssetDatabase.SaveAssets();

        Debug.Log("✅ Resources의 DataCenter Prefab에 ScriptableObject 등록 완료!");
    }

    /// <summary>
    /// 리플렉션으로 특정 컴포넌트를 가진 Prefab을 Resources에서 찾기
    /// </summary>
    private static GameObject FindComponentInResources<T>() where T : Component
    {
        string[] resourcesPath = { "Assets/Resources" };
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", resourcesPath);

        foreach (string guid in prefabGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (loadedPrefab != null && loadedPrefab.GetComponent<T>() != null)
            {
                Debug.Log($"🔍 {typeof(T).Name} Prefab 발견됨: {assetPath}");
                return loadedPrefab;
            }
        }

        return null;
    }

    /// <summary>
    /// 리플렉션을 사용하여 모든 SO 타입을 자동으로 찾고 등록
    /// </summary>
    private static void RegisterAllSOsUsingReflection(DataCenter dataCenter)
    {
        // 1. DataCenter의 모든 Dictionary 필드 클리어
        ClearAllDictionaries(dataCenter);

        // 2. SO 폴더에서 모든 SO 타입 자동 발견
        var soTypes = DiscoverScriptableObjectTypes();

        int totalRegistered = 0;

        foreach (var soType in soTypes)
        {
            try
            {
                // 3. 각 SO 타입에 대한 등록 메서드를 리플렉션으로 찾아서 실행
                int registered = RegisterSOTypeUsingReflection(dataCenter, soType);
                totalRegistered += registered;
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ {soType.Name} 등록 중 오류: {e.Message}");
            }
        }

        Debug.Log($"✅ 총 {totalRegistered}개의 ScriptableObject가 {soTypes.Count}개 타입으로 등록되었습니다!");
    }

    /// <summary>
    /// DataCenter의 모든 Dictionary 필드를 리플렉션으로 클리어
    /// </summary>
    private static void ClearAllDictionaries(DataCenter dataCenter)
    {
        var allFields = GetAllFields(typeof(DataCenter));

        foreach (var field in allFields)
        {
            if (IsDictionaryField(field))
            {
                var dict = field.GetValue(dataCenter) as IDictionary;
                if (dict != null)
                {
                    dict.Clear();
                    Debug.Log($"🧹 {field.Name} 클리어됨");
                }
            }
        }
    }

    /// <summary>
    /// 클래스의 모든 필드를 재귀적으로 가져오기 (상속 포함)
    /// </summary>
    private static FieldInfo[] GetAllFields(Type type)
    {
        var fields = new List<FieldInfo>();

        while (type != null)
        {
            fields.AddRange(type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance));
            type = type.BaseType;
        }

        return fields.Distinct().ToArray();
    }

    /// <summary>
    /// 필드가 Dictionary 타입인지 확인
    /// </summary>
    private static bool IsDictionaryField(FieldInfo field)
    {
        return field.FieldType.IsGenericType &&
               field.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>);
    }

    /// <summary>
    /// SO 폴더 구조를 분석하여 모든 ScriptableObject 타입 자동 발견
    /// </summary>
    private static List<Type> DiscoverScriptableObjectTypes()
    {
        var soTypes = new List<Type>();

        if (!Directory.Exists(SO_ROOT_PATH))
        {
            Debug.LogWarning($"⚠️ SO 루트 폴더가 없습니다: {SO_ROOT_PATH}");
            return soTypes;
        }

        // SO 폴더의 하위 폴더들 검사
        var subDirectories = Directory.GetDirectories(SO_ROOT_PATH);

        foreach (var dir in subDirectories)
        {
            string typeName = Path.GetFileName(dir);

            // 타입 이름으로 실제 Type 찾기
            Type soType = FindTypeByName(typeName);

            if (soType != null && typeof(ScriptableObject).IsAssignableFrom(soType))
            {
                soTypes.Add(soType);
                Debug.Log($"📦 발견된 SO 타입: {soType.Name} → {dir}");
            }
            else
            {
                Debug.LogWarning($"⚠️ 타입을 찾을 수 없음: {typeName}");
            }
        }

        return soTypes;
    }

    /// <summary>
    /// 타입 이름으로 Type 객체 찾기 (모든 로드된 어셈블리에서 검색)
    /// </summary>
    private static Type FindTypeByName(string typeName)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .FirstOrDefault(type =>
                type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase) ||
                type.Name.Equals(typeName + "SO", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 리플렉션으로 특정 SO 타입을 등록
    /// </summary>
    private static int RegisterSOTypeUsingReflection(DataCenter dataCenter, Type soType)
    {
        string folderPath = Path.Combine(SO_ROOT_PATH, soType.Name);

        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning($"⚠️ {soType.Name} 폴더가 존재하지 않습니다: {folderPath}");
            return 0;
        }

        // 1. 해당 타입의 모든 SO 에셋 로드
        var soAssets = LoadAllSOAssetsOfType(soType, folderPath);

        // 2. DataCenter에서 해당 타입을 위한 Register 메서드 찾기
        var registerMethod = FindRegisterMethodForType(dataCenter, soType);

        if (registerMethod == null)
        {
            Debug.LogWarning($"⚠️ {soType.Name}을 위한 Register 메서드를 찾을 수 없습니다.");
            return 0;
        }

        // 3. 각 SO를 등록
        int count = 0;
        foreach (var so in soAssets)
        {
            // ID 필드 값 추출
            int id = ExtractIdFromObject(so);
            if (id > 0)
            {
                // Register 메서드 호출
                registerMethod.Invoke(dataCenter, new object[] { id, so });
                count++;
            }
            else
            {
                Debug.LogWarning($"⚠️ {so.name}에서 유효한 ID를 찾을 수 없습니다.");
            }
        }

        Debug.Log($"📦 {soType.Name}: {count}개 등록됨");
        return count;
    }

    /// <summary>
    /// 특정 타입의 모든 SO 에셋을 로드
    /// </summary>
    private static ScriptableObject[] LoadAllSOAssetsOfType(Type soType, string folderPath)
    {
        var assets = AssetDatabase.FindAssets($"t:{soType.Name}", new[] { folderPath });
        var soList = new List<ScriptableObject>();

        foreach (var assetGuid in assets)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            var so = AssetDatabase.LoadAssetAtPath(assetPath, soType) as ScriptableObject;

            if (so != null)
            {
                soList.Add(so);
            }
        }

        return soList.ToArray();
    }

    /// <summary>
    /// DataCenter에서 특정 SO 타입을 위한 Register 메서드 찾기
    /// </summary>
    private static MethodInfo FindRegisterMethodForType(DataCenter dataCenter, Type soType)
    {
        var methods = typeof(DataCenter).GetMethods(BindingFlags.Public | BindingFlags.Instance);

        // Register로 시작하고 해당 SO 타입을 매개변수로 받는 메서드 찾기
        foreach (var method in methods)
        {
            if (method.Name.StartsWith("Register") &&
                method.GetParameters().Length == 2)
            {
                var parameters = method.GetParameters();
                if (parameters[0].ParameterType == typeof(int) &&
                    parameters[1].ParameterType == soType)
                {
                    return method;
                }
            }
        }
        if (soType == typeof(ReinforcementRecipeSO))
        {
            return typeof(DataCenter)
                .GetMethod("RegisterReinforcementRecipe", BindingFlags.Public | BindingFlags.Instance);
        }
        // Register 메서드가 없다면 자동으로 생성하려고 시도
        return CreateRegisterMethodForType(dataCenter, soType);
    }

    /// <summary>
    /// 특정 SO 타입을 위한 Register 메서드를 자동으로 생성 (고급 리플렉션)
    /// </summary>
    private static MethodInfo CreateRegisterMethodForType(DataCenter dataCenter, Type soType)
    {
        // DataCenter에서 해당 SO 타입의 Dictionary 필드 찾기
        var dictionaryField = FindDictionaryFieldForType(dataCenter, soType);

        if (dictionaryField == null)
        {
            Debug.LogError($"❌ {soType.Name}을 위한 Dictionary 필드를 찾을 수 없습니다.");
            return null;
        }

        // 동적으로 Register 메서드 생성 및 실행을 위한 래퍼 생성
        return CreateDynamicRegisterMethod(dictionaryField);
    }

    /// <summary>
    /// 특정 SO 타입을 위한 Dictionary 필드 찾기
    /// </summary>
    private static FieldInfo FindDictionaryFieldForType(DataCenter dataCenter, Type soType)
    {
        var fields = GetAllFields(typeof(DataCenter));

        foreach (var field in fields)
        {
            if (IsDictionaryField(field))
            {
                var genericArgs = field.FieldType.GetGenericArguments();
                if (genericArgs.Length == 2 &&
                    genericArgs[0] == typeof(int) &&
                    genericArgs[1] == soType)
                {
                    return field;
                }
            }
        }
        return null;
    }
    /// <summary>
    /// 동적으로 Register 메서드 생성
    /// </summary>
    private static MethodInfo CreateDynamicRegisterMethod(FieldInfo dictionaryField)
    {
        // 이것은 복잡한 동적 메서드 생성이므로 간단한 래퍼로 대체
        return typeof(ScriptableObjectAutoRegistrar)
            .GetMethod(nameof(GenericRegisterMethod), BindingFlags.NonPublic | BindingFlags.Static);
    }

    /// <summary>
    /// 범용 등록 메서드 (리플렉션으로 Dictionary에 직접 접근)
    /// </summary>
    private static void GenericRegisterMethod(DataCenter dataCenter, FieldInfo dictionaryField, int id, ScriptableObject so)
    {
        var dictionary = dictionaryField.GetValue(dataCenter) as IDictionary;
        if (dictionary != null)
        {
            dictionary[id] = so;
        }
    }

    /// <summary>
    /// 객체에서 ID 필드 값을 리플렉션으로 추출
    /// </summary>
    private static int ExtractIdFromObject(object obj)
    {
        var idField = obj.GetType().GetField("ID", BindingFlags.Public | BindingFlags.Instance);
        if (idField != null && idField.FieldType == typeof(int))
        {
            return (int)idField.GetValue(obj);
        }

        // ID 필드가 없다면 다른 필드명도 시도
        var alternativeFields = new[] { "id", "Id", "identifier", "key" };
        foreach (var fieldName in alternativeFields)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (field != null && field.FieldType == typeof(int))
            {
                return (int)field.GetValue(obj);
            }
        }

        return 0;
    }

    /// <summary>
    /// 특정 경로에서 특정 컴포넌트를 가진 Prefab 찾기 (범용적)
    /// </summary>
    public static GameObject FindPrefabWithComponent<T>(string searchPath) where T : Component
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { searchPath });

        return prefabGuids
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Select(assetPath => AssetDatabase.LoadAssetAtPath<GameObject>(assetPath))
            .FirstOrDefault(prefab => prefab != null && prefab.GetComponent<T>() != null);
    }

    /// <summary>
    /// 디버그: DataCenter의 모든 데이터 정보 출력
    /// </summary>
    [MenuItem("Tools/Data/DataCenter 데이터 정보 출력")]
    public static void PrintDataCenterInfo()
    {
        var dataCenterPrefab = FindComponentInResources<DataCenter>();
        if (dataCenterPrefab == null)
        {
            Debug.LogError("❌ DataCenter Prefab을 찾을 수 없습니다!");
            return;
        }

        var dataCenter = dataCenterPrefab.GetComponent<DataCenter>();
        PrintAllDictionaryInfo(dataCenter);
    }

    /// <summary>
    /// DataCenter의 모든 Dictionary 정보 출력
    /// </summary>
    private static void PrintAllDictionaryInfo(DataCenter dataCenter)
    {
        Debug.Log("=== DataCenter Dictionary 정보 ===");

        var fields = GetAllFields(typeof(DataCenter));

        foreach (var field in fields)
        {
            if (IsDictionaryField(field))
            {
                var dict = field.GetValue(dataCenter) as IDictionary;
                if (dict != null)
                {
                    var genericArgs = field.FieldType.GetGenericArguments();
                    string keyType = genericArgs[0].Name;
                    string valueType = genericArgs[1].Name;

                    Debug.Log($"📚 {field.Name} ({keyType}, {valueType}): {dict.Count}개");
                }
            }
        }
    }
}
#endif