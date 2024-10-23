using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;


/// <summary>
/// Handles the game data while the game is being played.
/// </summary>
public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public static DataPersistenceManager instance { get; private set; }


    /// <summary>
    /// Called immediately when the script is loaded.
    /// Checks to see if there is another DataPersistenceManager
    /// object present in the scene.
    /// </summary>
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("Found more than one Data Persistence Manager in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
    }


    /// <summary>
    /// Called when the object becomes enabled.
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }


    /// <summary>
    /// Called when the object becomes disabled.
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }


    /// <summary>
    /// Loads game data at the start of every scene.
    /// </summary>
    /// <param name="scene">Which scene it is.</param>
    /// <param name="mode">Scene mode.</param>
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded Called");
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }


    /// <summary>
    /// Saves game data at the end of every scene.
    /// </summary>
    /// <param name="scene">Which Scene it is.</param>
    public void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("OnSceneUnloaded Called");
        SaveGame();
    }


    /// <summary>
    /// Resets game data on NewGame()
    /// </summary>
    public void NewGame()
    {
        this.gameData = new GameData();
    }
    

    /// <summary>
    /// Loads the game.
    /// </summary>
    public void LoadGame()
    {
        this.gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("No data was found. Initialising data to defaults.");
            NewGame();
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

    }


    /// <summary>
    /// Saves the game.
    /// </summary>
    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }


    /// <summary>
    /// Calls upon SaveGame() when the game is exited.
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveGame();
    }


    /// <summary>
    /// Converts all of the game data into a list.
    /// Useful for future proofing.
    /// </summary>
    /// <returns></returns>
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = 
            FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        foreach (var obj in dataPersistenceObjects)
        {
            Debug.Log($"Found IDataPersistence object: {obj.GetType().Name}");
        }

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
