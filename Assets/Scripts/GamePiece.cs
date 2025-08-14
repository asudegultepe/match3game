using UnityEngine;

public enum PieceType
{
    Red,
    Blue,
    Green,
    Yellow,
    Purple,
    Orange
}

[System.Serializable]
public class PieceSprites
{
    [Header("Piece Sprites")]
    public Sprite redSprite;
    public Sprite blueSprite;
    public Sprite greenSprite;
    public Sprite yellowSprite;
    public Sprite purpleSprite;
    public Sprite orangeSprite;
    
    public Sprite GetSpriteForType(PieceType type)
    {
        switch (type)
        {
            case PieceType.Red: return redSprite;
            case PieceType.Blue: return blueSprite;
            case PieceType.Green: return greenSprite;
            case PieceType.Yellow: return yellowSprite;
            case PieceType.Purple: return purpleSprite;
            case PieceType.Orange: return orangeSprite;
            default: return null;
        }
    }
}

public class GamePiece : MonoBehaviour
{
    [SerializeField] private PieceType pieceType;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    // Static sprite referansları - tüm piece'ler aynı sprite'ları kullanacak
    private static PieceSprites pieceSprites;
    
    public PieceType Type { get { return pieceType; } }
    public int GridX { get; set; }
    public int GridY { get; set; }
    
    public static void SetPieceSprites(PieceSprites sprites)
    {
        pieceSprites = sprites;
    }
    
    public void Initialize(PieceType type, int gridX, int gridY)
    {
        pieceType = type;
        GridX = gridX;
        GridY = gridY;
        
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        
        SetSprite();
    }
    
    private void SetSprite()
    {
        if (spriteRenderer == null) return;
        
        if (pieceSprites != null)
        {
            // Sprite kullan
            Sprite targetSprite = pieceSprites.GetSpriteForType(pieceType);
            if (targetSprite != null)
            {
                spriteRenderer.sprite = targetSprite;
                spriteRenderer.color = Color.white; // Sprite'ın orijinal rengini koru
            }
            else
            {
                // Fallback: renk kullan
                SetFallbackColor();
            }
        }
        else
        {
            // Sprite yoksa renk kullan
            SetFallbackColor();
        }
    }
    
    private void SetFallbackColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = GetColorForType();
        }
    }
    
    private Color GetColorForType()
    {
        switch (pieceType)
        {
            case PieceType.Red:
                return Color.red;
            case PieceType.Blue:
                return Color.blue;
            case PieceType.Green:
                return Color.green;
            case PieceType.Yellow:
                return Color.yellow;
            case PieceType.Purple:
                return Color.magenta;
            case PieceType.Orange:
                return new Color(1f, 0.5f, 0f);
            default:
                return Color.white;
        }
    }
    
    public void MoveTo(Vector3 targetPosition, float duration = 0.3f)
    {
        StartCoroutine(MoveCoroutine(targetPosition, duration));
    }
    
    private System.Collections.IEnumerator MoveCoroutine(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }
        
        transform.position = targetPosition;
    }
}