using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PredictionManager : Singleton<PredictionManager> {
    public GameObject obstacles;
    public int maxIterations;

    Scene currentScene;
    Scene predictionScene;

    PhysicsScene2D currentPhysicsScene;
    PhysicsScene2D predictionPhysicsScene;

    List<GameObject> dummyObstacles = new List<GameObject>();

    LineRenderer lineRenderer;
    GameObject dummy;

    void Start() {
        //Physics2D.autoSimulation = false;

        Physics2D.simulationMode = SimulationMode2D.Script;

        currentScene = SceneManager.GetActiveScene();
        currentPhysicsScene = currentScene.GetPhysicsScene2D();

        CreateSceneParameters parameters = new CreateSceneParameters(LocalPhysicsMode.Physics2D);
        predictionScene = SceneManager.CreateScene("Prediction", parameters);
        predictionPhysicsScene = predictionScene.GetPhysicsScene2D();

        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update() {
        if (Input.GetMouseButtonUp(1) && dummy != null) {
            Debug.Log("Adding force to dummy");
            dummy.GetComponent<Rigidbody2D>().AddForce(new Vector2(-200f, 300f));
        }
    }

    void FixedUpdate() {
        
        if (currentPhysicsScene.IsValid()) {
            currentPhysicsScene.Simulate(Time.fixedDeltaTime);
        }
        
    }

    public void copyAllObstacles() {
        foreach (Transform t in obstacles.transform) {
            if (t.gameObject.GetComponent<Collider2D>() != null) {
                GameObject fakeT = Instantiate(t.gameObject);
                fakeT.transform.position = t.position;
                fakeT.transform.rotation = t.rotation;
                SpriteRenderer fakeR = fakeT.GetComponent<SpriteRenderer>();
                if (fakeR) {
                    fakeR.enabled = false;
                }
                SceneManager.MoveGameObjectToScene(fakeT, predictionScene);
                dummyObstacles.Add(fakeT);
            }
        }
    }

    void killAllObstacles() {
        foreach (var o in dummyObstacles) {
            Destroy(o);
        }
        dummyObstacles.Clear();
    }

    public void predict(GameObject subject, Vector2 currentPosition, Vector2 force) {
        if (currentPhysicsScene.IsValid() && predictionPhysicsScene.IsValid()) {
            if (dummy == null) {
                //Debug.Log("Created dummy object");
                dummy = Instantiate(subject);
                SceneManager.MoveGameObjectToScene(dummy, predictionScene);
            }

            PlayerController playerController = dummy.GetComponent<PlayerController>();
            if (playerController)
                Destroy(playerController);

            dummy.transform.position = currentPosition;
            dummy.GetComponent<Rigidbody2D>().AddForce(force);
            lineRenderer.positionCount = 0;
            lineRenderer.positionCount = maxIterations;

            for (int i = 0; i < maxIterations; i++) {
                //Debug.Log(dummy.transform.position);
                predictionPhysicsScene.Simulate(Time.fixedDeltaTime);
                lineRenderer.SetPosition(i, dummy.transform.position);
            }
            Debug.Log("Predict force: " + force);
            //Destroy(dummy);
        }
    }

    void OnDestroy() {
        killAllObstacles();
    }

    public void ClearSimulation() {
        for (int i = 0; i < maxIterations; i++) {
            lineRenderer.SetPosition(i, Vector2.zero);
        }
    }

}
