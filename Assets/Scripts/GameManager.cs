using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private bool gameStarted = false;
    [SerializeField] private GameObject gameBoardPrefab;
    [SerializeField] private GameObject inputManagerPrefab;
    [SerializeField] private GameObject uiCanvasPrefab;
    
    public static GameManager Instance { get; private set; }
    
    public bool IsGameActive { get { return gameStarted; } }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        StartGame();
    }
    
    public void StartGame()
    {
        if (gameStarted) return;
        
        gameStarted = true;
        
        // Initialize game components
        StartCoroutine(InitializeGame());
    }
    
    private IEnumerator InitializeGame()
    {
        // Wait a frame to ensure all components are ready
        yield return null;
        
        // Create level manager if it doesn't exist
        if (LevelManager.Instance == null)
        {
            GameObject levelManagerObj = new GameObject("LevelManager");
            levelManagerObj.AddComponent<LevelManager>();
            Debug.Log("GameManager: Created LevelManager");
        }
        
        // Create game board if it doesn't exist
        if (GameBoard.Instance == null && gameBoardPrefab != null)
        {
            Instantiate(gameBoardPrefab);
        }
        
        // Create input manager if it doesn't exist
        if (FindFirstObjectByType<InputManager>() == null && inputManagerPrefab != null)
        {
            Instantiate(inputManagerPrefab);
        }
        
        // Create UI canvas if it doesn't exist
        if (FindFirstObjectByType<Canvas>() == null && uiCanvasPrefab != null)
        {
            Instantiate(uiCanvasPrefab);
        }
        
        Debug.Log("Match3 Game Started!");
    }
    
    public void RestartGame()
    {
        gameStarted = false;
        
        // Reset score
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetGame();
        }
        
        // Destroy existing board
        if (GameBoard.Instance != null)
        {
            Destroy(GameBoard.Instance.gameObject);
        }
        
        // Restart
        StartGame();
    }
    
    public void PauseGame()
    {
        Time.timeScale = 0f;
        Debug.Log("Game Paused");
    }
    
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Game Resumed");
    }
    
    public void OnPieceMatched(int pieceCount, bool isCombo)
    {
        Debug.Log($"GameManager: Pieces matched: {pieceCount}, Combo: {isCombo}");
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(pieceCount, isCombo);
        }
    }
    
    public void OnMoveUsed()
    {
        Debug.Log("GameManager: Move used");
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.UseMove();
        }
    }
}