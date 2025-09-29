using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private TMP_Text audioClipName;
    [SerializeField] private MusicPlaylist playlist;
    
    private AudioSource _audioSource;
    private Canvas _canvas;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _canvas = GetComponentInChildren<Canvas>();
        _canvas.gameObject.SetActive(false);
    }

    private void Start()
    {
        playButton.onClick.AddListener(OnPlay);
        stopButton.onClick.AddListener(OnStop);
        nextButton.onClick.AddListener(OnNext);
        prevButton.onClick.AddListener(OnPrev);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Tab)) return;
        _canvas.gameObject.SetActive(!_canvas.gameObject.activeSelf);
        Cursor.visible = _canvas.gameObject.activeSelf;
        Cursor.lockState = _canvas.gameObject.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void OnPlay()
    {
        _audioSource.Play();
        audioClipName.text = _audioSource.clip.name;
    }
    
    private void OnStop()
    {
        _audioSource.Stop();
    }
    
    private void OnNext()
    {
        _audioSource.clip = playlist.GetNextClip();
        _audioSource.Play();
        audioClipName.text = _audioSource.clip.name;
    }

    private void OnPrev()
    {
        _audioSource.clip = playlist.GetPreviousClip();
        _audioSource.Play();
        audioClipName.text = _audioSource.clip.name;
    }
    
    private void OnDestroy()
    {
        playButton.onClick.RemoveAllListeners();
        stopButton.onClick.RemoveAllListeners();
        nextButton.onClick.RemoveAllListeners();
        prevButton.onClick.RemoveAllListeners();
    }
}
