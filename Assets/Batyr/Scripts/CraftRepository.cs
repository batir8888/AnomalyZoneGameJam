using System.Collections.Generic;

namespace Batyr.Scripts
{
    public class CraftRepository
    {
        public int Seed = SaveLoadSystem.Instance.Load<int>("seed");

        public List<List<ArtifactData>> Artifacts = new()
        {
            SaveLoadSystem.Instance.Load<List<ArtifactData>>("tierOne"),
            SaveLoadSystem.Instance.Load<List<ArtifactData>>("tierTwo"),
            SaveLoadSystem.Instance.Load<List<ArtifactData>>("tierThree"),
            SaveLoadSystem.Instance.Load<List<ArtifactData>>("tierFour"),
            SaveLoadSystem.Instance.Load<List<ArtifactData>>("tierFive")
        };

        public List<TierMapping> Crafts = new()
        {
            SaveLoadSystem.Instance.Load<TierMapping>("mapTierTwo"),
            SaveLoadSystem.Instance.Load<TierMapping>("mapTierThree"),
            SaveLoadSystem.Instance.Load<TierMapping>("mapTierFour"),
            SaveLoadSystem.Instance.Load<TierMapping>("mapTierFive")
        };
    }
    
    
}