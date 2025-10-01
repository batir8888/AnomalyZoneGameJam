using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class MeshBaker : EditorWindow
{
    private List<GameObject> _objectsToCombine = new();
    private Vector2 _scrollPos;
    private string _combinedObjectName = "CombinedMesh";
    private bool _combineTextures = true;
    private int _atlasSize = 2048;
    private bool _generateLightmapUVs = true;
    private bool _keepOriginalObjects = true;
    private bool _optimizeMesh = true;
    private Transform _parentTransform;

    [MenuItem("Tools/Mesh Baker")]
    public static void ShowWindow()
    {
        GetWindow<MeshBaker>("Mesh Baker");
    }

    private void OnGUI()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        GUILayout.Label("Mesh Baker", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Настройки
        GUILayout.Label("Settings", EditorStyles.boldLabel);
        _combinedObjectName = EditorGUILayout.TextField("Combined Object Name", _combinedObjectName);
        _parentTransform = (Transform)EditorGUILayout.ObjectField("Parent Transform", _parentTransform, typeof(Transform), true);
        
        EditorGUILayout.Space();
        _combineTextures = EditorGUILayout.Toggle("Combine Textures", _combineTextures);
        
        if (_combineTextures)
        {
            EditorGUI.indentLevel++;
            _atlasSize = EditorGUILayout.IntPopup("Atlas Size", _atlasSize, 
                new[] { "512", "1024", "2048", "4096" },
                new[] { 512, 1024, 2048, 4096 });
            EditorGUI.indentLevel--;
        }
        
        _generateLightmapUVs = EditorGUILayout.Toggle("Generate Lightmap UVs", _generateLightmapUVs);
        _optimizeMesh = EditorGUILayout.Toggle("Optimize Mesh", _optimizeMesh);
        _keepOriginalObjects = EditorGUILayout.Toggle("Keep Original Objects", _keepOriginalObjects);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        // Список объектов
        GUILayout.Label("Objects to Combine", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Selected Objects"))
        {
            AddSelectedObjects();
        }
        if (GUILayout.Button("Clear List"))
        {
            _objectsToCombine.Clear();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Отображение списка объектов
        for (int i = _objectsToCombine.Count - 1; i >= 0; i--)
        {
            if (_objectsToCombine[i] == null)
            {
                _objectsToCombine.RemoveAt(i);
                continue;
            }

            EditorGUILayout.BeginHorizontal();
            _objectsToCombine[i] = (GameObject)EditorGUILayout.ObjectField(
                _objectsToCombine[i], typeof(GameObject), true);
            
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                _objectsToCombine.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        // Кнопка добавления вручную
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("+", GUILayout.Width(25)))
        {
            _objectsToCombine.Add(null);
        }
        GUILayout.Label("Add object manually");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        
        // Статистика
        int totalVertices = 0;
        int totalTriangles = 0;
        foreach (var obj in _objectsToCombine)
        {
            if (obj != null)
            {
                MeshFilter mf = obj.GetComponent<MeshFilter>();
                if (mf != null && mf.sharedMesh != null)
                {
                    totalVertices += mf.sharedMesh.vertexCount;
                    totalTriangles += mf.sharedMesh.triangles.Length / 3;
                }
            }
        }

        EditorGUILayout.HelpBox(
            $"Objects: {_objectsToCombine.Count(o => o != null)}\n" +
            $"Total Vertices: {totalVertices:N0}\n" +
            $"Total Triangles: {totalTriangles:N0}",
            MessageType.Info
        );

        EditorGUILayout.Space();

        GUI.enabled = _objectsToCombine.Count > 0 && _objectsToCombine.Any(o => o != null);
        if (GUILayout.Button("Combine Meshes", GUILayout.Height(40)))
        {
            CombineMeshes();
        }
        GUI.enabled = true;

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Mesh Baker объединяет несколько мешей в один для оптимизации.\n\n" +
            "• Уменьшает количество Draw Calls\n" +
            "• Может создавать атлас текстур\n" +
            "• Сохраняет UV координаты\n" +
            "• Поддерживает множественные материалы",
            MessageType.Info
        );

        EditorGUILayout.EndScrollView();
    }

    private void AddSelectedObjects()
    {
        GameObject[] selected = Selection.gameObjects;
        foreach (var obj in selected)
        {
            if (obj.GetComponent<MeshFilter>() != null && !_objectsToCombine.Contains(obj))
            {
                _objectsToCombine.Add(obj);
            }
        }
    }

    private void CombineMeshes()
    {
        if (_objectsToCombine.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "Нет объектов для объединения!", "OK");
            return;
        }

        // Фильтруем валидные объекты
        List<GameObject> validObjects = _objectsToCombine
            .Where(o => o && o.GetComponent<MeshFilter>())
            .ToList();

        if (validObjects.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "Нет валидных объектов с MeshFilter!", "OK");
            return;
        }

        try
        {
            EditorUtility.DisplayProgressBar("Mesh Baker", "Preparing meshes...", 0f);

            // Группируем объекты по материалам
            Dictionary<Material, List<CombineInstance>> materialGroups = new Dictionary<Material, List<CombineInstance>>();
            List<Material> materials = new List<Material>();

            for (int i = 0; i < validObjects.Count; i++)
            {
                GameObject obj = validObjects[i];
                MeshFilter mf = obj.GetComponent<MeshFilter>();
                MeshRenderer mr = obj.GetComponent<MeshRenderer>();

                if (!mf || !mf.sharedMesh || !mr) continue;

                EditorUtility.DisplayProgressBar("Mesh Baker", 
                    $"Processing {obj.name}...", (float)i / validObjects.Count);

                Material mat = mr.sharedMaterial;
                if (!mat) mat = new Material(Shader.Find("Standard"));

                if (!materialGroups.ContainsKey(mat))
                {
                    materialGroups[mat] = new List<CombineInstance>();
                    materials.Add(mat);
                }

                CombineInstance ci = new CombineInstance();
                ci.mesh = mf.sharedMesh;
                ci.transform = obj.transform.localToWorldMatrix;
                materialGroups[mat].Add(ci);
            }

            EditorUtility.DisplayProgressBar("Mesh Baker", "Combining meshes...", 0.8f);

            // Создаём объединённый объект
            GameObject combined = new GameObject(_combinedObjectName);
            if (_parentTransform)
            {
                combined.transform.SetParent(_parentTransform);
            }
            combined.transform.position = Vector3.zero;
            combined.transform.rotation = Quaternion.identity;
            combined.transform.localScale = Vector3.one;

            MeshFilter combinedMf = combined.AddComponent<MeshFilter>();
            MeshRenderer combinedMr = combined.AddComponent<MeshRenderer>();

            // Объединяем меши по группам материалов
            Mesh finalMesh = new Mesh
            {
                name = _combinedObjectName
            };

            if (materialGroups.Count == 1)
            {
                // Один материал - простое объединение
                CombineInstance[] combines = materialGroups.First().Value.ToArray();
                finalMesh.CombineMeshes(combines, true, true);
                combinedMr.sharedMaterial = materials[0];
            }
            else
            {
                // Несколько материалов - объединяем с subMeshes
                List<CombineInstance> allCombines = new List<CombineInstance>();
                finalMesh.subMeshCount = materials.Count;

                foreach (var mat in materials)
                {
                    CombineInstance[] combines = materialGroups[mat].ToArray();
                    Mesh subMesh = new Mesh();
                    subMesh.CombineMeshes(combines, true, true);
                    
                    CombineInstance ci = new CombineInstance();
                    ci.mesh = subMesh;
                    ci.transform = Matrix4x4.identity;
                    allCombines.Add(ci);
                }

                finalMesh.CombineMeshes(allCombines.ToArray(), false, false);
                combinedMr.sharedMaterials = materials.ToArray();
            }

            // Оптимизация меша
            if (_optimizeMesh)
            {
                finalMesh.Optimize();
            }

            // Генерация lightmap UV
            if (_generateLightmapUVs)
            {
                Unwrapping.GenerateSecondaryUVSet(finalMesh);
            }

            combinedMf.sharedMesh = finalMesh;

            // Сохранение меша
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Combined Mesh",
                _combinedObjectName,
                "asset",
                "Save combined mesh as asset"
            );

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(finalMesh, path);
                AssetDatabase.SaveAssets();
            }

            // Удаление оригинальных объектов
            if (!_keepOriginalObjects)
            {
                foreach (var obj in validObjects)
                {
                    if (obj)
                    {
                        DestroyImmediate(obj);
                    }
                }
            }

            Selection.activeGameObject = combined;
            EditorUtility.ClearProgressBar();

            EditorUtility.DisplayDialog("Success", 
                $"Mesh combined successfully!\n\n" +
                $"Vertices: {finalMesh.vertexCount:N0}\n" +
                $"Triangles: {finalMesh.triangles.Length / 3:N0}\n" +
                $"Materials: {materials.Count}",
                "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Error", $"Failed to combine meshes:\n{e.Message}", "OK");
            Debug.LogError($"Mesh Baker Error: {e}");
        }
    }
}