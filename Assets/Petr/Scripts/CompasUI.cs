using UnityEngine;

public class CompasUI : MonoBehaviour
{
    public void SetAngle(float angle)
    {
        transform.localEulerAngles = new Vector3(0, 0, angle);
    }
}
