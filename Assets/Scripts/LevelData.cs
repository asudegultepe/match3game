using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public enum TileType
{
    Empty,          // Boş alan - parça düşmez
    Normal,         // Normal oynanabilir alan
    Blocked,        // Tamamen engelli alan
    Ice,           // Buz - tek vuruşta kırılır
    DoubleIce,     // Çift buz - iki vuruşta kırılır
    Honey,         // Bal - parça yapışır
    Collectible,   // Toplanacak öğe
    Jelly,         // Jöle - temizlenmesi gereken alan
    DoubleJelly    // Çift jöle - iki kere vurmak gerekir
}

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
    
    [Header("Grid Layout")]
    public TileType[] gridLayout;
    public int[] collectibleTargets; // Her collectible türü için hedef sayısı
    
    private void OnValidate()
    {
        // Grid boyutu değiştiğinde array'i yeniden boyutlandır
        if (gridLayout == null || gridLayout.Length != boardWidth * boardHeight)
        {
            TileType[] newLayout = new TileType[boardWidth * boardHeight];
            
            // Eski verileri koru
            if (gridLayout != null)
            {
                int copyLength = Mathf.Min(gridLayout.Length, newLayout.Length);
                System.Array.Copy(gridLayout, newLayout, copyLength);
            }
            else
            {
                // Varsayılan olarak tümünü normal yap
                for (int i = 0; i < newLayout.Length; i++)
                {
                    newLayout[i] = TileType.Normal;
                }
            }
            
            gridLayout = newLayout;
        }
    }
    
    public TileType GetTileAt(int x, int y)
    {
        if (x < 0 || x >= boardWidth || y < 0 || y >= boardHeight) return TileType.Empty;
        return gridLayout[y * boardWidth + x];
    }
    
    public void SetTileAt(int x, int y, TileType tileType)
    {
        if (x < 0 || x >= boardWidth || y < 0 || y >= boardHeight) return;
        gridLayout[y * boardWidth + x] = tileType;
    }
}