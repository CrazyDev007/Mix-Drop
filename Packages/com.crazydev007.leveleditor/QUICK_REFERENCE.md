# Tube Color Level Editor - Quick Reference

## Keyboard Shortcuts (Editor Window)

| Action | Shortcut |
|--------|----------|
| Open Editor | `Window > Tube Color Level Editor` |
| Open Menu Tools | `Tools > Tube Color Level Editor` |
| Focus on Level Slider | Tab |
| Validate Levels | `Tools > Tube Color Level Editor > Validate All Levels` |

## Editor Window Layout

```
┌─────────────────────────────────────────────────┐
│ Tube Color Level Editor                         │
├─────────────────────────────────────────────────┤
│ [Load Levels] [Save Levels]                    │
│                                                 │
│ File Path: Packages/com.crazydev007.leveleditor/...
│                                                 │
│ Total Levels: 1000                             │
│ Select Level: [========●====================] 500
│                                                 │
│ ┌─ LEVEL 500 ────────────────────────────────┐ │
│ │ Level Properties                           │ │
│ │ Empty Tubes: [1]                           │ │
│ │ Max Moves: [0]                             │ │
│ │ Time Limit: [0] seconds                    │ │
│ │ Available Swaps: [0]                       │ │
│ │                                             │ │
│ │ Tubes Editor (scrollable):                 │ │
│ │ ┌─ Tube 1 (4/4) ─────────────────────────┐ │
│ │ │ Slot 1: [1] [Remove]                   │ │
│ │ │ Slot 2: [1] [Remove]                   │ │
│ │ │ Slot 3: [2] [Remove]                   │ │
│ │ │ Slot 4: [2] [Remove]                   │ │
│ │ │ [Clear Tube]                           │ │
│ │ └─────────────────────────────────────────┘ │
│ │ ┌─ Tube 2 (2/4) ─────────────────────────┐ │
│ │ │ Slot 1: [2] [Remove]                   │ │
│ │ │ Slot 2: [0] [Remove]                   │ │
│ │ │ Slot 3: (Empty)                        │ │
│ │ │ Slot 4: (Empty)                        │ │
│ │ │ Add Color: [0] [Add]                   │ │
│ │ └─────────────────────────────────────────┘ │
│ │                                             │ │
│ │ [Add New Tube]                             │ │
│ │ Remove Tube Index: [0] [Remove]            │ │
│ │                                             │ │
│ │ Locked Tubes                               │ │
│ │ Lock Tube Index: [0] [Lock]                │ │
│ └─────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────┘
```

## Color Value Reference

Use integers to represent colors:
- **0** = Red (or first color)
- **1** = Blue (or second color)
- **2** = Green (or third color)
- **3** = Yellow (or fourth color)
- **-1** = Invalid/Error

## Menu Options

### Window Menu
```
Window
└── Tube Color Level Editor     (Opens main editor)
```

### Tools Menu
```
Tools
└── Tube Color Level Editor
    ├── Validate All Levels     (Check all levels for errors)
    ├── Export Level Stats      (Generate statistics report)
    ├── Find Problem Levels     (Highlight invalid levels)
    └── Quick Test              (Debug first level)
```

## Common Tasks

### Load Levels
1. Click **"Load Levels"** button
2. Select JSON file
3. Wait for levels to load (shows count in console)

### Edit a Tube
1. Select level using slider
2. Find tube in scrollable list
3. Edit color values
4. Add/remove colors as needed

### Add Tube
1. Scroll to bottom of tubes list
2. Click **"Add New Tube"** button
3. New empty tube is added

### Remove Tube
1. Enter tube index in "Remove Tube Index" field
2. Click **"Remove"** button
3. Tube is deleted from level

### Lock Tube
1. Enter tube index in "Lock Tube Index" field
2. Click **"Lock"** button
3. Locked tubes are displayed below
4. Click **"Clear All Locked Tubes"** to unlock all

### Save Changes
1. Click **"Save Levels"** button
2. Check console for success message
3. Changes written to JSON file

### Validate Level
1. Go to `Tools > Tube Color Level Editor > Validate All Levels`
2. Check console for results
3. Problem levels are highlighted in yellow (warnings)

### Export Statistics
1. Go to `Tools > Tube Color Level Editor > Export Level Stats`
2. File saved to `Assets/level_stats.txt`
3. Contains per-level information and statistics

## Level Properties Explanation

| Property | Purpose | Example |
|----------|---------|---------|
| Empty Tubes | Number of empty tubes | 1 |
| Max Moves | Move limit (0 = unlimited) | 15 |
| Time Limit | Time limit in seconds (0 = no limit) | 60 |
| Available Swaps | Number of swap actions | 2 |
| Locked Tubes | Tubes player cannot pour | [0, 2] |

## Data Structure

### Tube
```
{
  "values": [1, 2, 0, 2]  // max 4 colors
}
```

### Level
```
{
  "level": 1,
  "tubes": [...],
  "emptyTubes": 1,
  "maxMoves": 20,
  "timeLimit": 0,
  "lockedTubes": [],
  "availableSwaps": 0,
  "twists": []
}
```

## Console Output Examples

```
// Successful load
Loaded 1000 levels

// Successful save
Levels saved successfully!

// Validation result
Validation Complete: 950 valid levels, 50 invalid levels

// Problem level found
Level 45 has errors:
Empty tubes mismatch: Expected 1, but found 2
Locked tube index 5 is out of range

// Quick test output
First level has 4 tubes with 12 total colors
Tube: 1,1,2,2
Tube: 2,0,0,1
Tube: 0,0,2,1
Tube: (empty)
```

## Tips & Tricks

### Speed up editing
- Use arrow keys to navigate level slider quickly
- Tab to focus on different fields
- Use number pad for numeric entry

### Batch operations
- Validate all levels before deploying
- Export stats to analyze difficulty distribution
- Use Find Problem Levels to catch issues early

### Level design
- Keep unique colors count ≤ (empty tubes + 1)
- Ensure at least one empty tube for gameplay
- Use locked tubes for puzzle variety

## Troubleshooting Checklist

- ✓ Editor window is open?
- ✓ Levels are loaded?
- ✓ JSON file exists and is readable?
- ✓ Changes saved to file?
- ✓ Console shows no errors?

## File Locations

- **Editor Window**: `Editor/TubeColorLevelEditorWindow.cs`
- **Data Classes**: `Runtime/Data/TubeData.cs`, `TubeColorLevel.cs`
- **Serializer**: `Runtime/Serialization/TubeColorLevelSerializer.cs`
- **Menu Tools**: `Editor/TubeColorLevelMenu.cs`
- **Utilities**: `Runtime/Utilities/TubeColorLevelUtilities.cs`
- **Level JSON**: `Editor/Resources/color_sort_1000_levels.json`
- **Statistics Export**: `Assets/level_stats.txt`

## Keyboard Modifiers (Future Enhancement)

| Modifier | Effect |
|----------|--------|
| Shift + Click | Multi-select |
| Ctrl + S | Save |
| Ctrl + Z | Undo (future) |
| Ctrl + Y | Redo (future) |

## Performance Notes

- **Load Time**: ~1-2 seconds for 1000 levels
- **Save Time**: ~1 second for 1000 levels
- **Validation**: ~0.5 seconds for all levels
- **Editor Responsiveness**: Smooth real-time editing

## Getting Help

1. Check the detailed guide: `TUBE_COLOR_EDITOR_GUIDE.md`
2. View implementation details: `IMPLEMENTATION_SUMMARY.md`
3. Check console for error messages
4. Review example levels in JSON file

---
**Version**: 1.0.0
**Last Updated**: October 2025
**Namespace**: `CrazyDev007.LevelEditor` (Runtime), `CrazyDev007.LevelEditor.Editor` (Editor)
