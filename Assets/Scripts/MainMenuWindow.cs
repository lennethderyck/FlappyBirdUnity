using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuWindow : MonoBehaviour
{
    public void quitGame()
    {
        Application.Quit();
    }

    public void playGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
