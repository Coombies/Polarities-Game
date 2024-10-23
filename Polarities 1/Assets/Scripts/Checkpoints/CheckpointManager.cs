using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour, IDataPersistence
{
    private bool blueCharacterAtCheckpoint = false;
    private bool redCharacterAtCheckpoint = false;
    private int currentLevelNum;
    private static CheckpointManager instance;

    [SerializeField] private int targetLevelNum;

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

    /// <summary>
    /// Call this method when the blue character reaches their checkpoint
    /// </summary>
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

    /// <summary>
    /// Call this method when the red character reaches their checkpoint
    /// </summary>
    public void RedCharacterReachedCheckpoint()
    {
        redCharacterAtCheckpoint = true;
        CheckLevelCompletion();
    }


    /// <summary>
    /// Check if both players are at their checkpoint.
    /// </summary>
    private void CheckLevelCompletion()
    {
        if (blueCharacterAtCheckpoint && redCharacterAtCheckpoint)
        {

            // Both characters have reached their checkpoints
            SceneManager.LoadScene(targetLevelNum);
            // Add logic to handle level completion
        }
    }

    
    /// <summary>
    /// Check if any player leaves their checkpoint.
    /// </summary>
    /// <param name="characterColor">Which player it is.</param>
    public void CharacterLeftCheckpoint(string characterColor)
    {
        if (characterColor == "blue")
            blueCharacterAtCheckpoint = false;
        else if (characterColor == "red")
            redCharacterAtCheckpoint = false;
    }


    /// <summary>
    /// Saves game data.
    /// If the player has reached a new level, save it to data.pol.
    /// </summary>
    /// <param name="data">Data from the games data.pol file</param>
    public void SaveData(ref GameData data)
    {
        if (targetLevelNum - 2 > data.levelCount)
        {
            data.levelCount = targetLevelNum - 2;
        }
    }


    /// <summary>
    /// Loads game data.
    /// </summary>
    /// <param name="data">Data from the games data.pol file</param>
    public void LoadData(GameData data)
    {
        // The Checkpoint Manager only Saves Data, it does not read it
    }
}

