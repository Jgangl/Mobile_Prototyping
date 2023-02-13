using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class GameData
{
    Level[] levels;

    public GameData() 
    {

    }

    public GameData(Level[] levels) 
    {
        this.levels = levels;
    }

    public void SetLevels(Level[] levels) 
    {
        this.levels = levels;
    }

    public Level[] GetLevels() 
    {
        return levels;
    }

    public Level GetLevel(int level)
    {
        // Check if index is out of bounds
        if (level > levels.Length || level < 0)
        {
            return new Level();
        }

        return levels[level - 1];
    }
}

[System.Serializable]
public class Level
{
    public int levelNumber;
    public bool completed;
    public int bestNumOfJumps;
    public double bestTime;

    public Level()
    {
        this.levelNumber = -1;
        this.completed = false;
        this.bestNumOfJumps = int.MaxValue;
        this.bestTime = double.MaxValue;
    }
    
    public Level(int levelNumber)
    {
        this.levelNumber = levelNumber;
        this.completed = false;
        this.bestNumOfJumps = int.MaxValue;
        this.bestTime = double.MaxValue;
    }
    
    public Level(int levelNumber, bool completed, int bestNumOfJumps, double bestTime)
    {
        this.levelNumber = levelNumber;
        this.completed = completed;
        this.bestNumOfJumps = bestNumOfJumps;
        this.bestTime = bestTime;
    }
}
