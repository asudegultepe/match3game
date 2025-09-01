using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class LevelCreator : EditorWindow
{
    private LevelData levelData;
    private string levelName = "New Level";
    private int levelNumber = 1;
    private LevelType levelType = LevelType.Score;
    private DifficultyLevel difficulty = DifficultyLevel.Easy;
    private int targetScore = 50;
    private int movesLimit = 15;
    private float timeLimit = 60f;
    
    // Grid Editor Variables
    private Vector2 scrollPos;
    private int selectedTileType = 1; // Default: Normal
    private bool isDragging = false;
    private Vector2 dragStartPos;
    private int gridWidth = 8;
    private int gridHeight = 8;
    private bool showGridEditor = false;
    
    // Tile Colors for Visual Editor
    private Color[] tileColors = new Color[]
    {
        new Color(0.3f, 0.3f, 0.3f),    // Empty - Dark Gray
        Color.white,                     // Normal - White
        Color.red,                       // Blocked - Red
        Color.cyan,                      // Ice - Cyan
        new Color(0.5f, 0.8f, 1f),     // Double Ice - Light Blue
        Color.yellow,                    // Honey - Yellow
        Color.green,                     // Collectible - Green
        new Color(1f, 0.5f, 1f),       // Jelly - Pink
        new Color(0.8f, 0.2f, 0.8f)    // Double Jelly - Purple
    };
    
    private string[] tileTypeNames = new string[]
    {
        "Empty", "Normal", "Blocked", "Ice", "Double Ice", 
        "Honey", "Collectible", "Jelly", "Double Jelly"
    };
    
    [MenuItem("Tools/Level Creator")]
    public static void ShowWindow()
    {
        GetWindow<LevelCreator>("Level Creator");
    }
    
    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        GUILayout.Label("Level Creator", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Existing Level Selection
        GUILayout.Label("Edit Existing Level", EditorStyles.boldLabel);
        LevelData newLevelData = (LevelData)EditorGUILayout.ObjectField("Level Data", levelData, typeof(LevelData), false);
        if (newLevelData != levelData)
        {
            levelData = newLevelData;
            if (levelData != null)
            {
                LoadLevelData();
            }
        }
        
        EditorGUILayout.Space();
        
        // Level Info
        GUILayout.Label("Level Info", EditorStyles.boldLabel);
        levelNumber = EditorGUILayout.IntField("Level Number", levelNumber);
        levelName = EditorGUILayout.TextField("Level Name", levelName);
        levelType = (LevelType)EditorGUILayout.EnumPopup("Level Type", levelType);
        difficulty = (DifficultyLevel)EditorGUILayout.EnumPopup("Difficulty", difficulty);
        
        EditorGUILayout.Space();
        
        // Board Settings
        GUILayout.Label("Board Settings", EditorStyles.boldLabel);
        int newWidth = EditorGUILayout.IntSlider("Board Width", gridWidth, 4, 12);
        int newHeight = EditorGUILayout.IntSlider("Board Height", gridHeight, 4, 12);
        
        if (newWidth != gridWidth || newHeight != gridHeight)
        {
            gridWidth = newWidth;
            gridHeight = newHeight;
            if (levelData != null)
            {
                levelData.boardWidth = gridWidth;
                levelData.boardHeight = gridHeight;
                EditorUtility.SetDirty(levelData);
            }
        }
        
        EditorGUILayout.Space();
        
        // Level Goals
        GUILayout.Label("Level Goals", EditorStyles.boldLabel);
        
        if (levelType == LevelType.Score || levelType == LevelType.Moves || levelType == LevelType.Clear)
        {
            targetScore = EditorGUILayout.IntField("Target Score", targetScore);
            movesLimit = EditorGUILayout.IntField("Moves Limit", movesLimit);
        }
        
        if (levelType == LevelType.Time)
        {
            targetScore = EditorGUILayout.IntField("Target Score", targetScore);
            timeLimit = EditorGUILayout.FloatField("Time Limit (seconds)", timeLimit);
        }
        
        EditorGUILayout.Space();
        
        // Grid Editor Toggle
        showGridEditor = EditorGUILayout.Foldout(showGridEditor, "Visual Grid Editor", true);
        
        if (showGridEditor)
        {
            DrawGridEditor();
        }
        
        EditorGUILayout.Space();
        
        // Action Buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create New Level", GUILayout.Height(30)))
        {
            CreateLevel();
        }
        
        if (levelData != null && GUILayout.Button("Save Changes", GUILayout.Height(30)))
        {
            SaveLevelData();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Instructions
        EditorGUILayout.HelpBox(
            "USAGE:\n" +
            "1. Create New: Set parameters and click 'Create New Level'\n" +
            "2. Edit Existing: Drag a LevelData asset to 'Level Data' field above\n" +
            "3. Visual Editor: Use the grid below to paint tiles\n" +
            "4. Save: Click 'Save Changes' to update existing levels\n\n" +
            "TILE TYPES:\n" +
            "• Normal: Regular playable tiles\n" +
            "• Empty: No pieces spawn here\n" +
            "• Ice: Breaks when matched nearby\n" +
            "• Honey: Makes pieces sticky\n" +
            "• Collectible: Items to collect for objectives", 
            MessageType.Info);
            
        EditorGUILayout.EndScrollView();
    }
    
    private void CreateLevel()
    {
        levelData = ScriptableObject.CreateInstance<LevelData>();
        
        levelData.levelNumber = levelNumber;
        levelData.levelName = levelName;
        levelData.levelType = levelType;
        levelData.difficulty = difficulty;
        
        levelData.targetScore = targetScore;
        levelData.movesLimit = movesLimit;
        levelData.timeLimit = timeLimit;
        
        // Default board settings
        levelData.boardWidth = gridWidth;
        levelData.boardHeight = gridHeight;
        levelData.gemColors = 5;
        levelData.specialPieceChance = 0.1f;
        
        // Default rewards
        levelData.baseReward = targetScore;
        levelData.perfectReward = targetScore * 2;
        
        // Generate description
        string description = "";
        switch (levelType)
        {
            case LevelType.Score:
                description = $"Score {targetScore} points in {movesLimit} moves!";
                break;
            case LevelType.Moves:
                description = $"Complete objectives within {movesLimit} moves!";
                break;
            case LevelType.Time:
                description = $"Score {targetScore} points in {timeLimit} seconds!";
                break;
            case LevelType.Clear:
                description = $"Clear all pieces in {movesLimit} moves!";
                break;
        }
        levelData.description = description;
        
        // Initialize grid layout
        levelData.gridLayout = new TileType[gridWidth * gridHeight];
        for (int i = 0; i < levelData.gridLayout.Length; i++)
        {
            levelData.gridLayout[i] = TileType.Normal;
        }
        
        // Save to Assets
        string path = $"Assets/Level_{levelNumber}_{levelName.Replace(" ", "_")}.asset";
        AssetDatabase.CreateAsset(levelData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        // Select the created asset
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = levelData;
        
        Debug.Log($"Created level: {path}");
        
        // Increment level number for next level
        levelNumber++;
    }
    
    private void LoadLevelData()
    {
        if (levelData == null) return;
        
        levelNumber = levelData.levelNumber;
        levelName = levelData.levelName;
        levelType = levelData.levelType;
        difficulty = levelData.difficulty;
        targetScore = levelData.targetScore;
        movesLimit = levelData.movesLimit;
        timeLimit = levelData.timeLimit;
        gridWidth = levelData.boardWidth;
        gridHeight = levelData.boardHeight;
    }
    
    private void SaveLevelData()
    {
        if (levelData == null) return;
        
        levelData.levelNumber = levelNumber;
        levelData.levelName = levelName;
        levelData.levelType = levelType;
        levelData.difficulty = difficulty;
        levelData.targetScore = targetScore;
        levelData.movesLimit = movesLimit;
        levelData.timeLimit = timeLimit;
        levelData.boardWidth = gridWidth;
        levelData.boardHeight = gridHeight;
        
        EditorUtility.SetDirty(levelData);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"Saved changes to {levelData.name}");
    }
    
    private void DrawGridEditor()
    {
        if (levelData == null) return;
        
        EditorGUILayout.Space();
        
        // Tile Type Selector
        GUILayout.Label("Selected Tile Type", EditorStyles.boldLabel);
        selectedTileType = GUILayout.Toolbar(selectedTileType, tileTypeNames);
        
        // Show selected tile type color
        Rect colorRect = GUILayoutUtility.GetRect(20, 20);
        EditorGUI.DrawRect(colorRect, tileColors[selectedTileType]);
        
        EditorGUILayout.Space();
        
        // Quick Fill Buttons
        GUILayout.Label("Quick Fill", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Fill All Normal"))
        {
            FillGrid(TileType.Normal);
        }
        
        if (GUILayout.Button("Create Border"))
        {
            CreateBorder();
        }
        
        if (GUILayout.Button("Clear All"))
        {
            FillGrid(TileType.Empty);
        }
        
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        
        // Grid Drawing
        GUILayout.Label("Grid Layout", EditorStyles.boldLabel);
        DrawGrid();
        
        EditorGUILayout.Space();
        
        // Grid Info
        EditorGUILayout.HelpBox($"Left Click: Paint tiles\nDrag: Paint multiple tiles\nGrid Size: {gridWidth}x{gridHeight}", MessageType.Info);
    }
    
    private void DrawGrid()
    {
        if (levelData?.gridLayout == null) return;
        
        float cellSize = 25f;
        float padding = 10f;
        
        // Calculate grid position
        Rect gridRect = GUILayoutUtility.GetRect(gridWidth * cellSize + padding * 2, gridHeight * cellSize + padding * 2);
        
        // Draw background
        EditorGUI.DrawRect(gridRect, Color.black);
        
        // Handle mouse events
        HandleGridInput(gridRect, cellSize, padding);
        
        // Draw cells
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                int index = y * gridWidth + x;
                if (index >= levelData.gridLayout.Length) continue;
                
                Rect cellRect = new Rect(
                    gridRect.x + padding + x * cellSize,
                    gridRect.y + padding + y * cellSize,
                    cellSize - 1,
                    cellSize - 1
                );
                
                // Draw cell
                TileType tileType = levelData.gridLayout[index];
                Color cellColor = tileColors[(int)tileType];
                EditorGUI.DrawRect(cellRect, cellColor);
                
                // Draw cell border
                Handles.color = Color.gray;
                Vector3[] points = new Vector3[5];
                points[0] = new Vector3(cellRect.xMin, cellRect.yMin, 0);
                points[1] = new Vector3(cellRect.xMax, cellRect.yMin, 0);
                points[2] = new Vector3(cellRect.xMax, cellRect.yMax, 0);
                points[3] = new Vector3(cellRect.xMin, cellRect.yMax, 0);
                points[4] = new Vector3(cellRect.xMin, cellRect.yMin, 0);
                Handles.DrawPolyLine(points);
            }
        }
        
        // Force repaint on mouse events
        if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
        {
            Repaint();
        }
    }
    
    private void HandleGridInput(Rect gridRect, float cellSize, float padding)
    {
        Event e = Event.current;
        Vector2 mousePos = e.mousePosition;
        
        // Check if mouse is within grid bounds
        if (!gridRect.Contains(mousePos)) return;
        
        // Calculate grid position
        int gridX = Mathf.FloorToInt((mousePos.x - gridRect.x - padding) / cellSize);
        int gridY = Mathf.FloorToInt((mousePos.y - gridRect.y - padding) / cellSize);
        
        if (gridX < 0 || gridX >= gridWidth || gridY < 0 || gridY >= gridHeight) return;
        
        bool needsRepaint = false;
        
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0) // Left click
                {
                    SetTileAt(gridX, gridY, (TileType)selectedTileType);
                    isDragging = true;
                    needsRepaint = true;
                    e.Use();
                }
                break;
                
            case EventType.MouseDrag:
                if (isDragging && e.button == 0)
                {
                    SetTileAt(gridX, gridY, (TileType)selectedTileType);
                    needsRepaint = true;
                    e.Use();
                }
                break;
                
            case EventType.MouseUp:
                if (e.button == 0)
                {
                    isDragging = false;
                    e.Use();
                }
                break;
        }
        
        if (needsRepaint)
        {
            EditorUtility.SetDirty(levelData);
            Repaint();
        }
    }
    
    private void SetTileAt(int x, int y, TileType tileType)
    {
        if (levelData?.gridLayout == null) return;
        
        int index = y * gridWidth + x;
        if (index >= 0 && index < levelData.gridLayout.Length)
        {
            levelData.gridLayout[index] = tileType;
        }
    }
    
    private void FillGrid(TileType tileType)
    {
        if (levelData?.gridLayout == null) return;
        
        for (int i = 0; i < levelData.gridLayout.Length; i++)
        {
            levelData.gridLayout[i] = tileType;
        }
        
        EditorUtility.SetDirty(levelData);
        Repaint();
    }
    
    private void CreateBorder()
    {
        if (levelData?.gridLayout == null) return;
        
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                int index = y * gridWidth + x;
                
                // Create border with blocked tiles
                if (x == 0 || x == gridWidth - 1 || y == 0 || y == gridHeight - 1)
                {
                    levelData.gridLayout[index] = TileType.Blocked;
                }
                else
                {
                    levelData.gridLayout[index] = TileType.Normal;
                }
            }
        }
        
        EditorUtility.SetDirty(levelData);
        Repaint();
    }
}
#endif