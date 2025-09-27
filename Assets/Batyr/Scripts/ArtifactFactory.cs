using System;
using System.Collections.Generic;
using UnityEngine;

// ===== ДАННЫЕ =====

// ===== ФАБРИКА =====
public static class ArtifactFactory
{
    // Tier1: 5 фиксированных
    public static List<ArtifactData> BuildTier1()
    {
        var list = new List<ArtifactData>(5);
        (SelectId sel, Color col)[] defs = new[]
        {
            (SelectId.Red,    Color.red),
            (SelectId.Blue,   Color.blue),
            (SelectId.Green,  Color.green),
            (SelectId.Purple, new Color(0.6f,0.2f,0.8f)),
            (SelectId.Gray,   Color.gray)
        };

        for (int i = 0; i < defs.Length; i++)
        {
            list.Add(new ArtifactData{
                Tier = 1,
                LocalId = i,
                Id = 1*1000 + i,
                Select = defs[i].sel,
                Color = defs[i].col,
                Bonus = (BonusType)(i % Enum.GetNames(typeof(BonusType)).Length),
                BonusValue = 1 // базовый минимум; при желании рулить отдельно
            });
        }
        return list;
    }

    // Tier>=2: контролируемое число результатов через BuildTierMap
    // nPrev = кол-во артов прошлого ранга; nTarget = желаемое число результатов этого ранга
    public static List<ArtifactData> BuildTierK(
        int tier, int nPrev, int nTarget, int seed,
        int bonusMin = 1, int bonusMax = 10)
    {
        if (tier < 2) throw new ArgumentException("tier >= 2");
        var map = CraftTierBuilder.BuildTierMap(nPrev, nTarget, seed);

        // Собираем уникальные resultId (0..nTarget-1)
        var results = new bool[nTarget];
        foreach (var kv in map) results[kv.Value] = true;

        var list = new List<ArtifactData>(nTarget);
        int bonusTypes = Enum.GetNames(typeof(BonusType)).Length;

        for (int resultId = 0; resultId < nTarget; resultId++)
        {
            if (!results[resultId]) continue; // защита, обычно все true

            // Детерминированный цвет от (tier, resultId, seed)
            Color color = ColorFromHash(Hash3(seed, tier, resultId));

            // Тип бонуса жёстко от resultId, чтобы был стабильный
            var bonusType = (BonusType)(resultId % bonusTypes);

            // Значение бонуса возьмём от репрезентативной пары этой корзины
            // Найдём первую пару, назначенную в resultId:
            (int A, int B) rep = FindRepresentativePair(map, resultId);

            int bonusVal = CraftTierBuilder.RollBonus(rep.A, rep.B, seed, bonusMin, bonusMax);

            // ВАЖНО: SelectId для Tier>=2 — это "первый стат", равный resultId корзины.
            // Приведём к SelectId. Значения >4 не перечислены в enum, но каст допустим.
            // Для UI можно хранить и raw int (LocalId) параллельно.
            var data = new ArtifactData{
                Tier = tier,
                LocalId = resultId,
                Id = tier*1000 + resultId,
                Select = (SelectId)resultId,
                Color = color,
                Bonus = bonusType,
                BonusValue = bonusVal
            };
            list.Add(data);
        }

        return list;
    }

    private static (int A, int B) FindRepresentativePair(Dictionary<(int,int), int> map, int resultId)
    {
        foreach (var kv in map)
            if (kv.Value == resultId) return kv.Key;
        return (0, 0); // fallback
    }

    // ===== Детерминированный цвет =====
    // Преобразуем 32-битный хеш в HSV без рандома Unity
    private static Color ColorFromHash(uint h)
    {
        // h → [0,1]
        float u = (h & 0xFFFFFF) / (float)0xFFFFFF;          // 24 бита
        float v = ((h >> 8) & 0xFFFFFF) / (float)0xFFFFFF;

        float H = u;                         // 0..1
        float S = 0.6f + 0.35f * v;          // 0.6..0.95 — поярче
        float V = 0.7f + 0.3f * (1f - v);    // 0.7..1.0 — чтобы не темнил
        Color c = Color.HSVToRGB(H, S, V);
        c.a = 1f;
        return c;
    }

    // Простой детерминированный 32-битный хеш от трех ints
    private static uint Hash3(int a, int b, int c)
    {
        unchecked
        {
            uint x = 2166136261;
            x = (x ^ (uint)a) * 16777619;
            x = (x ^ (uint)b) * 16777619;
            x = (x ^ (uint)c) * 16777619;
            x ^= x << 13; x ^= x >> 17; x ^= x << 5;
            return x;
        }
    }
}