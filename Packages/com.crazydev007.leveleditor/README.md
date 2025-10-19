# Level Editor Tool

## Overview
The Level Editor Tool is a custom editor extension for Unity that facilitates the creation and management of game levels. It provides a user-friendly interface for placing objects, configuring level settings, and customizing terrain features.

## Features
- **Level Creation**: Easily create and edit levels using a visual interface.
- **Object Placement**: Place and configure objects within the level, with support for grid snapping.
- **Terrain Painting**: Paint terrain features, including textures and height adjustments.
- **Custom Inspectors**: Enhanced inspectors for level data and placeable objects, allowing for intuitive property editing.

## Project Structure
```
LevelEditorTool
├── Editor
│   ├── LevelEditorWindow.cs
│   ├── LevelEditorSettings.cs
│   ├── Inspectors
│   │   ├── LevelDataInspector.cs
│   │   └── ObjectPlacementInspector.cs
│   ├── Tools
│   │   ├── ObjectPlacementTool.cs
│   │   ├── GridSnappingTool.cs
│   │   └── TerrainPaintTool.cs
│   └── Resources
│       ├── EditorStyles.uss
│       └── LevelEditorWindow.uxml
├── Runtime
│   ├── Data
│   │   ├── LevelData.cs
│   │   ├── ObjectPlacementData.cs
│   │   └── LevelConfiguration.cs
│   ├── Components
│   │   ├── PlaceableObject.cs
│   │   └── LevelBounds.cs
│   └── Serialization
│       ├── LevelSerializer.cs
│       └── LevelDeserializer.cs
├── Tests
│   ├── Editor
│   │   └── LevelEditorTests.cs
│   └── Runtime
│       └── LevelDataTests.cs
├── package.json
└── README.md
```

## Usage Instructions
1. Open the Level Editor Tool from the Unity menu.
2. Use the Object Placement Tool to add objects to your level.
3. Adjust settings in the Level Editor Settings to customize grid size and snapping options.
4. Utilize the Terrain Paint Tool to modify terrain features.
5. Save your level using the serialization features provided.

## Contribution
Contributions to the Level Editor Tool are welcome. Please submit a pull request or open an issue for any enhancements or bug fixes.

## License
This project is licensed under the MIT License. See the LICENSE file for more details.