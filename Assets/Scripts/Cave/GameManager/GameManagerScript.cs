﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    #region Singleton
    public static GameManagerScript instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More active bars found!");
            return;
        }
        instance = this;
    }

    #endregion

    // Start is called before the first frame update
    public GameObject playerRef;
    public GameObject playerPrefab;

    public bool dungeonReadyForPlayer = false;
    public bool forestReadyForPlayer = false;

    public bool dungeonNeedsRegeneration = false;
    public bool forestNeedsRegeneration = false;

    public bool dungeonInUse = false;
    public bool forestInUse = false;

    public bool movePlayer = false;

    public GameObject forestMapRef;
    public GameObject dungeonMapRef;

    public GameObject latestPlayerEntryPoint;
    public GameObject latestGeneratedEnvironment;


    public void Reset()
    {
        dungeonInUse = false;
        dungeonNeedsRegeneration = false;
        dungeonReadyForPlayer = false;

        forestInUse = false;
        forestNeedsRegeneration = false;
        forestReadyForPlayer = false;
    }
}
