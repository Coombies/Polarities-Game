using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


/// <summary>
/// Writes and reads all of the game data to the file data.pol.
/// Also encrypts the data to prevent potential cheaters.
/// </summary>
public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "gronk";

    /// <summary>
    /// Assigns variables to class.
    /// </summary>
    /// <param name="dataDirPath">Data Directory.</param>
    /// <param name="dataFileName">Data file name.</param>
    /// <param name="useEncryption">Bool for using encryption.</param>
    public FileDataHandler(
        string dataDirPath,
        string dataFileName,
        bool useEncryption
    )
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }


    /// <summary>
    /// Loads game data in Json format.
    /// </summary>
    /// <returns>Loaded Data from the data.pol file.</returns>
    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                // load the serialised data from the file
                string dataToLoad = "";
                using (FileStream stream = 
                    new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // optionally decrypt the data
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                // deserialise the data from Json back into the C# object
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError(
                    "Error occured when trying to load data from file: " +
                    fullPath +
                    "\n" +
                    e
                );
            }
        }
        return loadedData;
    }


    /// <summary>
    /// Writes the data to the data.pol file.
    /// </summary>
    /// <param name="data">Game data</param>
    public void Save(GameData data)
    {
        // use Path.Combine to account for different OS's
        // having different path separators
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            // create the directory the file will be
            // written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // serialise the C# game data object into Json
            string dataToStore = JsonUtility.ToJson(data, true);

            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            // wrtie the serialised data to the file
            using (FileStream stream = 
                new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(
                "Error occured when trying to save data to file: " +
                fullPath +
                "\n" +
                e
            );
        }
    }

    /// <summary>
    /// Below is a simple implementation of XOR encryption.
    /// </summary>
    /// <param name="data">Game data.</param>
    /// <returns>Encrypted data.</returns>
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ 
                encryptionCodeWord[i % encryptionCodeWord.Length]);
        }

        return modifiedData;
    }
}
