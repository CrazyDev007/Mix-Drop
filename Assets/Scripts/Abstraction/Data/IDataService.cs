using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Interface for the data service that provides access to all repositories
/// </summary>
public interface IDataService
{
    /// <summary>
    /// Gets the level repository
    /// </summary>
    ILevelRepository LevelRepository { get; }

    /// <summary>
    /// Gets the progress repository
    /// </summary>
    IProgressRepository ProgressRepository { get; }

    /// <summary>
    /// Initializes the data service
    /// </summary>
    /// <returns>A task that represents the initialization operation</returns>
    Task InitializeAsync();

    /// <summary>
    /// Saves all pending changes
    /// </summary>
    /// <returns>A task that represents the save operation</returns>
    Task SaveChangesAsync();

    /// <summary>
    /// Discards all pending changes
    /// </summary>
    void DiscardChanges();
}
 
