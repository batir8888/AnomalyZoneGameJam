using System.Collections.Generic;
using Batyr.Scripts;
using UnityEngine;

public class ArtifactSpawner : MonoBehaviour
{ 
    [SerializeField] private List<GameObject> prefabs;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private int maxSpawn;
    [SerializeField] private bool useRandomSpawnPoint = true; // можно спавнить по порядку
    [SerializeField] private bool allowDuplicates = true; // разрешить повторы артефактов
    
    private List<ArtifactData> _tier1Data;
    
    private void Awake()
    {
        _tier1Data = ArtifactDataFactory.BuildTier1();
        
        // Валидация
        if (maxSpawn > spawnPoints.Count && !useRandomSpawnPoint)
        {
            Debug.LogWarning($"maxSpawn ({maxSpawn}) > spawnPoints.Count ({spawnPoints.Count}). Будет заспавнено только {spawnPoints.Count} артефактов.");
            maxSpawn = spawnPoints.Count;
        }
        
        if (maxSpawn > _tier1Data.Count && !allowDuplicates)
        {
            Debug.LogWarning($"maxSpawn ({maxSpawn}) > доступных артефактов ({_tier1Data.Count}). Будет заспавнено только {_tier1Data.Count} артефактов.");
            maxSpawn = _tier1Data.Count;
        }
        
        SpawnArtifacts();
    }

    private void SpawnArtifacts()
    {
        if (allowDuplicates)
        {
            // Старая логика - с повторами
            for (int i = 0; i < maxSpawn; i++)
            {
                var data = _tier1Data[Random.Range(0, _tier1Data.Count)];
                Spawn(data, i);
            }
        }
        else
        {
            // Без повторов - перемешиваем и берём первые N
            var shuffled = new List<ArtifactData>(_tier1Data);
            ShuffleList(shuffled);
            
            for (int i = 0; i < Mathf.Min(maxSpawn, shuffled.Count); i++)
            {
                Spawn(shuffled[i], i);
            }
        }
    }

    private void Spawn(ArtifactData data, int spawnIndex)
    {
        // Выбор точки спавна
        Transform spawnPoint;
        if (useRandomSpawnPoint)
        {
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        }
        else
        {
            spawnPoint = spawnPoints[spawnIndex % spawnPoints.Count];
        }
        
        // Проверка на корректность prefab индекса
        int prefabIndex = (int)data.Select;
        if (prefabIndex < 0 || prefabIndex >= prefabs.Count)
        {
            Debug.LogError($"Некорректный SelectId={prefabIndex} для артефакта Tier={data.Tier}, LocalId={data.LocalId}. Доступно prefabs: {prefabs.Count}");
            return;
        }
        
        // Спавн
        var go = Instantiate(prefabs[prefabIndex], spawnPoint.position, Quaternion.identity);
        var artifact = go.GetComponent<Artifact>();
        
        if (artifact == null)
        {
            Debug.LogError($"Prefab {prefabs[prefabIndex].name} не содержит компонент Artifact!");
            Destroy(go);
            return;
        }
        
        artifact.SetData(data);
        
        Debug.Log("Артефакт успешкно заспавнен");
    }
    
    // Fisher-Yates shuffle
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}