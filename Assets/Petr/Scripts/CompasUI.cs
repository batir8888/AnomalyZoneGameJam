using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompasUI : MonoBehaviour
{

    [SerializeField] GameObject Target;
    Vector3 dir;

    // Update is called once per frame
    void Update()
    {
        dir.z= Target.transform.eulerAngles.y;
        transform.localEulerAngles = dir;
    }
}
