using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject levelSectionStart;
    public GameObject levelSection;

    private void Awake() {

    }

    // Start is called before the first frame update
    void Start()
    {
        Vector2 levelSectionStartPos = Vector2.zero;
        SpawnLevelSection(levelSectionStart, levelSectionStartPos);
    }

    private void SpawnLevelSection(GameObject levelSection, Vector2 spawnPosition) {
        GameObject levelSectionGO = Instantiate(levelSection, spawnPosition, Quaternion.identity);

        GameObject collidablesRoot = levelSectionGO.transform.Find("Collidables").gameObject;

        // Update simulation collidables
        TrajectoryPredictor.Instance.UpdateSimObjects(collidablesRoot);
    }
}
