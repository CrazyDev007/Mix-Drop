# Migration Guide: IMGUI to UI Toolkit

## Overview
This guide helps you understand the migration from the old IMGUI-based Tube Color Level Editor to the new UI Toolkit version.

## Key Differences

### Before (IMGUI)
```csharp
private void OnGUI()
{
    EditorGUILayout.LabelField("Tube Color Level Editor", EditorStyles.boldLabel);
    
    if (GUILayout.Button("Load Levels", GUILayout.Width(120)))
    {
        LoadLevels();
    }
    
    selectedLevelIndex = EditorGUILayout.IntSlider("Select Level", selectedLevelIndex, 0, levels.Count - 1);
}
```

### After (UI Toolkit)
```csharp
public void CreateGUI()
{
    var visualTree = Resources.Load<VisualTreeAsset>("TubeColorLevelEditor");
    visualTree.CloneTree(rootVisualElement);
    
    var loadButton = root.Q<Button>("loadButton");
    loadButton.clicked += LoadLevels;
    
    levelListView = root.Q<ListView>("levelListView");
    levelListView.itemsSource = levels;
}
```

## Migration Steps

### Step 1: Create UXML Layout
1. Create `TubeColorLevelEditor.uxml` in `Editor/Resources/`
2. Define UI structure using XML-like syntax
3. Assign unique names to interactive elements

### Step 2: Create USS Stylesheet
1. Create `TubeColorLevelEditor.uss` in `Editor/Resources/`
2. Define styles using CSS-like syntax
3. Use classes for reusable styles

### Step 3: Update C# Code
1. Replace `OnGUI()` with `CreateGUI()`
2. Load UXML and USS resources
3. Query UI elements using `root.Q<Type>("elementName")`
4. Register event handlers using `element.clicked += Method`

### Step 4: Update Data Binding
1. Use `RegisterValueChangedCallback` for fields
2. Use `ListView.itemsSource` for lists
3. Call `Rebuild()` to refresh ListView

## Code Patterns

### Pattern 1: Button Handling

**IMGUI:**
```csharp
if (GUILayout.Button("Save"))
{
    SaveLevels();
}
```

**UI Toolkit:**
```csharp
var saveButton = root.Q<Button>("saveButton");
saveButton.clicked += SaveLevels;
```

### Pattern 2: Text Field

**IMGUI:**
```csharp
level.maxMoves = EditorGUILayout.IntField("Max Moves", level.maxMoves);
```

**UI Toolkit:**
```csharp
var maxMovesField = root.Q<IntegerField>("maxMovesField");
maxMovesField.value = level.maxMoves;
maxMovesField.RegisterValueChangedCallback(evt => {
    level.maxMoves = evt.newValue;
});
```

### Pattern 3: List Display

**IMGUI:**
```csharp
for (int i = 0; i < levels.Count; i++)
{
    if (GUILayout.Button($"Level {levels[i].level}"))
    {
        selectedLevelIndex = i;
    }
}
```

**UI Toolkit:**
```csharp
levelListView.makeItem = () => new Label();
levelListView.bindItem = (element, index) => {
    var label = element as Label;
    label.text = $"Level {levels[index].level}";
};
levelListView.itemsSource = levels;
levelListView.selectionChanged += OnLevelSelected;
```

### Pattern 4: Styling

**IMGUI:**
```csharp
var style = new GUIStyle(EditorStyles.boldLabel);
style.normal.textColor = Color.white;
EditorGUILayout.LabelField("Title", style);
```

**UI Toolkit:**
```css
.title-label {
    -unity-font-style: bold;
    color: #ffffff;
}
```

```csharp
var label = new Label("Title");
label.AddToClassList("title-label");
```

## Element Query Reference

### Basic Query
```csharp
// By name
var element = root.Q("elementName");
var button = root.Q<Button>("buttonName");

// By class
var elements = root.Query(className: "my-class").ToList();

// By type
var buttons = root.Query<Button>().ToList();
```

### Complex Queries
```csharp
// Descendants
var nestedElement = root.Q<VisualElement>("parent").Q<Button>("child");

// Multiple conditions
var element = root.Q<Button>(name: "myButton", className: "primary");
```

## Event Handling

### Button Events
```csharp
button.clicked += () => {
    Debug.Log("Button clicked");
};
```

### Field Events
```csharp
field.RegisterValueChangedCallback(evt => {
    Debug.Log($"Value changed from {evt.previousValue} to {evt.newValue}");
});
```

### Mouse Events
```csharp
element.RegisterCallback<MouseDownEvent>(evt => {
    Debug.Log("Mouse down");
});
```

## Common Pitfalls

### ❌ Pitfall 1: Null References
```csharp
// Bad: No null check
var button = root.Q<Button>("missingButton");
button.clicked += DoSomething; // NullReferenceException!
```

```csharp
// Good: Check for null
var button = root.Q<Button>("buttonName");
if (button != null)
{
    button.clicked += DoSomething;
}
```

### ❌ Pitfall 2: Wrong Element Type
```csharp
// Bad: Querying wrong type
var field = root.Q<TextField>("integerField"); // Returns null if it's IntegerField
```

```csharp
// Good: Use correct type
var field = root.Q<IntegerField>("integerField");
```

### ❌ Pitfall 3: Not Rebuilding ListView
```csharp
// Bad: ListView won't update
levels.Add(newLevel);
```

```csharp
// Good: Rebuild after changes
levels.Add(newLevel);
levelListView.Rebuild();
```

### ❌ Pitfall 4: Inline Styles
```csharp
// Bad: Inline styles (harder to maintain)
element.style.backgroundColor = new StyleColor(Color.red);
```

```csharp
// Good: Use USS classes
element.AddToClassList("error-state");
```

```css
.error-state {
    background-color: #d73a49;
}
```

## Performance Tips

### 1. Use ListView for Large Lists
- ListView virtualizes items (only renders visible ones)
- Handles thousands of items efficiently

### 2. Cache Element Queries
```csharp
// Bad: Query every frame
void Update()
{
    var label = root.Q<Label>("statusLabel");
    label.text = GetStatus();
}

// Good: Cache query result
Label statusLabel;
void CreateGUI()
{
    statusLabel = root.Q<Label>("statusLabel");
}
void Update()
{
    statusLabel.text = GetStatus();
}
```

### 3. Use USS Classes for Styling
- USS is more performant than inline styles
- Allows Unity to batch style changes

### 4. Minimize Rebuilds
- Only call `ListView.Rebuild()` when data changes
- Use `RefreshItem()` to update individual items

## Debugging Tips

### 1. Use UI Toolkit Debugger
```
Window > UI Toolkit > Debugger
```
- Inspect element hierarchy
- View applied styles
- Debug layout issues

### 2. Log Element Queries
```csharp
var element = root.Q("elementName");
Debug.Log($"Element found: {element != null}");
```

### 3. Check Element Names
- Names in UXML must match C# queries exactly
- Names are case-sensitive

### 4. Verify Resource Loading
```csharp
var visualTree = Resources.Load<VisualTreeAsset>("TubeColorLevelEditor");
if (visualTree == null)
{
    Debug.LogError("Could not load UXML");
    return;
}
```

## Benefits Summary

| Aspect | IMGUI | UI Toolkit |
|--------|-------|------------|
| **Performance** | Redraws every frame | Retained mode, only updates when changed |
| **Code Organization** | Mixed UI/logic | Separated: UXML + USS + C# |
| **Learning Curve** | Unity-specific | Similar to web development |
| **Maintainability** | Harder to modify | Easier to modify |
| **Styling** | Programmatic | Declarative (CSS-like) |
| **Layout** | Manual | Flexbox (automatic) |
| **Scalability** | Poor with large datasets | Excellent with ListView |
| **Modern Features** | Limited | Rich (data binding, animations, etc.) |

## Checklist for Migration

- [ ] Create UXML layout file
- [ ] Create USS stylesheet file
- [ ] Replace `OnGUI()` with `CreateGUI()`
- [ ] Load UXML and USS resources
- [ ] Query all UI elements
- [ ] Register event handlers
- [ ] Test button clicks
- [ ] Test field value changes
- [ ] Test ListView selection
- [ ] Verify styling
- [ ] Test search functionality
- [ ] Test validation
- [ ] Test load/save operations
- [ ] Remove old IMGUI code
- [ ] Update documentation

## Additional Resources

- [Unity UI Toolkit Manual](https://docs.unity3d.com/Manual/UIElements.html)
- [UI Toolkit Samples](https://github.com/Unity-Technologies/ui-toolkit-sample)
- [UXML Element Reference](https://docs.unity3d.com/Manual/UIE-ElementRef.html)
- [USS Properties Reference](https://docs.unity3d.com/Manual/UIE-USS-Properties-Reference.html)

---

**Congratulations!** You've successfully migrated from IMGUI to UI Toolkit. The new editor is more maintainable, performant, and provides a better user experience.
