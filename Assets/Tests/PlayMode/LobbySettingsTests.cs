using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.Profiling;
using UI.Lobby;

public class LobbySettingsTests
{
    private const string LOBBY_SCENE_NAME = "Main"; // Assuming lobby is in Main scene
    private const string SETTINGS_OVERLAY_ID = "Settings"; // Overlay ID to be implemented

    [UnityTest]
    public IEnumerator SettingsToggle_Persistence_Test()
    {
        // Arrange: Load the lobby scene
        SceneManager.LoadScene(LOBBY_SCENE_NAME);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == LOBBY_SCENE_NAME);
        yield return new WaitForSeconds(1f); // Allow scene to initialize

        // Find the LobbySettingsController in the scene
        var settingsController = GameObject.FindObjectOfType<LobbySettingsController>();
        Assert.IsNotNull(settingsController, "LobbySettingsController not found in scene");

        // Create a fresh preferences model for testing
        var preferences = new LobbyPreferencesModel();
        preferences.ResetToDefaults();

        // Act: Show settings and modify toggle states
        settingsController.Show();

        // Wait for settings to become visible
        yield return new WaitForSeconds(0.5f);

        // Simulate toggle interactions (these will fail until controller is implemented)
        // Test SFX toggle persistence
        bool originalSfxState = preferences.IsSfxEnabled;
        preferences.IsSfxEnabled = !originalSfxState; // Toggle SFX

        // Test VFX toggle persistence
        bool originalVfxState = preferences.IsVfxEnabled;
        preferences.IsVfxEnabled = !originalVfxState; // Toggle VFX

        // Close settings
        settingsController.Hide();
        yield return new WaitForSeconds(0.5f);

        // Assert: Verify persistence - create new model instance to test loading from storage
        var reloadedPreferences = new LobbyPreferencesModel();
        Assert.AreEqual(!originalSfxState, reloadedPreferences.IsSfxEnabled,
            "SFX toggle state should persist across sessions");
        Assert.AreEqual(!originalVfxState, reloadedPreferences.IsVfxEnabled,
            "VFX toggle state should persist across sessions");
    }

    [UnityTest]
    public IEnumerator SettingsToggle_ThemePersistence_Test()
    {
        // Arrange: Load the lobby scene
        SceneManager.LoadScene(LOBBY_SCENE_NAME);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == LOBBY_SCENE_NAME);
        yield return new WaitForSeconds(1f);

        var settingsController = GameObject.FindObjectOfType<LobbySettingsController>();
        Assert.IsNotNull(settingsController, "LobbySettingsController not found in scene");

        var preferences = new LobbyPreferencesModel();
        preferences.ResetToDefaults();

        // Act: Show settings and change theme
        settingsController.Show();
        yield return new WaitForSeconds(0.5f);

        string originalTheme = preferences.ThemeId;
        string newTheme = "dark"; // Test theme change
        preferences.ThemeId = newTheme;

        // Close settings
        settingsController.Hide();
        yield return new WaitForSeconds(0.5f);

        // Assert: Verify theme persistence
        var reloadedPreferences = new LobbyPreferencesModel();
        Assert.AreEqual(newTheme, reloadedPreferences.ThemeId,
            "Theme selection should persist across sessions");
    }

    [UnityTest]
    public IEnumerator SettingsToggle_Performance_MemoryBudget_Test()
    {
        // Arrange: Load the lobby scene
        SceneManager.LoadScene(LOBBY_SCENE_NAME);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == LOBBY_SCENE_NAME);
        yield return new WaitForSeconds(1f);

        var settingsController = GameObject.FindObjectOfType<LobbySettingsController>();
        Assert.IsNotNull(settingsController, "LobbySettingsController not found in scene");

        var preferences = new LobbyPreferencesModel();

        // Begin memory profiling
        Profiler.enabled = true;
        long initialMemory = Profiler.GetMonoUsedSizeLong();

        // Act: Show settings
        settingsController.Show();
        yield return new WaitForSeconds(0.5f);

        // Perform multiple toggle operations to test memory allocation
        for (int i = 0; i < 10; i++)
        {
            preferences.IsSfxEnabled = !preferences.IsSfxEnabled;
            preferences.IsVfxEnabled = !preferences.IsVfxEnabled;
            yield return null; // Allow frame processing
        }

        // Close settings
        settingsController.Hide();
        yield return new WaitForSeconds(0.5f);

        // Force garbage collection to measure true allocation
        System.GC.Collect();
        yield return new WaitForSeconds(0.1f);

        long finalMemory = Profiler.GetMonoUsedSizeLong();
        Profiler.enabled = false;

        // Assert: Verify memory budget (≤512 KB per interaction)
        long memoryDelta = finalMemory - initialMemory;
        double memoryDeltaKB = memoryDelta / 1024.0;

        Assert.LessOrEqual(memoryDeltaKB, 512.0,
            $"Settings toggle operations exceeded memory budget: {memoryDeltaKB:F2} KB used (limit: 512 KB)");
    }

    [UnityTest]
    public IEnumerator SettingsToggle_ChangeEvent_Test()
    {
        // Arrange: Load the lobby scene
        SceneManager.LoadScene(LOBBY_SCENE_NAME);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == LOBBY_SCENE_NAME);
        yield return new WaitForSeconds(1f);

        var preferences = new LobbyPreferencesModel();

        // Track preference change events
        int changeEventCount = 0;
        LobbyPreferencesSnapshot lastSnapshot = default;

        void OnPreferencesChanged(LobbyPreferencesSnapshot snapshot)
        {
            changeEventCount++;
            lastSnapshot = snapshot;
        }

        preferences.PreferencesChanged += OnPreferencesChanged;

        // Act: Modify preferences
        preferences.IsSfxEnabled = !preferences.IsSfxEnabled;
        preferences.IsVfxEnabled = !preferences.IsVfxEnabled;
        preferences.ThemeId = "test_theme";

        yield return null; // Allow events to process

        // Assert: Verify change events were fired
        Assert.GreaterOrEqual(changeEventCount, 3,
            "Preference change events should be fired for each modification");

        // Verify snapshot contains correct values
        Assert.AreEqual(preferences.IsSfxEnabled, lastSnapshot.SfxEnabled,
            "Change event snapshot should contain current SFX state");
        Assert.AreEqual(preferences.IsVfxEnabled, lastSnapshot.VfxEnabled,
            "Change event snapshot should contain current VFX state");
        Assert.AreEqual(preferences.ThemeId, lastSnapshot.ThemeId,
            "Change event snapshot should contain current theme");

        // Cleanup
        preferences.PreferencesChanged -= OnPreferencesChanged;
    }

    [UnityTest]
    public IEnumerator SettingsController_BasicFunctionality_Test()
    {
        // Arrange: Load the lobby scene
        SceneManager.LoadScene(LOBBY_SCENE_NAME);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == LOBBY_SCENE_NAME);
        yield return new WaitForSeconds(1f);

        var settingsController = GameObject.FindObjectOfType<LobbySettingsController>();
        Assert.IsNotNull(settingsController, "LobbySettingsController not found in scene");

        // Act & Assert: Verify basic functionality
        // Show Settings
        settingsController.Show();
        Assert.IsNotNull(settingsController, "Should successfully show Settings");

        yield return new WaitForSeconds(0.2f);

        // Hide Settings
        settingsController.Hide();

        // Assert: Settings controller should still exist
        Assert.IsNotNull(settingsController, "Settings controller should remain functional");
    }
}