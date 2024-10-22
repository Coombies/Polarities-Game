using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{

    public int levelNum;
    // Start is called before the first frame update
    public void OpenScene()
    {
        SceneManager.LoadScene(levelNum + 1);
    }
}
