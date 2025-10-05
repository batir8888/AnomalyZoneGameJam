using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Door : MonoBehaviour
{
    private bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        // ���������, ��� �������� ������ � ��� �����
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
        // ���� ����� ������ � ������ ������� E
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("�����! ������� E ������ � ���������� ����.");
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Hub")) SceneManager.LoadScene("MainScene1");
            else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainScene1")) SceneManager.LoadScene("Hub");
        }
    }
}
