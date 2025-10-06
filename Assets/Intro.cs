using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Next());
    }

    IEnumerator Next()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Petr");
    }
}
