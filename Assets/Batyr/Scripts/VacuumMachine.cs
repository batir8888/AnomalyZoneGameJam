using System;
using System.Collections;
using System.Collections.Generic;
using Batyr.Scripts;
using UnityEngine;
using UnityEngine.VFX;

public class VacuumMachine : MonoBehaviour
{
    private Collider[] _colliders;
    private GameObject _gameObjectToDestroy;
    private VisualEffect _vfx;

    [SerializeField] private float force;
    [SerializeField] private float maxAngleToAttract;
    [SerializeField] private float attractorRadius;
    [SerializeField] private float radiusToTake;
    [SerializeField] private Transform attractor;

    private void Awake()
    {
        _colliders = new Collider[4];
    }

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
        
        else if (Input.GetMouseButton(0))
        {
            Attract();
        }
        
        else if (Input.GetMouseButtonUp(0))
        {
            _vfx.Stop();
        }
    }

    private void CheckArtifacts()
    {
        Physics.OverlapSphereNonAlloc(attractor.position, attractorRadius, _colliders, LayerMask.GetMask("Artifact"));
    }
    private void Attract()
    {
        CheckArtifacts();
        foreach (var collider in _colliders)
        {
            if (!collider) continue;
            if (Vector3.Angle(attractor.forward, collider.transform.position - transform.position) < maxAngleToAttract)
            {
                collider.GetComponent<Artifact>().BeAttracted(attractor.position, force * Time.deltaTime);
            }
            if (Vector3.Distance(attractor.position, collider.transform.position) < radiusToTake) _gameObjectToDestroy = collider.gameObject;
            // if (collider.transform.position != default)
            //     Debug.Log($"{collider.name} {Vector3.Angle(transform.forward, collider.transform.position - transform.position)}");
        }
        if (_gameObjectToDestroy) Destroy(_gameObjectToDestroy);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attractor.position, attractorRadius);
    }
}
