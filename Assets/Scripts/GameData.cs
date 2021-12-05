using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class GameData
{
    List<int> completedLevels;

    public GameData() {

    }

    public GameData(List<int> completedLevels) {
        this.completedLevels = completedLevels;
    }

    public void SetCompletedLevels(List<int> completedLevels) {
        this.completedLevels = completedLevels;
    }

    public List<int> GetCompletedLevels() {
        return completedLevels;
    }
}
