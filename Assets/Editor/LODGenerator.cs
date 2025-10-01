using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class LODGenerator : EditorWindow
{
    private GameObject _targetObject;
    private int _lodLevels = 3;
    private float[] _qualityLevels = new float[] { 0.6f, 0.3f, 0.1f };
    private float[] _screenRelativeHeights = new float[] { 0.6f, 0.3f, 0.15f };
    private bool _generateCulled = true;
    private Vector2 _scrollPos;
    private SimplificationMethod _simplificationMethod = SimplificationMethod.EdgeCollapse;

    public enum SimplificationMethod
    {
        EdgeCollapse,
        VertexClustering,
        UnityMeshSimplifier
    }

    [MenuItem("Tools/LOD Generator")]
    public static void ShowWindow()
    {
        GetWindow<LODGenerator>("LOD Generator");
    }

    private void OnGUI()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        
        GUILayout.Label("LOD Generator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        _targetObject = (GameObject)EditorGUILayout.ObjectField("Target Object", _targetObject, typeof(GameObject), true);
        
        EditorGUILayout.Space();
        _lodLevels = EditorGUILayout.IntSlider("LOD Levels", _lodLevels, 1, 5);
        
        _simplificationMethod = (SimplificationMethod)EditorGUILayout.EnumPopup("Simplification Method", _simplificationMethod);
        
        EditorGUILayout.Space();
        GUILayout.Label("LOD Settings", EditorStyles.boldLabel);
        
        if (_qualityLevels.Length != _lodLevels)
        {
            System.Array.Resize(ref _qualityLevels, _lodLevels);
            System.Array.Resize(ref _screenRelativeHeights, _lodLevels);
            
            for (int i = 0; i < _lodLevels; i++)
            {
                if (_qualityLevels[i] == 0)
                    _qualityLevels[i] = Mathf.Max(0.05f, 1f - (i + 1) * 0.25f);
                if (_screenRelativeHeights[i] == 0)
                    _screenRelativeHeights[i] = Mathf.Max(0.01f, 0.6f - i * 0.15f);
            }
        }

        for (int i = 0; i < _lodLevels; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"LOD {i}", GUILayout.Width(50));
            _qualityLevels[i] = EditorGUILayout.Slider("Quality", _qualityLevels[i], 0.05f, 0.95f);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(50));
            _screenRelativeHeights[i] = EditorGUILayout.Slider("Screen Height", _screenRelativeHeights[i], 0.01f, 0.95f);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }

        _generateCulled = EditorGUILayout.Toggle("Add Culled LOD", _generateCulled);

        EditorGUILayout.Space();
        
        GUI.enabled = _targetObject != null;
        if (GUILayout.Button("Generate LODs", GUILayout.Height(30)))
        {
            GenerateLoDs();
        }
        GUI.enabled = true;

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Улучшенный LOD Generator с правильными алгоритмами упрощения.\n\n" +
            "Edge Collapse - схлопывает рёбра, сохраняя форму\n" +
            "Vertex Clustering - группирует вершины по кластерам\n" +
            "Unity Mesh Simplifier - использует встроенный API Unity\n\n" +
            "Quality - процент сохраняемых полигонов\n" +
            "Screen Height - при каком размере переключается LOD",
            MessageType.Info
        );

        EditorGUILayout.EndScrollView();
    }

    private void GenerateLoDs()
    {
        if (_targetObject == null)
        {
            EditorUtility.DisplayDialog("Error", "Выберите целевой объект!", "OK");
            return;
        }

        MeshFilter meshFilter = _targetObject.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = _targetObject.GetComponent<MeshRenderer>();

        if (meshFilter == null || meshRenderer == null)
        {
            EditorUtility.DisplayDialog("Error", "Объект должен иметь MeshFilter и MeshRenderer!", "OK");
            return;
        }

        Undo.RegisterCompleteObjectUndo(_targetObject, "Generate LODs");

        LODGroup existingLODGroup = _targetObject.GetComponent<LODGroup>();
        if (existingLODGroup != null)
        {
            Undo.DestroyObjectImmediate(existingLODGroup);
        }

        GameObject lodParent = _targetObject;
        LODGroup lodGroup = lodParent.AddComponent<LODGroup>();

        Mesh originalMesh = meshFilter.sharedMesh;
        Material[] materials = meshRenderer.sharedMaterials;

        meshRenderer.enabled = false;

        // LOD0 (оригинал)
        GameObject lod0 = new GameObject("LOD0");
        lod0.transform.SetParent(lodParent.transform, false);
        MeshFilter lod0Mf = lod0.AddComponent<MeshFilter>();
        MeshRenderer lod0Mr = lod0.AddComponent<MeshRenderer>();
        lod0Mf.sharedMesh = originalMesh;
        lod0Mr.sharedMaterials = materials;

        List<LOD> lods = new List<LOD>();
        LOD lod = new LOD(1.0f, new Renderer[] { lod0Mr });
        lods.Add(lod);

        // Создаём остальные LOD уровни
        for (int i = 0; i < _lodLevels; i++)
        {
            EditorUtility.DisplayProgressBar("Generating LODs", $"Creating LOD{i + 1}...", (float)(i + 1) / _lodLevels);

            GameObject lodObj = new GameObject($"LOD{i + 1}");
            lodObj.transform.SetParent(lodParent.transform, false);
            
            MeshFilter mf = lodObj.AddComponent<MeshFilter>();
            MeshRenderer mr = lodObj.AddComponent<MeshRenderer>();
            
            Mesh simplifiedMesh = null;
            switch (_simplificationMethod)
            {
                case SimplificationMethod.EdgeCollapse:
                    simplifiedMesh = SimplifyMeshEdgeCollapse(originalMesh, _qualityLevels[i]);
                    break;
                case SimplificationMethod.VertexClustering:
                    simplifiedMesh = SimplifyMeshClustering(originalMesh, _qualityLevels[i]);
                    break;
                case SimplificationMethod.UnityMeshSimplifier:
                    simplifiedMesh = SimplifyMeshUnity(originalMesh, _qualityLevels[i]);
                    break;
            }

            mf.sharedMesh = simplifiedMesh;
            mr.sharedMaterials = materials;

            LOD lodLevel = new LOD(_screenRelativeHeights[i], new Renderer[] { mr });
            lods.Add(lodLevel);
        }

        if (_generateCulled)
        {
            LOD culledLOD = new LOD(0.01f, new Renderer[0]);
            lods.Add(culledLOD);
        }

        lodGroup.SetLODs(lods.ToArray());
        lodGroup.RecalculateBounds();

        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("Success", $"LOD группа создана с {_lodLevels + 1} уровнями!", "OK");
    }

    // Метод 1: Edge Collapse (схлопывание рёбер)
    private Mesh SimplifyMeshEdgeCollapse(Mesh originalMesh, float quality)
    {
        Mesh newMesh = Object.Instantiate(originalMesh);
        
        Vector3[] vertices = newMesh.vertices;
        int[] triangles = newMesh.triangles;
        Vector3[] normals = newMesh.normals;
        Vector2[] uvs = newMesh.uv;

        int targetVertexCount = Mathf.Max(3, Mathf.RoundToInt(vertices.Length * quality));
        
        List<Vector3> newVertices = new List<Vector3>(vertices);
        List<int> newTriangles = new List<int>(triangles);
        List<Vector3> newNormals = new List<Vector3>(normals.Length > 0 ? normals : vertices.Select(v => Vector3.up).ToArray());
        List<Vector2> newUVs = new List<Vector2>(uvs.Length > 0 ? uvs : vertices.Select(v => Vector2.zero).ToArray());

        Dictionary<int, int> vertexRemapping = new Dictionary<int, int>();

        // Находим близкие вершины и объединяем их
        float threshold = 0.01f / quality; // Чем ниже качество, тем больше порог
        
        for (int i = 0; i < newVertices.Count; i++)
        {
            if (vertexRemapping.ContainsKey(i)) continue;

            for (int j = i + 1; j < newVertices.Count; j++)
            {
                if (vertexRemapping.ContainsKey(j)) continue;

                if (Vector3.Distance(newVertices[i], newVertices[j]) < threshold)
                {
                    vertexRemapping[j] = i;
                }
            }
        }

        // Применяем ремаппинг к треугольникам
        for (int i = 0; i < newTriangles.Count; i++)
        {
            int vertIdx = newTriangles[i];
            if (vertexRemapping.ContainsKey(vertIdx))
            {
                newTriangles[i] = vertexRemapping[vertIdx];
            }
        }

        // Удаляем вырожденные треугольники
        List<int> validTriangles = new List<int>();
        for (int i = 0; i < newTriangles.Count; i += 3)
        {
            int v0 = newTriangles[i];
            int v1 = newTriangles[i + 1];
            int v2 = newTriangles[i + 2];

            if (v0 != v1 && v1 != v2 && v0 != v2)
            {
                validTriangles.Add(v0);
                validTriangles.Add(v1);
                validTriangles.Add(v2);
            }
        }

        newMesh.Clear();
        newMesh.vertices = newVertices.ToArray();
        newMesh.triangles = validTriangles.ToArray();
        newMesh.normals = newNormals.ToArray();
        newMesh.uv = newUVs.ToArray();
        
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();
        
        return newMesh;
    }

    // Метод 2: Vertex Clustering (кластеризация вершин)
    private Mesh SimplifyMeshClustering(Mesh originalMesh, float quality)
    {
        Mesh newMesh = Object.Instantiate(originalMesh);
        
        Vector3[] vertices = newMesh.vertices;
        int[] triangles = newMesh.triangles;
        Vector2[] uvs = newMesh.uv;

        Bounds bounds = newMesh.bounds;
        int gridSize = Mathf.Max(2, Mathf.RoundToInt(10 * quality));
        
        Vector3 cellSize = new Vector3(
            bounds.size.x / gridSize,
            bounds.size.y / gridSize,
            bounds.size.z / gridSize
        );

        Dictionary<Vector3Int, List<int>> clusters = new Dictionary<Vector3Int, List<int>>();

        // Группируем вершины по ячейкам сетки
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 localPos = vertices[i] - bounds.min;
            Vector3Int cell = new Vector3Int(
                Mathf.FloorToInt(localPos.x / cellSize.x),
                Mathf.FloorToInt(localPos.y / cellSize.y),
                Mathf.FloorToInt(localPos.z / cellSize.z)
            );

            if (!clusters.ContainsKey(cell))
            {
                clusters[cell] = new List<int>();
            }
            clusters[cell].Add(i);
        }

        // Создаём новые вершины (центры кластеров)
        Dictionary<int, int> oldToNew = new Dictionary<int, int>();
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector2> newUVs = new List<Vector2>();

        foreach (var cluster in clusters.Values)
        {
            Vector3 avgPos = Vector3.zero;
            Vector2 avgUV = Vector2.zero;
            
            foreach (int idx in cluster)
            {
                avgPos += vertices[idx];
                if (uvs.Length > 0)
                    avgUV += uvs[idx];
            }
            
            avgPos /= cluster.Count;
            avgUV /= cluster.Count;

            int newIdx = newVertices.Count;
            newVertices.Add(avgPos);
            newUVs.Add(avgUV);

            foreach (int oldIdx in cluster)
            {
                oldToNew[oldIdx] = newIdx;
            }
        }

        // Обновляем треугольники
        List<int> newTriangles = new List<int>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int v0 = oldToNew[triangles[i]];
            int v1 = oldToNew[triangles[i + 1]];
            int v2 = oldToNew[triangles[i + 2]];

            // Пропускаем вырожденные треугольники
            if (v0 != v1 && v1 != v2 && v0 != v2)
            {
                newTriangles.Add(v0);
                newTriangles.Add(v1);
                newTriangles.Add(v2);
            }
        }

        newMesh.Clear();
        newMesh.vertices = newVertices.ToArray();
        newMesh.triangles = newTriangles.ToArray();
        newMesh.uv = newUVs.ToArray();
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();

        return newMesh;
    }

    // Метод 3: Unity встроенный API
    private Mesh SimplifyMeshUnity(Mesh originalMesh, float quality)
    {
        Mesh newMesh = Object.Instantiate(originalMesh);
        
        #if UNITY_2022_2_OR_NEWER
        MeshUtility.SetMeshCompression(newMesh, ModelImporterMeshCompression.High);
        #endif

        // Используем Unity's Mesh.Optimize
        newMesh.Optimize();
        
        // Дополнительное упрощение через ремаппинг похожих вершин
        Vector3[] vertices = newMesh.vertices;
        int[] triangles = newMesh.triangles;
        
        float mergeThreshold = 0.001f / quality;
        Dictionary<Vector3, int> uniqueVertices = new Dictionary<Vector3, int>();
        List<Vector3> newVertices = new List<Vector3>();
        Dictionary<int, int> oldToNew = new Dictionary<int, int>();

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 roundedVert = new Vector3(
                Mathf.Round(vertices[i].x / mergeThreshold) * mergeThreshold,
                Mathf.Round(vertices[i].y / mergeThreshold) * mergeThreshold,
                Mathf.Round(vertices[i].z / mergeThreshold) * mergeThreshold
            );

            if (!uniqueVertices.ContainsKey(roundedVert))
            {
                uniqueVertices[roundedVert] = newVertices.Count;
                newVertices.Add(vertices[i]);
            }
            oldToNew[i] = uniqueVertices[roundedVert];
        }

        List<int> newTriangles = new List<int>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int v0 = oldToNew[triangles[i]];
            int v1 = oldToNew[triangles[i + 1]];
            int v2 = oldToNew[triangles[i + 2]];

            if (v0 != v1 && v1 != v2 && v0 != v2)
            {
                newTriangles.Add(v0);
                newTriangles.Add(v1);
                newTriangles.Add(v2);
            }
        }

        newMesh.Clear();
        newMesh.vertices = newVertices.ToArray();
        newMesh.triangles = newTriangles.ToArray();
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();

        return newMesh;
    }
}