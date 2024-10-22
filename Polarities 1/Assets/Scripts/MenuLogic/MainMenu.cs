using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour, IDataPersistence
{

    private int levelsUnlocked;
    // Start is called before the first frame update
    public void StartGame()
    {
        DataPersistenceManager.instance.NewGame();

        SceneManager.LoadSceneAsync(2);
    }

    // Update is called once per frame
    public void ContinueGame()
    {
        SceneManager.LoadScene(levelsUnlocked + 1);
    }

    public void LevelSelect()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SaveData(ref GameData data)
    {
        // The Level Select Manager only reads the saved data, and does not need to save any
    }

    public void LoadData(GameData data)
    {
        Debug.Log("BOP");
        levelsUnlocked = data.levelCount;
    }
}
