using System.Collections.Generic;

namespace Batyr.Scripts
{
    public class CraftRepository
    {
        public int Seed = SaveLoadSystem.Instance.Load<int>("seed");

        public List<List<ArtifactData>> Crafts = new()
        {
            SaveLoadSystem.Instance.Load<List<ArtifactData>>("tierOne"),
            SaveLoadSystem.Instance.Load<List<ArtifactData>>("tierTwo"),
            SaveLoadSystem.Instance.Load<List<ArtifactData>>("tierThree"),
            SaveLoadSystem.Instance.Load<List<ArtifactData>>("tierFour"),
            SaveLoadSystem.Instance.Load<List<ArtifactData>>("tierFive")
        };

    }
    
    
}