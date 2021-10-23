using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public float spawnLevelDistance = 5f;

    public GameObject levelSectionStart;
    public GameObject levelSection;

    public GameObject playerObject;

    private Vector3 lastEndPositon;

    private void Awake() {

    }

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        
        Vector2 cameraLowerBoundary = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0f, Camera.main.nearClipPlane));
        Debug.Log(cameraLowerBoundary);
        Vector2 levelSectionStartPos = cameraLowerBoundary;
        Transform startSectionTransform = SpawnLevelSection(levelSectionStart, levelSectionStartPos);

        lastEndPositon = startSectionTransform.Find("EndPosition").position;
        Debug.Log(lastEndPositon);
    }

    private void Update() {
        if (Vector2.Distance(lastEndPositon, playerObject.transform.position) <= spawnLevelDistance) {
            Transform levelSectionTransform = SpawnLevelSection(levelSection, lastEndPositon);
            Debug.Log(levelSectionTransform.position);
            lastEndPositon = levelSectionTransform.Find("EndPosition").position;

            Debug.Log("Spawned new level section");
        }
    }

    // Returns the transform of the spawned level section
    private Transform SpawnLevelSection(GameObject levelSection, Vector2 spawnPosition) {
        GameObject levelSectionGO = Instantiate(levelSection, spawnPosition, Quaternion.identity);

        GameObject collidablesRoot = levelSectionGO.transform.Find("Collidables").gameObject;

        // Update simulation collidables
        TrajectoryPredictor.Instance.UpdateSimObjects(collidablesRoot);

        return levelSectionGO.transform;
    }

    private void SpawnPlayer(Vector2 spawnPos) {
        playerObject.transform.position = new Vector3(spawnPos.x, spawnPos.y, playerObject.transform.position.z);
    }
}
