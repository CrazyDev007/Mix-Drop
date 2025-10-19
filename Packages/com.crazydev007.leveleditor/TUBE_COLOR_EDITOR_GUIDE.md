# Tube Color Level Editor - Usage Guide

A specialized editor tool for creating and managing levels in the **Tube Colors Mixing** game.

## Overview

The Tube Color Level Editor allows you to:
- Load and save level data from/to JSON files
- Edit individual levels with visual tube editors
- Modify tube colors, empty tubes, max moves, and time limits
- Lock and unlock tubes for puzzle difficulty
- Validate level integrity
- Export level statistics

## Features

### 1. **Main Editor Window**
Access via: `Window > Tube Color Level Editor`

#### Features:
- **Load/Save Levels**: Load levels from JSON and save changes
- **Level Navigation**: Navigate through all levels with a slider
- **Level Properties Editor**: Edit key properties like:
  - Empty Tubes
  - Max Moves
  - Time Limit
  - Available Swaps

### 2. **Tube Editor**
Edit individual tubes within a level:

#### For Each Tube:
- **View/Edit Colors**: See all 4 color slots
- **Add Colors**: Add color values to empty slots
- **Remove Colors**: Remove individual colors
- **Clear Tube**: Clear all colors at once

#### Color Values:
- Use integers to represent colors (e.g., 0, 1, 2, 3 for different colors)
- Each tube can hold up to 4 colors

### 3. **Tube Locking**
Lock tubes to prevent player from pouring them:
- Enter the tube index and click "Lock"
- Locked tubes will be displayed
- Clear all locked tubes if needed

## Workflow

### Step 1: Open the Editor
1. Go to `Window > Tube Color Level Editor`
2. The window opens with an empty level list

### Step 2: Load Levels
1. Click the **"Load Levels"** button
2. Select the JSON file containing your levels
3. All levels will be loaded into the editor

### Step 3: Edit a Level
1. Use the slider to select a level
2. Edit level properties (empty tubes, max moves, time limit)
3. For each tube:
   - View existing colors
   - Add new colors
   - Remove colors as needed
   - Clear and reorganize

### Step 4: Save Changes
1. Click the **"Save Levels"** button
2. Changes are written back to the JSON file

## JSON File Format

```json
{
  "levels": [
    {
      "level": 1,
      "tubes": [
        {
          "values": [1, 1, 2, 2]
        },
        {
          "values": [2, 0, 0, 1]
        },
        {
          "values": []
        }
      ],
      "emptyTubes": 1,
      "maxMoves": 0,
      "timeLimit": 0,
      "lockedTubes": [],
      "availableSwaps": 0,
      "twists": []
    }
  ]
}
```

### Field Descriptions:
- **level**: Level number/ID
- **tubes**: Array of tube objects
- **values**: Array of color values (0-3), max 4 items
- **emptyTubes**: Number of empty tubes
- **maxMoves**: Maximum moves allowed (0 = unlimited)
- **timeLimit**: Time limit in seconds (0 = no limit)
- **lockedTubes**: Array of tube indices that are locked
- **availableSwaps**: Number of available swap actions
- **twists**: Special game modifiers (if any)

## Menu Options

Access quick tools via: `Tools > Tube Color Level Editor`

### Validate All Levels
- Checks all levels for errors
- Reports invalid tube indices, mismatched empty tube counts
- Shows valid vs invalid count

### Export Level Stats
- Generates a text file with statistics
- Shows per-level info: tubes count, unique colors, solvability
- Saved to `Assets/level_stats.txt`

### Find Problem Levels
- Identifies levels with validation errors
- Highlights unsolvable levels
- Quick way to spot issues

### Quick Test
- Tests the first level in the file
- Displays tube contents in console
- Useful for debugging

## Tips & Best Practices

### Color Organization
- Keep track of which color integers represent which colors
- Example: 0=Red, 1=Blue, 2=Green, 3=Yellow

### Level Difficulty
- **Easy**: Few unique colors, many empty tubes
- **Hard**: Many unique colors, few empty tubes
- **Max Moves**: Set lower values for more difficulty

### Empty Tubes
- Must match the actual number of empty tubes in the level
- Use "Validate All Levels" to auto-check

### Locked Tubes
- Use for puzzle variety (player can't pour these tubes)
- Make sure locked tubes don't block puzzle solution

## Common Issues

### "Levels not loading"
- Check the file path is correct
- Ensure JSON file is valid format
- Check file permissions

### "Empty tubes mismatch"
- Run "Validate All Levels" to identify problems
- Manually check tube contents
- Use "Export Level Stats" for detailed info

### "Save not working"
- Ensure write permissions to the JSON file
- Check console for error messages
- Try saving to a different location first

## Data Classes

### TubeData
Represents a single tube with color values.

**Methods:**
- `IsFull()`: Check if tube has 4 colors
- `IsEmpty()`: Check if tube is empty
- `AddColor(int)`: Add a color to tube
- `RemoveTopColor()`: Remove top color
- `GetTopColor()`: Get top color value
- `ClearTube()`: Clear all colors

### TubeColorLevel
Represents a complete level.

**Methods:**
- `AddTube(TubeData)`: Add a tube to level
- `GetTotalTubes()`: Get total tube count
- `IsLocked(int)`: Check if tube is locked
- `LockTube(int)`: Lock a tube
- `UnlockTube(int)`: Unlock a tube

## Advanced Features

### Level Validation
Automatic checks include:
- At least one tube exists
- Empty tube count matches reality
- Locked tube indices are valid
- All color values are non-negative

### Level Helper Utilities
- **CountColors()**: Get unique color count
- **GetTotalColorCount()**: Get total colors in level
- **CanBeSolved()**: Check if level is solvable
- **ShuffleTubeColors()**: Randomize tube colors
- **FillTubeWithColor()**: Fill tube with one color

## Keyboard Shortcuts
(To be added in future versions)

## Future Enhancements
- Batch level editing
- Level preview/preview mode
- Color picker UI
- Level difficulty calculator
- Automatic level generation
- Level search/filter

## Troubleshooting

**Q: I can't find the editor window**
A: Go to `Window` menu in Unity menu bar and look for "Tube Color Level Editor"

**Q: Changes not saving**
A: Make sure to click "Save Levels" button and check console for errors

**Q: Levels won't load**
A: Verify the JSON file path and format. Try "Quick Test" to debug

**Q: Can't add colors to tubes**
A: Tubes can only hold 4 colors max. Use "Remove" to delete colors first

## Support

For issues or feature requests, contact the development team or check the project repository.
