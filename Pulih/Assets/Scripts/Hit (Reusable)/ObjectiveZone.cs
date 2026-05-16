using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ObjectiveZone : MonoBehaviour
{
    [Header("Settings")]
    public string targetTag = "Player";
    
    [Header("Events")]
    public UnityEvent onObjectiveReached;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            onObjectiveReached?.Invoke();
            
            Destroy(gameObject); 
        }
    }
}