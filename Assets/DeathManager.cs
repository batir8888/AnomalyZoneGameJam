using System.Collections;
using Batyr.Scripts;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class DeathManager : MonoBehaviour
{
    private Volume _volume;
    [SerializeField] private TimerUI timerUI;
    [SerializeField] private AudioClip audioClip;
    
    private Vignette _vignette;
    private AudioSource _audioSource;
    private bool _isClosely;
    
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _volume = GetComponent<Volume>();
        
        if (_volume && _volume.profile)
        {
            if (_volume.profile.TryGet(out Vignette vignette))
            {
                _vignette = vignette;
            }
        }
    }

    private void Update()
    {
        if (!_vignette || !timerUI) return;
    
        if (!_isClosely && timerUI.timeRemaining < 60f)
        {
            OnTimerCloselyToStop();
        }
        if (!timerUI.timerIsRunning)
        {
            OnTimerStop();
        }
    }

    [ContextMenu("Close")]
    private void OnTimerCloselyToStop()
    {
        _isClosely = true;
        StartCoroutine(CloselyToStop());
    }
    
    [ContextMenu("Stop")]
    private void OnTimerStop()
    {
        StartCoroutine(Death());
    }

    IEnumerator CloselyToStop()
    {
        _audioSource.PlayOneShot(audioClip);
        _vignette.color.value = Color.red;
        while (_isClosely)
        {
            float intensity = Mathf.Lerp(0.1f, 1f, 
                (Mathf.Sin(Time.time * 5) + 1f) / 2f);
            _vignette.intensity.value = intensity;
            yield return null;
        }
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(2f);
        Inventory.Instance.artifacts.Clear();
        SaveLoadSystem.Instance.DeleteAll();
        CraftBootstrap.Instance.Regenerate();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Petr");
    }
}
