using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Interface which makes saving and loading data easily
/// accessible across all stripts.
/// </summary>
public interface IDataPersistence
{
    void LoadData(GameData data);
    void SaveData(ref GameData data);
}
