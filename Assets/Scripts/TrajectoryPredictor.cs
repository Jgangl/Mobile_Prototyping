using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrajectoryPredictor : MonoBehaviour
{
    #region Singleton
    private static TrajectoryPredictor _instance;
    public static TrajectoryPredictor Instance {
        get {
            if (_instance == null) {
                Debug.Log(" The TrajectoryPredictor is Null");
            }
            return _instance;
        }
    }

    private void Awake() {
        _instance = this;

        CreateSceneParameters _param = new CreateSceneParameters(LocalPhysicsMode.Physics2D); //define the parameters of a new scene, this lets us have our own separate physics 
        _simScene = SceneManager.CreateScene("Simulation", _param); // create a new scene and implement the parameters we just created
        _physicsSim = _simScene.GetPhysicsScene2D(); // assign the physics of the scene so we can simulate on our own time. 
        //CreateSimObjects(); // send over simulated objects (see method below for details)
        line.positionCount = _steps; // set amount of points our drawn line will have
        points = new Vector3[_steps]; // set amount of points our simulation will record, these will later be passed into the line.

        CreateSimulatedPlayer();
    }
    #endregion

    private PhysicsScene2D _physicsSim;
    [SerializeField]
    private GameObject _simulatedObject; //drag your simulated player into the inspector

    //[SerializeField]
    //private GameObject _simulatedObject; //drag your simulated player into the inspector

    [SerializeField]
    LineRenderer line;//drag your lineRenderer into the inspector 
    Scene _simScene;
    [SerializeField]
    int _steps = 20; //how long we will be simulating for. More steps, more lenghth but also less performance
    Vector3[] points;

    public GameObject collidablesRoot;

    //public GameObject obstaclesRoot;

    Vector3 _lastForce = Vector3.zero; //used to track what the last force input was 

    // Start is called before the first frame update
    void Start()
    {
        UpdateSimObjects(collidablesRoot);
    }

    private void CreateSimulatedPlayer() {
        SceneManager.MoveGameObjectToScene(_simulatedObject, _simScene); // move the simulated player to the sim scene
    }

    private void CreateSimObjects(GameObject obstaclesRoot)  //all objects start in regulare scene, and get sent over on start. this way colliders are dynamic and we can grab refrence to simulated player in first scene.
    {
        SceneManager.MoveGameObjectToScene(_simulatedObject, _simScene); // move the simulated player to the sim scene

        foreach (Transform t in obstaclesRoot.transform) {
            if (t.gameObject.GetComponent<Collider2D>() != null) {
                GameObject fakeT = Instantiate(t.gameObject);
                fakeT.transform.position = t.position;
                fakeT.transform.rotation = t.rotation;
                SpriteRenderer fakeR = fakeT.GetComponent<SpriteRenderer>();
                if (fakeR) {
                    fakeR.enabled = false;
                }
                SceneManager.MoveGameObjectToScene(fakeT, _simScene);
                //dummyObstacles.Add(fakeT);
            }
        }
    }

    public void UpdateSimObjects(GameObject obstaclesRoot) {
        foreach (Transform t in obstaclesRoot.transform) {
            if (t.gameObject.GetComponent<Collider2D>() != null) {
                GameObject fakeT = Instantiate(t.gameObject);
                fakeT.transform.position = t.position;
                fakeT.transform.rotation = t.rotation;
                SpriteRenderer fakeR = fakeT.GetComponent<SpriteRenderer>();
                if (fakeR) {
                    fakeR.enabled = false;
                }
                SceneManager.MoveGameObjectToScene(fakeT, _simScene);
                //dummyObstacles.Add(fakeT);
            }
        }
    }

    public void SimulateLaunch(Transform player, Vector3 force)   //call this every frame while player is grabed;
    {
        GameObject simObject = Instantiate(_simulatedObject, player.position, player.rotation);
        SceneManager.MoveGameObjectToScene(simObject, _simScene);

        if (_lastForce != force) //if force hasnt changed, skip simulation;
        {
            Rigidbody2D[] bones = GetObjectBones(simObject);

            foreach (Rigidbody2D bone in bones)
                bone.AddForce(force);

            for (var i = 0; i < _steps; i++) // steps is how many physics steps will be done in a frame 
            {
                _physicsSim.Simulate(Time.fixedDeltaTime); // move the physics, one step ahead. (anymore than 1 step creates irregularity in the trajectory)

                Vector2 simObjectAvgPos = GetObjectAveragePosition(simObject);
                if (simObjectAvgPos != Vector2.zero) {
                    points[i] = simObjectAvgPos;
                }
                else {
                    points[i] = simObject.transform.position;
                }
                //points[i] = simObject.transform.position; //record the simulated objects position for that step
                //Debug.Log(simObject.transform.position);
                line.SetPosition(i, points[i]); //let the line render know where to plot a point

            }
        }
        _lastForce = force;

        /*
        _simulatedObject.transform.position = player.position; //set sim object to player position ;
        _simulatedObject.transform.rotation = player.rotation; // set sim object to player rotation;

        _simulatedObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero; // resets sim objects velocity to 0;

        if (_lastForce != force) //if force hasnt changed, skip simulation;
        {
            _simulatedObject.GetComponent<Rigidbody2D>().AddForce(force); //simulate the objects path
            for (var i = 0; i < _steps; i++) // steps is how many physics steps will be done in a frame 
            {
                //Debug.Log("STEP:  " + Time.time);
                _physicsSim.Simulate(Time.fixedDeltaTime); // move the physics, one step ahead. (anymore than 1 step creates irregularity in the trajectory)
                points[i] = _simulatedObject.transform.position; //record the simulated objects position for that step
                line.SetPosition(i, points[i]); //let the line render know where to plot a point
            }
        }
        _lastForce = force;
        */
        Destroy(simObject);
    }

    private Vector2 GetObjectAveragePosition(GameObject simObject) {
        Vector3 avgPos = Vector2.zero;

        // Get all rigidbodies of bones
        Rigidbody2D[] bones = simObject.GetComponentsInChildren<Rigidbody2D>();
        if (bones.Length == 0)
            return Vector2.zero;

        // Average all bone positions
        foreach (Rigidbody2D rb in bones) {
            avgPos += rb.transform.position;
        }

        return (avgPos / bones.Length);
    }

    public void ClearSimulation() {
        for (var i = 0; i < _steps; i++){ // steps is how many physics steps will be done in a frame 
            points[i] = Vector2.zero; //record the simulated objects position for that step
            line.SetPosition(i, points[i]); //let the line render know where to plot a point
        }
    }

    private Rigidbody2D[] GetObjectBones(GameObject simObject) {
        return simObject.GetComponentsInChildren<Rigidbody2D>();
    }
}
