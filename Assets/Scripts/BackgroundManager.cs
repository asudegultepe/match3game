using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [Header("Background Settings")]
    [SerializeField] private Color backgroundColor = new Color(0.2f, 0.3f, 0.8f, 1f); // Mavi ton
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private GameObject backgroundImagePrefab;
    
    private Camera mainCamera;
    private SpriteRenderer backgroundRenderer;
    
    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindFirstObjectByType<Camera>();
            
        SetupBackground();
    }
    
    private void SetupBackground()
    {
        if (mainCamera != null)
        {
            // Kamera arka plan rengini ayarla
            mainCamera.backgroundColor = backgroundColor;
        }
        
        // Eğer sprite varsa, arka plan resmi oluştur
        if (backgroundSprite != null)
        {
            CreateBackgroundSprite();
        }
    }
    
    private void CreateBackgroundSprite()
    {
        GameObject bgObject = new GameObject("Background");
        backgroundRenderer = bgObject.AddComponent<SpriteRenderer>();
        backgroundRenderer.sprite = backgroundSprite;
        backgroundRenderer.sortingOrder = -10; // En arkada olsun
        
        // Arka planı kameraya göre boyutlandır
        if (mainCamera != null)
        {
            ResizeBackgroundToCamera();
        }
        
        // Pozisyonu ayarla
        bgObject.transform.position = new Vector3(0, 0, 5f);
    }
    
    private void ResizeBackgroundToCamera()
    {
        if (mainCamera == null || backgroundRenderer == null) return;
        
        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        
        Vector3 spriteSize = backgroundRenderer.sprite.bounds.size;
        float scaleX = cameraWidth / spriteSize.x;
        float scaleY = cameraHeight / spriteSize.y;
        
        // En büyük scale'i kullan ki tüm ekranı kaplasın
        float scale = Mathf.Max(scaleX, scaleY);
        backgroundRenderer.transform.localScale = Vector3.one * scale;
    }
    
    // Çalışma zamanında arka plan rengini değiştir
    public void ChangeBackgroundColor(Color newColor)
    {
        backgroundColor = newColor;
        if (mainCamera != null)
        {
            mainCamera.backgroundColor = newColor;
        }
    }
    
    // Çalışma zamanında arka plan resmini değiştir
    public void ChangeBackgroundSprite(Sprite newSprite)
    {
        backgroundSprite = newSprite;
        
        if (backgroundRenderer != null)
        {
            backgroundRenderer.sprite = newSprite;
            ResizeBackgroundToCamera();
        }
        else if (newSprite != null)
        {
            CreateBackgroundSprite();
        }
    }
    
    // Önceden tanımlanmış arka plan temaları
    [ContextMenu("Blue Theme")]
    public void SetBlueTheme()
    {
        ChangeBackgroundColor(new Color(0.2f, 0.4f, 0.8f, 1f));
    }
    
    [ContextMenu("Green Theme")]
    public void SetGreenTheme()
    {
        ChangeBackgroundColor(new Color(0.2f, 0.6f, 0.3f, 1f));
    }
    
    [ContextMenu("Purple Theme")]
    public void SetPurpleTheme()
    {
        ChangeBackgroundColor(new Color(0.5f, 0.2f, 0.7f, 1f));
    }
    
    [ContextMenu("Dark Theme")]
    public void SetDarkTheme()
    {
        ChangeBackgroundColor(new Color(0.1f, 0.1f, 0.15f, 1f));
    }
    
    [ContextMenu("Create Gradient Background")]
    public void CreateGradientBackground()
    {
        CreateGradientTexture();
    }
    
    private void CreateGradientTexture()
    {
        int width = 256;
        int height = 256;
        Texture2D gradientTexture = new Texture2D(width, height);
        
        Color topColor = new Color(0.8f, 0.9f, 1f, 1f);    // Açık mavi
        Color bottomColor = new Color(0.2f, 0.3f, 0.8f, 1f); // Koyu mavi
        
        for (int y = 0; y < height; y++)
        {
            Color currentColor = Color.Lerp(bottomColor, topColor, (float)y / height);
            for (int x = 0; x < width; x++)
            {
                gradientTexture.SetPixel(x, y, currentColor);
            }
        }
        
        gradientTexture.Apply();
        
        Sprite gradientSprite = Sprite.Create(gradientTexture, 
            new Rect(0, 0, width, height), 
            new Vector2(0.5f, 0.5f));
            
        ChangeBackgroundSprite(gradientSprite);
    }
}