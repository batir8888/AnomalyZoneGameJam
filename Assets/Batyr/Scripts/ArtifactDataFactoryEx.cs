using System;
using System.Collections.Generic;
using UnityEngine;

public static class ArtifactDataFactoryEx
{
    // Tier 5: всегда 3 квестовых. nPrev = кол-во артов Tier 4
    public static List<ArtifactData> BuildTier5(int nPrev, int seed)
    {
        const int tier = 5;
        const int nTarget = 3;
        _ = CraftTierBuilder.BuildTierMap(nPrev, nTarget, seed);

        var list = new List<ArtifactData>(nTarget);
        for (int resultId = 0; resultId < nTarget; resultId++)
        {
            // цвет детерминируем от (tier, resultId, seed)
            Color color = ColorFromHash(Hash3(seed, tier, resultId));

            var data = new ArtifactData{
                Tier = tier,
                LocalId = resultId,
                Id = tier*1000 + resultId,
                Select = (SelectId)resultId, // первый стат остаётся индексом корзины
                Color = color,
                Bonus = (BonusType)(resultId % Enum.GetNames(typeof(BonusType)).Length),
                BonusValue = 0,          // квестовые могут не давать боевой бонус
                IsQuest = true,
                CanBeCombined = false    // стоп селекции на V тирах
            };
            list.Add(data);
        }
        return list;
    }

    // Вспомогалки — те же, что в твоей фабрике:
    private static uint Hash3(int a, int b, int c)
    {
        unchecked {
            uint x = 2166136261;
            x = (x ^ (uint)a) * 16777619;
            x = (x ^ (uint)b) * 16777619;
            x = (x ^ (uint)c) * 16777619;
            x ^= x << 13; x ^= x >> 17; x ^= x << 5;
            return x;
        }
    }
    private static Color ColorFromHash(uint h)
    {
        float u = (h & 0xFFFFFF) / (float)0xFFFFFF;
        float v = ((h >> 8) & 0xFFFFFF) / (float)0xFFFFFF;
        float H = u, S = 0.7f + 0.25f * v, V = 0.8f + 0.2f * (1f - v);
        var c = Color.HSVToRGB(H, S, V); c.a = 1f; return c;
    }
}