using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicPlaylist", menuName = "MusicPlaylist")]
public class MusicPlaylist : ScriptableObject
{
    [SerializeField] private List<AudioClip> audioClips = new();
    
    private int _currentIndex = -1;

    public AudioClip GetNextClip()
    {
        _currentIndex++;
        return audioClips[_currentIndex % audioClips.Count];
    }

    public AudioClip GetPreviousClip()
    {
        _currentIndex--;
        return audioClips[_currentIndex % audioClips.Count];
    }
}
