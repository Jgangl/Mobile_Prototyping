using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class LevelSelectButton : MonoBehaviour
{
    private int level;

    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Transform completeCheckmark;
    [SerializeField] private Transform lockTransform;
    [SerializeField] private Image bgImage;

    private bool bLocked = true;

    private void Start()
    {
        EnableLevelLock(bLocked);

        //float randZRot = Random.Range(0f, 360f);
        //Vector3 originalRot = bgImage.transform.rotation.eulerAngles;
        //Vector3 newRot = new Vector3(originalRot.x, originalRot.y, randZRot);
        
        //bgImage.transform.rotation = Quaternion.Euler(originalRot.x, originalRot.y, randZRot);
    }

    public void Setup(int level, Action<int> onClickAction)
    {
        this.level = level;
        button.onClick.AddListener(() => onClickAction(level));
        
        buttonText.text = level.ToString();
    }
    
    public void EnableLevelCompleteCheck(bool enabled) {
        completeCheckmark.gameObject.SetActive(enabled);
    }

    public void Complete()
    {
        Debug.Log("Completed: " + level);
        EnableLevelCompleteCheck(true);
    }

    public void Unlock()
    {
        Debug.Log("Unlocked: " + level);
        bLocked = false;
        EnableLevelLock(false);
    }
    
    private void EnableLevelLock(bool enabled) {
        lockTransform.gameObject.SetActive(enabled);

        // Disable button while locked
        button.interactable = !enabled;
    }

    public int GetLevel()
    {
        return level;
    }

    public void SetLocked(bool isLocked)
    {
        bLocked = isLocked;
    }
}
