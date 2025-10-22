using System.Threading.Tasks;

/// <summary>
/// Interface for data context management
/// </summary>
public interface IDataContext
{
    /// <summary>
    /// Initializes the data context
    /// </summary>
    /// <returns>A task that represents the initialization operation</returns>
    Task InitializeAsync();

    /// <summary>
    /// Saves all changes in the data context
    /// </summary>
    /// <returns>A task that represents the save operation</returns>
    Task SaveChangesAsync();

    /// <summary>
    /// Discards all pending changes in the data context
    /// </summary>
    void DiscardChanges();

    /// <summary>
    /// Checks if the data context has pending changes
    /// </summary>
    /// <returns>True if there are pending changes, false otherwise</returns>
    bool HasPendingChanges();

    /// <summary>
    /// Gets the number of pending changes
    /// </summary>
    /// <returns>The number of pending changes</returns>
    int GetPendingChangesCount();

    /// <summary>
    /// Disposes the data context and releases resources
    /// </summary>
    void Dispose();
}
 
