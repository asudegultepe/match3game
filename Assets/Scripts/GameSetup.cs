using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[System.Serializable]
public class GameSetup : MonoBehaviour
{
    [Header("Setup Configuration")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private Vector3 cameraPosition = new Vector3(0, 0, -10);
    [SerializeField] private float cameraSize = 6f;
    
    private void Start()
    {
        if (autoSetupOnStart)
        {
            SetupGame();
        }
    }
    
    [ContextMenu("Setup Game")]
    public void SetupGame()
    {
        SetupCamera();
        CreateManagers();
        Debug.Log("Match3 Game Setup Complete!");
    }
    
    private void SetupCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.position = cameraPosition;
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = cameraSize;
            
            // Güzel bir arka plan rengi ayarla
            mainCamera.backgroundColor = new Color(0.15f, 0.2f, 0.35f, 1f); // Koyu mavi-gri
            
            Debug.Log("Camera configured for Match3 game");
        }
    }
    
    private void CreateManagers()
    {
        // Create GameManager if it doesn't exist
        if (GameManager.Instance == null)
        {
            GameObject gameManagerObj = new GameObject("GameManager");
            gameManagerObj.AddComponent<GameManager>();
        }
        
        // Create ScoreManager if it doesn't exist
        if (ScoreManager.Instance == null)
        {
            GameObject scoreManagerObj = new GameObject("ScoreManager");
            scoreManagerObj.AddComponent<ScoreManager>();
        }
        
        // Create EffectsManager if it doesn't exist
        if (EffectsManager.Instance == null)
        {
            GameObject effectsManagerObj = new GameObject("EffectsManager");
            effectsManagerObj.AddComponent<EffectsManager>();
            effectsManagerObj.AddComponent<AudioSource>();
        }
        
        // Create InputManager if it doesn't exist
        if (FindFirstObjectByType<InputManager>() == null)
        {
            GameObject inputManagerObj = new GameObject("InputManager");
            inputManagerObj.AddComponent<InputManager>();
        }
        
        // Create LevelManager if it doesn't exist
        if (LevelManager.Instance == null)
        {
            GameObject levelManagerObj = new GameObject("LevelManager");
            levelManagerObj.AddComponent<LevelManager>();
        }
        
        // Create UIManager if it doesn't exist
        if (UIManager.Instance == null && FindFirstObjectByType<Canvas>() != null)
        {
            GameObject uiManagerObj = new GameObject("UIManager");
            uiManagerObj.AddComponent<UIManager>();
        }
    }
    
    [ContextMenu("Create Simple Piece Prefab")]
    public void CreateSimplePiecePrefab()
    {
        // Create a basic game piece prefab
        GameObject piece = new GameObject("GamePiece");
        
        // Add SpriteRenderer
        SpriteRenderer spriteRenderer = piece.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateCircleSprite();
        spriteRenderer.sortingOrder = 1;
        
        // Add Collider
        CircleCollider2D collider = piece.AddComponent<CircleCollider2D>();
        collider.radius = 0.4f;
        
        // Add GamePiece script
        piece.AddComponent<GamePiece>();
        
        Debug.Log("Simple GamePiece created! Save it as a prefab in Assets/Prefabs/");
    }
    
    [ContextMenu("Setup Sprites for GameBoard")]
    public void SetupSpritesForGameBoard()
    {
        // SpriteCreator oluştur veya bul
        SpriteCreator spriteCreator = FindFirstObjectByType<SpriteCreator>();
        if (spriteCreator == null)
        {
            GameObject creatorObj = new GameObject("SpriteCreator");
            spriteCreator = creatorObj.AddComponent<SpriteCreator>();
        }
        
        // Sprite'ları oluştur
        spriteCreator.CreateAllPieceSprites();
        
        // GameBoard'a sprite'ları ata
        GameBoard gameBoard = FindFirstObjectByType<GameBoard>();
        if (gameBoard != null)
        {
            Debug.Log("GameBoard bulundu! Şimdi Inspector'da Piece Sprites alanını SpriteCreator'dan kopyala.");
        }
        else
        {
            Debug.Log("GameBoard bulunamadı! Önce GameBoard oluştur.");
        }
    }
    
    private Sprite CreateCircleSprite()
    {
        // Create a simple circle texture
        int size = 128;
        Texture2D texture = new Texture2D(size, size);
        Color[] colors = new Color[size * size];
        
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float radius = size * 0.4f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);
                
                if (distance <= radius)
                {
                    colors[y * size + x] = Color.white;
                }
                else
                {
                    colors[y * size + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameSetup))]
public class GameSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        GameSetup setup = (GameSetup)target;
        
        if (GUILayout.Button("Setup Game"))
        {
            setup.SetupGame();
        }
        
        if (GUILayout.Button("Create Simple Piece Prefab"))
        {
            setup.CreateSimplePiecePrefab();
        }
    }
}
#endif
#endif