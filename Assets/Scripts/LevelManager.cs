using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private List<LevelData> levels = new List<LevelData>();
    [SerializeField] private int currentLevelIndex = 0;
    
    [Header("UI References")]
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject levelFailedPanel;
    [SerializeField] private GameObject levelInfoPanel;
    
    public static LevelManager Instance { get; private set; }
    
    private LevelData currentLevel;
    private bool levelActive = false;
    private int remainingMoves;
    private float remainingTime;
    private int currentScore;
    private bool isTimedLevel;
    
    public LevelData CurrentLevel => currentLevel;
    public int RemainingMoves => remainingMoves;
    public float RemainingTime => remainingTime;
    public int CurrentScore => currentScore;
    public bool IsLevelActive => levelActive;
    
    public System.Action<int> OnMovesChanged;
    public System.Action<float> OnTimeChanged;
    public System.Action<int> OnScoreChanged;
    public System.Action<LevelData> OnLevelLoaded;
    public System.Action OnLevelCompleted;
    public System.Action OnLevelFailed;
    
    private void Awake()
    {
        Debug.Log("LevelManager: Awake called");
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("LevelManager: Instance set successfully");
        }
        else
        {
            Debug.Log("LevelManager: Duplicate instance found, destroying");
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        Debug.Log("LevelManager: Start called");
        
        // Create a default test level if no levels are assigned
        if (levels.Count == 0)
        {
            Debug.Log("LevelManager: No levels found, creating test level");
            CreateTestLevel();
        }
        
        Debug.Log($"LevelManager: Loading level {currentLevelIndex}, levels count: {levels.Count}");
        
        // Ensure valid level index
        if (currentLevelIndex >= levels.Count)
        {
            currentLevelIndex = 0;
        }
        
        LoadLevel(currentLevelIndex);
        
        Debug.Log($"LevelManager: levelActive = {levelActive} after LoadLevel");
    }
    
    private void CreateTestLevel()
    {
        // Test level'ı sadece hiç level yoksa oluştur, varsayılan değerlerle
        LevelData testLevel = ScriptableObject.CreateInstance<LevelData>();
        testLevel.levelNumber = 1;
        testLevel.levelName = "Test Level";
        testLevel.levelType = LevelType.Score;
        
        // Bu değerleri Inspector'da değiştirebilirsin
        testLevel.targetScore = 50;    // Makul hedef
        testLevel.movesLimit = 15;     // Yeterli hamle
        testLevel.timeLimit = 60f;
        
        testLevel.boardWidth = 8;
        testLevel.boardHeight = 8;
        testLevel.gemColors = 5;
        testLevel.specialPieceChance = 0.1f;
        
        testLevel.difficulty = DifficultyLevel.Easy;
        testLevel.description = $"Score {testLevel.targetScore} points in {testLevel.movesLimit} moves!";
        testLevel.baseReward = 100;
        testLevel.perfectReward = 300;
        
        levels.Add(testLevel);
        Debug.Log($"Created test level: {testLevel.targetScore} points, {testLevel.movesLimit} moves");
    }
    
    private void Update()
    {
        if (levelActive && isTimedLevel)
        {
            remainingTime -= Time.deltaTime;
            OnTimeChanged?.Invoke(remainingTime);
            
            if (remainingTime <= 0)
            {
                remainingTime = 0;
                FailLevel();
            }
        }
    }
    
    public void LoadLevel(int levelIndex)
    {
        Debug.Log($"LoadLevel called with index {levelIndex}, levels.Count = {levels.Count}");
        
        if (levelIndex >= 0 && levelIndex < levels.Count)
        {
            currentLevelIndex = levelIndex;
            currentLevel = levels[levelIndex];
            
            // Reset score only when loading a level (not during gameplay)
            currentScore = 0;
            Debug.Log($"LoadLevel: Reset score to 0 for level {levelIndex}");
            
            InitializeLevel();
            OnLevelLoaded?.Invoke(currentLevel);
            
            Debug.Log($"Level {currentLevel.levelNumber}: {currentLevel.levelName} loaded!");
        }
        else
        {
            Debug.LogError($"LoadLevel: Invalid level index {levelIndex}, levels.Count = {levels.Count}");
        }
    }
    
    private void InitializeLevel()
    {
        levelActive = true;
        // DON'T reset score here - it should accumulate during gameplay
        // currentScore = 0; // REMOVED
        
        Debug.Log($"InitializeLevel: Starting with score={currentScore}");
        
        switch (currentLevel.levelType)
        {
            case LevelType.Moves:
                remainingMoves = currentLevel.movesLimit;
                isTimedLevel = false;
                break;
                
            case LevelType.Time:
                remainingTime = currentLevel.timeLimit;
                remainingMoves = 999; // Unlimited moves for time mode
                isTimedLevel = true;
                break;
                
            case LevelType.Score:
                remainingMoves = currentLevel.movesLimit;
                isTimedLevel = false;
                break;
                
            case LevelType.Clear:
                remainingMoves = currentLevel.movesLimit;
                isTimedLevel = false;
                break;
        }
        
        // Trigger initial UI updates
        OnMovesChanged?.Invoke(remainingMoves);
        OnTimeChanged?.Invoke(remainingTime);
        OnScoreChanged?.Invoke(currentScore);
        
        Debug.Log($"Level initialized: Score={currentScore}, Moves={remainingMoves}, Time={remainingTime}");
    }
    
    public void OnMoveUsed()
    {
        if (!levelActive) return;
        
        Debug.Log($"Move used! Level type: {currentLevel.levelType}");
        
        if (currentLevel.levelType != LevelType.Time)
        {
            remainingMoves--;
            Debug.Log($"Moves remaining: {remainingMoves}");
            OnMovesChanged?.Invoke(remainingMoves);
            
            // Level tamamlanma koşulları
            if (remainingMoves <= 0)
            {
                Debug.Log("No moves left, checking level completion");
                CheckLevelComplete();
            }
            else if (currentLevel.levelType == LevelType.Score && currentScore >= currentLevel.targetScore)
            {
                Debug.Log($"Target score reached! {currentScore}/{currentLevel.targetScore}");
                CompleteLevel();
            }
        }
    }
    
    public void AddScore(int score)
    {
        if (!levelActive) 
        {
            Debug.Log("LevelManager: Level not active, ignoring score");
            return;
        }
        
        currentScore += score;
        Debug.Log($"LevelManager: Score added: {score}, Total: {currentScore}, Target: {currentLevel.targetScore}");
        OnScoreChanged?.Invoke(currentScore);
        
        // Don't check level complete here - only check when moves run out or specific conditions
        // CheckLevelComplete(); // REMOVED - sadece hamle bitince kontrol et
    }
    
    public void CheckLevelComplete()
    {
        if (!levelActive) return;
        
        Debug.Log($"CheckLevelComplete: Score={currentScore}/{currentLevel.targetScore}, Moves={remainingMoves}");
        
        // Bu metod sadece hamleler bittiğinde çağrılır
        if (currentLevel.levelType == LevelType.Score)
        {
            if (currentScore >= currentLevel.targetScore)
            {
                Debug.Log("Level completed: Target score reached!");
                CompleteLevel();
            }
            else
            {
                Debug.Log("Level failed: Target score not reached!");
                FailLevel();
            }
        }
        else if (currentLevel.levelType == LevelType.Clear)
        {
            if (CheckBoardCleared())
            {
                CompleteLevel();
            }
            else
            {
                FailLevel();
            }
        }
        else
        {
            // Diğer level türleri için
            CompleteLevel();
        }
    }
    
    private bool CheckBoardCleared()
    {
        return true;
    }
    
    public void CompleteLevel()
    {
        if (!levelActive) return;
        
        levelActive = false;
        int reward = CalculateReward();
        
        Debug.Log($"Level {currentLevel.levelNumber} Complete! Reward: {reward}");
        Debug.Log("Game stopped - use UI buttons to continue");
        
        OnLevelCompleted?.Invoke();
        
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }
        
        // Level bitince dursun, manuel restart gereksin
    }
    
    public void FailLevel()
    {
        if (!levelActive) return;
        
        levelActive = false;
        
        Debug.Log($"Level {currentLevel.levelNumber} Failed!");
        Debug.Log("Game stopped - use UI buttons to retry or continue");
        
        OnLevelFailed?.Invoke();
        
        if (levelFailedPanel != null)
        {
            levelFailedPanel.SetActive(true);
        }
    }
    
    private int CalculateReward()
    {
        int reward = currentLevel.baseReward;
        
        float scoreRatio = (float)currentScore / currentLevel.targetScore;
        if (scoreRatio >= 2.0f)
        {
            reward = currentLevel.perfectReward;
        }
        else if (scoreRatio >= 1.5f)
        {
            reward = Mathf.RoundToInt(currentLevel.baseReward * 1.5f);
        }
        
        return reward;
    }
    
    public void NextLevel()
    {
        int nextIndex = currentLevelIndex + 1;
        if (nextIndex < levels.Count)
        {
            LoadLevel(nextIndex);
        }
        else
        {
            Debug.Log("All levels completed!");
        }
    }
    
    public void RestartLevel()
    {
        Debug.Log("RestartLevel: Restarting current level");
        
        // Restart level completely - reset everything
        LoadLevel(currentLevelIndex);
        
        Debug.Log($"RestartLevel: Level restarted with score reset to {currentScore}");
    }
    
    public void LoadSpecificLevel(int levelNumber)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i].levelNumber == levelNumber)
            {
                LoadLevel(i);
                break;
            }
        }
    }
}