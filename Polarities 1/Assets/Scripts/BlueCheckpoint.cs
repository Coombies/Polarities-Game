using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueCheckpoint : MonoBehaviour
{
    public CheckpointManager checkpointManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BlueMirror"))
        {
            checkpointManager.BlueCharacterReachedCheckpoint();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("BlueMirror"))
        {
            checkpointManager.CharacterLeftCheckpoint("blue");
        }
    }
}
