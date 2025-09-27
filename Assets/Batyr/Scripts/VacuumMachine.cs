using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VacuumMachine : MonoBehaviour
{
    private VisualEffect _vfx;

    private void Start()
    {
        _vfx = GetComponentInChildren<MagnetVfx>().Vfx;
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _vfx.Play();
        }
        
        else if (Input.GetMouseButtonUp(0))
        {
            _vfx.Stop();
        }
    }
}
