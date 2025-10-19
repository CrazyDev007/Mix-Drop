# UI Toolkit Level Editor - Quick Reference

## 🚀 Quick Start

### Opening the Editor
```
Unity Menu → Tools → Tube Color Level Editor → Open Editor
```

### Keyboard Shortcuts
| Action | Shortcut |
|--------|----------|
| Load Levels | None (click button) |
| Save Levels | None (click button) |
| Search Levels | Click search field |

## 📁 File Structure

```
Editor/Resources/
├── TubeColorLevelEditor.uxml      # UI Layout
├── TubeColorLevelEditor.uss       # Styles
├── TubeCardTemplate.uxml          # Tube component template
└── color_sort_1000_levels.json    # Level data
```

## 🎨 UI Components Reference

### Toolbar Buttons

| Button | Function | Color |
|--------|----------|-------|
| Load Levels | Load JSON file | Blue (Primary) |
| Save Levels | Save to JSON | Blue (Primary) |
| Browse... | Select file | Gray (Secondary) |
| New Level | Create level | Green (Success) |
| Duplicate Level | Clone selected | Gray (Secondary) |
| Delete Level | Remove selected | Red (Danger) |

### Level List View
- **Location**: Left panel
- **Features**: Searchable, scrollable, single-selection
- **Display**: "Level X (Y tubes)"

### Level Editor Panel
- **Location**: Right panel
- **Sections**:
  - Level Properties
  - Tubes
  - Locked Tubes
  - Validation

## 🔧 Common Tasks

### Task 1: Create a New Level
1. Click **New Level**
2. Level appears in list
3. Select it to edit

### Task 2: Add Tubes to Level
1. Select a level
2. Click **+ Add Tube**
3. Tube card appears
4. Enter color values (0-4 slots)

### Task 3: Edit Tube Colors
1. Select level
2. Find tube card
3. Enter values in integer fields
4. Click **✕** to remove a color
5. Click **+ Add** to add a color

### Task 4: Lock a Tube
1. Select level
2. Enter tube index (0-based)
3. Click **Lock**
4. Tube index appears in locked list

### Task 5: Validate Level
1. Select level
2. Scroll to Validation section
3. Click **Validate Level**
4. Check result (green = success, red = error)

## 🎯 CSS Classes Reference

### Container Classes
| Class | Purpose |
|-------|---------|
| `.root-container` | Main window container |
| `.header-container` | Title area |
| `.toolbar-container` | Button toolbar |
| `.main-content-container` | Split view container |
| `.left-panel` | Level list panel |
| `.right-panel` | Editor panel |

### Button Classes
| Class | Style |
|-------|-------|
| `.primary-button` | Blue (#007acc) |
| `.secondary-button` | Gray (#3e3e42) |
| `.success-button` | Green (#0e8a16) |
| `.danger-button` | Red (#d73a49) |

### Tube Classes
| Class | Purpose |
|-------|---------|
| `.tube-card` | Individual tube container |
| `.tube-header` | Tube title row |
| `.tube-colors` | Color slots container |
| `.tube-actions` | Button row |
| `.color-slot` | Individual color slot |

### State Classes
| Class | When Applied |
|-------|--------------|
| `.validation-success` | Validation passed |
| `.validation-error` | Validation failed |

## 📊 Data Model

### TubeColorLevel
```csharp
{
    int level;                  // Level number
    List<TubeData> tubes;       // Tubes in level
    int emptyTubes;            // Count of empty tubes
    int maxMoves;              // Maximum allowed moves
    int timeLimit;             // Time limit (seconds)
    List<int> lockedTubes;     // Indices of locked tubes
    int availableSwaps;        // Number of swaps available
}
```

### TubeData
```csharp
{
    List<int> values;  // Color values (max 4)
}
```

## 🎨 Color Palette

### Background Colors
- **Dark**: `#1e1e1e` (Main background)
- **Medium**: `#252526` (Panels)
- **Light**: `#2d2d30` (Cards, headers)
- **Accent**: `#3e3e42` (Borders, separators)

### Action Colors
- **Primary**: `#007acc` (Blue - main actions)
- **Success**: `#0e8a16` (Green - positive actions)
- **Danger**: `#d73a49` (Red - destructive actions)
- **Warning**: `#e68900` (Orange - caution)

### Text Colors
- **Primary**: `#ffffff` (White)
- **Secondary**: `#cccccc` (Light gray)
- **Tertiary**: `#a0a0a0` (Medium gray)
- **Accent**: `#87ceeb` (Sky blue - links/paths)

## 🔍 Element Name Reference

### Main Elements
```csharp
root.Q<Button>("loadButton")
root.Q<Button>("saveButton")
root.Q<Button>("browseButton")
root.Q<Button>("newLevelButton")
root.Q<Button>("duplicateButton")
root.Q<Button>("deleteButton")
root.Q<Label>("filePathLabel")
root.Q<Label>("levelCountLabel")
root.Q<Label>("statusLabel")
root.Q<ListView>("levelListView")
root.Q<TextField>("searchField")
```

### Level Editor Elements
```csharp
root.Q<Label>("levelTitle")
root.Q<IntegerField>("levelNumberField")
root.Q<IntegerField>("emptyTubesField")
root.Q<IntegerField>("maxMovesField")
root.Q<IntegerField>("timeLimitField")
root.Q<IntegerField>("availableSwapsField")
root.Q<VisualElement>("tubesContainer")
root.Q<Button>("addTubeButton")
```

### Locked Tubes Elements
```csharp
root.Q<IntegerField>("lockTubeIndexField")
root.Q<Button>("lockTubeButton")
root.Q<Button>("unlockTubeButton")
root.Q<Button>("clearLocksButton")
root.Q<Label>("lockedTubesLabel")
```

### Validation Elements
```csharp
root.Q<Button>("validateButton")
root.Q<Label>("validationResultLabel")
```

## 🐛 Troubleshooting Quick Fixes

### Problem: UI not showing
**Solution**: Check UXML/USS files are in Resources folder

### Problem: Buttons not working
**Solution**: Verify element names match between UXML and C#

### Problem: Styles not applied
**Solution**: Check USS class names and selector syntax

### Problem: ListView empty
**Solution**: Set `itemsSource` and call `Rebuild()`

### Problem: Changes not saving
**Solution**: Click Save Levels button after modifications

## 📝 Code Snippets

### Query an Element
```csharp
var button = root.Q<Button>("myButton");
```

### Register Button Click
```csharp
button.clicked += () => {
    Debug.Log("Clicked!");
};
```

### Listen to Field Changes
```csharp
field.RegisterValueChangedCallback(evt => {
    Debug.Log($"Changed to: {evt.newValue}");
});
```

### Add CSS Class
```csharp
element.AddToClassList("my-class");
```

### Update Label Text
```csharp
label.text = "New Text";
```

### Refresh ListView
```csharp
listView.itemsSource = myList;
listView.Rebuild();
```

## 🎓 Best Practices

1. ✅ Always check for null when querying elements
2. ✅ Use meaningful names for UI elements
3. ✅ Group related UI elements in containers
4. ✅ Use CSS classes instead of inline styles
5. ✅ Cache element references in fields
6. ✅ Update status bar for user feedback
7. ✅ Validate input before processing
8. ✅ Use ListView for lists (not ScrollView + Labels)

## 📚 Learning Resources

- **Unity Manual**: UI Toolkit section
- **UI Toolkit Debugger**: Window > UI Toolkit > Debugger
- **UI Samples**: Unity UI Toolkit samples on GitHub
- **This Project**: Read `UI_TOOLKIT_GUIDE.md` for details

---

**Pro Tip**: Use the UI Toolkit Debugger to inspect the live UI hierarchy and see applied styles in real-time!
