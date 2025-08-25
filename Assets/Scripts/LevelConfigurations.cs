using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Level Configurations", menuName = "Match3/Level Configurations")]
public class LevelConfigurations : ScriptableObject
{
    [Header("Default Level Settings")]
    public LevelDefaults defaults;
    
    [Header("Predefined Levels")]
    public List<LevelData> levels = new List<LevelData>();
    
    [Header("Level Generation")]
    public LevelGenerationRules generationRules;
}

[System.Serializable]
public class LevelDefaults
{
    [Header("Board Defaults")]
    public int defaultBoardWidth = 8;
    public int defaultBoardHeight = 8;
    public int defaultGemColors = 5;
    
    [Header("Score Defaults")]
    public int baseScoreTarget = 25;
    public int scoreIncreasePerLevel = 5;
    
    [Header("Moves Defaults")]
    public int baseMoveLimit = 25;
    public int moveDecreasePerDifficulty = 3;
    
    [Header("Time Defaults")]
    public float baseTimeLimit = 120f;
    public float timeDecreasePerDifficulty = 15f;
    
    [Header("Rewards")]
    public int baseReward = 100;
    public int rewardIncreasePerLevel = 50;
}

[System.Serializable]
public class LevelGenerationRules
{
    [Header("Type Distribution")]
    [Range(0f, 1f)]
    public float scoreTypeProbability = 0.4f;
    [Range(0f, 1f)]
    public float movesTypeProbability = 0.3f;
    [Range(0f, 1f)]
    public float timeTypeProbability = 0.2f;
    [Range(0f, 1f)]
    public float clearTypeProbability = 0.1f;
    
    [Header("Difficulty Progression")]
    public AnimationCurve difficultyProgression = AnimationCurve.Linear(0, 0, 1, 1);
    
    [Header("Special Features")]
    public bool enableSpecialPieces = true;
    public AnimationCurve specialPieceChanceProgression = AnimationCurve.Linear(0, 0.05f, 1, 0.2f);
}

public static class LevelGenerator
{
    public static LevelData GenerateLevel(int levelNumber, LevelConfigurations config)
    {
        LevelData newLevel = ScriptableObject.CreateInstance<LevelData>();
        
        newLevel.levelNumber = levelNumber;
        newLevel.levelName = $"Level {levelNumber}";
        
        // Determine difficulty based on level number
        float difficultyValue = config.generationRules.difficultyProgression.Evaluate((levelNumber - 1) / 100f);
        newLevel.difficulty = (DifficultyLevel)Mathf.FloorToInt(difficultyValue * 4);
        
        // Determine level type
        float typeRandom = Random.Range(0f, 1f);
        if (typeRandom < config.generationRules.scoreTypeProbability)
        {
            newLevel.levelType = LevelType.Score;
            newLevel.targetScore = config.defaults.baseScoreTarget + 
                                 (levelNumber - 1) * config.defaults.scoreIncreasePerLevel;
            newLevel.movesLimit = config.defaults.baseMoveLimit;
        }
        else if (typeRandom < config.generationRules.scoreTypeProbability + config.generationRules.movesTypeProbability)
        {
            newLevel.levelType = LevelType.Moves;
            newLevel.targetScore = config.defaults.baseScoreTarget;
            newLevel.movesLimit = config.defaults.baseMoveLimit - 
                                ((int)newLevel.difficulty * config.defaults.moveDecreasePerDifficulty);
        }
        else if (typeRandom < config.generationRules.scoreTypeProbability + 
                              config.generationRules.movesTypeProbability + 
                              config.generationRules.timeTypeProbability)
        {
            newLevel.levelType = LevelType.Time;
            newLevel.timeLimit = config.defaults.baseTimeLimit - 
                               ((int)newLevel.difficulty * config.defaults.timeDecreasePerDifficulty);
            newLevel.targetScore = config.defaults.baseScoreTarget;
        }
        else
        {
            newLevel.levelType = LevelType.Clear;
            newLevel.movesLimit = config.defaults.baseMoveLimit;
        }
        
        // Set board properties
        newLevel.boardWidth = config.defaults.defaultBoardWidth;
        newLevel.boardHeight = config.defaults.defaultBoardHeight;
        newLevel.gemColors = config.defaults.defaultGemColors;
        
        // Set special piece chance
        if (config.generationRules.enableSpecialPieces)
        {
            float specialChanceValue = config.generationRules.specialPieceChanceProgression.Evaluate((levelNumber - 1) / 100f);
            newLevel.specialPieceChance = specialChanceValue;
        }
        
        // Set rewards
        newLevel.baseReward = config.defaults.baseReward + 
                             (levelNumber - 1) * config.defaults.rewardIncreasePerLevel;
        newLevel.perfectReward = newLevel.baseReward * 3;
        
        // Generate description
        newLevel.description = GenerateLevelDescription(newLevel);
        
        return newLevel;
    }
    
    private static string GenerateLevelDescription(LevelData level)
    {
        string description = "";
        
        switch (level.levelType)
        {
            case LevelType.Score:
                description = $"Reach {level.targetScore} points in {level.movesLimit} moves.";
                break;
            case LevelType.Moves:
                description = $"Complete objectives within {level.movesLimit} moves.";
                break;
            case LevelType.Time:
                description = $"Score as much as possible in {level.timeLimit} seconds!";
                break;
            case LevelType.Clear:
                description = $"Clear all pieces from the board in {level.movesLimit} moves.";
                break;
        }
        
        if (level.difficulty == DifficultyLevel.Hard || level.difficulty == DifficultyLevel.Expert)
        {
            description += " This is a challenging level!";
        }
        
        return description;
    }
}