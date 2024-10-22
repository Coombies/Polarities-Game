using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour, IDataPersistence
{
    private bool blueCharacterAtCheckpoint = false;
    private bool redCharacterAtCheckpoint = false;
    private int currentLevelNum;
    private int maxLevelNum;
    private static CheckpointManager instance;

    public int targetLevelNum;

    private void Awake()
    {
        // Singleton implementation
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Call this method when the blue character reaches their checkpoint
    private void Start()
    {
        currentLevelNum = SceneManager.GetActiveScene().buildIndex - 1;
        Debug.Log(currentLevelNum);
    }

    public void BlueCharacterReachedCheckpoint()
    {
        blueCharacterAtCheckpoint = true;
        CheckLevelCompletion();
    }

    // Call this method when the red character reaches their checkpoint
    public void RedCharacterReachedCheckpoint()
    {
        redCharacterAtCheckpoint = true;
        CheckLevelCompletion();
    }

    // Check if both characters are at their checkpoints
    private void CheckLevelCompletion()
    {
        if (blueCharacterAtCheckpoint && redCharacterAtCheckpoint)
        {

            // Both characters have reached their checkpoints
            SceneManager.LoadScene(targetLevelNum);
            // Add logic to handle level completion
        }
    }

    // Call this if any character leaves the checkpoint
    public void CharacterLeftCheckpoint(string characterColor)
    {
        if (characterColor == "blue")
            blueCharacterAtCheckpoint = false;
        else if (characterColor == "red")
            redCharacterAtCheckpoint = false;
    }

    public void SaveData(ref GameData data)
    {
        if (targetLevelNum - 2 > data.levelCount)
        {
            data.levelCount = targetLevelNum - 2;
        }
    }

    public void LoadData(GameData data)
    {
        // The Checkpoint Manager only Saves Data, it does not read it
    }
}

