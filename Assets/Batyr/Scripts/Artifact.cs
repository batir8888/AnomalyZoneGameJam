using UnityEngine;

namespace Batyr.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class Artifact : MonoBehaviour
    {
        private Rigidbody _rb;
        
        [field:SerializeField] public ArtifactData Data { get; private set; }

        private void Awake()
        {
            gameObject.layer = LayerMask.NameToLayer("Artifact");
            _rb = GetComponent<Rigidbody>();
        }

        public void TakeToInventory()
        {
            Inventory.Instance.artifacts.Add(Data);
        }

        public void BeAttracted(Vector3 to, float force)
        {
            var direction = to - transform.position;
            _rb.AddForce(direction * force, ForceMode.Impulse);
        }
    }
}