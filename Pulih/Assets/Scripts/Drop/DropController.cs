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
            TryDrop();
        }
    }

    void TryDrop()
    {
        GameObject itemToDrop = GetItemToDrop(out string hand);

        if (itemToDrop == null) return;

        DropItem dropItem = itemToDrop.GetComponent<DropItem>();

        if (dropItem == null) return;


        StartCoroutine(DropRoutine(itemToDrop, dropItem, hand));
    }

    GameObject GetItemToDrop(out string hand)
    {
        var type = typeof(PickupController);
        var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

        GameObject bothHandItem = type.GetField("bothHandItem", flags)?.GetValue(pickupController) as GameObject;
        GameObject rightHandItem = type.GetField("rightHandItem", flags)?.GetValue(pickupController) as GameObject;
        GameObject leftHandItem  = type.GetField("leftHandItem",  flags)?.GetValue(pickupController) as GameObject;

        if (bothHandItem  != null) { hand = "both";  return bothHandItem;  }
        if (rightHandItem != null) { hand = "right"; return rightHandItem; }
        if (leftHandItem  != null) { hand = "left";  return leftHandItem;  }

        hand = null;
        return null;
    }

    void ClearHandItem(string hand)
    {
        var type  = typeof(PickupController);
        var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

        string fieldName = hand switch
        {
            "both"  => "bothHandItem",
            "right" => "rightHandItem",
            "left"  => "leftHandItem",
            _       => null
        };

        if (fieldName != null)
            type.GetField(fieldName, flags)?.SetValue(pickupController, null);
    }

    IEnumerator DropRoutine(GameObject item, DropItem dropItem, string hand)
    {
        if (animator != null && !string.IsNullOrEmpty(dropItem.dropAnimation))
            animator.SetTrigger(dropItem.dropAnimation);

        yield return new WaitForSeconds(dropItem.delayDrop);

        item.transform.SetParent(null);

        Collider col = item.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (item.GetComponent<BoxCollider>() == null)
            item.AddComponent<BoxCollider>();

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb == null) rb = item.AddComponent<Rigidbody>();

        rb.isKinematic = false;

        Vector3 throwDirection = playerCamera.transform.forward
                               + playerCamera.transform.up * dropItem.throwUpwardForce;

        rb.AddForce(throwDirection.normalized * dropItem.throwForce, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * dropItem.throwForce * 0.3f, ForceMode.Impulse);

        if (hand == "both")
        {
            PickupItem pickupItem = item.GetComponent<PickupItem>();
            if (pickupItem != null && animator != null && !string.IsNullOrEmpty(pickupItem.holdAnimation))
                animator.SetBool(pickupItem.holdAnimation, false);
        }

        ClearHandItem(hand);
    }
}