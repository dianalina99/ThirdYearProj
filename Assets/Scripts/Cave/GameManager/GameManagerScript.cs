using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    #region Singleton
    public static GameManagerScript instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More game managers found!!");
            return;
        }
        instance = this;
    }

    #endregion

    // Start is called before the first frame update
    public GameObject playerRef;
    public GameObject playerPrefab;
    public GameObject keyPrefab;

    public bool dungeonReadyForPlayer = false;
    public bool forestReadyForPlayer = false;

    public bool dungeonNeedsRegeneration = false;
    public bool forestNeedsRegeneration = false;

    public bool dungeonInUse = false;
    public bool forestInUse = false;

    public GameObject forestMapRef;
    public GameObject dungeonMapRef;

    public GameObject latestPlayerEntryPoint;
    public GameObject latestGeneratedEnvironmentDungeon;

    public bool playerIsCurrentlyTeleporting = false;

    public GameObject minimapRef;
    public Camera minimapCamera;

    public ForestGrid currentForestGrid;
    public ForestGrid previousForestGrid;


    #region Enemies Variables
    public AstarPath pathFinderAI;
    public int NoOfLockedDoors = 0;
    public int NoOfAvailableKeys = 0;
    #endregion

    private bool menuIsLoaded = false;

    public void Reset()
    {
        dungeonInUse = false;
        dungeonNeedsRegeneration = false;
        dungeonReadyForPlayer = false;

        forestInUse = false;
        forestNeedsRegeneration = false;
        forestReadyForPlayer = false;

       // playerIsCurrentlyTeleporting = false;
    }

    private void Update()
    {
        bool escIsUsedForExitingChest = this.playerRef.GetComponent<PlayerMovementScript>().focus != null;
        if(Input.GetKeyDown(KeyCode.Escape) && !escIsUsedForExitingChest  && !menuIsLoaded)
        {
            SceneManager.LoadSceneAsync("GameMenu", LoadSceneMode.Additive);
            this.menuIsLoaded = true;
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && !escIsUsedForExitingChest && menuIsLoaded)
        {
            SceneManager.UnloadSceneAsync("GameMenu");
            this.menuIsLoaded = false;
        }
    }


}
