// using UnityEngine;

// public class MoveOilBarrels : MonoBehaviour
// {
//     public Collider placeCollider;
//     public PickupController pickupController;
//     public int requiredBarrels = 3;
//     [HideInInspector] public int currentBarrels = 0;

//     public bool CheckBarrelsDropped()
//     {
//         GameObject[] barrels = GameObject.FindGameObjectsWithTag("Oil Barrel");
//         int count = 0;

//         foreach (GameObject barrel in barrels)
//         {
//             bool isHeld = pickupController != null && (
//                 pickupController.bothHandItem == barrel ||
//                 pickupController.rightHandItem == barrel ||
//                 pickupController.leftHandItem == barrel
//             );

//             if (!isHeld && placeCollider != null && placeCollider.bounds.Contains(barrel.transform.position))
//             {
//                 count++;
//             }
//         }

//         currentBarrels = count;
//         return currentBarrels >= requiredBarrels;
//     }
// }