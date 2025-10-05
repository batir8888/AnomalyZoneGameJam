using System.Collections.Generic;
using Batyr.Scripts;
using UnityEngine;

public class CraftCapsule : MonoBehaviour
{
    public bool CanCraft((ArtifactData, ArtifactData) pair, out ArtifactData artifact)
    {
        artifact = default;
        
        // Валидация входных данных

        // Проверка возможности комбинирования
        if (!pair.Item1.CanBeCombined || !pair.Item2.CanBeCombined)
        {
            Debug.LogWarning($"Артефакты не могут быть скомбинированы: " +
                           $"Item1.CanBeCombined={pair.Item1.CanBeCombined}, " +
                           $"Item2.CanBeCombined={pair.Item2.CanBeCombined}");
            return false;
        }
        
        // Проверка одинакового тира
        if (pair.Item1.Tier != pair.Item2.Tier)
        {
            Debug.LogWarning($"Артефакты разных тиров: {pair.Item1.Tier} != {pair.Item2.Tier}");
            return false;
        }
        
        int tier = pair.Item1.Tier;
        
        // ОСОБАЯ ЛОГИКА ДЛЯ TIER 5 (квестовые артефакты)
        if (tier == 5)
        {
            Debug.LogWarning("Артефакты Tier 5 являются квестовыми и не могут быть скрафчены дальше!");
            return false;
        }
        
        // Получение маппинга и списка артефактов
        TierMapping tierMap = CraftBootstrap.Instance.GetTiers(tier);
        List<ArtifactData> artifactList = CraftBootstrap.Instance.GetArtifacts(tier + 1);
        
        if (tierMap == null)
        {
            Debug.LogError($"TierMapping для тира {tier} не найден!");
            return false;
        }
        
        if (artifactList == null || artifactList.Count == 0)
        {
            Debug.LogError($"Список артефактов для тира {tier + 1} пуст!");
            return false;
        }
        
        // Нормализация порядка (a <= b)
        int localA = Mathf.Min(pair.Item1.LocalId, pair.Item2.LocalId);
        int localB = Mathf.Max(pair.Item1.LocalId, pair.Item2.LocalId);
        
        // Поиск рецепта
        int resultId = tierMap.GetResult(localA, localB);
        
        if (resultId < 0)
        {
            Debug.LogWarning($"Рецепт ({localA}, {localB}) не найден в маппинге тира {tier}");
            return false;
        }
        
        if (resultId >= artifactList.Count)
        {
            Debug.LogError($"ResultId {resultId} выходит за границы списка артефактов (Count={artifactList.Count})");
            return false;
        }
        
        // Успешный крафт
        artifact = artifactList[resultId];
        
        // ОСОБАЯ ОБРАБОТКА ДЛЯ TIER 5 РЕЗУЛЬТАТА
        if (artifact.Tier == 5)
        {
            Debug.Log($"<color=gold>КВЕСТОВЫЙ АРТЕФАКТ ПОЛУЧЕН!</color> ({localA},{localB}) tier {tier} -> QUEST artifact {artifact.Id}");
        }
        else
        {
            Debug.Log($"Крафт успешен: ({localA},{localB}) tier {tier} -> artifact {artifact.Id} tier {artifact.Tier}");
        }
        
        return true;
    }
    
    // Специальный метод для проверки, является ли артефакт квестовым
    public bool IsQuestArtifact(ArtifactData artifact)
    {
        return artifact.Tier == 5 && artifact.IsQuest;
    }
    
    // Получить информацию о возможности крафта
    public string GetCraftInfo((ArtifactData, ArtifactData) pair)
    {
        if (!pair.Item1.CanBeCombined || !pair.Item2.CanBeCombined)
            return "Эти артефакты нельзя комбинировать";
        
        if (pair.Item1.Tier != pair.Item2.Tier)
            return $"Разные тиры: {pair.Item1.Tier} и {pair.Item2.Tier}";
        
        if (pair.Item1.Tier == 5)
            return "Квестовые артефакты Tier 5 нельзя крафтить дальше";
        
        int tier = pair.Item1.Tier;
        int localA = Mathf.Min(pair.Item1.LocalId, pair.Item2.LocalId);
        int localB = Mathf.Max(pair.Item1.LocalId, pair.Item2.LocalId);
        
        var tierMap = CraftBootstrap.Instance.GetTiers(tier);
        if (tierMap == null)
            return $"Маппинг для Tier {tier} не найден";
        
        int resultId = tierMap.GetResult(localA, localB);
        if (resultId < 0)
            return $"Рецепт ({localA}, {localB}) не существует";
        
        var artifactList = CraftBootstrap.Instance.GetArtifacts(tier + 1);
        if (artifactList == null || resultId >= artifactList.Count)
            return "Результат крафта не найден";
        
        var resultArtifact = artifactList[resultId];
        
        if (resultArtifact.Tier == 5)
        {
            return $"<color=gold>Создаст КВЕСТОВЫЙ артефакт Tier 5!</color>\n" +
                   $"ID: {resultArtifact.Id}, LocalId: {resultArtifact.LocalId}";
        }
        
        return $"Создаст артефакт Tier {resultArtifact.Tier}\n" +
               $"ID: {resultArtifact.Id}, Бонус: {resultArtifact.Bonus} +{resultArtifact.BonusValue}";
    }
}