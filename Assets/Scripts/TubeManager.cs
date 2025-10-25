using System.Collections.Generic;
using System.Threading.Tasks;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class TubeManager : MonoBehaviour
{
    public static TubeManager Instance { get; private set; }

    [SerializeField] private TubeController tubePrefab;
    [SerializeField] private int maxTubes = 11; // Maximum number of tubes allowed

    private TubeController selectedTube;

    private List<Task> tasks = new List<Task>();
    private InputAction clickAction;

    private int availableSwaps;

    public List<TubeController> Tubes { get; } = new List<TubeController>();

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
        // Initialize Input System action for mouse click
        clickAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/leftButton");
        clickAction.performed += ctx => HandleMouseClick(Mouse.current.position.ReadValue());
        clickAction.Enable();
    }

    private void InitializeTubes(int tubeCount)
    {
        Debug.Log($"Tube Count: {tubeCount}");
        //reset tubes
        foreach (var tube in Tubes) Destroy(tube.gameObject);
        Tubes.Clear();
        //create new tubes
        var tubesContainerPrefab = Resources.Load<GameObject>($"{tubeCount} Tubes Container");
        var tubesPositions = Instantiate(tubesContainerPrefab, Vector3.zero, Quaternion.identity).GetComponentsInChildren<Transform>();
        for (int i = 1; i <= tubeCount; i++)
        {
            Tubes.Add(Instantiate(tubePrefab, tubesPositions[i].position, Quaternion.identity));
        }
    }

    public void SetLockedTubes(List<int> lockedTubes)
    {
        for (int i = 0; i < Tubes.Count; i++)
        {
            Tubes[i].IsLocked = lockedTubes.Contains(i);
        }
    }
    public void SetAvailableSwaps(int swaps)
    {
        availableSwaps = swaps;
        // Optional: Update UI
        // UIManager.Instance.UpdateSwapCounter(availableSwaps);
    }
    public bool SwapTopColors(TubeController tubeA, TubeController tubeB)
    {
        if (availableSwaps <= 0) return false;
        if (tubeA.IsEmpty || tubeB.IsEmpty) return false;

        int topA = tubeA.TopColor;
        int topB = tubeB.TopColor;

        tubeA.colors[tubeA.currentTopIndex - 1] = topB;
        tubeB.colors[tubeB.currentTopIndex - 1] = topA;

        tubeA.UpdateTubeMaterial();
        tubeB.UpdateTubeMaterial();

        availableSwaps--;
        //UIManager.Instance.UpdateSwapCounter(availableSwaps); // Optional UI update //need to work on this

        return true;
    }
    // Legacy Update removed: using Input System clickAction instead

    public void SimulateMouseClick(Vector3 mousePositionPixel)
    {
        HandleMouseClick(mousePositionPixel);
    }

    private void HandleMouseClick(Vector3 mousePositionPixel)
    {
        // Check if UI is open, if so, prevent tube interactions
        if (GameManager.Instance != null && GameManager.Instance.IsUIOpen)
        {
            Debug.Log("[TubeManager] UI is open, ignoring tube click");
            return;
        }

        // Play tap sound on tube click
        Vector2 testPos = Camera.main.ScreenToWorldPoint(mousePositionPixel);
        RaycastHit2D testHit = Physics2D.Raycast(testPos, Vector2.zero);
        if (testHit.collider != null)
        {
            AudioManager.Instance.PlayBottlePick();
        }
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mousePositionPixel);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            var tube = hit.collider.GetComponent<TubeController>();
            OnTubeClicked(tube);
        }
        else if (selectedTube != null)
        {
            DeselectTube();
        }
    }

    private void RestartGame(TubeLiquidModel level)
    {

        int tubeCount = level.GetTubeCount(GameManager.Instance.Level - 1);
        if (tubeCount != Tubes.Count)
        {
            InitializeTubes(tubeCount);
        }

        for (int i = 0; i < tubeCount; i++)
        {
            var tubeColors = level.GetColors(GameManager.Instance.Level-1, i);
            Tubes[i].SetupTube(tubeColors);
            //Tubes[i].IsLocked = level.levels[GameManager.Instance.Level-1].lockedTubes.Contains(i); // Add tube lock //tube lock feature temp removed

        }
        availableSwaps = level.levels[GameManager.Instance.Level - 1].availableSwaps; // set available swaps
    }

    public async void OnTubeClicked(TubeController tubeController)
    {
        if (tubeController.IsLocked)
        {
            AudioManager.Instance?.PlayBottlePickPourRestrict();
            tubeController.ShakeTube();
            return;
        }

        if (tubeController.CurrentTubeState == TubeState.Pouring) return;
        if (selectedTube == null && tubeController.CurrentTubeState != TubeState.Filling)
        {
            HandleFirstTubeSelection(tubeController);
        }
        else if (selectedTube == tubeController)
        {
            DeselectTube();
        }
        else
        {
            tasks.Add(HandleSecondTubeSelection(tubeController));
        }
    }

    private void HandleFirstTubeSelection(TubeController tubeController)
    {
        if (tubeController.IsEmpty)
        {
            AudioManager.Instance?.PlayBottlePickPourRestrict();
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
            var tempSelectedTube = selectedTube;
            selectedTube = null;
            var direction = tempSelectedTube.transform.position.x > tubeController.transform.position.x ? -1 : 1;
            AudioManager.Instance?.PlayLiquidPour();
            await tubeController.PourLiquid(tempSelectedTube, direction);
            
            // Notify GameManager that a move was made
            GameManager.Instance.OnMoveMade();
            
            CheckWinCondition(tubeController);
        }
        else
        {
            selectedTube.ShakeTube();
            AudioManager.Instance?.PlayBottlePickPourRestrict();
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
        foreach (var tube in Tubes)
        {
            if (tube.IsEmpty) tubeEmpty++;
            if (tube.IsFull) tubeFilled++;
        }

        if (tubeController.IsFull && (tubeFilled + tubeEmpty) == Tubes.Count)
        {
            GameManager.Instance.GameWin();
        }
    }

    private bool CanPour(TubeController fromTube, TubeController toTube)
    {
        if (toTube.IsLocked) return false;
        return toTube.IsEmpty || (fromTube.TopColor == toTube.TopColor &&
                                  fromTube.TopColorLevelCount <= toTube.TopEmptyLevelCount);
    }

    public void AddEmptyTube()
    {
        if (Tubes.Count >= maxTubes)
        {
            Debug.Log("Maximum number of tubes reached!");
            return;
        }

        // Calculate position for new tube
        var newPosition = Vector3.zero;

        // Create new tube
        var newTube = Instantiate(tubePrefab, newPosition, Quaternion.identity);
        newTube.SetupTube(new List<int>()); // Empty tube
        Tubes.Add(newTube);

        // Update tube container
        UpdateTubeContainer();
    }

    private void UpdateTubeContainer()
    {
        // Destroy existing container if any
        var existingContainer = GameObject.Find($"{Tubes.Count-1} Tubes Container(Clone)");
        if (existingContainer != null)
        {
            Destroy(existingContainer);
        }

        //
        Debug.Log($"{Tubes.Count} Tubes Container");
        //create new tubes
        var tubesContainerPrefab = Resources.Load<GameObject>($"{Tubes.Count} Tubes Container");
        var tubesPositions = Instantiate(tubesContainerPrefab, Vector3.zero, Quaternion.identity).GetComponentsInChildren<Transform>();
        for (var i = 0; i < Tubes.Count; i++)
        {
            Tubes[i].transform.SetPositionAndRotation(tubesPositions[i + 1].position, Quaternion.identity);
            Tubes[i].UpdateTubesInitialPosition();
        }
    }

    private void OnEnable()
    {
        GameManager.OnRestartGame += RestartGame;
    }
    private void OnDisable()
    {
        GameManager.OnRestartGame -= RestartGame;
        if (clickAction != null)
            clickAction.Disable();
    }

    public int GetCurrentTubeCount()
    {
        return Tubes.Count;
    }
}