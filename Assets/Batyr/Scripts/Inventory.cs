using System.Collections.Generic;

namespace Batyr.Scripts
{
    public class Inventory : Singleton<Inventory>
    {
        public List<ArtifactData> artifacts = new();

        private void Start()
        {
            artifacts = SaveLoadSystem.Instance.Load<List<ArtifactData>>("inventory");
        }

        private void OnDestroy()
        {
            SaveLoadSystem.Instance.Save(artifacts, "inventory");
        }
    }
}