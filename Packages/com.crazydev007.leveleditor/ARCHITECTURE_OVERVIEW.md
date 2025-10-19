# Tube Color Level Editor - Architecture Overview

## System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    TUBE COLOR LEVEL EDITOR                 │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │           EDITOR LAYER (Editor-Only)               │   │
│  ├─────────────────────────────────────────────────────┤   │
│  │                                                     │   │
│  │  ┌────────────────────────────────────────────┐   │   │
│  │  │  TubeColorLevelEditorWindow                │   │   │
│  │  │  (Main Editor UI)                          │   │   │
│  │  ├────────────────────────────────────────────┤   │   │
│  │  │  - Load/Save Levels                        │   │   │
│  │  │  - Navigate Levels                         │   │   │
│  │  │  - Edit Tube Colors                        │   │   │
│  │  │  - Edit Level Properties                   │   │   │
│  │  │  - Lock/Unlock Tubes                       │   │   │
│  │  └────────────────────────────────────────────┘   │   │
│  │                                                     │   │
│  │  ┌────────────────────────────────────────────┐   │   │
│  │  │  TubeColorLevelMenu                        │   │   │
│  │  │  (Utility Tools)                           │   │   │
│  │  ├────────────────────────────────────────────┤   │   │
│  │  │  - Validate All Levels                     │   │   │
│  │  │  - Export Level Stats                      │   │   │
│  │  │  - Find Problem Levels                     │   │   │
│  │  │  - Quick Test                              │   │   │
│  │  └────────────────────────────────────────────┘   │   │
│  │                                                     │   │
│  │  ┌────────────────────────────────────────────┐   │   │
│  │  │  Property Drawers                          │   │   │
│  │  ├────────────────────────────────────────────┤   │   │
│  │  │  - TubeDataPropertyDrawer                  │   │   │
│  │  │  - TubeColorLevelDrawer                    │   │   │
│  │  └────────────────────────────────────────────┘   │   │
│  │                                                     │   │
│  └─────────────────────────────────────────────────────┘   │
│                            ↓                                │
│                    (uses & manages)                         │
│                            ↓                                │
│  ┌─────────────────────────────────────────────────────┐   │
│  │          RUNTIME LAYER (Gameplay)                  │   │
│  ├─────────────────────────────────────────────────────┤   │
│  │                                                     │   │
│  │  ┌────────────────────────────────────────────┐   │   │
│  │  │  Data Classes                              │   │   │
│  │  ├────────────────────────────────────────────┤   │   │
│  │  │  TubeData                                  │   │   │
│  │  │  ├─ values: List<int>                      │   │   │
│  │  │  ├─ AddColor()                             │   │   │
│  │  │  ├─ RemoveTopColor()                       │   │   │
│  │  │  ├─ IsFull() / IsEmpty()                   │   │   │
│  │  │  └─ ClearTube()                            │   │   │
│  │  │                                             │   │   │
│  │  │  TubeColorLevel                            │   │   │
│  │  │  ├─ level: int                             │   │   │
│  │  │  ├─ tubes: List<TubeData>                  │   │   │
│  │  │  ├─ emptyTubes: int                        │   │   │
│  │  │  ├─ maxMoves: int                          │   │   │
│  │  │  ├─ timeLimit: int                         │   │   │
│  │  │  ├─ lockedTubes: List<int>                 │   │   │
│  │  │  └─ availableSwaps: int                    │   │   │
│  │  └────────────────────────────────────────────┘   │   │
│  │                                                     │   │
│  │  ┌────────────────────────────────────────────┐   │   │
│  │  │  Serialization                             │   │   │
│  │  ├────────────────────────────────────────────┤   │   │
│  │  │  TubeColorLevelSerializer                  │   │   │
│  │  │  ├─ SaveLevels()                           │   │   │
│  │  │  ├─ LoadLevels()                           │   │   │
│  │  │  └─ LoadSingleLevel()                      │   │   │
│  │  └────────────────────────────────────────────┘   │   │
│  │                                                     │   │
│  │  ┌────────────────────────────────────────────┐   │   │
│  │  │  Utilities & Validation                    │   │   │
│  │  ├────────────────────────────────────────────┤   │   │
│  │  │  TubeColorLevelValidator                   │   │   │
│  │  │  ├─ ValidateLevel()                        │   │   │
│  │  │  └─ IsLevelValid()                         │   │   │
│  │  │                                             │   │   │
│  │  │  TubeColorLevelHelper                      │   │   │
│  │  │  ├─ CountColors()                          │   │   │
│  │  │  ├─ GetTotalColorCount()                   │   │   │
│  │  │  ├─ CanBeSolved()                          │   │   │
│  │  │  ├─ ShuffleTubeColors()                    │   │   │
│  │  │  └─ FillTubeWithColor()                    │   │   │
│  │  └────────────────────────────────────────────┘   │   │
│  │                                                     │   │
│  └─────────────────────────────────────────────────────┘   │
│                            ↓                                │
│                       (reads/writes)                        │
│                            ↓                                │
│  ┌─────────────────────────────────────────────────────┐   │
│  │              JSON DATA STORAGE                      │   │
│  ├─────────────────────────────────────────────────────┤   │
│  │                                                     │   │
│  │  color_sort_1000_levels.json                       │   │
│  │  ┌─────────────────────────────────────────────┐   │   │
│  │  │ {                                           │   │   │
│  │  │   "levels": [                              │   │   │
│  │  │     {                                       │   │   │
│  │  │       "level": 1,                           │   │   │
│  │  │       "tubes": [{"values": [1,1,2,2]}, ...],   │   │
│  │  │       "emptyTubes": 1,                      │   │   │
│  │  │       "maxMoves": 0,                        │   │   │
│  │  │       ...                                   │   │   │
│  │  │     },                                      │   │   │
│  │  │     ...1000 levels...                       │   │   │
│  │  │   ]                                         │   │   │
│  │  │ }                                           │   │   │
│  │  └─────────────────────────────────────────────┘   │   │
│  │                                                     │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

## Data Flow Diagram

```
┌──────────────────┐
│  Editor Window   │
│  (User Input)    │
└────────┬─────────┘
         │
         ├─── Load ───→ TubeColorLevelSerializer ──→ JSON File
         │
         ├─── Edit ───→ TubeColorLevel/TubeData
         │
         ├─── Validate ─→ TubeColorLevelValidator
         │
         ├─── Analyze ──→ TubeColorLevelHelper
         │
         └─── Save ────→ TubeColorLevelSerializer ──→ JSON File
```

## Class Dependency Graph

```
TubeColorLevelEditorWindow
    │
    ├─ TubeColorLevelSerializer
    │   └─ TubeColorLevelsWrapper
    │       └─ TubeColorLevel
    │           └─ TubeData
    │
    ├─ TubeColorLevelValidator
    │   └─ TubeColorLevel
    │       └─ TubeData
    │
    └─ TubeColorLevelHelper
        └─ TubeColorLevel
            └─ TubeData

TubeColorLevelMenu
    ├─ TubeColorLevelSerializer
    ├─ TubeColorLevelValidator
    └─ TubeColorLevelHelper
```

## File Organization

```
com.crazydev007.leveleditor/
│
├── Runtime/
│   ├── Data/
│   │   ├── TubeData.cs
│   │   ├── TubeColorLevel.cs
│   │   └── ... (other data classes)
│   │
│   ├── Utilities/
│   │   └── TubeColorLevelUtilities.cs
│   │       ├── TubeColorLevelValidator
│   │       └── TubeColorLevelHelper
│   │
│   ├── Serialization/
│   │   └── TubeColorLevelSerializer.cs
│   │
│   ├── Components/
│   │   └── ... (gameplay components)
│   │
│   └── Examples/
│       └── TubeColorLevelEditorExamples.cs
│
├── Editor/
│   ├── TubeColorLevelEditorWindow.cs (Main UI)
│   ├── TubeColorLevelMenu.cs (Tools)
│   ├── Inspectors/
│   │   ├── TubeDataPropertyDrawer.cs
│   │   └── TubeColorLevelDrawer.cs
│   └── ... (other editor tools)
│
├── Resources/
│   └── color_sort_1000_levels.json (Data)
│
└── Documentation/
    ├── TUBE_COLOR_EDITOR_GUIDE.md
    ├── QUICK_REFERENCE.md
    ├── IMPLEMENTATION_SUMMARY.md
    ├── SETUP_VERIFICATION.md
    └── README.md
```

## Feature Breakdown

### Editor Window (TubeColorLevelEditorWindow)
```
┌─────────────────────────────────────┐
│  File Operations Section            │
├─────────────────────────────────────┤
│  [Load Levels] [Save Levels]        │
│  File Path: ...                     │
└─────────────────────────────────────┘
         ↓
┌─────────────────────────────────────┐
│  Navigation Section                 │
├─────────────────────────────────────┤
│  Total Levels: 1000                 │
│  Select Level: [===●===========] 500 │
└─────────────────────────────────────┘
         ↓
┌─────────────────────────────────────┐
│  Level Editor Section               │
├─────────────────────────────────────┤
│  Level Properties:                  │
│  - Empty Tubes                      │
│  - Max Moves                        │
│  - Time Limit                       │
│  - Available Swaps                  │
│                                     │
│  Tube Editor (Scrollable):          │
│  - View/Edit Tubes                  │
│  - Manage Colors                    │
│  - Lock/Unlock                      │
│                                     │
│  Operations:                        │
│  - Add Tube                         │
│  - Remove Tube                      │
│  - Lock Tube                        │
└─────────────────────────────────────┘
```

### Menu Tools (TubeColorLevelMenu)
```
Validate All Levels
    ├─ Check level integrity
    ├─ Verify empty tubes
    ├─ Check locked indices
    └─ Report results

Export Level Stats
    ├─ Count tubes per level
    ├─ Count colors per level
    ├─ Check solvability
    └─ Generate report file

Find Problem Levels
    ├─ Validate each level
    ├─ Check solvability
    └─ List problem levels

Quick Test
    └─ Display first level info
```

## Data Model

```
JSON File
    └─ TubeColorLevelsWrapper
        └─ levels: List<TubeColorLevel>
            ├─ level: int
            ├─ tubes: List<TubeData>
            │   └─ values: List<int>
            ├─ emptyTubes: int
            ├─ maxMoves: int
            ├─ timeLimit: int
            ├─ lockedTubes: List<int>
            ├─ availableSwaps: int
            └─ twists: List<object>
```

## Namespace Organization

```
CrazyDev007.LevelEditor (Runtime)
    ├─ TubeData
    ├─ TubeColorLevel
    ├─ TubeColorLevelsWrapper
    ├─ TubeColorLevelSerializer
    ├─ TubeColorLevelValidator
    ├─ TubeColorLevelHelper
    └─ Examples
        └─ TubeColorLevelEditorExamples

CrazyDev007.LevelEditor.Editor (Editor Only)
    ├─ TubeColorLevelEditorWindow
    ├─ TubeColorLevelMenu
    ├─ TubeDataPropertyDrawer
    └─ TubeColorLevelDrawer
```

## Usage Flow

```
User Action          Handler             Data Model          Storage
═══════════════════════════════════════════════════════════════════

Open Editor    →  EditorWindow    
               
Load File      →  EditorWindow    →  Serializer    →  JSON File
               
Select Level   →  EditorWindow    →  TubeColorLevel
               
Edit Tube      →  EditorWindow    →  TubeData
               
Add Color      →  EditorWindow    →  TubeData
               
Validate       →  EditorWindow    →  Validator    
               
Save File      →  EditorWindow    →  Serializer    →  JSON File
```

---

This architecture ensures:
- ✅ Clear separation of concerns
- ✅ Easy to test and debug
- ✅ Reusable components
- ✅ Easy to extend
- ✅ Performance optimized
- ✅ Well-organized code
