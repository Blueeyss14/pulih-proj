using UnityEngine;
using TMPro;

public class InteractUi : MonoBehaviour
{
    [Header("UI References")]
    public GameObject interactUi;
    public TextMeshProUGUI itemNameText;

    void Update()
    {
        GameObject target = CrosshairAim.currentTarget;

        if (target != null && target.TryGetComponent(out InventoryItem item))
        {
            if (target.CompareTag("Item") && !Chapter2Mission.canPickupItem)
            {
                interactUi.SetActive(false);
                return;
            }

            itemNameText.text = item.itemData.itemName;
            interactUi.SetActive(true);
        }
        else
        {
            interactUi.SetActive(false);
        }
    }
}
