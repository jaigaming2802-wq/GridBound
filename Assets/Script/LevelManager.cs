using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject homeScreen;
    [SerializeField] private GameObject levelSelect;
    [SerializeField] private GameObject winScreen;

    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private GameObject nextButton;

    private string currentLevel;
    private GameObject currentPlayer;

    // =========================================
    // LEVEL PROGRESS
    // =========================================

    private bool level1Completed = false;
    private bool level2Completed = false;


    // =========================================
    // AWAKE
    // =========================================

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


    // =========================================
    // ENABLE
    // =========================================

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    // =========================================
    // DISABLE
    // =========================================

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    // =========================================
    // START
    // =========================================

    private void Start()
    {
        homeScreen.SetActive(true);
        levelSelect.SetActive(false);
        winScreen.SetActive(false);

        if (nextButton != null)
        {
            nextButton.SetActive(false);
        }

        // Start every new game with Level 2 locked
        level1Completed = false;
        level2Completed = false;
    }


    // =========================================
    // LEVEL SELECT
    // =========================================

    public void OpenLevelSelect()
    {
        homeScreen.SetActive(false);
        levelSelect.SetActive(true);
        winScreen.SetActive(false);
    }


    // =========================================
    // OPEN LEVEL 1
    // =========================================

    public void OpenLevel1()
    {
        LoadLevel("Level01");
    }


    // =========================================
    // OPEN LEVEL 2
    // =========================================

    public void OpenLevel2()
    {
        // Level 1 MUST be completed first
        if (!level1Completed)
        {
            Debug.Log(
                "LEVEL 2 IS LOCKED! COMPLETE LEVEL 1 FIRST."
            );

            return;
        }

        LoadLevel("Level02");
    }


    // =========================================
    // LOAD LEVEL
    // =========================================

    public void LoadLevel(string levelName)
    {
        currentLevel = levelName;

        winScreen.SetActive(false);
        levelSelect.SetActive(false);

        if (nextButton != null)
        {
            nextButton.SetActive(false);
        }

        SceneManager.LoadSceneAsync(
            levelName,
            LoadSceneMode.Additive
        );
    }


    // =========================================
    // SCENE LOADED
    // =========================================

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != currentLevel)
            return;

        SpawnPlayer(scene);
    }


    // =========================================
    // SPAWN PLAYER
    // =========================================

    private void SpawnPlayer(Scene levelScene)
    {
        if (playerPrefab == null)
            return;

        string spawnName;

        if (currentLevel == "Level01")
        {
            spawnName = "PlayerSpawn 1";
        }
        else if (currentLevel == "Level02")
        {
            spawnName = "PlayerSpawn 2";
        }
        else
        {
            return;
        }

        GameObject spawnObject = null;

        foreach (GameObject obj in levelScene.GetRootGameObjects())
        {
            Transform[] transforms =
                obj.GetComponentsInChildren<Transform>(true);

            foreach (Transform t in transforms)
            {
                if (t.name == spawnName)
                {
                    spawnObject = t.gameObject;
                    break;
                }
            }

            if (spawnObject != null)
                break;
        }

        if (spawnObject == null)
        {
            Debug.LogError(
                "Spawn point not found: " + spawnName
            );

            return;
        }

        DestroyCurrentPlayer();

        currentPlayer = Instantiate(
            playerPrefab,
            spawnObject.transform.position,
            spawnObject.transform.rotation
        );

        SceneManager.MoveGameObjectToScene(
            currentPlayer,
            levelScene
        );
    }


    // =========================================
    // DESTROY CURRENT PLAYER
    // =========================================

    private void DestroyCurrentPlayer()
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
            currentPlayer = null;
        }
    }


    // =========================================
    // CHECK LEVEL COMPLETE
    // =========================================

    public void CheckLevelComplete()
    {
        Scene currentScene =
            SceneManager.GetSceneByName(currentLevel);

        if (!currentScene.IsValid() || !currentScene.isLoaded)
            return;

        Goal[] goals = null;

        foreach (GameObject rootObject in currentScene.GetRootGameObjects())
        {
            Goal[] foundGoals =
                rootObject.GetComponentsInChildren<Goal>(true);

            if (foundGoals.Length == 0)
                continue;

            if (goals == null)
            {
                goals = foundGoals;
            }
            else
            {
                Goal[] combined =
                    new Goal[goals.Length + foundGoals.Length];

                goals.CopyTo(combined, 0);
                foundGoals.CopyTo(combined, goals.Length);

                goals = combined;
            }
        }

        if (goals == null || goals.Length == 0)
            return;

        Debug.Log("TOTAL GOALS: " + goals.Length);

        // Check all goals
        foreach (Goal goal in goals)
        {
            if (!goal.IsCompleted())
                return;
        }

        Debug.Log(
            "ALL " + goals.Length + " GOALS COMPLETED!"
        );


        // =========================================
        // LEVEL 1 COMPLETE
        // =========================================

        if (currentLevel == "Level01")
        {
            level1Completed = true;

            Debug.Log("LEVEL 1 COMPLETED!");
            Debug.Log("LEVEL 2 UNLOCKED!");
        }


        // =========================================
        // LEVEL 2 COMPLETE
        // =========================================

        if (currentLevel == "Level02")
        {
            level2Completed = true;

            Debug.Log("LEVEL 2 COMPLETED!");
            Debug.Log("FINAL LEVEL COMPLETED!");
        }


        // Show win screen
        ShowWinScreen();
    }


    // =========================================
    // WIN SCREEN
    // =========================================

    public void ShowWinScreen()
    {
        winScreen.SetActive(true);

        if (nextButton != null)
        {
            // =====================================
            // LEVEL 1
            // =====================================

            if (currentLevel == "Level01")
            {
                nextButton.SetActive(true);
            }

            // =====================================
            // LEVEL 2
            // =====================================

            else if (currentLevel == "Level02")
            {
                nextButton.SetActive(false);
            }
        }
    }


    // =========================================
    // RETRY
    // =========================================

    public void RetryLevel()
    {
        winScreen.SetActive(false);

        if (nextButton != null)
        {
            nextButton.SetActive(false);
        }

        StartCoroutine(RetryLevelRoutine());
    }


    private IEnumerator RetryLevelRoutine()
    {
        DestroyCurrentPlayer();

        AsyncOperation unload =
            SceneManager.UnloadSceneAsync(currentLevel);

        if (unload != null)
        {
            while (!unload.isDone)
            {
                yield return null;
            }
        }

        SceneManager.LoadSceneAsync(
            currentLevel,
            LoadSceneMode.Additive
        );
    }


    // =========================================
    // NEXT LEVEL
    // =========================================

    public void NextLevel()
    {
        // Only Level 1 has Level 2 after it
        if (currentLevel != "Level01")
        {
            Debug.Log("NO NEXT LEVEL!");
            return;
        }

        // Safety check
        if (!level1Completed)
        {
            Debug.Log(
                "CANNOT GO NEXT! COMPLETE LEVEL 1 FIRST."
            );

            return;
        }

        winScreen.SetActive(false);

        if (nextButton != null)
        {
            nextButton.SetActive(false);
        }

        StartCoroutine(NextLevelRoutine());
    }


    private IEnumerator NextLevelRoutine()
    {
        DestroyCurrentPlayer();

        // Unload Level 1
        AsyncOperation unload =
            SceneManager.UnloadSceneAsync("Level01");

        if (unload != null)
        {
            while (!unload.isDone)
            {
                yield return null;
            }
        }

        // Load Level 2
        LoadLevel("Level02");
    }


    // =========================================
    // OPEN MENU / LEVEL SELECT
    // =========================================

    public void OpenMenu()
    {
        winScreen.SetActive(false);

        if (nextButton != null)
        {
            nextButton.SetActive(false);
        }

        StartCoroutine(OpenMenuRoutine());
    }


    private IEnumerator OpenMenuRoutine()
    {
        DestroyCurrentPlayer();

        AsyncOperation unload =
            SceneManager.UnloadSceneAsync(currentLevel);

        if (unload != null)
        {
            while (!unload.isDone)
            {
                yield return null;
            }
        }

        levelSelect.SetActive(true);
        homeScreen.SetActive(false);
    }
}