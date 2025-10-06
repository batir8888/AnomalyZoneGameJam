using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class AmbientPlayer : MonoBehaviour
{
    [SerializeField] private List<AudioClip> audioClips;
    
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!_audioSource.isPlaying) _audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Count)]);
    }
}
