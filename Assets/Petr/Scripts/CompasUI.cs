using UnityEngine;

public class CompasUI : MonoBehaviour
{
    public Transform target;
    
    private Vector3 _dir;

    // Update is called once per frame
    private void Update()
    {
        if (!target) return;
        _dir.z = target.eulerAngles.y;
        transform.localEulerAngles = _dir;
    }
}
