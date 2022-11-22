using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vectrosity;

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
        points = new Vector3[_steps]; // set amount of points our simulation will record, these will later be passed into the line.
    }
    #endregion

    private PhysicsScene2D _physicsSim;
    [SerializeField]
    private GameObject _playerObject; //drag your player into the inspector
    
    Scene _simScene;
    [SerializeField]
    int _steps = 20; //how long we will be simulating for. More steps, more lenghth but also less performance
    Vector3[] points;

    public GameObject collidablesRoot;
    Vector3 _lastForce = Vector3.zero; //used to track what the last force input was 

    public Texture lineTex;
    public Color lineColor = Color.green;
    public LineType lineType = LineType.Continuous;
    public float lineWidthStart = 13.0f;
    public float lineWidthEnd = 5.0f;

    private VectorLine trajectoryVisualLine;
    private List<float> lineWidths;
    private GameObject _simObject;

    // Start is called before the first frame update
    void Start()
    {
        collidablesRoot = GameObject.Find("Collidables");
        
        int lineWidthLength = _steps - 1;
        lineWidths = new List<float>(new float[lineWidthLength]);
        float stepSize = (lineWidthStart - lineWidthEnd) / lineWidthLength;
        
        for (int i = 0; i < lineWidths.Count; i++) {
            lineWidths[i] = Mathf.Clamp(lineWidthStart - (i * stepSize), lineWidthEnd, lineWidthStart);
        }

        trajectoryVisualLine = new VectorLine("Trajectory", new List<Vector2>(), lineTex, lineWidthStart, lineType);
        trajectoryVisualLine.color = lineColor;
        trajectoryVisualLine.textureScale = 1.0f;
        trajectoryVisualLine.smoothWidth = true;

        UpdateSimObjects(collidablesRoot);
    }

    /*
        private void CreateSimObjects(GameObject obstaclesRoot)  //all objects start in regulare scene, and get sent over on start. this way colliders are dynamic and we can grab refrence to simulated player in first scene.
        {
            foreach (Transform t in obstaclesRoot.transform) {
                if (t.gameObject.GetComponentInChildren<Collider2D>() != null) {
                    GameObject fakeT = Instantiate(t.gameObject);
                    fakeT.transform.position = t.position;
                    fakeT.transform.rotation = t.rotation;
                    SpriteRenderer fakeR = fakeT.GetComponent<SpriteRenderer>();
                    if (fakeR) {
                        fakeR.enabled = false;
                    }
                    SceneManager.MoveGameObjectToScene(fakeT, _simScene);
                }
            }
        }
    */
    public void UpdateSimObjects(GameObject obstaclesRoot) {
        foreach (Transform t in obstaclesRoot.transform) {
            if (t.gameObject.GetComponentInChildren<Collider2D>() != null) {
                GameObject fakeT = Instantiate(t.gameObject);
                fakeT.transform.position = t.position;
                fakeT.transform.rotation = t.rotation;

                Bouncer bouncer = fakeT.GetComponent<Bouncer>();
                if (bouncer)
                    bouncer.isSimulated = true;

                SpriteRenderer[] fakeSprites = fakeT.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer sprite in fakeSprites)
                    sprite.enabled = false;

                SceneManager.MoveGameObjectToScene(fakeT, _simScene);
            }
        }
        
        _simObject = Instantiate(_playerObject, _playerObject.transform.position, _playerObject.transform.rotation);
        _simObject.name = "SIM_PLAYER";
        SpriteRenderer playerSprite = _simObject.GetComponent<SpriteRenderer>();
        if (playerSprite) playerSprite.enabled = false;
        SceneManager.MoveGameObjectToScene(_simObject, _simScene);
    }

    public void SimulateLaunch(Transform player, Vector3 force)   //call this every frame while player is grabed;
    {
        //GameObject simObject = Instantiate(_playerObject, player.position, player.rotation);
        //SceneManager.MoveGameObjectToScene(simObject, _simScene);

        //simObject.layer = LayerMask.NameToLayer("PlayerSim");
        //foreach(Transform child in simObject.transform) {
        //    child.gameObject.layer = LayerMask.NameToLayer("PlayerSim");
        //}

        // Set player simulated flag in playercontroller
        //PlayerController playerController = simObject.GetComponent<PlayerController>();
        //if (playerController) {
        //    Debug.Log("Setting SIMULATED");
        //    playerController.isSimulated = true;
        //}

        //playerController.StartMovement();

        // Disable player sprite
        //SpriteRenderer sprite = simObject.GetComponent<SpriteRenderer>();
        //if (sprite)
        //    sprite.enabled = false;

        if (_lastForce != force) //if force hasnt changed, skip simulation;
        {
            _simObject.transform.position = player.transform.position;

            for (int i = 0; i < player.childCount; i++)
            {
                Vector2 playerBonePos = player.GetChild(i).position;
                _simObject.transform.GetChild(i).position = playerBonePos;
            }
            
            Rigidbody2D[] bones = GetObjectBones(_simObject);

            foreach (Rigidbody2D bone in bones)
                bone.velocity = Vector3.zero;
            
            foreach (Rigidbody2D bone in bones)
                bone.AddForce(force);

            for (var i = 0; i < _steps; i++) // steps is how many physics steps will be done in a frame 
            {
                _physicsSim.Simulate(Time.fixedDeltaTime); // move the physics, one step ahead. (anymore than 1 step creates irregularity in the trajectory)

                Vector2 simObjectAvgPos = GetObjectAveragePosition(_simObject);
                if (simObjectAvgPos != Vector2.zero) {
                    points[i] = simObjectAvgPos;
                }
                else {
                    points[i] = _simObject.transform.position;
                }

                Vector2 uiPoint = Camera.main.WorldToScreenPoint(points[i]);

                // VectorLine points need to be in ui space NOT world space
                //if (i >= lineDrawStart)
                trajectoryVisualLine.points2.Add(uiPoint);

                //line.SetPosition(i, points[i]); //let the line render know where to plot a point
            }

            // Update Trajectory visualizer
            trajectoryVisualLine.SetWidths(lineWidths);
            trajectoryVisualLine.Draw();
            trajectoryVisualLine.points2.Clear();
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
        //Destroy(simObject);
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
            //trajectoryVisualLine.points3[i] = points[i];
            //line.SetPosition(i, points[i]); //let the line render know where to plot a point
        }

        trajectoryVisualLine.points2.Clear();
        trajectoryVisualLine.Draw();
    }

    private Rigidbody2D[] GetObjectBones(GameObject simObject) {
        return simObject.GetComponentsInChildren<Rigidbody2D>();
    }
}
