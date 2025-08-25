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
    
    [MenuItem("Tools/Level Creator")]
    public static void ShowWindow()
    {
        GetWindow<LevelCreator>("Level Creator");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Level Creator", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Level Info
        GUILayout.Label("Level Info", EditorStyles.boldLabel);
        levelNumber = EditorGUILayout.IntField("Level Number", levelNumber);
        levelName = EditorGUILayout.TextField("Level Name", levelName);
        levelType = (LevelType)EditorGUILayout.EnumPopup("Level Type", levelType);
        difficulty = (DifficultyLevel)EditorGUILayout.EnumPopup("Difficulty", difficulty);
        
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
        
        // Create Level Button
        if (GUILayout.Button("Create Level", GUILayout.Height(30)))
        {
            CreateLevel();
        }
        
        EditorGUILayout.Space();
        
        // Instructions
        EditorGUILayout.HelpBox(
            "1. Set your level parameters above\n" +
            "2. Click 'Create Level' to generate the ScriptableObject\n" +
            "3. Save it in Assets/Levels/ folder\n" +
            "4. Add it to LevelManager's levels list in Inspector", 
            MessageType.Info);
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
        levelData.boardWidth = 8;
        levelData.boardHeight = 8;
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
}
#endif