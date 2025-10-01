using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Batyr.Scripts;
using UnityEngine;

public class CraftTierDebugger : MonoBehaviour
{
    [SerializeField] private int nPrev = 5;
    [SerializeField] private int nTarget = 8;
    [SerializeField] private int seed = 42;
    [SerializeField] private int bonusMin = 1;
    [SerializeField] private int bonusMax = 10;
    [SerializeField] private bool dumpCsv = true;
    [SerializeField] private string csvFileName = "craft_tier_debug.csv";

    [ContextMenu("Run Debug")]
    public void RunDebug()
    {
        // Построить карту распределения
        var mapping = CraftTierBuilder.BuildTierMap(nPrev, nTarget, seed);

        // Инвертировать: resultId -> список пар
        var byBin = new Dictionary<int, List<CraftPair>>(nTarget);
        for (int i = 0; i < nTarget; i++) 
            byBin[i] = new List<CraftPair>();

        // Распределить все пары по корзинам
        for (int i = 0; i < mapping.Pairs.Count; i++)
        {
            var pair = mapping.Pairs[i];
            int bin = mapping.Results[i];
            byBin[bin].Add(pair);
        }

        int r = mapping.Pairs.Count;
        int q = (r + nTarget - 1) / nTarget;
        Debug.Log($"[CraftTierDebugger] N_prev={nPrev}, N_target={nTarget}, R={r}, q=ceil(R/N_target)={q}, seed={seed}");

        // Печать загрузки корзин
        for (int bin = 0; bin < nTarget; bin++)
        {
            int c = byBin[bin].Count;
            Debug.Log($"bin {bin}: {c} рецептов");
        }

        // Детальная печать по корзинам
        for (int bin = 0; bin < nTarget; bin++)
        {
            var list = byBin[bin];
            list.Sort((x, y) => x.a == y.a ? x.b.CompareTo(y.b) : x.a.CompareTo(y.a));
            string header = $"== BIN {bin} ==";
            Debug.Log(header);
            foreach (var pair in list)
            {
                int bonus = CraftTierBuilder.RollBonus(pair.a, pair.b, seed, bonusMin, bonusMax);
                Debug.Log($"({pair.a},{pair.b}) -> result {bin}, bonus {bonus}");
            }
        }

        // CSV
        if (dumpCsv)
        {
            string path = Path.Combine(Application.persistentDataPath, csvFileName);
            try
            {
                using (var sw = new StreamWriter(path, false))
                {
                    sw.WriteLine("A,B,ResultId,Bonus");
                    for (int i = 0; i < mapping.Pairs.Count; i++)
                    {
                        var pair = mapping.Pairs[i];
                        int bin = mapping.Results[i];
                        int bonus = CraftTierBuilder.RollBonus(pair.a, pair.b, seed, bonusMin, bonusMax);
                        sw.WriteLine($"{pair.a},{pair.b},{bin},{bonus}");
                    }
                }
                Debug.Log($"CSV сохранён: {path}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка записи CSV: {e.Message}");
            }
        }
    }

    [ContextMenu("DebugPlease")]
    public void DebugPlease()
    {
        for (int tier = 2; tier <= 5; tier++)
        {
            Debug.Log($"=== TIER {tier} ===");
            var mapping = CraftBootstrap.Instance.GetTiers(tier);
            
            if (mapping == null)
            {
                Debug.LogWarning($"Tier {tier} mapping is null!");
                continue;
            }

            for (int i = 0; i < mapping.Pairs.Count; i++)
            {
                var pair = mapping.Pairs[i];
                int resultId = mapping.Results[i];
                Debug.Log($"Key: ({pair.a}, {pair.b}), Value: {resultId}");
            }
        }
    }

    // Вспомогательный метод для проверки конкретного рецепта
    [ContextMenu("Test Single Recipe")]
    public void TestSingleRecipe()
    {
        int testA = 0;
        int testB = 1;
        int testTier = 2;

        var mapping = CraftBootstrap.Instance.GetTiers(testTier);
        int result = mapping.GetResult(testA, testB);
        
        if (result == -1)
        {
            Debug.LogWarning($"Recipe ({testA},{testB}) not found in tier {testTier}");
        }
        else
        {
            Debug.Log($"Recipe ({testA},{testB}) in tier {testTier} -> result {result}");
        }
    }

    // Статистика по всем тирам
    [ContextMenu("Print All Tiers Stats")]
    public void PrintAllTiersStats()
    {
        for (int tier = 2; tier <= 5; tier++)
        {
            var mapping = CraftBootstrap.Instance.GetTiers(tier);
            
            if (mapping == null)
            {
                Debug.LogWarning($"Tier {tier}: null");
                continue;
            }

            // Подсчитать уникальные результаты
            var uniqueResults = new HashSet<int>(mapping.Results);
            
            Debug.Log($"Tier {tier}: {mapping.Pairs.Count} recipes -> {uniqueResults.Count} unique results");
            
            // Распределение по корзинам
            var distribution = new Dictionary<int, int>();
            foreach (int resultId in mapping.Results)
            {
                if (!distribution.ContainsKey(resultId))
                    distribution[resultId] = 0;
                distribution[resultId]++;
            }
            
            var sorted = distribution.OrderBy(kv => kv.Key).ToList();
            foreach (var kv in sorted)
            {
                Debug.Log($"  Result {kv.Key}: {kv.Value} recipes");
            }
        }
    }
}