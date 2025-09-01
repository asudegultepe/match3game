using UnityEngine;

public enum PieceType
{
    // Normal pieces
    Red,
    Blue,
    Green,
    Yellow,
    Purple,
    Orange,
    
    // Striped pieces (çizgili şekerler)
    RedStriped,
    BlueStriped,
    GreenStriped,
    YellowStriped,
    PurpleStriped,
    OrangeStriped,
    
    // Special pieces
    Bomb,       // Combo'dan oluşan özel parça - + şeklinde patlatır
    RowClear,   // Satırı temizler  
    ColClear    // Sütunu temizler
}

[System.Serializable]
public class PieceSprites
{
    [Header("Normal Piece Sprites")]
    public Sprite redSprite;
    public Sprite blueSprite;
    public Sprite greenSprite;
    public Sprite yellowSprite;
    public Sprite purpleSprite;
    public Sprite orangeSprite;
    
    [Header("Striped Piece Sprites")]
    public Sprite redStripedSprite;
    public Sprite blueStripedSprite;
    public Sprite greenStripedSprite;
    public Sprite yellowStripedSprite;
    public Sprite purpleStripedSprite;
    public Sprite orangeStripedSprite;
    
    [Header("Special Piece Sprites")]
    public Sprite bombSprite;
    public Sprite rowClearSprite;
    public Sprite colClearSprite;
    
    public Sprite GetSpriteForType(PieceType type)
    {
        switch (type)
        {
            // Normal pieces
            case PieceType.Red: return redSprite;
            case PieceType.Blue: return blueSprite;
            case PieceType.Green: return greenSprite;
            case PieceType.Yellow: return yellowSprite;
            case PieceType.Purple: return purpleSprite;
            case PieceType.Orange: return orangeSprite;
            
            // Striped pieces
            case PieceType.RedStriped: return redStripedSprite;
            case PieceType.BlueStriped: return blueStripedSprite;
            case PieceType.GreenStriped: return greenStripedSprite;
            case PieceType.YellowStriped: return yellowStripedSprite;
            case PieceType.PurpleStriped: return purpleStripedSprite;
            case PieceType.OrangeStriped: return orangeStripedSprite;
            
            // Special pieces
            case PieceType.Bomb: return bombSprite;
            case PieceType.RowClear: return rowClearSprite;
            case PieceType.ColClear: return colClearSprite;
            
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
    
    public bool IsSpecial => pieceType == PieceType.Bomb || pieceType == PieceType.RowClear || pieceType == PieceType.ColClear || IsStriped;
    public bool IsBomb => pieceType == PieceType.Bomb;
    public bool IsRowClear => pieceType == PieceType.RowClear;
    public bool IsColClear => pieceType == PieceType.ColClear;
    
    public bool IsStriped => pieceType == PieceType.RedStriped || pieceType == PieceType.BlueStriped || 
                            pieceType == PieceType.GreenStriped || pieceType == PieceType.YellowStriped || 
                            pieceType == PieceType.PurpleStriped || pieceType == PieceType.OrangeStriped;
    
    public PieceType GetBaseColor()
    {
        switch (pieceType)
        {
            case PieceType.Red:
            case PieceType.RedStriped:
                return PieceType.Red;
            case PieceType.Blue:
            case PieceType.BlueStriped:
                return PieceType.Blue;
            case PieceType.Green:
            case PieceType.GreenStriped:
                return PieceType.Green;
            case PieceType.Yellow:
            case PieceType.YellowStriped:
                return PieceType.Yellow;
            case PieceType.Purple:
            case PieceType.PurpleStriped:
                return PieceType.Purple;
            case PieceType.Orange:
            case PieceType.OrangeStriped:
                return PieceType.Orange;
            default:
                return pieceType;
        }
    }
    
    public static PieceType GetStripedVersion(PieceType baseType)
    {
        switch (baseType)
        {
            case PieceType.Red: return PieceType.RedStriped;
            case PieceType.Blue: return PieceType.BlueStriped;
            case PieceType.Green: return PieceType.GreenStriped;
            case PieceType.Yellow: return PieceType.YellowStriped;
            case PieceType.Purple: return PieceType.PurpleStriped;
            case PieceType.Orange: return PieceType.OrangeStriped;
            default: return baseType;
        }
    }
    
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
            // Normal pieces
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
                
            // Striped pieces - base color with lighter tint
            case PieceType.RedStriped:
                return Color.Lerp(Color.red, Color.white, 0.3f);
            case PieceType.BlueStriped:
                return Color.Lerp(Color.blue, Color.white, 0.3f);
            case PieceType.GreenStriped:
                return Color.Lerp(Color.green, Color.white, 0.3f);
            case PieceType.YellowStriped:
                return Color.Lerp(Color.yellow, Color.white, 0.3f);
            case PieceType.PurpleStriped:
                return Color.Lerp(Color.magenta, Color.white, 0.3f);
            case PieceType.OrangeStriped:
                return Color.Lerp(new Color(1f, 0.5f, 0f), Color.white, 0.3f);
                
            // Special pieces
            case PieceType.Bomb:
                return Color.black; // Bomba için siyah
            case PieceType.RowClear:
                return Color.cyan; // Satır temizleyici için cyan
            case PieceType.ColClear:
                return new Color(1f, 0.5f, 1f); // Sütun temizleyici için pembe
                
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