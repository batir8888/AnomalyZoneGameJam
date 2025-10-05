using System.Collections.Generic;

namespace Batyr.Scripts
{
    public class Inventory : Singleton<Inventory>
    {
        public List<ArtifactData> artifacts = new();

        private void Start()
        {
            var loaded = SaveLoadSystem.Instance.Load<List<ArtifactData>>("inventory");
            artifacts = loaded ?? new List<ArtifactData>();
        }

        private void OnDestroy()
        {
            SaveLoadSystem.Instance.Save(artifacts, "inventory");
        }
        
        public void DeleteArtifact(ArtifactData artifact)
        {
            artifacts.Remove(artifact);
        }
        
        // ИСПРАВЛЕНО: Поиск по Id, а не LocalId
        public ArtifactData GetDataById(int id)
        {
            return artifacts.Find(x => x.Id == id);
        }
        
        // Дополнительный метод поиска по LocalId (если нужен)
        public ArtifactData GetDataByLocalId(int tier, int localId)
        {
            return artifacts.Find(x => x.Tier == tier && x.LocalId == localId);
        }
        
        // Проверка наличия артефакта
        public bool HasArtifact(int id)
        {
            return artifacts.Exists(x => x.Id == id);
        }
        
        // Добавить артефакт
        public void AddArtifact(ArtifactData artifact)
        {
            artifacts.Add(artifact);
            SaveLoadSystem.Instance.Save(artifacts, "inventory");
        }
    }
}