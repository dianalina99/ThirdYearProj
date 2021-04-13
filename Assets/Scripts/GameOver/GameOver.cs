using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject instructions;

    // Start is called before the first frame update
    public void OnRestartClick()
    {
        SceneManager.LoadScene("MainLevel");
    }

    public void OnExitClick()
    {
        Application.Quit();
    }

    public void OnInstructionsClick()
    {
        instructions.SetActive(true);
    }
}
