using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelection : MonoBehaviour
{
    [SerializeField]
    private GameObject levelsPanel;

    [SerializeField]
    private GameObject levelButtonPrefab;

    private int numLevels;

    private List<GameObject> levelButtons;

    // Start is called before the first frame update
    void Start()
    {
        numLevels = Level_Manager.Instance.NumLevels;

        levelButtons = new List<GameObject>();

        AddLevelButtons();
        //Debug.Log("Update Level completion icons");
        UpdateLevelCompletionIcons();
    }

    private void AddLevelButtons() {
        //Debug.Log("Adding " + numLevels + " level buttons");
        for(int i = 0; i < numLevels; i++) {
            GameObject levelButtonObject = Instantiate(levelButtonPrefab, levelsPanel.transform);
            levelButtonObject.name = "Level_" + (i + 1).ToString();
            SetButtonLevelText(levelButtonObject, i + 1);
            Button levelButton = levelButtonObject.GetComponentInChildren<Button>();
            if (levelButton) {
                levelButton.onClick.AddListener(()=> OnLevelButtonPressed(levelButtonObject));
            }

            levelButtons.Add(levelButtonObject);
        }

        UpdateLevelCompletionIcons();
    }

    private void SetButtonLevelText(GameObject levelButton, int level) {
        TextMeshProUGUI buttonText = levelButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText) {
            buttonText.text = level.ToString();
        }
    }

    public void OnLevelButtonPressed(GameObject levelButtonObject) {
        string[] nameSplit = levelButtonObject.name.Split(char.Parse("_"));
        string levelNum = "";
        if (nameSplit.Length > 1)
            levelNum = nameSplit[1];

        if (levelNum != "") {
            int level;
            if (int.TryParse(levelNum, out level))
                Level_Manager.Instance.LoadLevel(level);
           
        }
    }

    private void UpdateLevelCompletionIcons() {
        List<int> completedLevels = Level_Manager.Instance.GetCompletedLevels();

        if (completedLevels == null)
            return;

        foreach(GameObject button in levelButtons) {
            string buttonName = button.name;
            // Get level number from name
            string[] buttonSplit = buttonName.Split('_');
            
            if (buttonSplit.Length > 1) {
                //Debug.Log("Button split: " + buttonSplit[1]);
                int outButtonNum;
                if (int.TryParse(buttonSplit[1], out outButtonNum)) {
                    // Compare button level number to completed levels
                    if (completedLevels.Contains(outButtonNum)) {
                        EnableLevelCompleteCheck(button, true);
                    }
                    else {
                        EnableLevelCompleteCheck(button, false);
                    }
                }
            }
        }
    }

    private void EnableLevelCompleteCheck(GameObject levelSelectButton, bool enabled) {
        Transform levelCompleteTransform = levelSelectButton.transform.Find("Button").transform.Find("LevelCompleteCheck");
        if (levelCompleteTransform) {
            levelCompleteTransform.gameObject.SetActive(enabled);
        }
    }
    /*
    public void OpenLevelSelctionMenu() {
        EnableLevelSelectionMenu(true);

        // Should probably update the level selection checkmarks here

        UpdateLevelCompletionIcons();
    }
    */
    /*
    public void EnableLevelSelectionMenu(bool enabled) {
        for(int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(enabled);
        }
    }
    */
    /*
    public void OnExitButtonPressed() {
        EnableLevelSelectionMenu(false);
    }
    */
}
