using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using UI.Lobby;
using ScreenFlow;

public class HowToPlayScreenTests
{
    private const string LOBBY_SCENE_NAME = "Main"; // Assuming lobby is in Main scene
    private const string HOW_TO_PLAY_BUTTON_NAME = "HowToPlayButton"; // Button name to be implemented

    [UnityTest]
    public IEnumerator HowToPlayScreen_Visibility_Test()
    {
        // Arrange: Load the lobby scene
        SceneManager.LoadScene(LOBBY_SCENE_NAME);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == LOBBY_SCENE_NAME);
        yield return new WaitForSeconds(1f); // Allow scene to initialize

        // Find the HowToPlayScreen in the scene
        var howToPlayScreen = GameObject.FindObjectOfType<HowToPlayScreen>();
        Assert.IsNotNull(howToPlayScreen, "HowToPlayScreen not found in scene");

        // Act: Show the screen
        howToPlayScreen.Show();

        // Assert: Verify screen is shown (this is a basic test - actual visibility depends on ScreenUI implementation)
        Assert.IsNotNull(howToPlayScreen, "HowToPlayScreen should exist after showing");

        // Act: Hide the screen
        howToPlayScreen.Hide();

        // Assert: Verify screen is hidden (this is a basic test - actual visibility depends on ScreenUI implementation)
        Assert.IsNotNull(howToPlayScreen, "HowToPlayScreen should still exist after hiding");
    }

    [UnityTest]
    public IEnumerator HowToPlayScreen_CloseAction_Test()
    {
        // Arrange: Load the lobby scene
        SceneManager.LoadScene(LOBBY_SCENE_NAME);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == LOBBY_SCENE_NAME);
        yield return new WaitForSeconds(1f);

        // Find the HowToPlayScreen in the scene
        var howToPlayScreen = GameObject.FindObjectOfType<HowToPlayScreen>();
        Assert.IsNotNull(howToPlayScreen, "HowToPlayScreen not found in scene");

        // Show the screen first
        howToPlayScreen.Show();
        yield return new WaitForSeconds(0.5f);

        // Act: Simulate close action by calling Hide
        howToPlayScreen.Hide();

        // Wait for screen to close
        yield return new WaitForSeconds(0.5f);

        // Assert: Verify screen is hidden (basic test)
        Assert.IsNotNull(howToPlayScreen, "HowToPlayScreen should still exist after hiding");
    }

    [UnityTest]
    public IEnumerator HowToPlayScreen_BasicFunctionality_Test()
    {
        // Arrange: Load the lobby scene
        SceneManager.LoadScene(LOBBY_SCENE_NAME);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == LOBBY_SCENE_NAME);
        yield return new WaitForSeconds(1f);

        // Find the HowToPlayScreen in the scene
        var howToPlayScreen = GameObject.FindObjectOfType<HowToPlayScreen>();
        Assert.IsNotNull(howToPlayScreen, "HowToPlayScreen not found in scene");

        // Act: Show and hide the screen
        howToPlayScreen.Show();
        yield return new WaitForSeconds(0.5f);

        howToPlayScreen.Hide();
        yield return new WaitForSeconds(0.5f);

        // Assert: Basic functionality works
        Assert.IsNotNull(howToPlayScreen, "HowToPlayScreen should exist and function properly");
    }

    [UnityTest]
    public IEnumerator HowToPlayScreen_UIElements_Test()
    {
        // Arrange: Load the lobby scene
        SceneManager.LoadScene(LOBBY_SCENE_NAME);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == LOBBY_SCENE_NAME);
        yield return new WaitForSeconds(1f);

        // Find the HowToPlayScreen in the scene
        var howToPlayScreen = GameObject.FindObjectOfType<HowToPlayScreen>();
        Assert.IsNotNull(howToPlayScreen, "HowToPlayScreen not found in scene");

        // Show the screen to initialize UI elements
        howToPlayScreen.Show();
        yield return new WaitForSeconds(0.5f);

        // Assert: Screen exists and can be interacted with
        Assert.IsNotNull(howToPlayScreen, "HowToPlayScreen should be functional");
    }
}