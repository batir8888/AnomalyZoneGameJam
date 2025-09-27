using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CraftTierDebugger : MonoBehaviour
{
    [Header("Входные параметры")]
    [Min(1)] public int nPrev = 5;          // N_{k-1}
    [Min(1)] public int nTarget = 8;        // N_k
    public int seed = 12345;

    [Header("Бонус для примера (опционально)")]
    public int bonusMin = 1;
    public int bonusMax = 10;

    [Header("CSV вывод")]
    public bool dumpCsv;
    public string csvFileName = "tier_debug.csv";

    [Header("Автозапуск")]
    public bool runOnStart = true;

    void Start()
    {
        if (runOnStart) RunDebug();
    }

    [ContextMenu("Run Debug")]
    public void RunDebug()
    {
        // Построить карту распределения
        var map = CraftTierBuilder.BuildTierMap(nPrev, nTarget, seed);

        // Инвертировать: resultId -> список пар
        var byBin = new Dictionary<int, List<(int a, int b)>>(nTarget);
        for (int i = 0; i < nTarget; i++) byBin[i] = new List<(int, int)>();

        // Сгенерировать все пары (A<=B) и прочитать их бин
        var allPairs = GeneratePairs(nPrev);
        foreach (var p in allPairs)
        {
            if (!map.TryGetValue((p.a, p.b), out int bin))
            {
                Debug.LogError($"Пары нет в map: ({p.a},{p.b})");
                continue;
            }
            byBin[bin].Add((p.a, p.b));
        }

        int r = allPairs.Count;
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
            foreach (var (a, b) in list)
            {
                int bonus = CraftTierBuilder.RollBonus(a, b, seed, bonusMin, bonusMax);
                Debug.Log($"({a},{b}) -> result {bin}, bonus {bonus}");
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
                    foreach (var (a, b) in allPairs)
                    {
                        int bin = map[(a, b)];
                        int bonus = CraftTierBuilder.RollBonus(a, b, seed, bonusMin, bonusMax);
                        sw.WriteLine($"{a},{b},{bin},{bonus}");
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

    // Генерация всех пар с самоскрещиванием: (A<=B)
    private static List<(int a, int b)> GeneratePairs(int nPrev)
    {
        var list = new List<(int, int)>(nPrev * (nPrev + 1) / 2);
        for (int a = 0; a < nPrev; a++)
            for (int b = a; b < nPrev; b++)
                list.Add((a, b));
        return list;
    }
}
