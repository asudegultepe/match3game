using UnityEngine;
using System.Collections;

public class EffectsManager : MonoBehaviour
{
    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem matchParticlesPrefab;
    [SerializeField] private ParticleSystem comboParticlesPrefab;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip matchSound;
    [SerializeField] private AudioClip comboSound;
    [SerializeField] private AudioClip swapSound;
    
    public static EffectsManager Instance { get; private set; }
    
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
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    public void PlayMatchEffect(Vector3 position, int pieceCount, bool isCombo = false)
    {
        StartCoroutine(PlayMatchEffectCoroutine(position, pieceCount, isCombo));
    }
    
    private IEnumerator PlayMatchEffectCoroutine(Vector3 position, int pieceCount, bool isCombo)
    {
        // Create particle effect
        if (isCombo && comboParticlesPrefab != null)
        {
            CreateParticleEffect(comboParticlesPrefab, position);
            PlaySound(comboSound);
        }
        else if (matchParticlesPrefab != null)
        {
            CreateParticleEffect(matchParticlesPrefab, position);
            PlaySound(matchSound);
        }
        
        // Screen shake for larger matches
        if (pieceCount >= 4)
        {
            yield return StartCoroutine(ScreenShake(0.1f, 0.2f));
        }
        
        // Flash effect for combos
        if (isCombo)
        {
            yield return StartCoroutine(FlashEffect());
        }
    }
    
    public void PlaySwapEffect()
    {
        PlaySound(swapSound);
    }
    
    private void CreateParticleEffect(ParticleSystem prefab, Vector3 position)
    {
        if (prefab != null)
        {
            ParticleSystem particles = Instantiate(prefab, position, Quaternion.identity);
            particles.Play();
            
            // Destroy after playing
            Destroy(particles.gameObject, particles.main.duration + particles.main.startLifetime.constantMax);
        }
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    private IEnumerator ScreenShake(float duration, float intensity)
    {
        Camera cam = Camera.main;
        if (cam == null) yield break;
        
        Vector3 originalPosition = cam.transform.position;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            
            cam.transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        cam.transform.position = originalPosition;
    }
    
    private IEnumerator FlashEffect()
    {
        Camera cam = Camera.main;
        if (cam == null) yield break;
        
        // Simple flash effect by adjusting camera background color
        Color originalColor = cam.backgroundColor;
        cam.backgroundColor = Color.white;
        
        yield return new WaitForSeconds(0.05f);
        
        cam.backgroundColor = originalColor;
    }
    
    public void CreateScorePopup(Vector3 position, int score)
    {
        StartCoroutine(ScorePopupCoroutine(position, score));
    }
    
    private IEnumerator ScorePopupCoroutine(Vector3 position, int score)
    {
        // Create a simple text popup
        GameObject popupObject = new GameObject("ScorePopup");
        popupObject.transform.position = position;
        
        // Add text mesh for score display
        TextMesh textMesh = popupObject.AddComponent<TextMesh>();
        textMesh.text = "+" + score;
        textMesh.fontSize = 20;
        textMesh.color = Color.yellow;
        textMesh.anchor = TextAnchor.MiddleCenter;
        
        // Animate the popup
        Vector3 startPos = position;
        Vector3 endPos = position + Vector3.up * 2f;
        float duration = 1f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            
            // Move up and fade out
            popupObject.transform.position = Vector3.Lerp(startPos, endPos, progress);
            Color color = textMesh.color;
            color.a = 1f - progress;
            textMesh.color = color;
            
            yield return null;
        }
        
        Destroy(popupObject);
    }
}