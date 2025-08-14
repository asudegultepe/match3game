# Match3 Game - Unity Project

A complete Match3 game implementation similar to Candy Crush, built with Unity.

## Features

- **Classic Match3 Gameplay**: Match 3 or more pieces of the same color to clear them
- **Cascading Matches**: New pieces fall down creating combo opportunities
- **Scoring System**: Points for matches with combo bonuses
- **Level Progression**: Limited moves per level with increasing difficulty
- **Visual Effects**: Particle effects, screen shake, and score popups
- **Audio Integration**: Sound effects for matches, combos, and swaps
- **Clean UI**: Score display, level info, and game controls

## Project Structure

```
Assets/
├── Scripts/           # Core game scripts
│   ├── GamePiece.cs         # Individual piece logic
│   ├── GameBoard.cs         # Board management and match detection
│   ├── InputManager.cs      # Touch/mouse input handling
│   ├── GameManager.cs       # Main game controller
│   ├── ScoreManager.cs      # Score and level management
│   ├── UIManager.cs         # UI system management
│   └── EffectsManager.cs    # Visual and audio effects
├── Prefabs/          # Game object prefabs
├── Materials/        # Materials and shaders
├── Textures/         # Sprite textures
├── Audio/           # Sound effects and music
└── Animations/      # Animation clips
```

## Setup Instructions

### 1. Unity Setup
1. Open Unity Hub and create a new 2D project
2. Import the scripts from the Assets/Scripts folder
3. Make sure Universal Render Pipeline (URP) is set up (already configured)

### 2. Create Game Prefabs

#### GamePiece Prefab:
1. Create an empty GameObject named "GamePiece"
2. Add a SpriteRenderer component
3. Add a Circle Collider2D component
4. Attach the GamePiece script
5. Create a simple colored circle sprite or use Unity's default sprite
6. Save as prefab in Assets/Prefabs/

#### GameBoard Prefab:
1. Create an empty GameObject named "GameBoard"
2. Attach the GameBoard script
3. Set the piecePrefab field to your GamePiece prefab
4. Configure board dimensions (default: 8x8)
5. Save as prefab

#### UI Canvas:
1. Create a Canvas (UI -> Canvas)
2. Add UI elements:
   - Score text (Top left)
   - Level text (Top center)
   - Moves text (Top right)
   - Pause button
3. Attach UIManager script to a UI Manager GameObject
4. Connect UI elements to the script fields

### 3. Scene Setup
1. Open the SampleScene
2. Set up the camera for 2D orthographic view
3. Position camera to center the game board
4. Add the GameBoard prefab to the scene
5. Add UI Canvas
6. Create GameManager GameObject and attach GameManager script
7. Create EffectsManager GameObject for visual effects
8. Create ScoreManager GameObject for score management

### 4. Input System Setup
1. The project uses Unity's new Input System
2. Create InputManager GameObject and attach InputManager script
3. Make sure the main camera is tagged as "MainCamera"

### 5. Audio Setup (Optional)
1. Add AudioSource component to EffectsManager
2. Import audio clips for:
   - Match sounds
   - Combo sounds
   - Swap sounds
3. Assign clips in EffectsManager script

### 6. Visual Effects Setup (Optional)
1. Create simple particle systems for match effects
2. Assign to EffectsManager script
3. Customize colors and behaviors

## How to Play

1. **Objective**: Match 3 or more pieces of the same color
2. **Controls**: 
   - Click and drag pieces to swap with adjacent pieces
   - Only valid swaps that create matches are allowed
3. **Scoring**:
   - Base points per piece: 100
   - Larger matches get multipliers
   - Combo chains give bonus points
4. **Levels**: Complete levels by reaching target scores within move limits

## Customization Options

### Board Configuration
- Change board size in GameBoard script (boardWidth, boardHeight)
- Adjust piece spacing
- Modify piece colors in GamePiece script

### Difficulty Settings
- Adjust moves per level in ScoreManager
- Change scoring values
- Modify level progression requirements

### Visual Customization
- Replace simple colored sprites with custom artwork
- Add animations for piece movements
- Enhance particle effects
- Create custom UI themes

## Code Architecture

### Core Classes:
- **GamePiece**: Handles individual piece behavior and movement
- **GameBoard**: Manages the game grid, match detection, and piece spawning
- **InputManager**: Processes user input and piece selection
- **GameManager**: Coordinates all game systems
- **ScoreManager**: Tracks score, levels, and moves
- **UIManager**: Handles all UI interactions
- **EffectsManager**: Manages visual and audio feedback

### Key Features:
- Singleton pattern for managers
- Coroutine-based animations
- Event-driven architecture
- Modular component system

## Future Enhancements

- Power-ups and special pieces
- More level objectives (clear certain pieces, reach bottom, etc.)
- Multiplayer support
- Social features
- More visual effects and animations
- Save/load game state
- In-app purchases integration

## Dependencies

- Unity 2022.3 LTS or newer
- Universal Render Pipeline (URP)
- Unity Input System package

## License

This project is provided as-is for educational and development purposes.