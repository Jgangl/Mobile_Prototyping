using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Shapes;
using TMPro;

public class Tutorial : MonoBehaviour
{
    [SerializeField] Shapes.Line lineRender;
    [SerializeField] Shapes.Disc tapIconRender;
    [SerializeField] Transform dragPoint;
    [SerializeField] Transform finger;
    [SerializeField] Transform fingerStartPoint;
    [SerializeField] Transform fingerEndPoint;
    [SerializeField] Transform fingerSecondPoint;
    [SerializeField] Transform fingerThirdPoint;
    [SerializeField] Transform fingerFourthPoint;
    [SerializeField] TextMeshProUGUI tutorialText;
    [SerializeField] float fingerMoveTime = 2.0f;
    [SerializeField] float tutorialDelayTime = 5.0f;
    PlayerController player;

    Vector2 startPoint;
    Vector2 endPoint;
    Vector2 currSwipeForce;

    bool dragging = false;

    // Start is called before the first frame update
    void Start()
    {
        Level_Manager.Instance.OnLevelLoaded += OnLevelLoaded;
        
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("Play drag animation");
            PlayDragAnimation();
        }
        
        if (!player)
            return;

        if (!dragging)
            return;

        SetEndPoint(dragPoint.position);

        Vector2 screenStartPoint = Camera.main.WorldToScreenPoint(startPoint);
        Vector2 screenEndPoint = Camera.main.WorldToScreenPoint(endPoint);

        currSwipeForce = player.CalculateSwipeForce(screenStartPoint, screenEndPoint);
        //Debug.Log(swipeForce);
        player.SimulateTrajectory(currSwipeForce);
    }

    void OnLevelLoaded()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        
        Level currentLevel = Level_Manager.Instance.GetCurrentLevel();
        
        // Play tutorial on 1st level if not completed
        if (currentLevel.levelNumber == 1 && !currentLevel.completed)
        {
            // Don't let player move before tutorial
            player.EnableInput(false);
            
            Invoke("PlayDragAnimation", tutorialDelayTime);
        }
    }

    public void PlayDragAnimation()
    {
        StartCoroutine(DragTutorialCoroutine());
    }

    public void StartDragAnimation()
    {
        dragging = true;
        lineRender.enabled = true;
        finger.gameObject.SetActive(true);
        tapIconRender.enabled = true;
        finger.position = fingerStartPoint.position;
        SetStartPoint(dragPoint.position);
        SetEndPoint(dragPoint.position);
    }

    public void StopDragAnimation()
    {
        dragging = false;
        player.GetComponent<BasicTrajectory>()?.ClearArc();
        lineRender.enabled = false;
        finger.gameObject.SetActive(false);
        tapIconRender.enabled = false;
    }

    void SetStartPoint(Vector2 position)
    {
        startPoint = position;
        lineRender.Start = position;
        tapIconRender.transform.position = position;
    }

    void SetEndPoint(Vector2 position)
    {
        endPoint = position;
        lineRender.End = position;
    }

    IEnumerator DragTutorialCoroutine()
    {
        StartDragAnimation();
        
        // disable player input
        player.EnableInput(false);

        tutorialText.text = "Press";
        yield return new WaitForSeconds(1.5f);
        tutorialText.text = "Drag";
        yield return finger.DOMove(fingerEndPoint.position, fingerMoveTime).WaitForCompletion();
        yield return new WaitForSeconds(1.0f);
        yield return finger.DOMove(fingerSecondPoint.position, fingerMoveTime).WaitForCompletion();
        yield return new WaitForSeconds(1.0f);
        yield return finger.DOMove(fingerThirdPoint.position, fingerMoveTime).WaitForCompletion();
        yield return new WaitForSeconds(1.0f);
        yield return finger.DOMove(fingerFourthPoint.position, fingerMoveTime).WaitForCompletion();

        tutorialText.text = "Release";
        
        yield return new WaitForSeconds(2.0f);
        
        StopDragAnimation();
        
        // Enable player input
        player.EnableInput(true);
        
        //player.EnableMovement();
        // Should actually throw player for tutorial to show effect
        //player.AddForce(currSwipeForce);
        //AudioManager.Instance.PlayLaunchSound();
        //player.PlaySlimeParticles();
    }
}
