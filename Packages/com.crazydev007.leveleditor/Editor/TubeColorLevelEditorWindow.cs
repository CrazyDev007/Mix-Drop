using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CrazyDev007.LevelEditor.Editor
{
    public class TubeColorLevelEditorWindow : EditorWindow
    {
        private List<TubeColorLevel> levels = new List<TubeColorLevel>();
        private TubeColorLevelSerializer serializer = new TubeColorLevelSerializer();
        private string jsonFilePath = "Packages/com.crazydev007.leveleditor/Editor/Resources/color_sort_1000_levels.json";
        
        private TubeColorLevel selectedLevel;
        private int selectedLevelIndex = -1;
        
        // UI Elements
        private VisualElement root;
        private ListView levelListView;
        private ScrollView levelEditorScrollView;
        private VisualElement noSelectionContainer;
        private Label filePathLabel;
        private Label levelCountLabel;
        private Label statusLabel;
        private VisualElement tubesContainer;
        private TextField searchField;

        [MenuItem("Tools/Tube Color Level Editor/Open Editor")]
        public static void ShowWindow()
        {
            TubeColorLevelEditorWindow window = GetWindow<TubeColorLevelEditorWindow>("Tube Color Editor");
            window.minSize = new Vector2(900, 600);
        }

        public void CreateGUI()
        {
            // Load UXML
            var visualTree = Resources.Load<VisualTreeAsset>("TubeColorLevelEditor");
            if (visualTree == null)
            {
                Debug.LogError("Could not load TubeColorLevelEditor.uxml");
                return;
            }

            root = rootVisualElement;
            root.style.flexGrow = 1f;
            root.style.flexShrink = 1f;
            root.style.flexDirection = FlexDirection.Column;
            root.style.minHeight = 0f;

            visualTree.CloneTree(root);

            // Load USS
            var styleSheet = Resources.Load<StyleSheet>("TubeColorLevelEditor");
            if (styleSheet != null)
            {
                root.styleSheets.Add(styleSheet);
            }

            // Initialize UI Elements
            InitializeUIElements();
            
            // Setup event handlers
            SetupEventHandlers();
            
            // Initialize state
            UpdateFilePathDisplay();
            UpdateLevelCount();
            UpdateStatusBar("Ready");
        }

        private void InitializeUIElements()
        {
            // Toolbar buttons
            root.Q<Button>("loadButton").clicked += LoadLevels;
            root.Q<Button>("saveButton").clicked += SaveLevels;
            root.Q<Button>("browseButton").clicked += BrowseJsonFile;
            root.Q<Button>("newLevelButton").clicked += CreateNewLevel;
            root.Q<Button>("duplicateButton").clicked += DuplicateLevel;
            root.Q<Button>("deleteButton").clicked += DeleteLevel;

            // Get UI references
            filePathLabel = root.Q<Label>("filePathLabel");
            levelCountLabel = root.Q<Label>("levelCountLabel");
            statusLabel = root.Q<Label>("statusLabel");
            levelListView = root.Q<ListView>("levelListView");
            levelEditorScrollView = root.Q<ScrollView>("levelEditorScrollView");
            noSelectionContainer = root.Q<VisualElement>("no-selection");
            tubesContainer = root.Q<VisualElement>("tubesContainer");
            searchField = root.Q<TextField>("searchField");

            // Setup ListView
            SetupLevelListView();

            ConfigureLayoutConstraints();
        }

        private void SetupEventHandlers()
        {
            // Search functionality
            searchField.RegisterValueChangedCallback(evt => FilterLevels(evt.newValue));

            // Level editor buttons
            root.Q<Button>("addTubeButton").clicked += AddTubeToCurrentLevel;
            root.Q<Button>("lockTubeButton").clicked += LockTube;
            root.Q<Button>("unlockTubeButton").clicked += UnlockTube;
            root.Q<Button>("clearLocksButton").clicked += ClearAllLocks;
            root.Q<Button>("validateButton").clicked += ValidateCurrentLevel;

            // Property fields change handlers
            var levelNumberField = root.Q<IntegerField>("levelNumberField");
            levelNumberField?.RegisterValueChangedCallback(evt => {
                if (selectedLevel != null) {
                    selectedLevel.level = evt.newValue;
                    RefreshLevelList();
                }
            });

            var emptyTubesField = root.Q<IntegerField>("emptyTubesField");
            emptyTubesField?.RegisterValueChangedCallback(evt => {
                if (selectedLevel != null) selectedLevel.emptyTubes = evt.newValue;
            });

            var maxMovesField = root.Q<IntegerField>("maxMovesField");
            maxMovesField?.RegisterValueChangedCallback(evt => {
                if (selectedLevel != null) selectedLevel.maxMoves = evt.newValue;
            });

            var timeLimitField = root.Q<IntegerField>("timeLimitField");
            timeLimitField?.RegisterValueChangedCallback(evt => {
                if (selectedLevel != null) selectedLevel.timeLimit = evt.newValue;
            });

            var availableSwapsField = root.Q<IntegerField>("availableSwapsField");
            availableSwapsField?.RegisterValueChangedCallback(evt => {
                if (selectedLevel != null) selectedLevel.availableSwaps = evt.newValue;
            });
        }

        private void SetupLevelListView()
        {
            levelListView.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
            levelListView.fixedItemHeight = 28f;
            levelListView.selectionType = SelectionType.Single;
            levelListView.style.flexGrow = 1f;
            levelListView.style.flexShrink = 1f;
            levelListView.style.flexBasis = 0f;
            levelListView.style.minHeight = 0f;
            levelListView.style.height = StyleKeyword.Auto;

            levelListView.makeItem = () => new Label();
            levelListView.bindItem = (element, index) => {
                if (index < levels.Count)
                {
                    var label = element as Label;
                    label.text = $"Level {levels[index].level} ({levels[index].tubes.Count} tubes)";
                    label.style.color = new StyleColor(Color.white);
                }
            };

            levelListView.selectionChanged += OnLevelSelected;
            levelListView.itemsSource = levels;
        }

        private void OnLevelSelected(IEnumerable<object> selectedItems)
        {
            var selected = selectedItems.FirstOrDefault();
            if (selected != null && selected is TubeColorLevel level)
            {
                selectedLevel = level;
                selectedLevelIndex = levels.IndexOf(level);
                DisplayLevelEditor(level);
            }
        }

        private void DisplayLevelEditor(TubeColorLevel level)
        {
            noSelectionContainer.style.display = DisplayStyle.None;
            levelEditorScrollView.style.display = DisplayStyle.Flex;

            // Update level title
            root.Q<Label>("levelTitle").text = $"Level {level.level}";

            // Update property fields
            root.Q<IntegerField>("levelNumberField").value = level.level;
            root.Q<IntegerField>("emptyTubesField").value = level.emptyTubes;
            root.Q<IntegerField>("maxMovesField").value = level.maxMoves;
            root.Q<IntegerField>("timeLimitField").value = level.timeLimit;
            root.Q<IntegerField>("availableSwapsField").value = level.availableSwaps;

            // Update tubes display
            RefreshTubesDisplay();

            // Update locked tubes display
            UpdateLockedTubesDisplay();

            UpdateStatusBar($"Editing Level {level.level}");
        }

        private void RefreshTubesDisplay()
        {
            if (selectedLevel == null) return;

            tubesContainer.Clear();

            for (int i = 0; i < selectedLevel.tubes.Count; i++)
            {
                var tubeCard = CreateTubeCard(i);
                tubesContainer.Add(tubeCard);
            }
        }

        private VisualElement CreateTubeCard(int tubeIndex)
        {
            var tube = selectedLevel.tubes[tubeIndex];
            
            var card = new VisualElement();
            card.AddToClassList("tube-card");

            // Tube header
            var header = new VisualElement();
            header.AddToClassList("tube-header");
            
            var title = new Label($"Tube {tubeIndex + 1}");
            title.AddToClassList("tube-title");
            
            var info = new Label($"{tube.values.Count}/4");
            info.AddToClassList("tube-info");
            
            header.Add(title);
            header.Add(info);
            card.Add(header);

            // Tube colors
            var colorsContainer = new VisualElement();
            colorsContainer.AddToClassList("tube-colors");

            for (int slotIndex = 0; slotIndex < 4; slotIndex++)
            {
                var colorSlot = new VisualElement();
                colorSlot.AddToClassList("color-slot");

                var slotLabel = new Label($"#{slotIndex + 1}");
                slotLabel.AddToClassList("color-slot-label");
                colorSlot.Add(slotLabel);

                if (slotIndex < tube.values.Count)
                {
                    var colorField = new IntegerField();
                    colorField.AddToClassList("color-value-field");
                    colorField.value = tube.values[slotIndex];
                    
                    int capturedSlot = slotIndex;
                    colorField.RegisterValueChangedCallback(evt => {
                        tube.values[capturedSlot] = evt.newValue;
                    });

                    colorSlot.Add(colorField);

                    var removeBtnColor = new Button(() => {
                        tube.values.RemoveAt(capturedSlot);
                        RefreshTubesDisplay();
                    });
                    removeBtnColor.text = "✕";
                    removeBtnColor.AddToClassList("tube-action-button");
                    removeBtnColor.AddToClassList("remove-tube-button");
                    colorSlot.Add(removeBtnColor);
                }
                else
                {
                    var emptyLabel = new Label("Empty");
                    emptyLabel.AddToClassList("color-value-field");
                    emptyLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                    colorSlot.Add(emptyLabel);
                }

                colorsContainer.Add(colorSlot);
            }

            card.Add(colorsContainer);

            // Tube actions
            var actionsContainer = new VisualElement();
            actionsContainer.AddToClassList("tube-actions");

            if (tube.values.Count < 4)
            {
                var addColorBtn = new Button(() => {
                    tube.AddColor(0);
                    RefreshTubesDisplay();
                });
                addColorBtn.text = "+ Add";
                addColorBtn.AddToClassList("tube-action-button");
                actionsContainer.Add(addColorBtn);
            }

            var clearBtn = new Button(() => {
                tube.ClearTube();
                RefreshTubesDisplay();
            });
            clearBtn.text = "Clear";
            clearBtn.AddToClassList("tube-action-button");
            actionsContainer.Add(clearBtn);

            var removeBtn = new Button(() => {
                selectedLevel.tubes.RemoveAt(tubeIndex);
                RefreshTubesDisplay();
            });
            removeBtn.text = "Remove";
            removeBtn.AddToClassList("tube-action-button");
            removeBtn.AddToClassList("remove-tube-button");
            actionsContainer.Add(removeBtn);

            card.Add(actionsContainer);

            return card;
        }

        private void UpdateLockedTubesDisplay()
        {
            if (selectedLevel == null) return;

            var lockedLabel = root.Q<Label>("lockedTubesLabel");
            if (selectedLevel.lockedTubes.Count > 0)
            {
                lockedLabel.text = $"Locked: {string.Join(", ", selectedLevel.lockedTubes)}";
            }
            else
            {
                lockedLabel.text = "No locked tubes";
            }
        }

        private void AddTubeToCurrentLevel()
        {
            if (selectedLevel == null)
            {
                UpdateStatusBar("No level selected");
                return;
            }

            selectedLevel.tubes.Add(new TubeData());
            RefreshTubesDisplay();
            UpdateStatusBar($"Added tube to Level {selectedLevel.level}");
        }

        private void LockTube()
        {
            if (selectedLevel == null) return;

            var lockIndexField = root.Q<IntegerField>("lockTubeIndexField");
            int tubeIndex = lockIndexField.value;

            if (tubeIndex >= 0 && tubeIndex < selectedLevel.tubes.Count)
            {
                selectedLevel.LockTube(tubeIndex);
                UpdateLockedTubesDisplay();
                UpdateStatusBar($"Locked tube {tubeIndex}");
            }
            else
            {
                UpdateStatusBar($"Invalid tube index: {tubeIndex}");
            }
        }

        private void UnlockTube()
        {
            if (selectedLevel == null) return;

            var lockIndexField = root.Q<IntegerField>("lockTubeIndexField");
            int tubeIndex = lockIndexField.value;

            if (selectedLevel.lockedTubes.Contains(tubeIndex))
            {
                selectedLevel.UnlockTube(tubeIndex);
                UpdateLockedTubesDisplay();
                UpdateStatusBar($"Unlocked tube {tubeIndex}");
            }
            else
            {
                UpdateStatusBar($"Tube {tubeIndex} is not locked");
            }
        }

        private void ClearAllLocks()
        {
            if (selectedLevel == null) return;

            selectedLevel.lockedTubes.Clear();
            UpdateLockedTubesDisplay();
            UpdateStatusBar("Cleared all locked tubes");
        }

        private void ValidateCurrentLevel()
        {
            if (selectedLevel == null) return;

            var validationLabel = root.Q<Label>("validationResultLabel");
            
            // Basic validation
            bool isValid = true;
            string message = "";

            if (selectedLevel.tubes.Count == 0)
            {
                isValid = false;
                message = "Level has no tubes";
            }
            else if (selectedLevel.emptyTubes > selectedLevel.tubes.Count)
            {
                isValid = false;
                message = "Empty tubes count exceeds total tubes";
            }
            else
            {
                message = "✓ Level validation passed";
            }

            validationLabel.text = message;
            validationLabel.RemoveFromClassList("validation-success");
            validationLabel.RemoveFromClassList("validation-error");
            validationLabel.AddToClassList(isValid ? "validation-success" : "validation-error");
        }

        private void CreateNewLevel()
        {
            int newLevelNumber = levels.Count > 0 ? levels.Max(l => l.level) + 1 : 1;
            var newLevel = new TubeColorLevel(newLevelNumber);
            levels.Add(newLevel);
            RefreshLevelList();
            UpdateStatusBar($"Created Level {newLevelNumber}");
        }

        private void DuplicateLevel()
        {
            if (selectedLevel == null)
            {
                UpdateStatusBar("No level selected to duplicate");
                return;
            }

            var json = JsonUtility.ToJson(selectedLevel);
            var duplicate = JsonUtility.FromJson<TubeColorLevel>(json);
            duplicate.level = levels.Max(l => l.level) + 1;
            levels.Add(duplicate);
            RefreshLevelList();
            UpdateStatusBar($"Duplicated Level {selectedLevel.level} as Level {duplicate.level}");
        }

        private void DeleteLevel()
        {
            if (selectedLevel == null)
            {
                UpdateStatusBar("No level selected to delete");
                return;
            }

            if (EditorUtility.DisplayDialog("Delete Level", 
                $"Are you sure you want to delete Level {selectedLevel.level}?", 
                "Delete", "Cancel"))
            {
                int deletedLevel = selectedLevel.level;
                levels.Remove(selectedLevel);
                selectedLevel = null;
                selectedLevelIndex = -1;
                
                noSelectionContainer.style.display = DisplayStyle.Flex;
                levelEditorScrollView.style.display = DisplayStyle.None;
                
                RefreshLevelList();
                UpdateStatusBar($"Deleted Level {deletedLevel}");
            }
        }

        private void FilterLevels(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                levelListView.itemsSource = levels;
            }
            else
            {
                var filtered = levels.Where(l => 
                    l.level.ToString().Contains(searchText) || 
                    l.tubes.Count.ToString().Contains(searchText)
                ).ToList();
                levelListView.itemsSource = filtered;
            }
            levelListView.Rebuild();
        }

        private void RefreshLevelList()
        {
            levelListView.itemsSource = levels;
            levelListView.Rebuild();
            UpdateLevelCount();
        }

        private void UpdateFilePathDisplay()
        {
            if (filePathLabel != null)
            {
                filePathLabel.text = string.IsNullOrEmpty(jsonFilePath) ? "No file loaded" : jsonFilePath;
            }
        }

        private void UpdateLevelCount()
        {
            if (levelCountLabel != null)
            {
                levelCountLabel.text = $"{levels.Count} level{(levels.Count != 1 ? "s" : "")}";
            }
        }

        private void UpdateStatusBar(string message)
        {
            if (statusLabel != null)
            {
                statusLabel.text = message;
            }
        }


        private void LoadLevels()
        {
            string fullPath = Path.Combine(Application.dataPath, "..", jsonFilePath);
            
            if (!File.Exists(fullPath))
            {
                UpdateStatusBar($"File not found: {jsonFilePath}");
                EditorUtility.DisplayDialog("File Not Found", $"Could not find file at:\n{fullPath}", "OK");
                return;
            }

            levels = serializer.LoadLevels(fullPath);
            RefreshLevelList();
            UpdateStatusBar($"Loaded {levels.Count} levels from {jsonFilePath}");
            Debug.Log($"Loaded {levels.Count} levels");
        }

        private void SaveLevels()
        {
            if (levels.Count == 0)
            {
                UpdateStatusBar("No levels to save");
                return;
            }

            string fullPath = Path.Combine(Application.dataPath, "..", jsonFilePath);
            serializer.SaveLevels(fullPath, levels);
            UpdateStatusBar($"Saved {levels.Count} levels to {jsonFilePath}");
            Debug.Log("Levels saved successfully!");
        }

        private void BrowseJsonFile()
        {
            string selectedPath = EditorUtility.OpenFilePanel("Select JSON Level File", Application.dataPath, "json");
            
            if (!string.IsNullOrEmpty(selectedPath))
            {
                // Convert absolute path to relative path from project root
                string projectRoot = Path.Combine(Application.dataPath, "..");
                if (selectedPath.StartsWith(projectRoot))
                {
                    jsonFilePath = selectedPath.Substring(projectRoot.Length + 1).Replace("\\", "/");
                }
                else
                {
                    jsonFilePath = selectedPath;
                }
                
                UpdateFilePathDisplay();
                UpdateStatusBar($"Selected file: {jsonFilePath}");
                Debug.Log($"Selected JSON file: {jsonFilePath}");
            }
        }

        private void ConfigureLayoutConstraints()
        {
            var mainContent = root.Q<VisualElement>("main-content");
            if (mainContent != null)
            {
                mainContent.style.flexGrow = 1f;
                mainContent.style.flexShrink = 1f;
                mainContent.style.minHeight = 0f;
            }

            var leftPanel = root.Q<VisualElement>("left-panel");
            if (leftPanel != null)
            {
                leftPanel.style.flexGrow = 0f;
                leftPanel.style.flexShrink = 0f;
                leftPanel.style.flexBasis = new StyleLength(320f);
                leftPanel.style.minHeight = 0f;
            }

            var levelListScroll = root.Q<ScrollView>("levelListScrollView");
            if (levelListScroll != null)
            {
                levelListScroll.style.flexGrow = 1f;
                levelListScroll.style.flexShrink = 1f;
                levelListScroll.style.flexBasis = new StyleLength(0f);
                levelListScroll.style.minHeight = 0f;
                levelListScroll.style.height = StyleKeyword.Auto;
            }

            if (levelListView != null)
            {
                levelListView.style.flexGrow = 1f;
                levelListView.style.flexShrink = 1f;
                levelListView.style.flexBasis = new StyleLength(0f);
                levelListView.style.minHeight = 0f;
                levelListView.style.height = StyleKeyword.Auto;
            }

            var rightPanel = root.Q<VisualElement>("right-panel");
            if (rightPanel != null)
            {
                rightPanel.style.flexGrow = 1f;
                rightPanel.style.flexShrink = 1f;
                rightPanel.style.minHeight = 0f;
            }

            if (levelEditorScrollView != null)
            {
                levelEditorScrollView.style.flexGrow = 1f;
                levelEditorScrollView.style.flexShrink = 1f;
                levelEditorScrollView.style.minHeight = 0f;
                levelEditorScrollView.style.height = StyleKeyword.Auto;
            }
        }
    }
}
