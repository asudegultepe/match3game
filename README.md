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