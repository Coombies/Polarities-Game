using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Default values for when the game is started.
/// </summary>
[System.Serializable]

public class GameData
{
    public int levelCount;

    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load

    public GameData()
    {
        this.levelCount = 1;
    }
}
