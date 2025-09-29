using UnityEngine;
using UnityEngine.SceneManagement;

namespace Batyr.Scripts
{
    public class SceneManagerHelper : Singleton<SceneManagerHelper>
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainMenu"))
                    SceneManager.LoadScene("Yarik/New Scene");
                else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Yarik/New Scene"))
                    SceneManager.LoadScene("MainMenu");
            }
        }
    }
}