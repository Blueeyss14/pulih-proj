using UnityEngine;
using UnityEngine.UI;

public class CrosshairAim : MonoBehaviour
{
    public static GameObject currentTarget;

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

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance, itemLayer))
        {
            crosshair.color = red;

            currentTarget = hit.collider.gameObject;
        }
        else
        {
            crosshair.color = white;

            currentTarget = null;
        }
    }
}