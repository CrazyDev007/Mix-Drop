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
        Level = PlayerPrefs.GetInt("ActiveLevel", 1);
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
    private Coroutine timerCoroutine;

    private void SetupLevelRules()
    {
        RemainingMoves = currentLevelData.maxMoves > 0 ? currentLevelData.maxMoves : 50000;
        LevelTime = currentLevelData.timeLimit;
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
        }
        OnLevelFailed();
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
        RestartGame();
    }

    internal void RestartGame()
    {
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