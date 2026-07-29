using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftItemDetail : MonoBehaviour
{
    public static System.Action OnItemCrafted;

    public Image thumbnail;
    public TMP_Text itemName;
    public TMP_Text description;

    [Header("Crafting Requirements UI")]
    public GameObject collectItemPrefab;
    public Transform collectItemContent;
    public Button craftButton;

    private CraftItem.ItemData currentItemData;
    private InventoryController inventoryController;

    void Start()
    {
        inventoryController = Object.FindFirstObjectByType<InventoryController>(FindObjectsInactive.Include);
        if (inventoryController == null)
        {
            inventoryController = Object.FindObjectOfType<InventoryController>(true);
        }

        if (craftButton != null)
        {
            craftButton.onClick.AddListener(OnCraftButtonClicked);
        }
    }

    void OnEnable()
    {
        RefreshRequirements();
    }

    public void UpdateDetail(CraftItem.ItemData itemData)
    {
        currentItemData = itemData;
        if (thumbnail != null) thumbnail.sprite = itemData.thumbnail;
        if (itemName != null) itemName.text = itemData.itemName;
        if (description != null) description.text = itemData.description;

        RefreshRequirements();
    }

    public void RefreshRequirements()
    {
        if (currentItemData == null) return;

        if (collectItemContent != null)
        {
            foreach (Transform child in collectItemContent)
            {
                Destroy(child.gameObject);
            }
        }

        if (inventoryController == null)
        {
            inventoryController = Object.FindFirstObjectByType<InventoryController>(FindObjectsInactive.Include);
            if (inventoryController == null)
            {
                inventoryController = Object.FindObjectOfType<InventoryController>(true);
            }
        }

        bool allRequirementsMet = true;

        if (currentItemData.requirements != null && collectItemPrefab != null && collectItemContent != null)
        {
            foreach (var req in currentItemData.requirements)
            {
                GameObject reqObj = Instantiate(collectItemPrefab, collectItemContent);
                Image icon = reqObj.transform.Find("Icon")?.GetComponent<Image>();
                TMP_Text text = reqObj.transform.Find("Text")?.GetComponent<TMP_Text>();

                int ownedCount = GetOwnedItemCount(req.itemName);
                
                if (icon != null)
                {
                    if (ownedCount == 0)
                    {
                        icon.sprite = req.iconQuestionMark;
                    }
                    else if (ownedCount < req.itemCount)
                    {
                        icon.sprite = req.iconPartial;
                    }
                    else
                    {
                        icon.sprite = req.iconDone;
                    }
                }

                if (text != null)
                {
                    text.text = $"Collect {req.itemName} ({ownedCount}/{req.itemCount})";
                }

                if (ownedCount < req.itemCount)
                {
                    allRequirementsMet = false;
                }
            }
        }
    }
    private int GetOwnedItemCount(string reqItemName)
    {
        if (inventoryController == null) return 0;
        int count = 0;
        foreach (var item in inventoryController.savedItems)
        {
            if (item.itemName == reqItemName)
            {
                count++;
            }
        }
        return count;
    }

    private void OnCraftButtonClicked()
    {
        if (currentItemData == null || inventoryController == null) return;

        foreach (var req in currentItemData.requirements)
        {
            if (GetOwnedItemCount(req.itemName) < req.itemCount)
            {
                return;
            }
        }

        foreach (var req in currentItemData.requirements)
        {
            int toRemove = req.itemCount;
            for (int i = inventoryController.savedItems.Count - 1; i >= 0 && toRemove > 0; i--)
            {
                if (inventoryController.savedItems[i].itemName == req.itemName)
                {
                    inventoryController.savedItems.RemoveAt(i);
                    toRemove--;
                }
            }
        }

        RefreshInventoryUI();

        if (currentItemData.itemPrefab != null)
        {
            GameObject craftedObj = Instantiate(currentItemData.itemPrefab);
            craftedObj.SetActive(false);

            InventoryItem invItem = craftedObj.GetComponent<InventoryItem>();
            if (invItem != null)
            {
                invItem.itemData.worldObject = craftedObj;
                inventoryController.AddItemToInventory(invItem.itemData);
            }
            else
            {
                Destroy(craftedObj);
            }
        
        }

        RefreshRequirements();
        OnItemCrafted?.Invoke();
    }

    private void RefreshInventoryUI()
    {
        if (inventoryController == null || inventoryController.content == null) return;
        
        foreach (Transform child in inventoryController.content)
        {
            Destroy(child.gameObject);
        }

        var itemsToKeep = new List<InventoryItem.ItemData>(inventoryController.savedItems);
        inventoryController.savedItems.Clear();
        
        inventoryController.selectedItem = null;
        inventoryController.selectedCard = null;
        if (inventoryController.itemDetail != null)
        {
            inventoryController.itemDetail.UpdateDetail(null, "", "");
        }

        foreach (var item in itemsToKeep)
        {
            inventoryController.AddItemToInventory(item);
        }
        
        if (inventoryController.savedItems.Count > 0)
        {
            inventoryController.SelectFirstItem();
        }
    }
}
