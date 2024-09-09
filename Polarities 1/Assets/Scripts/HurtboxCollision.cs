using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HurtboxCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the hurtbox collided with the spike
        if (collision.gameObject.layer == LayerMask.NameToLayer("Spikes"))
        {
            Debug.Log("what the gronk");
            // Reload the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
