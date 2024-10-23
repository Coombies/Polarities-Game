using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles checkpoint logic for the red character.
/// This class is responsible for detecting when the blue character reaches
/// or leaves a checkpoint and notifying the checkpoint manager accordingly.
/// </summary>
public class RedCheckpoint : MonoBehaviour
{
    [SerializeField] private CheckpointManager checkpointManager;


    /// <summary>
    /// Called when another collider enters the trigger collider attached to this game object.
    /// Notifies the checkpoint manager if the red character reaches the checkpoint.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("RedMirror"))
        {
            checkpointManager.RedCharacterReachedCheckpoint();
        }
    }


    /// <summary>
    /// Called when another collider exits the trigger collider attached to this game object.
    /// Notifies the checkpoint manager if the red character leaves the checkpoint.
    /// </summary>
    /// <param name="other">The collider that exited the trigger.</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("RedMirror"))
        {
            checkpointManager.CharacterLeftCheckpoint("red");
        }
    }
}
