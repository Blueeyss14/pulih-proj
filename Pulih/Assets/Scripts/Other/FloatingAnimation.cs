using UnityEngine;

public class floatingAnimation : MonoBehaviour
{
    public float amplitude = 0.5f;
    public float frequency = 1f;

    private Vector3 startPos;
    private PickupController pickupController;
    private bool disabledForever = false;

    void Start()
    {
        startPos = transform.position;
        pickupController = Object.FindFirstObjectByType<PickupController>();
    }

    void Update()
    {
        if (disabledForever) return;

        bool picked =
            pickupController != null &&
            (
                pickupController.rightHandItem == gameObject ||
                pickupController.leftHandItem == gameObject ||
                pickupController.bothHandItem == gameObject
            );

        if (picked)
        {
            disabledForever = true;
            return;
        }

        Vector3 tempPos = startPos;
        tempPos.y += Mathf.Sin(Time.time * Mathf.PI * frequency) * amplitude;
        transform.position = tempPos;
    }
}