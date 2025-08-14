using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    
    [Header("Game UI")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text movesText;
    [SerializeField] private Text targetScoreText;
    
    [Header("Buttons")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    
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
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    private void Start()
    {
        SetupButtons();
        ShowGamePanel();
        UpdateUI();
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
    }
    
    public void UpdateUI()
    {
        if (ScoreManager.Instance != null)
        {
            if (scoreText != null)
                scoreText.text = "Score: " + ScoreManager.Instance.CurrentScore;
                
            if (levelText != null)
                levelText.text = "Level: " + ScoreManager.Instance.CurrentLevel;
                
            if (movesText != null)
                movesText.text = "Moves: " + ScoreManager.Instance.MovesRemaining;
                
            if (targetScoreText != null)
                targetScoreText.text = "Target: " + (ScoreManager.Instance.CurrentLevel * 1000);
        }
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