using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class TubeClick
{
    private Vector3 mousePosition1 = new Vector3(349.09f, 1162.91f, 0.00f);
    private Vector3 mousePosition2 = new Vector3(724.36f, 1149.82f, 0.00f);
    private Vector3 mousePosition3 = new Vector3(353.45f, 493.09f, 0.00f);
    private Vector3 mousePosition4 = new Vector3(733.09f, 504.00f, 0.00f);
    private Vector3 mousePositionExtra = new Vector3(174.55f, 1396.36f, 0.00f);


    // A Test behaves as an ordinary method
    [Test]
    public void TubeClickSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator TubeClickWithEnumeratorPasses()
    {
        // Load Scene
        var testSceneName = "GamePlay";
        SceneManager.LoadScene(testSceneName);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == testSceneName);
        yield return new WaitForSeconds(3);

        // Get TubeManager
        var tubeManger = GameObject.FindFirstObjectByType<TubeManager>();
        Assert.IsNotNull(tubeManger, "Tube Manager not found");
        
        tubeManger.OnTubeClicked(tubeManger.Tubes[2]);
        yield return new WaitForSeconds(1f);
        tubeManger.OnTubeClicked(tubeManger.Tubes[1]);
        yield return new WaitForSeconds(0.5f);
        tubeManger.OnTubeClicked(tubeManger.Tubes[3]);
        yield return new WaitForSeconds(0.05f);
        tubeManger.OnTubeClicked(tubeManger.Tubes[1]);
        yield return new WaitForSeconds(1.3f);
        Debug.Log(">>>>> UNET "+tubeManger.Tubes[1].CurrentTubeState);
        tubeManger.OnTubeClicked(tubeManger.Tubes[1]);
        yield return new WaitForSeconds(3f);
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}