using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vectrosity;

public class TrajectoryPredictor_New : MonoBehaviour
{
    #region Singleton
    private static TrajectoryPredictor_New _instance;
    public static TrajectoryPredictor_New Instance {
        get {
            if (_instance == null) {
                Debug.Log(" The TrajectoryPredictor is Null");
            }
            return _instance;
        }
    }
    #endregion

    Scene _simScene;
    PhysicsScene2D _physicsScene;
    
    [SerializeField]
    int _steps = 20; //how long we will be simulating for. More steps, more lenghth but also less performance
    Vector3[] points;

    public Transform collidablesRoot;
    Vector3 _lastForce = Vector3.zero; //used to track what the last force input was 

    public Texture lineTex;
    public Color lineColor = Color.green;
    public LineType lineType = LineType.Continuous;
    public float lineWidthStart = 13.0f;
    public float lineWidthEnd = 5.0f;

    private Camera mainCam;

    [SerializeField] private GameObject _simObjectPrefab;
    private GameObject _simObject;

    //[SerializeField]
    //private int lineDrawStart = 1;

    private VectorLine trajectoryVisualLine;

    private List<float> lineWidths;

    private void Awake() {
        _instance = this;

        mainCam = Camera.main;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        int lineWidthLength = _steps - 1;
        lineWidths = new List<float>(new float[lineWidthLength]);
        float stepSize = (lineWidthStart - lineWidthEnd) / lineWidthLength;

        for (int i = 0; i < lineWidths.Count; i++) {
            lineWidths[i] = Mathf.Clamp(lineWidthStart - (i * stepSize), lineWidthEnd, lineWidthStart);
        }

        points = new Vector3[_steps];

        trajectoryVisualLine = new VectorLine("Trajectory", new List<Vector2>(), lineTex, lineWidthStart, lineType);
        trajectoryVisualLine.color = lineColor;
        trajectoryVisualLine.textureScale = 1.0f;
        trajectoryVisualLine.smoothWidth = true;

        CreatePhysicsScene();
    }

    private void CreatePhysicsScene()
    {
        _simScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
        _physicsScene = _simScene.GetPhysicsScene2D();

        foreach (Transform obj in collidablesRoot) {
            var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
            Renderer renderer = ghostObj.GetComponent<Renderer>();
            if (renderer)
                renderer.enabled = false;
            SceneManager.MoveGameObjectToScene(ghostObj, _simScene);
        }
        
        _simObject = Instantiate(_simObjectPrefab, _simObjectPrefab.transform.position, _simObjectPrefab.transform.rotation);
        _simObject.name = "SIM_PLAYER";
        SpriteRenderer sprite = _simObject.GetComponent<SpriteRenderer>();
        if (sprite) sprite.enabled = false;
        SceneManager.MoveGameObjectToScene(_simObject, _simScene);
    }

    public void SimulateTrajectory(GameObject player, Vector3 force)   //call this every frame while player is grabed;
    {
        /*
        GameObject _simObject = Instantiate(_simObjectPrefab, _simObjectPrefab.transform.position, _simObjectPrefab.transform.rotation);
        _simObject.name = "SIM_PLAYER";
        SpriteRenderer sprite = _simObject.GetComponent<SpriteRenderer>();
        if (sprite) sprite.enabled = false;
        SceneManager.MoveGameObjectToScene(_simObject, _simScene);
        */
        if (_lastForce != force) //if force hasn't changed, skip simulation;
        {
            Rigidbody2D rb = _simObject.GetComponent<Rigidbody2D>();
            rb.velocity = Vector3.zero;
            
            _simObject.transform.position = player.transform.position;
            _simObject.transform.rotation = player.transform.rotation;


            rb.AddForce(force);

            for (int i = 0; i < _steps; i++) // steps is how many physics steps will be done in a frame 
            {
                _physicsScene.Simulate(Time.fixedDeltaTime); // move the physics, one step ahead. (anymore than 1 step creates irregularity in the trajectory)
                
                points[i] = _simObject.transform.position;

                Vector2 uiPoint = mainCam.WorldToScreenPoint(points[i]);
                trajectoryVisualLine.points2.Add(uiPoint);
            }

            // Update Trajectory visualizer
            trajectoryVisualLine.SetWidths(lineWidths);
            trajectoryVisualLine.Draw();
            trajectoryVisualLine.points2.Clear();
        }
        
        _lastForce = force;
        //Destroy(_simObject);
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

}
