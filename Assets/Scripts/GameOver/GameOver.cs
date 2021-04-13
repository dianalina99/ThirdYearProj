using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnRestartClick()
    {
        SceneManager.LoadScene("MainLevel");
    }

    public void OnExitClick()
    {
        Application.Quit();
    }
}
