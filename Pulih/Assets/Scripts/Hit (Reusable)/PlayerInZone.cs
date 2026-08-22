using UnityEngine;
using UnityEngine.Events;

public class PlayerInZone : MonoBehaviour
{
    [Header("Settings")]
    public string targetTag = "Player";
    public static bool isInZone = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            isInZone = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            isInZone = false;
        }
    }
}