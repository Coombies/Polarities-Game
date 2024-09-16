using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private bool blueCharacterAtCheckpoint = false;
    private bool redCharacterAtCheckpoint = false;

    // Call this method when the blue character reaches their checkpoint
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
            Debug.Log("Level Completed!");
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
}
