using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TubeManager : MonoBehaviour
{
    public static TubeManager Instance { get; private set; }

    [SerializeField] private TubeController tubePrefab;

    private List<TubeController> tubes = new List<TubeController>();
    private TubeController selectedTube;
    private bool canPour = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeTubes(int tubeCount)
    {
        Debug.Log($"Tube Count: {tubeCount}");
        //reset tubes
        foreach (var tube in tubes) Destroy(tube.gameObject);
        tubes.Clear();
        //create new tubes
        var tubesContainerPrefab = Resources.Load<GameObject>($"{tubeCount} Tubes Container");
        Transform[] tubesPositions = Instantiate(tubesContainerPrefab, Vector3.zero, Quaternion.identity).GetComponentsInChildren<Transform>();
        for (int i = 1; i <= tubeCount; i++)
        {
            tubes.Add(Instantiate(tubePrefab, tubesPositions[i].position, Quaternion.identity));
        }
    }

    private void Update()
    {
        if (canPour && Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            OnTubeClicked(hit.collider.GetComponent<TubeController>());
        }
        else if (selectedTube != null)
        {
            DeselectTube();
        }
    }

    private void RestartGame(TubeLiquidModel level)
    {
        int tubeCount = level.GetTubeCount(GameManager.Instance.Level);
        if (tubeCount != tubes.Count)
        {
            InitializeTubes(tubeCount);
        }
        for (int i = 0; i < tubeCount; i++)
        {
            var tubeColors = level.GetColors(GameManager.Instance.Level, i);
            tubes[i].SetupTube(tubeColors);
        }
    }

    public async void OnTubeClicked(TubeController tubeController)
    {
        if (selectedTube == null)
        {
            HandleFirstTubeSelection(tubeController);
        }
        else if (selectedTube == tubeController)
        {
            DeselectTube();
        }
        else
        {
            await HandleSecondTubeSelection(tubeController);
        }
    }

    private void HandleFirstTubeSelection(TubeController tubeController)
    {
        if (tubeController.IsEmpty)
        {
            tubeController.ShakeTube();
        }
        else
        {
            selectedTube = tubeController;
            selectedTube.OnSelectTube();
        }
    }

    private async Task HandleSecondTubeSelection(TubeController tubeController)
    {
        if (CanPour(selectedTube, tubeController))
        {
            canPour = false;
            int direction = selectedTube.transform.position.x > tubeController.transform.position.x ? -1 : 1;
            await selectedTube.PourLiquid(tubeController, direction);
            selectedTube = null;
            CheckWinCondition(tubeController);
            canPour = true;
        }
        else
        {
            selectedTube.ShakeTube();
        }
    }

    private void DeselectTube()
    {
        selectedTube.OnDeSelectTube();
        selectedTube = null;
    }

    private void CheckWinCondition(TubeController tubeController)
    {
        int tubeFilled = 0;
        int tubeEmpty = 0;
        foreach (var tube in tubes)
        {
            if (tube.IsEmpty) tubeEmpty++;
            if (tube.IsFull) tubeFilled++;
        }
        if (tubeController.IsFull && (tubeFilled + tubeEmpty) == tubes.Count)
        {
            GameManager.Instance.GameWin();
        }
    }

    private bool CanPour(TubeController fromTube, TubeController toTube)
    {
        return toTube.IsEmpty || (fromTube.TopColor == toTube.TopColor && fromTube.TopColorLevelCount <= toTube.TopEmptyLevelCount);
    }

    private void OnEnable() => GameManager.OnRestartGame += RestartGame;
    private void OnDisable() => GameManager.OnRestartGame -= RestartGame;
}