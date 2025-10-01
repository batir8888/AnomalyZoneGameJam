using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

[Serializable]
public class TierMapping
{
    [JsonProperty("pairs")]
    public List<CraftPair> Pairs = new();
    
    [JsonProperty("results")]
    public List<int> Results = new();

    // Получить resultId для пары (a,b)
    public int GetResult(int a, int b)
    {
        for (int i = 0; i < Pairs.Count; i++)
        {
            if (Pairs[i].a == a && Pairs[i].b == b)
                return Results[i];
        }
        return -1; // не найдено
    }

    // Добавить маппинг
    public void Add(int a, int b, int resultId)
    {
        Pairs.Add(new CraftPair { a = a, b = b });
        Results.Add(resultId);
    }

    // Найти первую пару для данного resultId
    public CraftPair FindFirstPair(int resultId)
    {
        for (int i = 0; i < Results.Count; i++)
        {
            if (Results[i] == resultId)
                return Pairs[i];
        }
        return new CraftPair { a = 0, b = 0 };
    }
}

[Serializable]
public struct CraftPair
{
    [JsonProperty("a")]
    public int a;
    
    [JsonProperty("b")]
    public int b;
}

// ===== Билдер системы крафта =====
public static class CraftTierBuilder
{
    public static TierMapping BuildTierMap(
        int nPrev,
        int nTarget,
        int seed)
    {
        // 1) Все пары с самоскрещиванием
        var pairs = new List<CraftPair>(nPrev * (nPrev + 1) / 2);
        for (int a = 0; a < nPrev; a++)
            for (int b = a; b < nPrev; b++)
                pairs.Add(new CraftPair { a = a, b = b });

        // 2) Детерминированная сортировка
        pairs = pairs.OrderBy(p => HashPair(p.a, p.b, seed)).ToList();

        // 3) Вместимость корзин
        int r = pairs.Count;
        int q = (r + nTarget - 1) / nTarget; // ceil

        // 4) Раскладываем по корзинам
        var load = new int[nTarget];
        var mapping = new TierMapping();
        int bin = 0;
        
        foreach (var p in pairs)
        {
            // найти следующую корзину с местом
            int tries = 0;
            while (load[bin] >= q && tries < nTarget)
            {
                bin = (bin + 1) % nTarget;
                tries++;
            }
            if (tries == nTarget) bin = 0; // страховка

            mapping.Add(p.a, p.b, bin);
            load[bin]++;
            bin = (bin + 1) % nTarget;
        }
        
        return mapping;
    }

    private static uint HashPair(int a, int b, int seed)
    {
        unchecked
        {
            var x = (uint)(a * 73856093) ^ (uint)(b * 19349663) ^ (uint)seed;
            x ^= x << 13; x ^= x >> 17; x ^= x << 5; // xorshift
            return x;
        }
    }

    // Роллим бонусы стабильно для экземпляра результата
    public static int RollBonus(int a, int b, int seed, int min, int max)
    {
        unchecked
        {
            uint x = (uint)(a * 83492791) ^ (uint)(b * 2654435761) ^ (uint)seed;
            x ^= x << 13; x ^= x >> 17; x ^= x << 5;
            int range = max - min + 1;
            return min + (int)(x % (uint)range);
        }
    }
}