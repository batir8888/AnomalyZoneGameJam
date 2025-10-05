using UnityEngine;
using UnityEngine.Audio;

public class TestScript : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    private void Start()
    {
        audioMixer.SetFloat("sfx", Mathf.Log10(Mathf.Max(PlayerPrefs.GetFloat("Master") / 100, 0.0001f)) * 20);
        audioMixer.SetFloat("Music", Mathf.Log10(Mathf.Max(PlayerPrefs.GetFloat("Music") / 100, 0.0001f)) * 20);
        Debug.Log(Mathf.Log10(Mathf.Max(PlayerPrefs.GetFloat("Master") / 100, 0.0001f)) * 20);
    }
}