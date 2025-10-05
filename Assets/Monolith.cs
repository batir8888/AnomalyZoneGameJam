using System.Collections;
using System.Collections.Generic;
using Batyr.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Monolith : MonoBehaviour
{
    [SerializeField] private List<AudioClip> greetings;
    [SerializeField] private List<AudioClip> weaks;
    [SerializeField] private AudioClip final;
    
    private AudioSource _audioSource;
    private Camera _camera;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _camera = Camera.main;
    }

    private void Start()
    {
        StartCoroutine(Greeting());
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + Time.deltaTime * Random.Range(4, 20), 0);
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance: 5f))
            {
                if (hit.collider.TryGetComponent<Monolith>(out _))
                {
                    if (Inventory.Instance.HasQuestArtifacts()) StartCoroutine(Final());
                    else
                    {
                        if (!_audioSource.isPlaying) _audioSource.PlayOneShot(weaks[Random.Range(0, weaks.Count)]);
                    }
                }
            }
        }
    }

    IEnumerator Final()
    {
        _audioSource.Stop();
        _audioSource.PlayOneShot(final);
        yield return new WaitForSeconds(final.length);
        Inventory.Instance.artifacts.Clear();
        SaveLoadSystem.Instance.Delete("inventory");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Petr");
    }

    IEnumerator Greeting()
    {
        if (Inventory.Instance.HasQuestArtifacts()) yield break;
        yield return new WaitForSeconds(30);
        if (!_audioSource.isPlaying)
        {
            var audioClip = greetings[Random.Range(0, greetings.Count)];
            _audioSource.PlayOneShot(audioClip);
        }
        StartCoroutine(Greeting());
    }
}
