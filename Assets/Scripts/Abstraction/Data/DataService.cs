using System.Threading.Tasks;

/// <summary>
/// Implementation of the data service that provides access to all repositories
/// </summary>
public class DataService : IDataService
{
    private readonly ILevelRepository _levelRepository;
    private readonly IProgressRepository _progressRepository;

    /// <summary>
    /// Initializes a new instance of the DataService class
    /// </summary>
    /// <param name="levelRepository">The level repository</param>
    /// <param name="progressRepository">The progress repository</param>
    public DataService(ILevelRepository levelRepository, IProgressRepository progressRepository)
    {
        _levelRepository = levelRepository ?? throw new System.ArgumentNullException(nameof(levelRepository));
        _progressRepository = progressRepository ?? throw new System.ArgumentNullException(nameof(progressRepository));
    }

    /// <summary>
    /// Gets the level repository
    /// </summary>
    public ILevelRepository LevelRepository => _levelRepository;

    /// <summary>
    /// Gets the progress repository
    /// </summary>
    public IProgressRepository ProgressRepository => _progressRepository;

    /// <summary>
    /// Initializes the data service
    /// </summary>
    /// <returns>A task that represents the initialization operation</returns>
    public async Task InitializeAsync()
    {
        // Initialize repositories if needed
        // This could load data from persistent storage
        await Task.CompletedTask;
    }

    /// <summary>
    /// Saves all pending changes
    /// </summary>
    /// <returns>A task that represents the save operation</returns>
    public async Task SaveChangesAsync()
    {
        // Save changes in repositories
        // This could persist data to storage
        await Task.CompletedTask;
    }

    /// <summary>
    /// Discards all pending changes
    /// </summary>
    public void DiscardChanges()
    {
        // Discard changes in repositories
        // This could reset any cached changes
    }
}
 
