using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Manager : Singleton<Level_Manager>
{
    List<int> completedLevels;

    public void LevelCompleted(int level) {
        completedLevels.Add(level);
    }

    public List<int> GetCompletedLevels() {
        return completedLevels;
    }
}
