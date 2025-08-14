using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameBoard : MonoBehaviour
{
    [Header("Board Settings")]
    [SerializeField] private int boardWidth = 8;
    [SerializeField] private int boardHeight = 8;
    [SerializeField] private float pieceSpacing = 1f;
    
    [Header("Piece Settings")]
    [SerializeField] private GameObject piecePrefab;
    [SerializeField] private Transform boardParent;
    [SerializeField] private PieceSprites pieceSprites;
    
    private GamePiece[,] pieces;
    private bool isProcessingMatches = false;
    
    public static GameBoard Instance { get; private set; }
    
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
        InitializeBoard();
    }
    
    private void InitializeBoard()
    {
        if (Instance != this) return; // Don't initialize if this isn't the active instance
        
        pieces = new GamePiece[boardWidth, boardHeight];
        
        if (boardParent == null)
            boardParent = transform;
        
        // Sprite'ları GamePiece'e aktar
        if (pieceSprites != null)
        {
            GamePiece.SetPieceSprites(pieceSprites);
        }
        
        CreatePieces();
        CenterBoard();
    }
    
    private void CreatePieces()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                CreatePieceAt(x, y);
            }
        }
    }
    
    private void CreatePieceAt(int x, int y)
    {
        GameObject pieceObject;
        Vector3 position = new Vector3(x * pieceSpacing, y * pieceSpacing, 0);
        
        if (piecePrefab != null)
        {
            pieceObject = Instantiate(piecePrefab, position, Quaternion.identity, boardParent);
        }
        else
        {
            // Create a simple piece if no prefab is assigned
            pieceObject = CreateSimplePiece(position);
        }
        
        GamePiece piece = pieceObject.GetComponent<GamePiece>();
        if (piece == null)
        {
            piece = pieceObject.AddComponent<GamePiece>();
        }
        
        // Ensure required components exist
        if (piece.GetComponent<SpriteRenderer>() == null)
        {
            SpriteRenderer sr = pieceObject.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 1;
            sr.sprite = CreateSimpleSprite();
        }
        
        if (piece.GetComponent<Collider2D>() == null)
        {
            CircleCollider2D collider = pieceObject.AddComponent<CircleCollider2D>();
            collider.radius = 0.4f;
        }
        
        PieceType randomType = GetRandomValidType(x, y);
        piece.Initialize(randomType, x, y);
        pieces[x, y] = piece;
    }
    
    private GameObject CreateSimplePiece(Vector3 position)
    {
        GameObject piece = new GameObject("GamePiece");
        piece.transform.position = position;
        piece.transform.SetParent(boardParent);
        return piece;
    }
    
    private Sprite CreateSimpleSprite()
    {
        // Create a simple white circle sprite
        Texture2D texture = new Texture2D(64, 64);
        Color[] colors = new Color[64 * 64];
        
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.white;
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
    }
    
    private PieceType GetRandomValidType(int x, int y)
    {
        List<PieceType> availableTypes = new List<PieceType>();
        
        for (int i = 0; i < System.Enum.GetValues(typeof(PieceType)).Length; i++)
        {
            PieceType type = (PieceType)i;
            if (!WouldCreateMatch(x, y, type))
            {
                availableTypes.Add(type);
            }
        }
        
        if (availableTypes.Count == 0)
        {
            return (PieceType)Random.Range(0, System.Enum.GetValues(typeof(PieceType)).Length);
        }
        
        return availableTypes[Random.Range(0, availableTypes.Count)];
    }
    
    private bool WouldCreateMatch(int x, int y, PieceType type)
    {
        int horizontalCount = 1;
        int verticalCount = 1;
        
        // Check horizontal matches to the left
        for (int i = x - 1; i >= 0 && pieces[i, y] != null && pieces[i, y].Type == type; i--)
        {
            horizontalCount++;
        }
        
        // Check horizontal matches to the right
        for (int i = x + 1; i < boardWidth && pieces[i, y] != null && pieces[i, y].Type == type; i++)
        {
            horizontalCount++;
        }
        
        // Check vertical matches below
        for (int i = y - 1; i >= 0 && pieces[x, i] != null && pieces[x, i].Type == type; i--)
        {
            verticalCount++;
        }
        
        // Check vertical matches above
        for (int i = y + 1; i < boardHeight && pieces[x, i] != null && pieces[x, i].Type == type; i++)
        {
            verticalCount++;
        }
        
        return horizontalCount >= 3 || verticalCount >= 3;
    }
    
    private void CenterBoard()
    {
        float boardCenterX = (boardWidth - 1) * pieceSpacing * 0.5f;
        float boardCenterY = (boardHeight - 1) * pieceSpacing * 0.5f;
        
        boardParent.position = new Vector3(-boardCenterX, -boardCenterY, 0);
    }
    
    public bool SwapPieces(int x1, int y1, int x2, int y2)
    {
        if (!IsValidPosition(x1, y1) || !IsValidPosition(x2, y2))
            return false;
        
        if (isProcessingMatches)
            return false;
        
        // Check if pieces are adjacent
        if (Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2) != 1)
            return false;
        
        StartCoroutine(SwapPiecesCoroutine(x1, y1, x2, y2));
        return true;
    }
    
    private IEnumerator SwapPiecesCoroutine(int x1, int y1, int x2, int y2)
    {
        isProcessingMatches = true;
        
        GamePiece piece1 = pieces[x1, y1];
        GamePiece piece2 = pieces[x2, y2];
        
        // Swap positions in array
        pieces[x1, y1] = piece2;
        pieces[x2, y2] = piece1;
        
        // Update grid positions
        piece1.GridX = x2;
        piece1.GridY = y2;
        piece2.GridX = x1;
        piece2.GridY = y1;
        
        // Animate pieces to new positions
        Vector3 pos1 = GetWorldPosition(x2, y2);
        Vector3 pos2 = GetWorldPosition(x1, y1);
        
        piece1.MoveTo(pos1, 0.3f);
        piece2.MoveTo(pos2, 0.3f);
        
        yield return new WaitForSeconds(0.3f);
        
        // Check for matches
        List<GamePiece> matches = FindAllMatches();
        
        if (matches.Count == 0)
        {
            // Swap back if no matches
            pieces[x1, y1] = piece1;
            pieces[x2, y2] = piece2;
            piece1.GridX = x1;
            piece1.GridY = y1;
            piece2.GridX = x2;
            piece2.GridY = y2;
            
            piece1.MoveTo(GetWorldPosition(x1, y1), 0.3f);
            piece2.MoveTo(GetWorldPosition(x2, y2), 0.3f);
            
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            // Use a move when a successful swap is made
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnMoveUsed();
            }
            
            yield return StartCoroutine(ProcessMatches());
        }
        
        isProcessingMatches = false;
    }
    
    private Vector3 GetWorldPosition(int x, int y)
    {
        return boardParent.position + new Vector3(x * pieceSpacing, y * pieceSpacing, 0);
    }
    
    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < boardWidth && y >= 0 && y < boardHeight;
    }
    
    private List<GamePiece> FindAllMatches()
    {
        HashSet<GamePiece> matchedPieces = new HashSet<GamePiece>();
        
        // Check horizontal matches
        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth - 2; x++)
            {
                if (pieces[x, y] != null && pieces[x + 1, y] != null && pieces[x + 2, y] != null)
                {
                    if (pieces[x, y].Type == pieces[x + 1, y].Type && 
                        pieces[x + 1, y].Type == pieces[x + 2, y].Type)
                    {
                        matchedPieces.Add(pieces[x, y]);
                        matchedPieces.Add(pieces[x + 1, y]);
                        matchedPieces.Add(pieces[x + 2, y]);
                        
                        // Check for longer matches
                        for (int i = x + 3; i < boardWidth && pieces[i, y] != null && 
                             pieces[i, y].Type == pieces[x, y].Type; i++)
                        {
                            matchedPieces.Add(pieces[i, y]);
                        }
                    }
                }
            }
        }
        
        // Check vertical matches
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight - 2; y++)
            {
                if (pieces[x, y] != null && pieces[x, y + 1] != null && pieces[x, y + 2] != null)
                {
                    if (pieces[x, y].Type == pieces[x, y + 1].Type && 
                        pieces[x, y + 1].Type == pieces[x, y + 2].Type)
                    {
                        matchedPieces.Add(pieces[x, y]);
                        matchedPieces.Add(pieces[x, y + 1]);
                        matchedPieces.Add(pieces[x, y + 2]);
                        
                        // Check for longer matches
                        for (int i = y + 3; i < boardHeight && pieces[x, i] != null && 
                             pieces[x, i].Type == pieces[x, y].Type; i++)
                        {
                            matchedPieces.Add(pieces[x, i]);
                        }
                    }
                }
            }
        }
        
        return new List<GamePiece>(matchedPieces);
    }
    
    private IEnumerator ProcessMatches()
    {
        List<GamePiece> matches = FindAllMatches();
        bool isCombo = false;
        
        while (matches.Count > 0)
        {
            // Add score for matches
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPieceMatched(matches.Count, isCombo);
            }
            
            // Calculate center position for effects
            Vector3 effectPosition = CalculateMatchCenter(matches);
            
            // Play match effects
            if (EffectsManager.Instance != null)
            {
                EffectsManager.Instance.PlayMatchEffect(effectPosition, matches.Count, isCombo);
            }
            
            // Remove matched pieces
            foreach (GamePiece piece in matches)
            {
                if (piece != null)
                {
                    pieces[piece.GridX, piece.GridY] = null;
                    Destroy(piece.gameObject);
                }
            }
            
            yield return new WaitForSeconds(0.2f);
            
            // Drop pieces down
            yield return StartCoroutine(DropPieces());
            
            // Fill empty spaces
            yield return StartCoroutine(FillBoard());
            
            // Check for new matches (combos)
            matches = FindAllMatches();
            isCombo = true; // Subsequent matches are combos
        }
    }
    
    private IEnumerator DropPieces()
    {
        bool piecesDropped = false;
        
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 1; y < boardHeight; y++)
            {
                if (pieces[x, y] != null)
                {
                    int dropY = y;
                    while (dropY > 0 && pieces[x, dropY - 1] == null)
                    {
                        dropY--;
                    }
                    
                    if (dropY != y)
                    {
                        pieces[x, dropY] = pieces[x, y];
                        pieces[x, y] = null;
                        pieces[x, dropY].GridY = dropY;
                        pieces[x, dropY].MoveTo(GetWorldPosition(x, dropY), 0.3f);
                        piecesDropped = true;
                    }
                }
            }
        }
        
        if (piecesDropped)
        {
            yield return new WaitForSeconds(0.3f);
        }
    }
    
    private IEnumerator FillBoard()
    {
        bool piecesCreated = false;
        
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (pieces[x, y] == null)
                {
                    Vector3 startPosition = GetWorldPosition(x, boardHeight);
                    GameObject pieceObject = Instantiate(piecePrefab, startPosition, Quaternion.identity, boardParent);
                    
                    GamePiece piece = pieceObject.GetComponent<GamePiece>();
                    if (piece == null)
                    {
                        piece = pieceObject.AddComponent<GamePiece>();
                    }
                    
                    PieceType randomType = (PieceType)Random.Range(0, System.Enum.GetValues(typeof(PieceType)).Length);
                    piece.Initialize(randomType, x, y);
                    pieces[x, y] = piece;
                    
                    piece.MoveTo(GetWorldPosition(x, y), 0.3f);
                    piecesCreated = true;
                }
            }
        }
        
        if (piecesCreated)
        {
            yield return new WaitForSeconds(0.3f);
        }
    }
    
    public GamePiece GetPieceAt(int x, int y)
    {
        if (IsValidPosition(x, y))
        {
            return pieces[x, y];
        }
        return null;
    }
    
    private Vector3 CalculateMatchCenter(List<GamePiece> matches)
    {
        if (matches.Count == 0) return Vector3.zero;
        
        Vector3 center = Vector3.zero;
        foreach (GamePiece piece in matches)
        {
            if (piece != null)
            {
                center += GetWorldPosition(piece.GridX, piece.GridY);
            }
        }
        
        center /= matches.Count;
        return center;
    }
}