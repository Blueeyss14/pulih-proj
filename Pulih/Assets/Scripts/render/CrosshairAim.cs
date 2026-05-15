using UnityEngine;
using UnityEngine.UI;

public class CrosshairAim : MonoBehaviour
{
    public Image crosshair;
    public Camera playerCamera;
    public float distance = 20f;
    public LayerMask itemLayer;

    private Color white = Color.white;
    private Color red = Color.red;

    void Update()
    {
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, distance, itemLayer))
        {
            crosshair.color = red;
        }
        else
        {
            crosshair.color = white;
        }
    }
}