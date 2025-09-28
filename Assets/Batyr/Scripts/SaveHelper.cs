using UnityEngine;

namespace Batyr.Scripts
{
    public class SaveHelper : Singleton<SaveHelper>
    {
        [SerializeField] private GameObject example;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                var vector = example.transform.position;
                var saveData = new Vector(vector.x, vector.y, vector.z);
                SaveLoadSystem.Instance.Save(saveData, "example");
                Debug.Log($"Saved example {saveData}");
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                var loadData = SaveLoadSystem.Instance.Load<Vector>("example");
                example.transform.position = new Vector3(loadData.X, loadData.Y, loadData.Z);
                Debug.Log($"Loaded example {loadData}");
            }
        }
    }

    public struct Vector
    {
        public float X;
        public float Y;
        public float Z;

        public Vector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}, Z: {Z}";
        }
    }
}