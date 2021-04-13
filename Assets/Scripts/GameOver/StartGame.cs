using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public GameObject firstInstructionPage;
    public GameObject secondInstructionPage;
    public GameObject thirdInstructionPage;

    public void OnStartClick()
    {
        SceneManager.LoadScene("MainLevel");
    }

    public void OnFirstNextClick()
    {
        firstInstructionPage.SetActive(false);
        secondInstructionPage.SetActive(true);
    }

    public void OnSecondNextClick()
    {
        secondInstructionPage.SetActive(false);
        thirdInstructionPage.SetActive(true);
    }
    public void OnFirstBackClick()
    {
        secondInstructionPage.SetActive(false);
        firstInstructionPage.SetActive(true);
    }

    public void OnSecondBackClick()
    {
        thirdInstructionPage.SetActive(false);
        secondInstructionPage.SetActive(true);
    }


}
