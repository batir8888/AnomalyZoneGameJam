using System;
using System.Collections.Generic;
using UnityEngine;

public static class ArtifactDataFactory
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
            (SelectId.Purple, new Color(0.6f, 0.2f, 0.8f)),
            (SelectId.Gray,   Color.gray)
        };

        for (int i = 0; i < defs.Length; i++)
        {
            list.Add(new ArtifactData
            {
                Tier = 1,
                LocalId = i,
                Id = 1 * 1000 + i,
                Select = defs[i].sel,
                Color = defs[i].col,
                Bonus = (BonusType)(i % Enum.GetNames(typeof(BonusType)).Length),
                BonusValue = 5,
                CanBeCombined = true,
                IsQuest = false
            });
        }
        return list;
    }

    // Tier>=2: контролируемое число результатов
    public static List<ArtifactData> BuildTierK(
        int tier, int nPrev, int nTarget, int seed,
        int bonusMin = 1, int bonusMax = 10)
    {
        if (tier < 2) throw new ArgumentException("tier >= 2");
        var mapping = CraftTierBuilder.BuildTierMap(nPrev, nTarget, seed);

        // Собираем уникальные resultId (0..nTarget-1)
        var results = new bool[nTarget];
        foreach (var resultId in mapping.Results)
            results[resultId] = true;

        var list = new List<ArtifactData>(nTarget);
        int bonusTypes = Enum.GetNames(typeof(BonusType)).Length;

        for (int resultId = 0; resultId < nTarget; resultId++)
        {
            if (!results[resultId]) continue;

            // Детерминированный цвет
            Color color = ColorFromHash(Hash3(seed, tier, resultId));

            // Тип бонуса жёстко от resultId
            var bonusType = (BonusType)(resultId % bonusTypes);

            // Значение бонуса от репрезентативной пары
            CraftPair rep = mapping.FindFirstPair(resultId);
            int bonusVal = CraftTierBuilder.RollBonus(rep.a, rep.b, seed, bonusMin, bonusMax);

            var data = new ArtifactData
            {
                Tier = tier,
                LocalId = resultId,
                Id = tier * 1000 + resultId,
                Select = (SelectId)resultId,
                Color = color,
                Bonus = bonusType,
                BonusValue = bonusVal,
                IsQuest = false,
                CanBeCombined = true
            };
            list.Add(data);
        }

        return list;
    }

    // Tier 5: всегда 3 квестовых
    public static List<ArtifactData> BuildTier5(int nPrev, int seed)
    {
        const int tier = 5;
        const int nTarget = 3;
        _ = CraftTierBuilder.BuildTierMap(nPrev, nTarget, seed);

        var list = new List<ArtifactData>(nTarget);
        for (int resultId = 0; resultId < nTarget; resultId++)
        {
            Color color = ColorFromHash(Hash3(seed, tier, resultId));

            var data = new ArtifactData
            {
                Tier = tier,
                LocalId = resultId,
                Id = tier * 1000 + resultId,
                Select = (SelectId)resultId,
                Color = color,
                Bonus = (BonusType)(resultId % Enum.GetNames(typeof(BonusType)).Length),
                BonusValue = 0,
                IsQuest = true,
                CanBeCombined = false
            };
            list.Add(data);
        }
        return list;
    }

    // ===== Вспомогательные функции =====
    private static Color ColorFromHash(uint h)
    {
        float u = (h & 0xFFFFFF) / (float)0xFFFFFF;
        float v = ((h >> 8) & 0xFFFFFF) / (float)0xFFFFFF;

        float H = u;
        float S = 0.6f + 0.35f * v;
        float V = 0.7f + 0.3f * (1f - v);
        Color c = Color.HSVToRGB(H, S, V);
        c.a = 1f;
        return c;
    }

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