using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private int basePointsPerPiece = 1;
    [SerializeField] private int comboMultiplier = 1;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text movesText;
    
    [Header("Level Settings")]
    [SerializeField] private int movesPerLevel = 20;
    [SerializeField] private int pointsToNextLevel = 1000;
    
    private int currentScore = 0;
    private int currentLevel = 1;
    private int currentMoves = 0;
    private int currentCombo = 0;
    
    public static ScoreManager Instance { get; private set; }
    
    public int CurrentScore 
    { 
        get 
        { 
            // If LevelManager exists, use its score
            if (LevelManager.Instance != null)
                return LevelManager.Instance.CurrentScore;
            else
                return currentScore; 
        } 
    }
    public int CurrentLevel { get { return currentLevel; } }
    public int MovesRemaining { get { return movesPerLevel - currentMoves; } }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    private void Start()
    {
        // Force correct values if inspector has old values
        if (basePointsPerPiece > 50)
        {
            Debug.Log($"ScoreManager: Forcing basePointsPerPiece from {basePointsPerPiece} to 1");
            basePointsPerPiece = 1;
        }
        if (comboMultiplier > 10)
        {
            Debug.Log($"ScoreManager: Forcing comboMultiplier from {comboMultiplier} to 1");
            comboMultiplier = 1;
        }
        
        ResetGame();
    }
    
    public void ResetGame()
    {
        Debug.Log("ScoreManager: ResetGame called");
        currentScore = 0;
        currentLevel = 1;
        currentMoves = 0;
        currentCombo = 0;
        UpdateUI();
    }
    
    public void AddScore(int piecesMatched, bool isCombo = false)
    {
        if (isCombo)
        {
            currentCombo++;
        }
        else
        {
            currentCombo = 0;
        }
        
        int points = CalculatePoints(piecesMatched);
        Debug.Log($"ScoreManager: Calculated {points} points for {piecesMatched} pieces");
        
        // If LevelManager exists, let it handle scoring completely
        if (LevelManager.Instance != null)
        {
            Debug.Log("ScoreManager: Delegating to LevelManager");
            LevelManager.Instance.AddScore(points);
            // DON'T update currentScore here - let LevelManager handle everything
        }
        else
        {
            // Legacy mode - handle scoring ourselves
            Debug.Log("ScoreManager: Using legacy scoring");
            currentScore += points;
            CheckLevelUp();
            UpdateUI();
        }
        
        // Show score popup
        ShowScorePopup(points);
    }
    
    private int CalculatePoints(int piecesMatched)
    {
        int basePoints = piecesMatched * basePointsPerPiece;
        
        // Bonus for larger matches (çok düşük bonus)
        if (piecesMatched >= 4)
        {
            basePoints += 2; // 4'lü için +2 bonus
        }
        if (piecesMatched >= 5)
        {
            basePoints += 3; // 5'li için +3 bonus (toplam +5)
        }
        if (piecesMatched >= 6)
        {
            basePoints += 5; // 6+ için +5 bonus (toplam +10)
        }
        
        // Combo bonus (çok düşük)
        int comboBonus = currentCombo * comboMultiplier;
        
        return basePoints + comboBonus;
    }
    
    public void UseMove()
    {
        // If LevelManager exists, let it handle move counting
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnMoveUsed();
            // Don't increment currentMoves here, let LevelManager handle it
        }
        else
        {
            // Legacy mode - handle moves ourselves
            currentMoves++;
            UpdateUI();
            
            // Check game over for legacy mode
            if (currentMoves >= movesPerLevel)
            {
                CheckGameOver();
            }
        }
    }
    
    private void CheckLevelUp()
    {
        int targetScore = currentLevel * pointsToNextLevel;
        
        if (currentScore >= targetScore)
        {
            currentLevel++;
            currentMoves = 0; // Reset moves for new level
            
            // Show level up message
            ShowLevelUpMessage();
        }
    }
    
    private void CheckGameOver()
    {
        // Simple game over condition - ran out of moves
        Debug.Log("Game Over! Final Score: " + currentScore);
        
        // Show game over UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOverPanel(currentScore);
        }
        else
        {
            // Fallback - restart immediately
            ResetGame();
        }
    }
    
    private void UpdateUI()
    {
        // Update legacy UI elements if present
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString();
        }
        
        if (levelText != null)
        {
            levelText.text = "Level: " + currentLevel.ToString();
        }
        
        if (movesText != null)
        {
            movesText.text = "Moves: " + MovesRemaining.ToString();
        }
        
        // Update main UI manager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateUI();
        }
    }
    
    private void ShowScorePopup(int points)
    {
        // Simple debug message - in a full game you'd create animated UI
        Debug.Log("+" + points + " points!");
        
        if (currentCombo > 0)
        {
            Debug.Log("Combo x" + (currentCombo + 1) + "!");
        }
    }
    
    private void ShowLevelUpMessage()
    {
        Debug.Log("Level Up! Now Level " + currentLevel);
    }
}