using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action<TubeLiquidModel> OnRestartGame;

    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI moveCountText;
    [SerializeField] private Color[] colors;
    [SerializeField] private TubeModel[] tubeData;
    [SerializeField] private float timeToMove = 1f;
    [SerializeField] private float timeToRotate = 1f;
    [SerializeField] private float tubeUpOffset = 1f;

    public int Level { get; private set; }
    private string levelName;
    //private TubeLiquidModel TubeLiquidModelEasy { get; set; }
    //private TubeLiquidModel TubeLiquidModelMedium { get; set; }
    //private TubeLiquidModel TubeLiquidModelHard { get; set; }

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
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGame()
    {
        UpdateLevelText();
    }

    //private TubeLiquidModel GetLevel()
    //{
    //    var hardness = PlayerPrefs.GetInt("Hardness", 0); //Random.Range(0, 3);
    //    Level = PlayerPrefs.GetInt("ActiveLevel", 1); //Random.Range(0, 10000) + 1;
    //    //
    //    switch (hardness)
    //    {
    //        case 0:
    //            levelName = "Easy";
    //            TubeLiquidModelEasy =
    //                JsonUtility.FromJson<TubeLiquidModel>(Resources.Load<TextAsset>("levels-easy").text);
    //            return TubeLiquidModelEasy;
    //        case 1:
    //            levelName = "Medium";
    //            TubeLiquidModelMedium =
    //                JsonUtility.FromJson<TubeLiquidModel>(Resources.Load<TextAsset>("levels-normal").text);
    //            return TubeLiquidModelMedium;
    //        case 2:
    //            levelName = "Hard";
    //            TubeLiquidModelHard =
    //                JsonUtility.FromJson<TubeLiquidModel>(Resources.Load<TextAsset>("levels-hard").text);
    //            return TubeLiquidModelHard;
    //        default:
    //            TubeLiquidModelEasy =
    //                JsonUtility.FromJson<TubeLiquidModel>(Resources.Load<TextAsset>("levels-easy").text);
    //            return TubeLiquidModelEasy;
    //    }
    //}

    private TubeLiquidData currentLevelData;
    private TubeLiquidModel GetLevelData()
    {
        Level = PlayerPrefs.GetInt("ActiveLevel", 1);
        //
        var TubeLiquidModel = JsonUtility.FromJson<TubeLiquidModel>(Resources.Load<TextAsset>("color_sort_1000_levels").text);
        Debug.Log("==> Level count : "+ TubeLiquidModel.TotalLevels);
        currentLevelData = TubeLiquidModel.levels[Level - 1]; // Load current level
        return TubeLiquidModel;

    }

    private void Start()
    {
        RestartGame();
    }
    public int RemainingMoves = 0;
    public float LevelTime = 0;
    private Coroutine timerCoroutine;

    private void SetupLevelRules()
    {
        RemainingMoves = currentLevelData.maxMoves > 0 ? currentLevelData.maxMoves : 50000;
        LevelTime = currentLevelData.timeLimit;
        if (LevelTime > 0)
        {
            if (timerCoroutine != null) StopCoroutine(timerCoroutine);
            timerCoroutine = StartCoroutine(LevelTimer());
        }
    }

    private IEnumerator LevelTimer()
    {
        float timeLeft = LevelTime;
        while (timeLeft > 0)
        {
            timerText.text = $"{Mathf.CeilToInt(timeLeft)} Sec Left";
            yield return new WaitForSeconds(1f);
            timeLeft -= 1f;
        }

        OnLevelFailed();
    }
    public void OnLevelFailed()
    {
        //do stuff here on level failed
        Debug.Log("Level Failed! Try Again.");
    }
    private void SetupTwists()
    {
        TubeManager.Instance.SetLockedTubes(currentLevelData.lockedTubes);
        TubeManager.Instance.SetAvailableSwaps(currentLevelData.availableSwaps);
    }
    public void GameWin()
    {
        var completedLevel = PlayerPrefs.GetInt("CompletedLevels", 0);
        if (completedLevel <= Level)
            PlayerPrefs.SetInt("CompletedLevels", Level);
        PlayerPrefs.SetInt("ActiveLevel", Level + 1);
        RestartGame();
    }

    private void RestartGame()
    {
        var model = GetLevelData();      // Load level & set currentLevelData
        SetupLevelRules();           // Setup timer & move limit
        SetupTwists();

        OnRestartGame?.Invoke(model);
        UpdateLevelText();
    }

    private void UpdateLevelText() => levelText.text = $"{Level}";
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