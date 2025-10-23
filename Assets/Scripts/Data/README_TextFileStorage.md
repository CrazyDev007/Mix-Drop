# Text File Game Data Storage System

This system provides a simple, human-readable text file-based storage solution for game data in the Mix-Drop Unity project.

## Features

- Stores current level, completed levels, stars, best times, and attempt counts
- Auto-saves data 2 seconds after any change (configurable)
- Creates backup files before each save
- Human-readable text format for easy viewing and editing
- Editor menu items for convenient access
- Integrated with the existing GameManager

## Setup Instructions

1. **Create the TextFileStorage GameObject**
   - Create an empty GameObject in your scene
   - Name it "TextFileStorage"
   - Add the `TextFileGameDataStorage` component to this GameObject

2. **Connect to GameManager**
   - Select your GameManager object in the scene
   - Drag the TextFileStorage GameObject to the "Text File Storage" field in the Inspector

3. **(Optional) Add Test Component**
   - Create another empty GameObject named "TextFileStorageTest"
   - Add the `TextFileStorageTest` component for testing functionality

## How It Works

The system stores game data in a simple text format with sections for different types of data:

```
# Mix-Drop Game Data
# Generated on 2023-10-23 11:27:00

[GameData]
CurrentLevel=5
CompletedLevelsCount=3
TotalStars=7

[CompletedLevels]
1
2
3
4

[LevelStars]
1=3
2=2
3=3
4=1

[LevelBestTimes]
1=45.30
2=67.80
3=52.10
4=98.50

[LevelAttempts]
1=3
2=5
3=2
4=8
```

## Integration with GameManager

The text file storage system integrates automatically with the GameManager:

- When a level is completed, data is saved to both PlayerPrefs and the text file
- Current level is synchronized between PlayerPrefs and the text file
- No additional code is required in your game logic

## Editor Menu Items

After implementation, you'll have these new menu options:

- `MixDrop/Text File Storage/Open Save Data File` - Opens the save file in the default text editor
- `MixDrop/Text File Storage/Print Game Statistics` - Prints formatted statistics to the console
- `MixDrop/Text File Storage/Reset All Game Data` - Resets all saved game data

## File Location

The save file is stored at:
- **Windows**: `%USERPROFILE%\AppData\LocalLow\<CompanyName>\<ProductName>\MixDrop_GameData.txt`
- **macOS**: `~/Library/Application Support/<CompanyName>/<ProductName>/MixDrop_GameData.txt`
- **Android**: `/data/data/<packagename>/files/MixDrop_GameData.txt`
- **iOS**: `/var/mobile/Applications/[application-identifier]/Documents/MixDrop_GameData.txt`

## Configuration Options

In the TextFileGameDataStorage component, you can configure:

- **Save File Name**: Name of the save file (default: "MixDrop_GameData.txt")
- **Create Backup On Save**: Whether to create a backup before saving (default: true)
- **Auto Save**: Whether to automatically save when data changes (default: true)
- **Auto Save Delay**: Delay in seconds before auto-saving (default: 2 seconds)

## Testing

Use the TextFileStorageTest component to verify the implementation:

1. Add the component to a GameObject
2. Reference the TextFileStorage GameObject
3. Configure test parameters (level, stars, time, attempts)
4. Right-click the component in the Inspector and select "Run Tests"
5. Check the Console for test results

## API Reference

### TextFileGameDataStorage Class

#### Properties
- `CurrentLevel`: Gets the current level
- `CompletedLevelsCount`: Gets the number of completed levels
- `TotalStars`: Gets the total stars earned

#### Methods
- `Initialize()`: Initializes the storage system
- `SetCurrentLevel(int level)`: Sets the current level
- `CompleteLevel(int level, int stars, float timeSeconds, int attempts)`: Marks a level as completed
- `GetLevelStars(int level)`: Gets the stars earned for a specific level
- `GetLevelBestTime(int level)`: Gets the best completion time for a specific level
- `GetLevelAttempts(int level)`: Gets the number of attempts for a specific level
- `IsLevelCompleted(int level)`: Checks if a level is completed
- `ResetAllData()`: Resets all game data
- `SaveGameData()`: Manually saves the game data
- `LoadGameData()`: Manually loads the game data
- `GetStatisticsString()`: Gets a formatted string with all game data statistics

## Troubleshooting

### "TextFileGameDataStorage type not found" Error

This error occurs when the GameManager can't find the TextFileGameDataStorage class. To fix it:

1. Make sure both files are in the same assembly
2. Check that there are no namespace conflicts
3. Ensure the TextFileGameDataStorage.cs file is saved and compiled

### Save File Not Created

If the save file is not being created:

1. Check that the TextFileGameDataStorage component is properly initialized
2. Verify that the application has write permissions to the persistent data path
3. Look for error messages in the Console

### Data Not Persisting

If data is not being saved between sessions:

1. Make sure `SaveGameData()` is being called
2. Check if auto-save is enabled and working
3. Verify that the file is being written to the correct location

## Benefits of This System

1. **Transparency**: Easy to view and understand the saved data
2. **Portability**: The text file can be easily shared between devices or backed up
3. **Debugging**: Easy to manually edit for testing purposes
4. **Redundancy**: Works alongside your existing PlayerPrefs system
5. **Extensibility**: Easy to add new data fields to the format