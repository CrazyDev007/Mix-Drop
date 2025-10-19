# Tube Color Level Editor - UI Toolkit Version

## Overview
The Tube Color Level Editor has been completely redesigned using Unity UI Toolkit, providing a modern, responsive, and intuitive interface for creating and managing game levels.

## Features

### 🎨 Modern UI Design
- **Dark Theme**: Professional dark theme matching Unity Editor aesthetics
- **Responsive Layout**: Split-view design with resizable panels
- **Visual Feedback**: Hover effects, color-coded buttons, and status indicators

### 📋 Level Management
- **Load/Save Levels**: Import and export level data from JSON files
- **Create New Levels**: Add new levels with automatic numbering
- **Duplicate Levels**: Clone existing levels for quick iteration
- **Delete Levels**: Remove unwanted levels with confirmation dialog
- **Search Functionality**: Filter levels by number or tube count

### 🧪 Tube Editor
- **Visual Tube Cards**: Each tube is displayed as a card with 4 color slots
- **Add/Remove Colors**: Easily modify tube contents
- **Clear Tubes**: Reset tube contents with one click
- **Remove Tubes**: Delete entire tubes from the level

### 🔒 Lock Management
- **Lock/Unlock Tubes**: Mark specific tubes as locked
- **Visual Display**: See all locked tubes at a glance
- **Clear All Locks**: Remove all locks quickly

### ✅ Validation
- **Level Validation**: Check level integrity before saving
- **Visual Feedback**: Color-coded validation results (green for success, red for errors)

## Architecture

### File Structure
```
Editor/
├── TubeColorLevelEditorWindow.cs    # Main editor window (UI Toolkit)
├── Resources/
│   ├── TubeColorLevelEditor.uxml    # UI layout definition
│   ├── TubeColorLevelEditor.uss     # Stylesheet
│   └── color_sort_1000_levels.json  # Level data
```

### Key Components

#### TubeColorLevelEditorWindow.cs
- **CreateGUI()**: Initializes the UI Toolkit interface
- **SetupLevelListView()**: Configures the ListView for level selection
- **DisplayLevelEditor()**: Shows the level editor panel
- **RefreshTubesDisplay()**: Updates the tubes display
- **CreateTubeCard()**: Generates a visual card for each tube

#### TubeColorLevelEditor.uxml
- **Header**: Title and subtitle
- **Toolbar**: Action buttons (Load, Save, Browse, New, Duplicate, Delete)
- **Main Content**: Split view with level list and editor
- **Status Bar**: Display current operation status

#### TubeColorLevelEditor.uss
- **Color Palette**: Unity Editor-inspired color scheme
- **Layout Classes**: Flexbox-based responsive layout
- **Component Styles**: Buttons, fields, cards, and containers
- **State Styles**: Hover, active, selected states

## Usage Guide

### Opening the Editor
1. In Unity, go to **Tools > Tube Color Level Editor > Open Editor**
2. The editor window will open with a modern UI

### Loading Levels
1. Click the **Load Levels** button in the toolbar
2. The editor will load levels from the default JSON path
3. Or click **Browse...** to select a different JSON file

### Creating a New Level
1. Click **New Level** in the toolbar
2. A new level will be created with automatic numbering
3. The level will appear in the list on the left

### Editing a Level
1. Select a level from the list on the left
2. The right panel will show the level editor
3. Modify level properties:
   - Level Number
   - Empty Tubes
   - Max Moves
   - Time Limit
   - Available Swaps

### Managing Tubes
1. Click **+ Add Tube** to add a new tube to the level
2. Each tube card shows:
   - Tube number and color count (e.g., "Tube 1 (3/4)")
   - 4 color slots
   - Action buttons (Add, Clear, Remove)
3. Modify colors:
   - Enter color values in the integer fields
   - Click **✕** next to a color to remove it
   - Click **+ Add** to add a new color (if space available)
4. Click **Clear** to empty the tube
5. Click **Remove** to delete the tube

### Locking Tubes
1. Enter a tube index in the "Lock Tube Index" field
2. Click **Lock** to lock the tube
3. Click **Unlock** to unlock the tube
4. Click **Clear All Locks** to remove all locks

### Validating Levels
1. Click **Validate Level** in the Validation section
2. The editor will check:
   - Level has at least one tube
   - Empty tubes count is valid
3. Results are displayed with color coding

### Saving Levels
1. Make your changes
2. Click **Save Levels** in the toolbar
3. Levels will be saved to the current JSON file
4. A confirmation message appears in the status bar

## UI Toolkit Advantages

### Performance
- **Efficient Rendering**: UI Toolkit uses a retained mode approach
- **Faster Updates**: Only changed elements are redrawn
- **Better for Large Datasets**: ListView virtualization for hundreds of levels

### Maintainability
- **Separation of Concerns**: UI layout (UXML) separate from styling (USS) and logic (C#)
- **Declarative UI**: UXML is easier to read and modify than IMGUI code
- **Reusable Styles**: USS classes can be reused across elements

### Modern Features
- **Flexbox Layout**: Responsive design that adapts to window size
- **CSS-like Styling**: Familiar syntax for web developers
- **Data Binding**: (Can be extended) Automatic UI updates when data changes
- **Event System**: Cleaner event handling with lambda expressions

## Customization

### Modifying Colors
Edit `TubeColorLevelEditor.uss` to change colors:
```css
.primary-button {
    background-color: #007acc;  /* Change to your preferred color */
    color: #ffffff;
}
```

### Adding New Fields
1. Add field to `TubeColorLevelEditor.uxml`:
```xml
<ui:IntegerField name="newField" label="New Field" class="property-field"/>
```

2. Add styling to `TubeColorLevelEditor.uss`:
```css
.property-field {
    /* Your styles */
}
```

3. Add logic to `TubeColorLevelEditorWindow.cs`:
```csharp
var newField = root.Q<IntegerField>("newField");
newField.RegisterValueChangedCallback(evt => {
    if (selectedLevel != null) selectedLevel.newProperty = evt.newValue;
});
```

### Extending the Tube Card
Modify `CreateTubeCard()` method to add more functionality:
```csharp
private VisualElement CreateTubeCard(int tubeIndex)
{
    var card = new VisualElement();
    card.AddToClassList("tube-card");
    
    // Add your custom elements here
    
    return card;
}
```

## Comparison: IMGUI vs UI Toolkit

| Feature | IMGUI (Old) | UI Toolkit (New) |
|---------|-------------|------------------|
| **Code Readability** | Mixed UI and logic | Separated UI/logic |
| **Performance** | Redraws every frame | Retained mode |
| **Styling** | Hardcoded in C# | CSS-like USS files |
| **Layout** | Manual positioning | Flexbox layout |
| **Maintainability** | Harder to modify | Easier to modify |
| **Learning Curve** | Unity-specific | Web-like (familiar) |
| **Visual Design** | Basic | Modern and polished |

## Best Practices

1. **Keep UI and Logic Separate**: Use UXML for structure, USS for styling, C# for logic
2. **Use Meaningful Names**: Name your UI elements clearly (e.g., "loadButton", "levelListView")
3. **Add CSS Classes**: Use classes for styling instead of inline styles
4. **Leverage ListView**: Use ListView for large lists to maintain performance
5. **Handle Edge Cases**: Always check for null before accessing UI elements
6. **Provide Feedback**: Update the status bar to inform users of actions
7. **Validate Input**: Check user input before processing

## Troubleshooting

### UXML/USS Not Loading
- Ensure files are in the `Resources` folder
- Check that file names match exactly (case-sensitive)
- Verify files have correct extensions (.uxml, .uss)

### UI Elements Not Responding
- Check that element names in C# match UXML names
- Verify event handlers are registered in `SetupEventHandlers()`
- Use Debug.Log to check if elements are found

### Styling Not Applied
- Check USS selector syntax
- Verify class names match between UXML and USS
- Use Unity's UI Toolkit Debugger (Window > UI Toolkit > Debugger)

## Future Enhancements

- **Drag and Drop**: Reorder tubes by dragging
- **Undo/Redo**: Full undo/redo support
- **Preview**: Visual preview of tube colors
- **Export**: Export individual levels
- **Templates**: Level templates for quick creation
- **Batch Operations**: Apply changes to multiple levels

## Resources

- [Unity UI Toolkit Documentation](https://docs.unity3d.com/Manual/UIElements.html)
- [USS Selectors Reference](https://docs.unity3d.com/Manual/UIE-USS-Selectors.html)
- [UXML Format Reference](https://docs.unity3d.com/Manual/UIE-UXML.html)
- [UI Toolkit Best Practices](https://docs.unity3d.com/Manual/UIE-BestPractices.html)

---

**Version**: 2.0  
**Last Updated**: October 2025  
**Unity Version**: 2021.3 LTS+
