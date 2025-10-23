using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine.SceneManagement;
using MixDrop.Data;

[DefaultExecutionOrder(0)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action<TubeLiquidModel> OnRestartGame;

    public GameplayScreen gamePlayScreenUIref;
    [SerializeField] private Color[] colors;
    [SerializeField] private TubeModel[] tubeData;
    [SerializeField] private float timeToMove = 1f;
    [SerializeField] private float timeToRotate = 1f;
    [SerializeField] private float tubeUpOffset = 1f;
    
    [Header("Text File Storage")]
    [Tooltip("Reference to the text file storage system")]
    [SerializeField] private TextFileGameDataStorage textFileStorage;
    
    public int Level { get; private set; }
    private string levelName;

    public Color[] Colors => colors;
    public TubeModel[] TubeData => tubeData;
    public float TimeToMove => timeToMove;
    public float TimeToRotate => timeToRotate;
    public float TubeUpOffset => tubeUpOffset;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGame()
    {
        // Use text file storage if available, otherwise default to level 1
        if (textFileStorage != null && textFileStorage.CurrentLevel > 0)
        {
            Level = textFileStorage.CurrentLevel;
        }
        else
        {
            Level = 1;
        }
        UpdateLevelText();
    }

    private TubeLiquidData currentLevelData;
    private TubeLiquidModel GetLevelData()
    {
        // Level property is already set in RestartGame(), so we don't need to set it again here
        //
        var TubeLiquidModel = JsonUtility.FromJson<TubeLiquidModel>(Resources.Load<TextAsset>("color_sort_220_levels").text);
        //Debug.Log("==> Level count : "+ TubeLiquidModel.TotalLevels);
        currentLevelData = TubeLiquidModel.levels[Level - 1]; // Load current level
        return TubeLiquidModel;

    }

    private async void Start()
    {
        // Initialize Unity Services
        await UnityServices.InitializeAsync();
        
        // Check if user is authenticated
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            // Load login scene if not authenticated
            //SceneManager.LoadScene("Main");
            //return;
        }
        
        // Initialize text file storage if available
        if (textFileStorage != null)
        {
            textFileStorage.Initialize();
            
            // Sync current level with text file storage
            Level = Mathf.Max(Level, textFileStorage.CurrentLevel);
        }
        
        // Proceed with game initialization
        RestartGame();
    }
    public int RemainingMoves = 0;
    public float LevelTime = 0;
    private int movesUsed = 0;
    private float timeUsed = 0;
    private Coroutine timerCoroutine;

    private void SetupLevelRules()
    {
        RemainingMoves = currentLevelData.maxMoves > 0 ? currentLevelData.maxMoves : 50000;
        LevelTime = currentLevelData.timeLimit;
        movesUsed = 0; // Reset moves counter
        timeUsed = 0; // Reset time counter
        var _movesLeft = RemainingMoves >= 50000 ? "Unlimited Moves" : $"{RemainingMoves} Moves left";
        gamePlayScreenUIref.UpdateMoves(_movesLeft);
        if (LevelTime > 0)
        {
            if (timerCoroutine != null)
                StopCoroutine(timerCoroutine);
            
            timerCoroutine = StartCoroutine(LevelTimer());
        }
        else
        {
            gamePlayScreenUIref.UpdateTimer("Unlimited Time");
        }
    }

    private IEnumerator LevelTimer()
    {
        float timeLeft = LevelTime;
        while (timeLeft > 0)
        {
            var _timeLeft = $"{timeLeft} Sec Left";
            gamePlayScreenUIref.UpdateTimer(_timeLeft);
            yield return new WaitForSeconds(1f);
            timeLeft -= 1f;
            timeUsed += 1f; // Track time used
        }
        OnLevelFailed();
    }
    
    // Call this method whenever a move is made
    public void OnMoveMade()
    {
        movesUsed++;
        // Update UI to show remaining moves
        if (RemainingMoves < 50000)
        {
            int movesLeft = RemainingMoves - movesUsed;
            if (movesLeft < 0)
            {
                OnLevelFailed();
                return;
            }
            gamePlayScreenUIref.UpdateMoves($"{movesLeft} Moves left");
        }
    }
    public void OnLevelFailed()
    {
        //do stuff here on level failed
        Debug.Log("Level Failed! Try Again.");
        
        // Save attempt data even when level fails
        if (textFileStorage != null)
        {
            // Save attempt data for failed level
            // We don't have a SaveLevelAttempt method, so we'll just increment the attempt count
            int currentAttempts = textFileStorage.GetLevelAttempts(Level);
            textFileStorage.CompleteLevel(Level, 0, timeUsed, currentAttempts + 1);
            // Force immediate save to ensure data is written
            textFileStorage.ForceSaveGameData();
        }
        
        gamePlayScreenUIref.OnLevelFailed();
    }
    private void SetupTwists() //Will work on this later
    {
        //TubeManager.Instance.SetLockedTubes(currentLevelData.lockedTubes);
        //TubeManager.Instance.SetAvailableSwaps(currentLevelData.availableSwaps);
    }
    public void GameWin()
    {
        // Calculate stars earned
        int starsEarned = CalculateStars();
        
        // Save to text file storage if available
        if (textFileStorage != null)
        {
            textFileStorage.CompleteLevel(Level, starsEarned, timeUsed, movesUsed);
            textFileStorage.SetCurrentLevel(Level + 1);
            // Force immediate save to ensure data is written
            textFileStorage.ForceSaveGameData();
        }
        
        // Don't restart immediately - wait for user action
        // This allows the level completed screen to be shown first
        EventManager.OnLevelComplteted?.Invoke();
    }
    
    private int CalculateStars()
    {
        int stars = 1; // Minimum 1 star for completing the level
        
        // Check if level has move limit
        if (currentLevelData.maxMoves > 0 && currentLevelData.maxMoves < 50000)
        {
            // Calculate star rating based on moves used
            float moveRatio = (float)movesUsed / currentLevelData.maxMoves;
            if (moveRatio <= 0.5f) // Used 50% or less of allowed moves
                stars = 3;
            else if (moveRatio <= 0.75f) // Used 75% or less of allowed moves
                stars = 2;
        }
        else if (currentLevelData.timeLimit > 0)
        {
            // Calculate star rating based on time used
            float timeRatio = timeUsed / currentLevelData.timeLimit;
            if (timeRatio <= 0.5f) // Used 50% or less of allowed time
                stars = 3;
            else if (timeRatio <= 0.75f) // Used 75% or less of allowed time
                stars = 2;
        }
        else
        {
            // If no move or time limit, give 3 stars by default
            stars = 3;
        }
        
        return stars;
    }

    internal void RestartGame()
    {
        // Ensure Level property is up to date before loading level data
        if (textFileStorage != null)
        {
            Level = textFileStorage.CurrentLevel;
        }
        else
        {
            Level = 1; // Default to level 1 if text file storage is not available
        }
        
        var model = GetLevelData();      // Load level & set currentLevelData
        SetupLevelRules();           // Setup timer & move limit
        SetupTwists();

        OnRestartGame?.Invoke(model);
        UpdateLevelText();
    }

    private void UpdateLevelText() => gamePlayScreenUIref.UpdateLevel(Level);
    
    #region Public API for Level Data
    
    /// <summary>
    /// Gets the current level number
    /// </summary>
    /// <returns>Current level number</returns>
    public int GetCurrentLevel()
    {
        return Level;
    }
    
    /// <summary>
    /// Gets the stars earned for a specific level
    /// </summary>
    /// <param name="level">Level number</param>
    /// <returns>Stars earned (0-3)</returns>
    public int GetLevelStars(int level)
    {
        if (textFileStorage != null)
        {
            return textFileStorage.GetLevelStars(level);
        }
        return 0;
    }
    
    /// <summary>
    /// Gets the best completion time for a specific level
    /// </summary>
    /// <param name="level">Level number</param>
    /// <returns>Best time in seconds, or 0 if not completed</returns>
    public float GetLevelBestTime(int level)
    {
        if (textFileStorage != null)
        {
            return textFileStorage.GetLevelBestTime(level);
        }
        return 0f;
    }
    
    /// <summary>
    /// Gets the number of attempts for a specific level
    /// </summary>
    /// <param name="level">Level number</param>
    /// <returns>Number of attempts</returns>
    public int GetLevelAttempts(int level)
    {
        if (textFileStorage != null)
        {
            return textFileStorage.GetLevelAttempts(level);
        }
        return 0;
    }
    
    /// <summary>
    /// Checks if a level is completed
    /// </summary>
    /// <param name="level">Level number</param>
    /// <returns>True if the level is completed</returns>
    public bool IsLevelCompleted(int level)
    {
        if (textFileStorage != null)
        {
            return textFileStorage.IsLevelCompleted(level);
        }
        return false;
    }
    
    /// <summary>
    /// Proceeds to the next level
    /// </summary>
    public void ProceedToNextLevel()
    {
        RestartGame();
    }
    
    #endregion
}

[Serializable]
public struct TubeModel
{
    public float tubeRotationAngle;
    public float liquidRotationAngle;
    public float fillAmount;
}

[Serializable]
public class TubeLiquidModel
{
    public List<TubeLiquidData> levels;
    public int TotalLevels => levels.Count;
    public int GetTubeCount(int level) => levels[level].tubes.Count;
    public List<int> GetColors(int level, int tubeNo) => levels[level].tubes[tubeNo].values;
}

[Serializable]
public class TubeLiquidData
{
    public int level;
    public List<TubeLiquidColorData> tubes;
    public int emptyTubes;
    public int maxMoves;
    public float timeLimit;
    public List<int> lockedTubes = new List<int>(); // default empty
    public int availableSwaps;
    public List<string> twists = new List<string>(); // default empty
}

[Serializable]
public class TubeLiquidColorData
{
    public List<int> values;
}