using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class MenuUI : MonoBehaviour
{

    [SerializeField] GameObject panel;
    [SerializeField] GameObject panelSettings;
    [SerializeField] TMP_Text countMusic;
    [SerializeField] TMP_Text countMaster;
    [SerializeField] AudioMixer audioMixer;

    float VFX, Music, Master;

    int firsttime;

    private void Start()
    {
        firsttime = PlayerPrefs.GetInt("ft");
        if (firsttime != 1) 
        {
            PlayerPrefs.SetFloat("Master", 50);
            PlayerPrefs.SetFloat("Music", 50);
            PlayerPrefs.SetInt("ft", 1);
        }

        Music = PlayerPrefs.GetFloat("Music");
        Master = PlayerPrefs.GetFloat("Master");

        audioMixer.SetFloat("Music", Mathf.Log10(Mathf.Max(PlayerPrefs.GetFloat("Music") / 100, 0.0001f)) * 20);
        audioMixer.SetFloat("sfx", Mathf.Log10(Mathf.Max(PlayerPrefs.GetFloat("Master") / 100, 0.0001f)) * 20);
    }

    private void Update()
    {
        countMaster.text = Master.ToString();
        countMusic.text = Music.ToString();


    }

    public void Play()
    {
        SceneManager.LoadScene("Hub");
    }

    public void SettingsON()
    {
        panel.SetActive(false);
        panelSettings.SetActive(true);
    }
    public void SettingsOFF()
    {
        panelSettings.SetActive(false);
        panel.SetActive(true);
    }

    public void AudModifPLUS(string type)
    {
        if(type == "Music")
        {
            Music = Mathf.Clamp(Music + 10, 0, 100);
            PlayerPrefs.SetFloat("Music", Music);
            audioMixer.SetFloat("Music", Mathf.Log10(Mathf.Max(PlayerPrefs.GetFloat("Music") / 100, 0.0001f)) * 20);
        }
        if (type == "Master")
        {
            Master = Mathf.Clamp(Master + 10, 0, 100);
            PlayerPrefs.SetFloat("Master", Master);
            audioMixer.SetFloat("sfx", Mathf.Log10(Mathf.Max(PlayerPrefs.GetFloat("Master") / 100, 0.0001f)) * 20);

        }
    }

    public void AudModifMINUS(string type)
    {
        if (type == "Music")
        {
            Music = Mathf.Clamp(Music - 10, 0, 100);
            PlayerPrefs.SetFloat("Music", Music);
            audioMixer.SetFloat("Music", Mathf.Log10(Mathf.Max(PlayerPrefs.GetFloat("Music") / 100, 0.0001f)) * 20);


        }
        if (type == "Master")
        {
            Master = Mathf.Clamp(Master - 10, 0, 100);
            PlayerPrefs.SetFloat("Master", Master);
            audioMixer.SetFloat("sfx", Mathf.Log10(Mathf.Max(PlayerPrefs.GetFloat("Master") / 100, 0.0001f)) * 20);
        }
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
