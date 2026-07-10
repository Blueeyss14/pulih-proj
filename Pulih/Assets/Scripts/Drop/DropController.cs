using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class DropController : MonoBehaviour
{
    [Header("References")]
    public PickupController pickupController;
    public Camera playerCamera;
    public Animator animator;

    void Update()
    {
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            GameObject itemToDrop = GetItemToDrop(out _);
            if (itemToDrop != null && itemToDrop.GetComponent<ThrowWithLineRender>() != null)
                return;

            TryDrop();
        }
    }

    void TryDrop()
    {
        GameObject itemToDrop = GetItemToDrop(out string hand);

        if (itemToDrop == null)
            return;

        DropItem dropItem = itemToDrop.GetComponent<DropItem>();

        if (dropItem == null)
            return;

        StartCoroutine(DropRoutine(itemToDrop, dropItem, hand));
    }

    GameObject GetItemToDrop(out string hand)
    {
        if (pickupController.bothHandItem != null)
        {
            hand = "both";
            return pickupController.bothHandItem;
        }

        if (pickupController.rightHandItem != null)
        {
            hand = "right";
            return pickupController.rightHandItem;
        }

        if (pickupController.leftHandItem != null)
        {
            hand = "left";
            return pickupController.leftHandItem;
        }

        hand = null;
        return null;
    }

    void ClearHandItem(string hand)
    {
        switch (hand)
        {
            case "both":
                pickupController.bothHandItem = null;
                break;

            case "right":
                pickupController.rightHandItem = null;
                break;

            case "left":
                pickupController.leftHandItem = null;
                break;
        }

        if (pickupController.rightHandItem == null &&
            pickupController.leftHandItem == null &&
            pickupController.bothHandItem == null)
        {
            pickupController.targetBothWeight = 0f;
            pickupController.targetLeftWeight = 0f;
            animator.SetTrigger("EmptyHold");
        }
    }

    IEnumerator DropRoutine(GameObject item, DropItem dropItem, string hand)
    {
        PickupItem pickupItem = item.GetComponent<PickupItem>();

        if (animator != null && !string.IsNullOrEmpty(dropItem.dropAnimation))
            animator.SetTrigger(dropItem.dropAnimation);

        if (pickupItem != null &&
            animator != null &&
            !string.IsNullOrEmpty(pickupItem.holdAnimation))

            // animator.SetBool(pickupItem.holdAnimation, false);

            animator.SetTrigger("EmptyHold");

        pickupController.targetBothWeight = 1f;
        pickupController.targetLeftWeight = 0f;

        ClearHandItem(hand);

        yield return new WaitForSeconds(dropItem.delayDrop);

        item.transform.SetParent(null);

        Collider col = item.GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = true;

            if (col is MeshCollider meshCol)
            {
                meshCol.convex = true;
            }
        }

        Rigidbody rb = item.GetComponent<Rigidbody>();

        if (rb == null)
            rb = item.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100f);
        }

        Vector3 throwDir = (targetPoint - item.transform.position).normalized;
        Vector3 finalThrowDir = throwDir + playerCamera.transform.up * dropItem.throwUpwardForce;

        rb.AddForce(finalThrowDir.normalized * dropItem.throwForce, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * dropItem.throwForce * 0.3f, ForceMode.Impulse);

        AliceController aliceController = Object.FindFirstObjectByType<AliceController>();

        if (aliceController != null)
            aliceController.enabled = true;
    }
}