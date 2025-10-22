using System.Threading.Tasks;

/// <summary>
/// Implementation of the data context for managing data operations
/// </summary>
public class DataContext : IDataContext
{
    private readonly IDataService _dataService;
    private bool _hasPendingChanges;
    private int _pendingChangesCount;

    /// <summary>
    /// Initializes a new instance of the DataContext class
    /// </summary>
    /// <param name="dataService">The data service</param>
    public DataContext(IDataService dataService)
    {
        _dataService = dataService ?? throw new System.ArgumentNullException(nameof(dataService));
        _hasPendingChanges = false;
        _pendingChangesCount = 0;
    }

    /// <summary>
    /// Initializes the data context
    /// </summary>
    /// <returns>A task that represents the initialization operation</returns>
    public async Task InitializeAsync()
    {
        await _dataService.InitializeAsync();
    }

    /// <summary>
    /// Saves all changes in the data context
    /// </summary>
    /// <returns>A task that represents the save operation</returns>
    public async Task SaveChangesAsync()
    {
        if (_hasPendingChanges)
        {
            await _dataService.SaveChangesAsync();
            _hasPendingChanges = false;
            _pendingChangesCount = 0;
        }
    }

    /// <summary>
    /// Discards all pending changes in the data context
    /// </summary>
    public void DiscardChanges()
    {
        if (_hasPendingChanges)
        {
            _dataService.DiscardChanges();
            _hasPendingChanges = false;
            _pendingChangesCount = 0;
        }
    }

    /// <summary>
    /// Checks if the data context has pending changes
    /// </summary>
    /// <returns>True if there are pending changes, false otherwise</returns>
    public bool HasPendingChanges()
    {
        return _hasPendingChanges;
    }

    /// <summary>
    /// Gets the number of pending changes
    /// </summary>
    /// <returns>The number of pending changes</returns>
    public int GetPendingChangesCount()
    {
        return _pendingChangesCount;
    }

    /// <summary>
    /// Marks that there are pending changes
    /// </summary>
    public void MarkAsChanged()
    {
        _hasPendingChanges = true;
        _pendingChangesCount++;
    }

    /// <summary>
    /// Disposes the data context and releases resources
    /// </summary>
    public void Dispose()
    {
        // Clean up resources if needed
        DiscardChanges();
    }
}
 
