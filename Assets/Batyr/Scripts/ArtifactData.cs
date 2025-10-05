using System;
using UnityEngine;

[Serializable]
public struct ArtifactData
{
    public int Id;          // глобальный id = Tier*1000 + LocalId
    public int Tier;        // ранг
    public int LocalId;     // для Tier1 = SelectId (0..4); для Tier>=2 = resultId из корзины
    public SelectId Select; // первый стат для селекции (Tier1 — enum выше; Tier>=2 — каст к SelectId)
    public Color Color;     // визуал
    public BonusType Bonus; // тип бонуса
    public int BonusValue;  // величина
    
    public bool IsQuest;        // только для Tier 5
    public bool CanBeCombined;  // = false для Tier 5

    public override string ToString() =>
        $"{Id}   {Tier}    {Select}";
}