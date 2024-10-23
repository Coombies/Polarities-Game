using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Selects which scene to go to when a button is pressed.
/// </summary>
public class LevelSelect : MonoBehaviour
{

    [SerializeField] private int levelNum;
    

    /// <summary>
    /// OpenScene() is called when a button is pressed.
    /// </summary>
    public void OpenScene()
    {
        SceneManager.LoadScene(levelNum + 1);
    }
}
