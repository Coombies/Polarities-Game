using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour, IDataPersistence
{

    private int levelsUnlocked;


    /// <summary>
    /// Resets the players progress.
    /// Should add a "Do you wish to continue" option for
    /// end user considerations in the future.
    /// </summary>
    public void StartGame()
    {
        DataPersistenceManager.instance.NewGame();

        SceneManager.LoadSceneAsync(2);
    }

    
    /// <summary>
    /// Continues from where the player left off
    /// </summary>
    public void ContinueGame()
    {
        SceneManager.LoadScene(levelsUnlocked + 1);
    }


    /// <summary>
    /// Takes the player to a new scene where they can select which
    /// desired level they would like to attempt
    /// </summary>
    public void LevelSelect()
    {
        SceneManager.LoadScene(1);
    }
    

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
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
    /// Reads which level the player is up to so that they can continue.
    /// </summary>
    /// <param name="data">Data from the games data.pol file</param>
    public void LoadData(GameData data)
    {
        levelsUnlocked = data.levelCount;
    }
}
