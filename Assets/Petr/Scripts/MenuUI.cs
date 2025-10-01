using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuUI : MonoBehaviour
{

    [SerializeField] GameObject panel;
    [SerializeField] GameObject panelSettings;
    [SerializeField] TMP_Text countVFX;
    [SerializeField] TMP_Text countMusic;
    [SerializeField] TMP_Text countMaster;

    int VFX, Music, Master;

    int firsttime;

    private void Start()
    {
        firsttime = PlayerPrefs.GetInt("ft");
        if (firsttime != 1) 
        {
            PlayerPrefs.SetInt("VFX", 50);
            PlayerPrefs.SetInt("Master", 50);
            PlayerPrefs.SetInt("Music", 50);
            PlayerPrefs.SetInt("ft", 1);
        }

        VFX = PlayerPrefs.GetInt("VFX");
        Music = PlayerPrefs.GetInt("Music");
        Master = PlayerPrefs.GetInt("Master");
    }

    private void Update()
    {
        countMaster.text = Master.ToString();
        countVFX.text = VFX.ToString();
        countMusic.text = Music.ToString();
    }

    public void Play()
    {
        SceneManager.LoadScene("MainMenu");
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
        if(type == "VFX")
        {
            VFX = Mathf.Clamp(VFX + 10, 0, 100);
            PlayerPrefs.SetInt("VFX", VFX);
        }
        if(type == "Music")
        {
            Music = Mathf.Clamp(Music + 10, 0, 100);
            PlayerPrefs.SetInt("Music", Music);
        }
        if (type == "Master")
        {
            Master = Mathf.Clamp(Master + 10, 0, 100);
            PlayerPrefs.SetInt("Master", Master);
        }
    }

    public void AudModifMINUS(string type)
    {
        if (type == "VFX")
        {
            VFX = Mathf.Clamp(VFX-10, 0, 100);
            PlayerPrefs.SetInt("VFX", VFX);
        }
        if (type == "Music")
        {
            Music = Mathf.Clamp(Music - 10, 0, 100);
            PlayerPrefs.SetInt("Music", Music);
        }
        if (type == "Master")
        {
            Master = Mathf.Clamp(Master - 10, 0, 100);
            PlayerPrefs.SetInt("Master", Master);
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
