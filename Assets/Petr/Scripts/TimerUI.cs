using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TimerUI : MonoBehaviour
{
    public TMP_Text tmpro;
    public float timeRemaining;
    public bool timerIsRunning = false;

    private void Start()
    {
        timeRemaining = timeRemaining * 60;
        timerIsRunning = true;
    }
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        tmpro.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
