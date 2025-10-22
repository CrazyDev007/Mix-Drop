using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Test script for the data access layer implementation
/// </summary>
public class DataAccessLayerTest : MonoBehaviour
{
    private async void Start()
    {
        Debug.Log("Starting Data Access Layer Test...");

        // Test DataService instantiation
        var levelRepository = new LevelRepository();
        var progressRepository = new ProgressRepository();
        var dataService = new DataService(levelRepository, progressRepository);

        Debug.Log("DataService created successfully");

        // Test DataContext instantiation
        var dataContext = new DataContext(dataService);
        Debug.Log("DataContext created successfully");

        // Test initialization
        await dataService.InitializeAsync();
        await dataContext.InitializeAsync();
        Debug.Log("Data services initialized successfully");

        // Test level repository
        var allLevels = dataService.LevelRepository.GetAllLevels();
        Debug.Log($"Found {allLevels.Count()} levels");

        // Test progress repository
        var allProgress = dataService.ProgressRepository.GetAllProgress();
        Debug.Log($"Found {allProgress.Count} progress entries");

        // Test saving changes
        await dataService.SaveChangesAsync();
        await dataContext.SaveChangesAsync();
        Debug.Log("Data changes saved successfully");

        Debug.Log("Data Access Layer Test completed successfully!");
    }
}
 
