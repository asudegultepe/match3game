using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Camera mainCamera;
    private GamePiece selectedPiece;
    private bool isDragging = false;
    private Vector3 startPosition;
    private bool isPressed = false;
    
    [Header("Input Settings")]
    [SerializeField] private float dragThreshold = 0.5f;
    
    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindObjectOfType<Camera>();
    }
    
    private void Update()
    {
        HandleInput();
    }
    
    private void HandleInput()
    {
        // Get current pointer position
        Vector2 pointerPosition = GetPointerPosition();
        bool pointerPressed = GetPointerPressed();
        bool pointerReleased = GetPointerReleased();
        
        if (pointerPressed)
        {
            HandlePointerDown(pointerPosition);
        }
        else if (isPressed && selectedPiece != null)
        {
            HandlePointerDrag(pointerPosition);
        }
        
        if (pointerReleased)
        {
            HandlePointerUp();
        }
    }
    
    private Vector2 GetPointerPosition()
    {
        if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }
        return Vector2.zero;
    }
    
    private bool GetPointerPressed()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            isPressed = true;
            return true;
        }
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            isPressed = true;
            return true;
        }
        return false;
    }
    
    private bool GetPointerReleased()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isPressed = false;
            return true;
        }
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
        {
            isPressed = false;
            return true;
        }
        return false;
    }
    
    private void HandlePointerDown(Vector2 screenPosition)
    {
        Vector3 worldPos = GetWorldPosition(screenPosition);
        GamePiece piece = GetPieceAtPosition(worldPos);
        
        if (piece != null)
        {
            selectedPiece = piece;
            startPosition = worldPos;
            isDragging = false;
            
            // Visual feedback for selection
            HighlightPiece(selectedPiece, true);
        }
    }
    
    private void HandlePointerDrag(Vector2 screenPosition)
    {
        if (selectedPiece == null) return;
        
        Vector3 currentWorldPos = GetWorldPosition(screenPosition);
        float dragDistance = Vector3.Distance(startPosition, currentWorldPos);
        
        if (dragDistance > dragThreshold && !isDragging)
        {
            isDragging = true;
            
            // Determine drag direction
            Vector3 dragDirection = (currentWorldPos - startPosition).normalized;
            Vector2Int targetGridPos = GetTargetGridPosition(selectedPiece, dragDirection);
            
            if (IsValidGridPosition(targetGridPos))
            {
                // Attempt swap
                if (GameBoard.Instance != null)
                {
                    bool swapSuccessful = GameBoard.Instance.SwapPieces(
                        selectedPiece.GridX, selectedPiece.GridY,
                        targetGridPos.x, targetGridPos.y
                    );
                    
                    if (swapSuccessful)
                    {
                        HighlightPiece(selectedPiece, false);
                        selectedPiece = null;
                    }
                }
            }
        }
    }
    
    private void HandlePointerUp()
    {
        if (selectedPiece != null)
        {
            if (!isDragging)
            {
                // Simple click - just deselect
                HighlightPiece(selectedPiece, false);
            }
            
            selectedPiece = null;
            isDragging = false;
        }
    }
    
    private Vector3 GetWorldPosition(Vector2 screenPosition)
    {
        Vector3 screenPos = new Vector3(screenPosition.x, screenPosition.y, 10f);
        return mainCamera.ScreenToWorldPoint(screenPos);
    }
    
    private GamePiece GetPieceAtPosition(Vector3 worldPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
        
        if (hit.collider != null)
        {
            return hit.collider.GetComponent<GamePiece>();
        }
        
        // Fallback: check grid positions
        if (GameBoard.Instance != null)
        {
            // Convert world position to grid coordinates
            Vector3 localPos = worldPosition - GameBoard.Instance.transform.position;
            int gridX = Mathf.RoundToInt(localPos.x);
            int gridY = Mathf.RoundToInt(localPos.y);
            
            return GameBoard.Instance.GetPieceAt(gridX, gridY);
        }
        
        return null;
    }
    
    private Vector2Int GetTargetGridPosition(GamePiece piece, Vector3 direction)
    {
        int targetX = piece.GridX;
        int targetY = piece.GridY;
        
        // Determine the strongest direction component
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Horizontal movement
            targetX += direction.x > 0 ? 1 : -1;
        }
        else
        {
            // Vertical movement
            targetY += direction.y > 0 ? 1 : -1;
        }
        
        return new Vector2Int(targetX, targetY);
    }
    
    private bool IsValidGridPosition(Vector2Int gridPos)
    {
        if (GameBoard.Instance != null)
        {
            return GameBoard.Instance.GetPieceAt(gridPos.x, gridPos.y) != null;
        }
        return false;
    }
    
    private void HighlightPiece(GamePiece piece, bool highlight)
    {
        if (piece != null)
        {
            SpriteRenderer renderer = piece.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                float brightness = highlight ? 1.2f : 1f;
                Color currentColor = renderer.color;
                renderer.color = new Color(currentColor.r * brightness, 
                                         currentColor.g * brightness, 
                                         currentColor.b * brightness, 
                                         currentColor.a);
            }
        }
    }
}