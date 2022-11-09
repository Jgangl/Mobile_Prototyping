using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_Manager : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button levelSelectionButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button mainMenuButton;

    // Start is called before the first frame update
    void Start()
    {
        // Start Button starts game at current uncompleted level
        startButton.onClick.AddListener(GameManager.Instance.StartGame);
        quitButton.onClick.AddListener(GameManager.Instance.QuitGame);
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
