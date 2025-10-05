using System.Collections;
using Batyr.Scripts;
using UnityEngine;

public class CraftCapsule : MonoBehaviour
{
    private static readonly int Crafting = Animator.StringToHash("Crafting");
    private Animator _animator;
    private bool _crafting;
    private WaitForSeconds _craftingWait;

    private void Awake()
    {
        _craftingWait = new WaitForSeconds(2.2f);
        _animator = GetComponent<Animator>();    
    }
    
    public bool CanCraft((ArtifactData, ArtifactData) pair, out ArtifactData artifact)
    {
        bool canCombined = pair.Item1.CanBeCombined && pair.Item2.CanBeCombined;
        bool sameTier = pair.Item1.Tier == pair.Item2.Tier;
        
        if (canCombined && sameTier)
        {
            var tierMap = CraftBootstrap.Instance.GetTiers(pair.Item1.Tier);
            var artifactList = CraftBootstrap.Instance.GetArtifacts(pair.Item1.Tier + 1);
            
            // ВАЖНО: пары хранятся как (a <= b), нужно нормализовать порядок
            int localA = Mathf.Min(pair.Item1.LocalId, pair.Item2.LocalId);
            int localB = Mathf.Max(pair.Item1.LocalId, pair.Item2.LocalId);
            
            var localIdOfNewArtifact = tierMap.GetResult(localA, localB);
            
            // Проверка на случай, если рецепт не найден
            if (localIdOfNewArtifact == -1 || localIdOfNewArtifact >= artifactList.Count)
            {
                artifact = default;
                return false;
            }

            StartCoroutine(Craft());
            
            artifact = artifactList[localIdOfNewArtifact];
            return true;
        }

        artifact = default;
        return false;
    }

    private IEnumerator Craft()
    {
        _animator.SetBool(Crafting, true);
        yield return _craftingWait;
        _animator.SetBool(Crafting, false);
    }

    [ContextMenu("Craft")]
    public void DebugAnimator()
    {
        StartCoroutine(Craft());
    }
}