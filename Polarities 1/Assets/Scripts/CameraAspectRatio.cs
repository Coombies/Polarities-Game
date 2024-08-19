using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAspectRatio : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        // Set the desired resolution
        Screen.SetResolution(640, 480, FullScreenMode.ExclusiveFullScreen, new RefreshRate() { numerator = 60, denominator = 1 });

        // Set the camera orthographic size accordingly

        Camera.main.orthographicSize = 5.625f;

    }
}
