using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Door : MonoBehaviour
{
    private bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что вошедший объект — это игрок
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    private void Update()
    {
        // Если игрок внутри и нажата клавиша E
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Успех! Клавиша E нажата в триггерной зоне.");
            SceneManager.LoadScene("MainScene1");
        }
    }
}
