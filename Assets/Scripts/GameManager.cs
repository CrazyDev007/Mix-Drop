using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action<TubeLiquidModel> OnRestartGame;

    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Color[] colors;
    [SerializeField] private TubeModel[] tubeData;
    [SerializeField] private float timeToMove = 1f;
    [SerializeField] private float timeToRotate = 1f;
    [SerializeField] private float tubeUpOffset = 1f;

    public int Level { get; private set; }
    private string levelName;
    private TubeLiquidModel TubeLiquidModelEasy { get; set; }
    private TubeLiquidModel TubeLiquidModelMedium { get; set; }
    private TubeLiquidModel TubeLiquidModelHard { get; set; }

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

    private TubeLiquidModel GetLevel()
    {
        var hardness = PlayerPrefs.GetInt("Hardness", 0); //Random.Range(0, 3);
        Level = PlayerPrefs.GetInt("Level", 1); //Random.Range(0, 10000) + 1;
        //
        switch (hardness)
        {
            case 0:
                levelName = "Easy";
                TubeLiquidModelEasy =
                    JsonUtility.FromJson<TubeLiquidModel>(Resources.Load<TextAsset>("levels-easy").text);
                return TubeLiquidModelEasy;
            case 1:
                levelName = "Medium";
                TubeLiquidModelMedium =
                    JsonUtility.FromJson<TubeLiquidModel>(Resources.Load<TextAsset>("levels-normal").text);
                return TubeLiquidModelMedium;
            case 2:
                levelName = "Hard";
                TubeLiquidModelHard =
                    JsonUtility.FromJson<TubeLiquidModel>(Resources.Load<TextAsset>("levels-hard").text);
                return TubeLiquidModelHard;
            default:
                TubeLiquidModelEasy =
                    JsonUtility.FromJson<TubeLiquidModel>(Resources.Load<TextAsset>("levels-easy").text);
                return TubeLiquidModelEasy;
        }
    }

    private void Start()
    {
        RestartGame();
    }

    public void GameWin()
    {
        PlayerPrefs.SetInt("Level", Level + 1);
        RestartGame();
    }

    private void RestartGame()
    {
        OnRestartGame?.Invoke(GetLevel());
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
    public int GetTubeCount(int level) => levels[level].map.Count;
    public List<int> GetColors(int level, int tubeNo) => levels[level].map[tubeNo].values;
}

[Serializable]
public class TubeLiquidData
{
    public int no;
    public List<TubeLiquidColorData> map;
}

[Serializable]
public class TubeLiquidColorData
{
    public List<int> values;
}