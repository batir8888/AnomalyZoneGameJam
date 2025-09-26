using System.Collections.Generic;
using System.Linq;

public static class CraftTierBuilder
{
    public struct Pair { public int A, B; } // A<=B

    public static Dictionary<(int,int), int> BuildTierMap(
        int nPrev,           // N_{k-1}
        int nTarget,         // N_k
        int seed)
    {
        // 1) Все пары с самоскрещиванием
        var pairs = new List<Pair>(nPrev * (nPrev + 1) / 2);
        for (int a = 0; a < nPrev; a++)
            for (int b = a; b < nPrev; b++)
                pairs.Add(new Pair{ A = a, B = b });

        pairs = pairs.OrderBy(p => H(p.A, p.B)).ToList();

        // 3) Вместимость корзин
        int r = pairs.Count;
        int q = (r + nTarget - 1) / nTarget; // ceil

        // 4) Раскладываем по корзинам
        var load = new int[nTarget];
        var map = new Dictionary<(int,int), int>(r);
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

            map[(p.A, p.B)] = bin;
            load[bin]++;
            bin = (bin + 1) % nTarget;
        }
        return map; // (i,j)->resultId

        // 2) Детерминированный хеш и сортировка
        uint H(int a, int b)
        {
            unchecked
            {
                var x = (uint)(a * 73856093) ^ (uint)(b * 19349663) ^ (uint)seed;
                x ^= x << 13; x ^= x >> 17; x ^= x << 5; // xorshift
                return x;
            }
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
            return min + (int)(x % range);
        }
    }
}
