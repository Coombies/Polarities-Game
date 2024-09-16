using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCheckpoint : MonoBehaviour
{
    public CheckpointManager checkpointManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("RedMirror"))
        {
            checkpointManager.RedCharacterReachedCheckpoint();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("RedMirror"))
        {
            checkpointManager.CharacterLeftCheckpoint("red");
        }
    }
}
