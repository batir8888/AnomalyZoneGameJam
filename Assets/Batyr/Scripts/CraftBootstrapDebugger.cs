using UnityEngine;

namespace Batyr.Scripts
{
    public class CraftBootstrapDebugger : MonoBehaviour
    {
        public void DebugPlease()
        {
            var bootstrap = CraftBootstrap.Instance;
            Debug.Log($"The seed is {bootstrap.GetSeed()}");
            for (int i = 2; i <= 5; i++)
            {
                foreach (var artifactData in bootstrap.GetArtifacts(i))
                {
                    Debug.Log(artifactData);
                }
            }
        }
    }
}