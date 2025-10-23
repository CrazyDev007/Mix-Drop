using UnityEngine;

namespace MixDrop.Test
{
    /// <summary>
    /// Test script for the TextFileGameDataStorage system
    /// </summary>
    public class TextFileStorageTest : MonoBehaviour
    {
        [Header("Test Configuration")]
        [Tooltip("Reference to the text file storage system")]
        [SerializeField] private TextFileGameDataStorage textFileStorage;
        
        [Header("Test Data")]
        [Tooltip("Level to use for testing")]
        [SerializeField] private int testLevel = 5;
        
        [Tooltip("Stars to use for testing")]
        [Range(0, 3)]
        [SerializeField] private int testStars = 3;
        
        [Tooltip("Time to use for testing")]
        [SerializeField] private float testTime = 120.5f;
        
        [Tooltip("Attempts to use for testing")]
        [SerializeField] private int testAttempts = 5;
        
        private void Start()
        {
            if (textFileStorage == null)
            {
                Debug.LogError("TextFileStorageTest: TextFileGameDataStorage reference is not set!");
                return;
            }
            
            // Initialize the storage system
            textFileStorage.Initialize();
            
            // Run tests after a short delay
            Invoke(nameof(RunTests), 1f);
        }
        
        [ContextMenu("Run Tests")]
        public void RunTests()
        {
            if (textFileStorage == null)
            {
                Debug.LogError("TextFileStorageTest: TextFileGameDataStorage reference is not set!");
                return;
            }
            
            Debug.Log("=== Running TextFileGameDataStorage Tests ===");
            
            // Test 1: Set current level
            Debug.Log($"Test 1: Setting current level to {testLevel}");
            textFileStorage.SetCurrentLevel(testLevel);
            Debug.Log($"Current level is now: {textFileStorage.CurrentLevel}");
            
            // Test 2: Complete a level
            Debug.Log($"Test 2: Completing level {testLevel} with {testStars} stars, {testTime}s time, {testAttempts} attempts");
            textFileStorage.CompleteLevel(testLevel, testStars, testTime, testAttempts);
            Debug.Log($"Level {testLevel} completed: {textFileStorage.IsLevelCompleted(testLevel)}");
            Debug.Log($"Level {testLevel} stars: {textFileStorage.GetLevelStars(testLevel)}");
            Debug.Log($"Level {testLevel} best time: {textFileStorage.GetLevelBestTime(testLevel)}");
            Debug.Log($"Level {testLevel} attempts: {textFileStorage.GetLevelAttempts(testLevel)}");
            
            // Test 3: Complete the same level with worse performance
            Debug.Log($"Test 3: Completing level {testLevel} again with worse performance");
            textFileStorage.CompleteLevel(testLevel, 1, testTime * 2, testAttempts + 3);
            Debug.Log($"Level {testLevel} stars (should be unchanged): {textFileStorage.GetLevelStars(testLevel)}");
            Debug.Log($"Level {testLevel} best time (should be unchanged): {textFileStorage.GetLevelBestTime(testLevel)}");
            Debug.Log($"Level {testLevel} attempts (should be increased): {textFileStorage.GetLevelAttempts(testLevel)}");
            
            // Test 4: Complete the same level with better performance
            Debug.Log($"Test 4: Completing level {testLevel} again with better performance");
            textFileStorage.CompleteLevel(testLevel, testStars, testTime * 0.8f, testAttempts + 5);
            Debug.Log($"Level {testLevel} stars (should be unchanged): {textFileStorage.GetLevelStars(testLevel)}");
            Debug.Log($"Level {testLevel} best time (should be improved): {textFileStorage.GetLevelBestTime(testLevel)}");
            Debug.Log($"Level {testLevel} attempts (should be increased): {textFileStorage.GetLevelAttempts(testLevel)}");
            
            // Test 5: Save and reload data
            Debug.Log("Test 5: Saving and reloading data");
            textFileStorage.SaveGameData();
            
            // Reset the storage system
            textFileStorage.ResetAllData();
            Debug.Log($"After reset - Current level: {textFileStorage.CurrentLevel}");
            Debug.Log($"After reset - Level {testLevel} completed: {textFileStorage.IsLevelCompleted(testLevel)}");
            
            // Reload data
            textFileStorage.LoadGameData();
            Debug.Log($"After reload - Current level: {textFileStorage.CurrentLevel}");
            Debug.Log($"After reload - Level {testLevel} completed: {textFileStorage.IsLevelCompleted(testLevel)}");
            Debug.Log($"After reload - Level {testLevel} stars: {textFileStorage.GetLevelStars(testLevel)}");
            Debug.Log($"After reload - Level {testLevel} best time: {textFileStorage.GetLevelBestTime(testLevel)}");
            Debug.Log($"After reload - Level {testLevel} attempts: {textFileStorage.GetLevelAttempts(testLevel)}");
            
            // Test 6: Print statistics
            Debug.Log("Test 6: Printing statistics");
            Debug.Log(textFileStorage.GetStatisticsString());
            
            Debug.Log("=== TextFileGameDataStorage Tests Complete ===");
        }
        
        [ContextMenu("Reset Test Data")]
        public void ResetTestData()
        {
            if (textFileStorage != null)
            {
                textFileStorage.ResetAllData();
                textFileStorage.SaveGameData();
                Debug.Log("Test data has been reset.");
            }
        }
    }
}