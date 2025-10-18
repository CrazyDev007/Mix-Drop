using UnityEngine;
using UnityEngine.UIElements;

public class PauseScreenController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    private VisualElement root;
    private Button resumeButton;
    private Button restartButton;
    private Button homeButton;

    private IPauseManager pauseManager;

    private void Awake()
    {
        pauseManager = FindFirstObjectByType<PauseManager>();
        if (pauseManager == null)
        {
            Debug.LogError("PauseManager not found in scene!");
            return;
        }

        if (uiDocument == null)
        {
            uiDocument = GetComponent<UIDocument>();
        }

        SetupUI();
        HideScreen();
    }

    private void SetupUI()
    {
        root = uiDocument.rootVisualElement;
        resumeButton = root.Q<Button>("ResumeButton");
        restartButton = root.Q<Button>("RestartButton");
        homeButton = root.Q<Button>("HomeButton");

        if (resumeButton != null)
            resumeButton.clicked += OnResumeClicked;
        if (restartButton != null)
            restartButton.clicked += OnRestartClicked;
        if (homeButton != null)
            homeButton.clicked += OnHomeClicked;

        pauseManager.OnPauseStateChanged += OnPauseStateChanged;
    }

    private void OnPauseStateChanged(bool isPaused)
    {
        if (isPaused)
        {
            ShowScreen();
        }
        else
        {
            HideScreen();
        }
    }

    private void OnResumeClicked()
    {
        pauseManager.ResumeGame();
    }

    private void OnRestartClicked()
    {
        pauseManager.RestartLevel();
    }

    private void OnHomeClicked()
    {
        pauseManager.GoToHome();
    }

    private void ShowScreen()
    {
        if (root != null)
        {
            root.style.display = DisplayStyle.Flex;
        }
    }

    private void HideScreen()
    {
        if (root != null)
        {
            root.style.display = DisplayStyle.None;
        }
    }

    private void OnDestroy()
    {
        if (pauseManager != null)
        {
            pauseManager.OnPauseStateChanged -= OnPauseStateChanged;
        }
    }
}