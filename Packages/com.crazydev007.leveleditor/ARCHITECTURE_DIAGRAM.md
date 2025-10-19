# Level Editor Architecture Diagram

## System Overview

```
┌────────────────────────────────────────────────────────────────┐
│                    Unity Editor Window                         │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │          TubeColorLevelEditorWindow.cs                   │ │
│  │                  (Controller/Logic)                       │ │
│  └────────────┬────────────────────────────┬────────────────┘ │
│               │                             │                   │
│               │ Loads                       │ Queries           │
│               ▼                             ▼                   │
│  ┌──────────────────────┐    ┌──────────────────────────────┐ │
│  │ TubeColorLevelEditor │    │ TubeColorLevelEditor.uss     │ │
│  │        .uxml          │    │       (Stylesheet)           │ │
│  │    (UI Structure)     │    │                              │ │
│  └──────────────────────┘    └──────────────────────────────┘ │
└────────────────────────────────────────────────────────────────┘
                         │
                         │ Updates/Reads
                         ▼
            ┌────────────────────────┐
            │   Data Model Layer     │
            │  ┌──────────────────┐  │
            │  │ TubeColorLevel   │  │
            │  │   - level        │  │
            │  │   - tubes[]      │  │
            │  │   - properties   │  │
            │  └────────┬─────────┘  │
            │           │             │
            │           │ Contains    │
            │           ▼             │
            │  ┌──────────────────┐  │
            │  │    TubeData      │  │
            │  │   - values[]     │  │
            │  └──────────────────┘  │
            └────────────────────────┘
                         │
                         │ Serializes
                         ▼
            ┌────────────────────────┐
            │ TubeColorLevelSerializer│
            └────────────┬───────────┘
                         │
                         │ Reads/Writes
                         ▼
            ┌────────────────────────┐
            │  color_sort_1000_      │
            │    levels.json          │
            │    (Data File)          │
            └────────────────────────┘
```

## UI Component Hierarchy

```
Root (VisualElement)
│
├── Header (VisualElement)
│   ├── Title (Label)
│   └── Subtitle (Label)
│
├── Toolbar (VisualElement)
│   ├── Toolbar Group 1
│   │   ├── Load Button
│   │   ├── Save Button
│   │   └── Browse Button
│   └── Toolbar Group 2
│       ├── New Level Button
│       ├── Duplicate Button
│       └── Delete Button
│
├── File Info (VisualElement)
│   ├── Label: "File Path:"
│   └── File Path Label (dynamic)
│
├── Main Content (VisualElement - Split View)
│   │
│   ├── Left Panel (VisualElement)
│   │   ├── Panel Header
│   │   │   ├── Title: "Levels"
│   │   │   └── Level Count Label
│   │   ├── Search Container
│   │   │   └── Search Field
│   │   └── Level List (ListView)
│   │       └── Level Items (dynamic)
│   │
│   └── Right Panel (VisualElement)
│       │
│       ├── No Selection View (VisualElement)
│       │   ├── Title: "No Level Selected"
│       │   └── Subtitle
│       │
│       └── Level Editor (ScrollView)
│           │
│           ├── Level Header
│           │   └── Level Title
│           │
│           ├── Properties Section
│           │   ├── Level Number Field
│           │   ├── Empty Tubes Field
│           │   ├── Max Moves Field
│           │   ├── Time Limit Field
│           │   └── Available Swaps Field
│           │
│           ├── Tubes Section
│           │   ├── Section Header + Add Button
│           │   └── Tubes Container
│           │       └── Tube Cards (dynamic)
│           │           ├── Tube Header
│           │           ├── Color Slots (4x)
│           │           └── Action Buttons
│           │
│           ├── Locked Tubes Section
│           │   ├── Lock Controls
│           │   ├── Locked Tubes Display
│           │   └── Clear Locks Button
│           │
│           └── Validation Section
│               ├── Validate Button
│               └── Validation Result Label
│
└── Status Bar (VisualElement)
    └── Status Label
```

## Data Flow Diagram

```
┌─────────────┐
│    User     │
│  Interaction│
└──────┬──────┘
       │
       │ Click/Edit
       ▼
┌────────────────────────┐
│   UI Element Event     │
│  (Button/Field/List)   │
└──────┬─────────────────┘
       │
       │ Triggers
       ▼
┌────────────────────────┐
│  Event Handler (C#)    │
│  - LoadLevels()        │
│  - SaveLevels()        │
│  - AddTube()           │
│  - etc.                │
└──────┬─────────────────┘
       │
       │ Updates
       ▼
┌────────────────────────┐
│   Data Model           │
│  - levels[]            │
│  - selectedLevel       │
└──────┬─────────────────┘
       │
       │ Reflects Changes
       ▼
┌────────────────────────┐
│   UI Update            │
│  - RefreshList()       │
│  - RefreshTubes()      │
│  - UpdateStatus()      │
└──────┬─────────────────┘
       │
       │ Renders
       ▼
┌────────────────────────┐
│   User Sees            │
│   Updated UI           │
└────────────────────────┘
```

## File Load/Save Flow

```
┌──────────────┐
│  User clicks │
│ Load/Browse  │
└──────┬───────┘
       │
       ▼
┌──────────────────┐
│ BrowseJsonFile() │
│ or LoadLevels()  │
└──────┬───────────┘
       │
       ▼
┌─────────────────────┐
│ TubeColorLevel      │
│ Serializer          │
│ .LoadLevels()       │
└──────┬──────────────┘
       │
       │ Reads JSON
       ▼
┌─────────────────────┐
│ color_sort_1000_    │
│ levels.json         │
└──────┬──────────────┘
       │
       │ Deserializes
       ▼
┌─────────────────────┐
│ List<TubeColorLevel>│
└──────┬──────────────┘
       │
       │ Stores
       ▼
┌─────────────────────┐
│ levels field        │
└──────┬──────────────┘
       │
       │ Displays
       ▼
┌─────────────────────┐
│ ListView updates    │
│ UI refreshes        │
└─────────────────────┘

┌──────────────┐
│  User clicks │
│     Save     │
└──────┬───────┘
       │
       ▼
┌──────────────────┐
│  SaveLevels()    │
└──────┬───────────┘
       │
       ▼
┌─────────────────────┐
│ TubeColorLevel      │
│ Serializer          │
│ .SaveLevels()       │
└──────┬──────────────┘
       │
       │ Serializes
       ▼
┌─────────────────────┐
│ JSON string         │
└──────┬──────────────┘
       │
       │ Writes
       ▼
┌─────────────────────┐
│ color_sort_1000_    │
│ levels.json         │
└─────────────────────┘
```

## Event Flow for Tube Editing

```
┌────────────────────┐
│ User selects Level │
└────────┬───────────┘
         │
         ▼
┌──────────────────────┐
│ OnLevelSelected()    │
│ - Sets selectedLevel │
└────────┬─────────────┘
         │
         ▼
┌──────────────────────┐
│ DisplayLevelEditor() │
│ - Shows editor panel │
│ - Updates fields     │
└────────┬─────────────┘
         │
         ▼
┌──────────────────────┐
│ RefreshTubesDisplay()│
└────────┬─────────────┘
         │
         │ For each tube
         ▼
┌──────────────────────┐
│  CreateTubeCard(i)   │
│  - Creates card      │
│  - Adds color slots  │
│  - Adds buttons      │
└────────┬─────────────┘
         │
         │ Registers events
         ▼
┌──────────────────────────┐
│ Button Event Handlers    │
│ - Add color: AddColor()  │
│ - Remove: RemoveAt()     │
│ - Clear: ClearTube()     │
└────────┬─────────────────┘
         │
         │ On action
         ▼
┌──────────────────────────┐
│ RefreshTubesDisplay()    │
│ (Rebuilds all tubes)     │
└──────────────────────────┘
```

## Styling Cascade

```
┌───────────────────────┐
│ TubeColorLevelEditor  │
│      .uss              │
│  (Global Styles)      │
└──────────┬────────────┘
           │
           │ Applied to
           ▼
┌───────────────────────┐
│  Visual Elements      │
│  with CSS Classes     │
└──────────┬────────────┘
           │
           │ Example:
           ▼
┌────────────────────────────┐
│ <Button class="primary-    │
│          button">          │
│                            │
│ .primary-button {          │
│   background: #007acc;     │
│   color: white;            │
│ }                          │
│                            │
│ .primary-button:hover {    │
│   background: #005a9e;     │
│ }                          │
└────────────────────────────┘
```

## Class Inheritance

```
EditorWindow
    │
    └── TubeColorLevelEditorWindow
            │
            ├── Uses: VisualElement (root)
            │         │
            │         ├── Button (multiple)
            │         ├── Label (multiple)
            │         ├── TextField
            │         ├── IntegerField (multiple)
            │         ├── ListView
            │         ├── ScrollView
            │         └── VisualElement (containers)
            │
            ├── Uses: TubeColorLevel (data)
            │         └── TubeData (nested)
            │
            └── Uses: TubeColorLevelSerializer
```

## MVC Pattern

```
┌─────────────────────┐
│       Model         │
│  ┌───────────────┐  │
│  │TubeColorLevel │  │
│  │  TubeData     │  │
│  └───────────────┘  │
│  - Data structure   │
│  - Business logic   │
└──────────┬──────────┘
           │
           │ Reads/Writes
           ▼
┌─────────────────────┐      ┌─────────────────────┐
│    Controller       │      │        View         │
│  ┌───────────────┐  │      │  ┌───────────────┐  │
│  │TubeColorLevel │  │◄────►│  │    .uxml      │  │
│  │EditorWindow.cs│  │      │  │    .uss       │  │
│  └───────────────┘  │      │  └───────────────┘  │
│  - UI logic         │      │  - Visual layout    │
│  - Event handling   │      │  - Styling          │
│  - Data binding     │      │  - No logic         │
└─────────────────────┘      └─────────────────────┘
```

## Key Design Patterns

### 1. Observer Pattern
```
UI Element → Event → Handler → Update Model → Refresh UI
```

### 2. Factory Pattern
```
CreateTubeCard(index) → Returns configured VisualElement
```

### 3. Template Method Pattern
```
CreateGUI() → InitializeUIElements() → SetupEventHandlers()
```

### 4. Singleton Pattern
```
EditorWindow.GetWindow<T>() → Returns single instance
```

---

## Performance Optimization

### ListView Virtualization
```
┌─────────────────────────────┐
│  ListView with 1000 items   │
│                             │
│  Only renders visible:      │
│  ┌─────────────────┐        │
│  │ Level 45        │ ←── Rendered
│  │ Level 46        │ ←── Rendered
│  │ Level 47        │ ←── Rendered
│  │ Level 48        │ ←── Rendered
│  │ Level 49        │ ←── Rendered
│  └─────────────────┘        │
│                             │
│  Items 1-44: Not rendered   │
│  Items 50-1000: Not rendered│
│                             │
│  Memory: ~5 items           │
│  Not: 1000 items            │
└─────────────────────────────┘
```

### Retained Mode Rendering
```
IMGUI (Immediate Mode):
Frame 1: Draw UI
Frame 2: Draw UI
Frame 3: Draw UI
...every frame...

UI Toolkit (Retained Mode):
Frame 1: Draw UI (when changed)
Frame 2: (no redraw)
Frame 3: (no redraw)
...only when data changes...
```

---

This architecture provides:
- ✅ Clear separation of concerns
- ✅ Maintainable code structure
- ✅ Efficient rendering
- ✅ Scalable design
- ✅ Reusable components
