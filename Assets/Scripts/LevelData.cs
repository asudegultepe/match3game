using UnityEngine;

[System.Serializable]
public enum LevelType
{
    Score,      // Belirli skoru ulaşma hedefi
    Moves,      // Sınırlı hamle ile hedefi tamamlama
    Time,       // Süre sınırı ile oynama
    Clear       // Tüm taşları temizleme
}

[System.Serializable]
public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard,
    Expert
}

[CreateAssetMenu(fileName = "New Level", menuName = "Match3/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public int levelNumber;
    public string levelName;
    public DifficultyLevel difficulty;
    public LevelType levelType;
    
    [Header("Level Goals")]
    public int targetScore;
    public int movesLimit = 30;
    public float timeLimit = 60f;
    
    [Header("Board Settings")]
    public int boardWidth = 8;
    public int boardHeight = 8;
    public int gemColors = 5;
    
    [Header("Special Pieces")]
    [Range(0f, 1f)]
    public float specialPieceChance = 0.1f;
    
    [Header("Rewards")]
    public int baseReward = 100;
    public int perfectReward = 500;
    
    [Header("Level Description")]
    [TextArea(3, 5)]
    public string description;
}