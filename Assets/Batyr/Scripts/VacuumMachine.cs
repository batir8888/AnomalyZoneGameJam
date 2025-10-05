using Batyr.Scripts;
using UnityEngine;
using UnityEngine.VFX;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class VacuumMachine : MonoBehaviour
{
    // Кэшированные компоненты
    private VisualEffect _vfx;
    private CompasUI _compassUI;
    private Light _light;
    private TankUI _slider;
    private AudioSource _audioSource;

    // Настройки из инспектора
    [Header("Settings")]
    [SerializeField] private float force = 10f;
    [SerializeField] private float maxAngleToAttract = 45f;
    [SerializeField] private float attractorRadius = 5f;
    [SerializeField] private float radiusToTake = 1f;
    [SerializeField] private Transform attractor;
    [SerializeField] private AudioClip magnetSfx;

    // Временные данные
    private List<Collider> _nearbyArtifacts = new();
    private Transform _closestArtifact;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _compassUI = GetComponentInChildren<CompasUI>();
        _light = GetComponentInChildren<Light>();
        _slider = GetComponentInChildren<TankUI>();
        _audioSource.clip = magnetSfx;
    }

    private void Start()
    {
        var magnetVfx = GetComponentInChildren<MagnetVfx>();
        if (magnetVfx)
        {
            _vfx = magnetVfx.Vfx;
        }
        else
        {
            Debug.LogWarning("MagnetVfx component not found on child.", gameObject);
        }
        
    }

    private void Update()
    {
        UpdateClosestArtifact();
        if (Input.GetMouseButtonDown(0) && _vfx)
        {
            _audioSource.Play();
            _vfx.Play();
            _light.intensity = 3f;
        }
        else if (Input.GetMouseButton(0))
        {
            AttractArtifacts();
        }
        else if (Input.GetMouseButtonUp(0) && _vfx)
        {
            _audioSource.Stop();
            _vfx.Stop();
            _light.intensity = 0f;
        }
    }

    private void AttractArtifacts()
    {
        FindNearbyArtifacts();
        UpdateClosestArtifact();

        foreach (var collider in _nearbyArtifacts)
        {
            if (!collider) continue;

            var artifactTransform = collider.transform;
            var direction = artifactTransform.position - attractor.position;

            if (Vector3.Angle(attractor.forward, direction) > maxAngleToAttract) continue;

            var artifact = collider.GetComponent<Artifact>();
            if (artifact)
            {
                artifact.BeAttracted(attractor.position, force * Time.deltaTime);
            }

            if (direction.magnitude < radiusToTake)
            {
                artifact.TakeToInventory();
                _slider.UpdateSlider();
                collider.gameObject.SetActive(false);
            }
        }
    }

    private void FindNearbyArtifacts()
    {
        _nearbyArtifacts.Clear();
        var colliders = Physics.OverlapSphere(attractor.position, attractorRadius, LayerMask.GetMask("Artifact"));
        foreach (var c in colliders)
        {
            if (c) _nearbyArtifacts.Add(c);
        }
    }

    private void UpdateClosestArtifact()
    {
        float closestDistance = float.MaxValue;
        _closestArtifact = null;

        foreach (var col in _nearbyArtifacts)
        {
            if (!col) continue;

            var distance = Vector3.SqrMagnitude(col.transform.position - attractor.position);
            if (!(distance < closestDistance)) continue;
            closestDistance = distance;
            _closestArtifact = col.transform;
        }

        if (!_closestArtifact || !_compassUI) return;
        var angle = -Vector3.SignedAngle(attractor.forward, _closestArtifact.position - attractor.position, Vector3.up);
        _compassUI.SetAngle(angle);
    }

    private void OnDrawGizmos()
    {
        if (!attractor) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attractor.position, attractorRadius);
    }
}