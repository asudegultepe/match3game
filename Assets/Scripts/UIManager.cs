using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject levelFailedPanel;
    
    [Header("Game UI")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text movesText;
    [SerializeField] private Text targetScoreText;
    [SerializeField] private Text timeText;
    [SerializeField] private Text levelNameText;
    [SerializeField] private Text levelDescriptionText;
    
    [Header("Buttons")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button retryLevelButton;
    
    [Header("Game Over UI")]
    [SerializeField] private Text finalScoreText;
    [SerializeField] private Button playAgainButton;
    
    public static UIManager Instance { get; private set; }
    
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
    
    
    private void Start()
    {
        SetupButtons();
        SubscribeToLevelEvents();
        ShowGamePanel();
        UpdateUI();
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromLevelEvents();
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    private void SubscribeToLevelEvents()
    {
        // Delay subscription to ensure LevelManager is initialized
        StartCoroutine(DelayedSubscription());
    }
    
    private System.Collections.IEnumerator DelayedSubscription()
    {
        // Wait for LevelManager to be ready
        while (LevelManager.Instance == null)
        {
            yield return null;
        }
        
        Debug.Log("UIManager: Subscribing to LevelManager events");
        
        // Subscribe to events
        LevelManager.Instance.OnLevelLoaded += OnLevelLoaded;
        LevelManager.Instance.OnLevelCompleted += OnLevelCompleted;
        LevelManager.Instance.OnLevelFailed += OnLevelFailed;
        LevelManager.Instance.OnScoreChanged += OnScoreChanged;
        LevelManager.Instance.OnMovesChanged += OnMovesChanged;
        LevelManager.Instance.OnTimeChanged += OnTimeChanged;
        
        Debug.Log("UIManager: Events subscribed, updating UI");
        
        // Force initial UI update
        UpdateUI();
    }
    
    private void UnsubscribeFromLevelEvents()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelLoaded -= OnLevelLoaded;
            LevelManager.Instance.OnLevelCompleted -= OnLevelCompleted;
            LevelManager.Instance.OnLevelFailed -= OnLevelFailed;
            LevelManager.Instance.OnScoreChanged -= OnScoreChanged;
            LevelManager.Instance.OnMovesChanged -= OnMovesChanged;
            LevelManager.Instance.OnTimeChanged -= OnTimeChanged;
        }
    }
    
    private void SetupButtons()
    {
        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);
            
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
            
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
            
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(RestartGame);
            
        if (menuButton != null)
            menuButton.onClick.AddListener(GoToMenu);
            
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(NextLevel);
            
        if (retryLevelButton != null)
            retryLevelButton.onClick.AddListener(RetryLevel);
    }
    
    public void UpdateUI()
    {
        Debug.Log("UIManager: UpdateUI called");
        
        // Update with LevelManager data if available
        if (LevelManager.Instance != null && LevelManager.Instance.CurrentLevel != null)
        {
            Debug.Log("UIManager: Using LevelManager UI");
            UpdateLevelUI(LevelManager.Instance.CurrentLevel);
        }
        // Fallback to ScoreManager for legacy mode
        else if (ScoreManager.Instance != null)
        {
            Debug.Log("UIManager: Using ScoreManager UI");
            UpdateLegacyUI();
        }
        else
        {
            Debug.Log("UIManager: No manager found for UI update");
        }
    }
    
    private void UpdateLevelUI(LevelData level)
    {
        Debug.Log($"UIManager: Updating Level UI - Score: {LevelManager.Instance.CurrentScore}");
        
        if (scoreText != null)
        {
            scoreText.text = "Score: " + LevelManager.Instance.CurrentScore;
            Debug.Log($"UIManager: Updated score text to: {scoreText.text}");
        }
        else
        {
            Debug.Log("UIManager: scoreText is null!");
        }
            
        if (levelText != null)
            levelText.text = "Level: " + level.levelNumber;
            
        if (levelNameText != null)
            levelNameText.text = level.levelName;
            
        if (levelDescriptionText != null)
            levelDescriptionText.text = level.description;
            
        if (targetScoreText != null && level.levelType == LevelType.Score)
            targetScoreText.text = "Target: " + level.targetScore;
            
        if (movesText != null && level.levelType != LevelType.Time)
            movesText.text = "Moves: " + LevelManager.Instance.RemainingMoves;
            
        if (timeText != null && level.levelType == LevelType.Time)
            timeText.text = "Time: " + Mathf.CeilToInt(LevelManager.Instance.RemainingTime);
    }
    
    private void UpdateLegacyUI()
    {
        Debug.Log($"UIManager: Updating Legacy UI - Score: {ScoreManager.Instance.CurrentScore}");
        
        if (scoreText != null)
        {
            scoreText.text = "Score: " + ScoreManager.Instance.CurrentScore;
            Debug.Log($"UIManager: Updated legacy score text to: {scoreText.text}");
        }
        else
        {
            Debug.Log("UIManager: scoreText is null in legacy mode!");
        }
            
        if (levelText != null)
            levelText.text = "Level: " + ScoreManager.Instance.CurrentLevel;
            
        if (movesText != null)
            movesText.text = "Moves: " + ScoreManager.Instance.MovesRemaining;
            
        if (targetScoreText != null)
            targetScoreText.text = "Target: " + (ScoreManager.Instance.CurrentLevel * 1000);
    }
    
    public void ShowGamePanel()
    {
        SetPanelActive(gamePanel, true);
        SetPanelActive(pausePanel, false);
        SetPanelActive(gameOverPanel, false);
    }
    
    public void ShowPausePanel()
    {
        SetPanelActive(gamePanel, false);
        SetPanelActive(pausePanel, true);
        SetPanelActive(gameOverPanel, false);
    }
    
    public void ShowGameOverPanel(int finalScore)
    {
        SetPanelActive(gamePanel, false);
        SetPanelActive(pausePanel, false);
        SetPanelActive(gameOverPanel, true);
        
        if (finalScoreText != null)
            finalScoreText.text = "Final Score: " + finalScore;
    }
    
    private void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }
    
    public void PauseGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseGame();
        }
        ShowPausePanel();
    }
    
    public void ResumeGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
        ShowGamePanel();
    }
    
    public void RestartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
        ShowGamePanel();
    }
    
    public void GoToMenu()
    {
        // In a full game, this would load the main menu scene
        Debug.Log("Going to main menu...");
        RestartGame();
    }
    
    public void NextLevel()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.NextLevel();
        }
        ShowGamePanel();
    }
    
    public void RetryLevel()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RestartLevel();
        }
        else if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
        ShowGamePanel();
    }
    
    // Level Event Handlers
    private void OnLevelLoaded(LevelData levelData)
    {
        ShowGamePanel();
        UpdateUI();
    }
    
    private void OnLevelCompleted()
    {
        ShowLevelCompletePanel();
    }
    
    private void OnLevelFailed()
    {
        ShowLevelFailedPanel();
    }
    
    private void OnScoreChanged(int newScore)
    {
        Debug.Log($"UIManager: Score changed to {newScore}");
        if (scoreText != null)
            scoreText.text = "Score: " + newScore;
    }
    
    private void OnMovesChanged(int remainingMoves)
    {
        Debug.Log($"UIManager: Moves changed to {remainingMoves}");
        if (movesText != null)
            movesText.text = "Moves: " + remainingMoves;
    }
    
    private void OnTimeChanged(float remainingTime)
    {
        Debug.Log($"UIManager: Time changed to {remainingTime}");
        if (timeText != null)
            timeText.text = "Time: " + Mathf.CeilToInt(remainingTime);
    }
    
    public void ShowLevelCompletePanel()
    {
        SetPanelActive(gamePanel, false);
        SetPanelActive(pausePanel, false);
        SetPanelActive(gameOverPanel, false);
        SetPanelActive(levelFailedPanel, false);
        SetPanelActive(levelCompletePanel, true);
    }
    
    public void ShowLevelFailedPanel()
    {
        SetPanelActive(gamePanel, false);
        SetPanelActive(pausePanel, false);
        SetPanelActive(gameOverPanel, false);
        SetPanelActive(levelCompletePanel, false);
        SetPanelActive(levelFailedPanel, true);
    }
    
    public void ShowScorePopup(Vector3 worldPosition, int score)
    {
        // Convert world position to screen position for UI popup
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(worldPosition);
            
            if (EffectsManager.Instance != null)
            {
                EffectsManager.Instance.CreateScorePopup(worldPosition, score);
            }
        }
    }
}