using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompasUI : MonoBehaviour
{

    [SerializeField] Transform Target;
    Vector3 dir;

    // Update is called once per frame
    void Update()
    {
        dir.z= Target.eulerAngles.y;
        transform.localEulerAngles = dir;
    }
}
