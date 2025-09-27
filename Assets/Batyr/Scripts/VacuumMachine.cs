using System;
using System.Collections;
using System.Collections.Generic;
using Batyr.Scripts;
using UnityEngine;
using UnityEngine.VFX;

public class VacuumMachine : MonoBehaviour
{
    private Collider[] _colliders;
    private Transform _closestArtifact;
    private GameObject _gameObjectToDestroy;
    private VisualEffect _vfx;
    private CompasUI _compasUI;

    [SerializeField] private float force;
    [SerializeField] private float maxAngleToAttract;
    [SerializeField] private float attractorRadius;
    [SerializeField] private float radiusToTake;
    [SerializeField] private Transform attractor;

    private void Awake()
    {
        _colliders = new Collider[4];
        _compasUI = GetComponentInChildren<CompasUI>();
    }

    private void Start()
    {
        _vfx = GetComponentInChildren<MagnetVfx>().Vfx;
    }
    
    private void Update()
    {
        CheckArtifacts();
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
        var distance = float.MaxValue;
        Physics.OverlapSphereNonAlloc(attractor.position, attractorRadius, _colliders, LayerMask.GetMask("Artifact"));
        foreach (var collider in _colliders)
        {
            if (!collider) continue;
            var newDistance = Vector3.Distance(attractor.position, collider.transform.position);
            if (newDistance <= distance) _closestArtifact = collider.transform;
        }
        _compasUI.target = _closestArtifact;
    }
    private void Attract()
    {
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
