using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpriteCreator : MonoBehaviour
{
    [Header("Sprite Creation Settings")]
    [SerializeField] private int spriteSize = 128;
    [SerializeField] private PieceSprites generatedSprites;
    
    [Header("Sprite Shapes")]
    [SerializeField] private SpriteShape spriteShape = SpriteShape.Circle;
    
    public enum SpriteShape
    {
        Circle,
        Square,
        Diamond,
        Star,
        Heart,
        Hexagon
    }
    
    [ContextMenu("Create All Sprites")]
    public void CreateAllPieceSprites()
    {
        if (generatedSprites == null)
        {
            generatedSprites = new PieceSprites();
        }
        
        generatedSprites.redSprite = CreateSpriteForColor(Color.red, "RedPiece");
        generatedSprites.blueSprite = CreateSpriteForColor(Color.blue, "BluePiece");
        generatedSprites.greenSprite = CreateSpriteForColor(Color.green, "GreenPiece");
        generatedSprites.yellowSprite = CreateSpriteForColor(Color.yellow, "YellowPiece");
        generatedSprites.purpleSprite = CreateSpriteForColor(Color.magenta, "PurplePiece");
        generatedSprites.orangeSprite = CreateSpriteForColor(new Color(1f, 0.5f, 0f), "OrangePiece");
        
        Debug.Log("Tüm sprite'lar oluşturuldu!");
        
        // GameBoard'a sprite'ları aktar
        GameBoard gameBoard = FindObjectOfType<GameBoard>();
        if (gameBoard != null)
        {
            // Inspector'dan manuel olarak atanması gerek
            Debug.Log("GameBoard bulundu! Inspector'dan Piece Sprites alanını doldur.");
        }
    }
    
    public Sprite CreateSpriteForColor(Color color, string spriteName)
    {
        Texture2D texture = new Texture2D(spriteSize, spriteSize);
        Color[] pixels = new Color[spriteSize * spriteSize];
        
        Vector2 center = new Vector2(spriteSize * 0.5f, spriteSize * 0.5f);
        float radius = spriteSize * 0.4f;
        
        for (int y = 0; y < spriteSize; y++)
        {
            for (int x = 0; x < spriteSize; x++)
            {
                Vector2 pos = new Vector2(x, y);
                bool isInside = IsInsideShape(pos, center, radius);
                
                if (isInside)
                {
                    // Ana renk
                    Color finalColor = color;
                    
                    // Gölge ve highlight efekti
                    float distanceFromCenter = Vector2.Distance(pos, center) / radius;
                    
                    if (distanceFromCenter < 0.3f)
                    {
                        // Merkez - daha parlak
                        finalColor = Color.Lerp(color, Color.white, 0.3f);
                    }
                    else if (distanceFromCenter > 0.8f)
                    {
                        // Kenar - daha koyu
                        finalColor = Color.Lerp(color, Color.black, 0.2f);
                    }
                    
                    pixels[y * spriteSize + x] = finalColor;
                }
                else
                {
                    pixels[y * spriteSize + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        texture.name = spriteName;
        
        return Sprite.Create(texture, new Rect(0, 0, spriteSize, spriteSize), new Vector2(0.5f, 0.5f));
    }
    
    private bool IsInsideShape(Vector2 pos, Vector2 center, float radius)
    {
        switch (spriteShape)
        {
            case SpriteShape.Circle:
                return Vector2.Distance(pos, center) <= radius;
                
            case SpriteShape.Square:
                return Mathf.Abs(pos.x - center.x) <= radius && Mathf.Abs(pos.y - center.y) <= radius;
                
            case SpriteShape.Diamond:
                return Mathf.Abs(pos.x - center.x) + Mathf.Abs(pos.y - center.y) <= radius;
                
            case SpriteShape.Star:
                return IsInsideStar(pos, center, radius);
                
            case SpriteShape.Heart:
                return IsInsideHeart(pos, center, radius);
                
            case SpriteShape.Hexagon:
                return IsInsideHexagon(pos, center, radius);
                
            default:
                return Vector2.Distance(pos, center) <= radius;
        }
    }
    
    private bool IsInsideStar(Vector2 pos, Vector2 center, float radius)
    {
        Vector2 relative = pos - center;
        float angle = Mathf.Atan2(relative.y, relative.x);
        float distance = relative.magnitude;
        
        // 5 köşeli yıldız
        float starRadius = radius * (0.5f + 0.5f * Mathf.Cos(5 * angle));
        return distance <= starRadius;
    }
    
    private bool IsInsideHeart(Vector2 pos, Vector2 center, float radius)
    {
        Vector2 relative = (pos - center) / radius;
        float x = relative.x;
        float y = relative.y;
        
        // Kalp şekli denklemi
        float heartEq = (x * x + y * y - 1) * (x * x + y * y - 1) * (x * x + y * y - 1) - x * x * y * y * y;
        return heartEq <= 0;
    }
    
    private bool IsInsideHexagon(Vector2 pos, Vector2 center, float radius)
    {
        Vector2 relative = pos - center;
        float distance = relative.magnitude;
        float angle = Mathf.Atan2(relative.y, relative.x);
        
        // 6 köşeli hexagon
        float hexRadius = radius / Mathf.Cos(Mathf.PI / 6 * Mathf.Round(6 * angle / Mathf.PI));
        return distance <= Mathf.Abs(hexRadius);
    }
    
    public PieceSprites GetGeneratedSprites()
    {
        return generatedSprites;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SpriteCreator))]
public class SpriteCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        SpriteCreator creator = (SpriteCreator)target;
        
        if (GUILayout.Button("Create All Piece Sprites"))
        {
            creator.CreateAllPieceSprites();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "1. Sprite Shape'i seç\n" +
            "2. 'Create All Piece Sprites' butonuna tıkla\n" +
            "3. GameBoard'da Piece Sprites alanını doldur\n" +
            "4. Oyunu çalıştır!", 
            MessageType.Info);
    }
}
#endif