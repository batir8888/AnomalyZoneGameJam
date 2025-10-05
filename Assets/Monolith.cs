using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monolith : MonoBehaviour
{
    [SerializeField] private List<AudioClip> greetings;
    
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + Time.deltaTime * Random.Range(4, 20), 0);
    }

    IEnumerator Greeting()
    {
        
        var audioClip = greetings[Random.Range(0, greetings.Count)];
        _audioSource.PlayOneShot(audioClip);
        yield return new WaitForSeconds(audioClip.length);
    }
}
