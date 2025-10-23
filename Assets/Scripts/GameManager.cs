using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine.SceneManagement;

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
        Level = PlayerPrefs.GetInt("ActiveLevel", 1);
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
        gamePlayScreenUIref.OnLevelFailed();
    }
    private void SetupTwists() //Will work on this later
    {
        //TubeManager.Instance.SetLockedTubes(currentLevelData.lockedTubes);
        //TubeManager.Instance.SetAvailableSwaps(currentLevelData.availableSwaps);
    }
    public void GameWin()
    {
        var completedLevel = PlayerPrefs.GetInt("CompletedLevels", 0);
        if (completedLevel <= Level)
            PlayerPrefs.SetInt("CompletedLevels", Level);
        PlayerPrefs.SetInt("ActiveLevel", Level + 1);
        
        // Calculate and save stars
        int starsEarned = CalculateStars();
        SaveStarsForLevel(Level, starsEarned);
        
        RestartGame();
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
    
    private void SaveStarsForLevel(int level, int stars)
    {
        // Get existing stars for this level
        int existingStars = PlayerPrefs.GetInt($"Level{level}Stars", 0);
        
        // Only update if new stars are better
        if (stars > existingStars)
        {
            PlayerPrefs.SetInt($"Level{level}Stars", stars);
            
            // Update total stars
            int totalStars = PlayerPrefs.GetInt("TotalStars", 0);
            totalStars += (stars - existingStars); // Add the difference
            PlayerPrefs.SetInt("TotalStars", totalStars);
            
            PlayerPrefs.Save(); // Ensure data is saved immediately
            
            Debug.Log($"Level {level} completed with {stars} stars! Total stars: {totalStars}");
        }
        else
        {
            Debug.Log($"Level {level} completed with {stars} stars (no improvement from {existingStars})");
        }
    }

    internal void RestartGame()
    {
        // Ensure Level property is up to date before loading level data
        Level = PlayerPrefs.GetInt("ActiveLevel", 1);
        
        var model = GetLevelData();      // Load level & set currentLevelData
        SetupLevelRules();           // Setup timer & move limit
        SetupTwists();

        OnRestartGame?.Invoke(model);
        UpdateLevelText();
    }

    private void UpdateLevelText() => gamePlayScreenUIref.UpdateLevel(Level);
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