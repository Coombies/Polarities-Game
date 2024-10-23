using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Handles the level select scene.
/// Reads from thegam
/// </summary>
public class LevelSelectManager : MonoBehaviour, IDataPersistence
{
    // Array of buttons for each level
    [SerializeField] private Button[] levelButtons;

    // Number of unlocked levels (could be saved/loaded from PlayerPrefs)
    private int levelsUnlocked;

    // The opacity to lower locked buttons to
    [Range(0f, 1f)]
    [SerializeField] private float lockedOpacity = 0.5f;


    /// <summary>
    /// Updates the scene every time the level select screen is loaded.
    /// </summary>
    private void Start()
    {
        UpdateLevelSelect();
    }


    /// <summary>
    /// Makes buttons interactable and not interactable
    /// depending on how many levels the player as unlocked.
    /// </summary>
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

    
    /// <summary>
    /// Helper Method to adjust button opacity
    /// </summary>
    /// <param name="button">Which button to lower opacity of.</param>
    /// <param name="opacity">Desired opacity of the button.</param>
    public void SetButtonOpacity(Button button, float opacity)
    {
        Image buttonImage = button.GetComponent<Image>();
        Color color = buttonImage.color;
        color.a = opacity;
        buttonImage.color = color;
    }


    /// <summary>
    /// Saves game data.
    /// </summary>
    /// <param name="data">Data from the games data.pol file</param>
    public void SaveData(ref GameData data)
    {
        // The Level Select Manager only reads the saved data,
        // and does not need to save any
    }


    /// <summary>
    /// Loads game data.
    /// Uses it to decide how many levels the player has
    /// unlocked in their previous playthroughs.
    /// </summary>
    /// <param name="data">Data from the games data.pol file</param>
    public void LoadData(GameData data)
    {
        levelsUnlocked = data.levelCount;
    }
}