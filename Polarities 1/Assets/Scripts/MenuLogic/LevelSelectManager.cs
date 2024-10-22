using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour, IDataPersistence
{
    // Array of buttons for each level
    public Button[] levelButtons;

    // Number of unlocked levels (could be saved/loaded from PlayerPrefs)
    private int levelsUnlocked;

    // The opacity to lower locked buttons to
    [Range(0f, 1f)]
    public float lockedOpacity = 0.5f;

    private void Start()
    {
        UpdateLevelSelect();
    }

    public void UpdateLevelSelect()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i < levelsUnlocked)
            {
                // Level is unlocked
                levelButtons[i].interactable = true;
                SetButtonOpacity(levelButtons[i], 1f); // Full opacity
            }
            else
            {
                // Level is locked
                levelButtons[i].interactable = false;
                SetButtonOpacity(levelButtons[i], lockedOpacity); // Lower opacity
            }
        }
    }

    // Helper method to adjust button opacity
    public void SetButtonOpacity(Button button, float opacity)
    {
        Image buttonImage = button.GetComponent<Image>();
        Color color = buttonImage.color;
        color.a = opacity;
        buttonImage.color = color;
    }

    public void SaveData(ref GameData data)
    {
        // The Level Select Manager only reads the saved data, and does not need to save any
    }

    public void LoadData(GameData data)
    {
        levelsUnlocked = data.levelCount;
    }
}