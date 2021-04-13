using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void OnStartClick()
    {
        SceneManager.LoadScene("MainLevel");
    }

}
