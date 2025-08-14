using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private int basePointsPerPiece = 100;
    [SerializeField] private int comboMultiplier = 50;
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
    
    public int CurrentScore { get { return currentScore; } }
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
        ResetGame();
    }
    
    public void ResetGame()
    {
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
        currentScore += points;
        
        CheckLevelUp();
        UpdateUI();
        
        // Show score popup
        ShowScorePopup(points);
    }
    
    private int CalculatePoints(int piecesMatched)
    {
        int basePoints = piecesMatched * basePointsPerPiece;
        
        // Bonus for larger matches
        if (piecesMatched >= 4)
        {
            basePoints *= 2;
        }
        if (piecesMatched >= 5)
        {
            basePoints *= 2;
        }
        
        // Combo bonus
        int comboBonus = currentCombo * comboMultiplier;
        
        return basePoints + comboBonus;
    }
    
    public void UseMove()
    {
        currentMoves++;
        UpdateUI();
        
        // Check game over
        if (currentMoves >= movesPerLevel)
        {
            CheckGameOver();
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