using UnityEngine;

public class ScreenSetting : MonoBehaviour
{
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        
        Application.targetFrameRate = 120;
        
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
    }
}