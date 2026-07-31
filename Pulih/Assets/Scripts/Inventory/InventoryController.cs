using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryController : MonoBehaviour
{
    [Header("Inventory Data")]
    public List<InventoryItem.ItemData> savedItems = new List<InventoryItem.ItemData>();

    [Header("UI Spawn")]
    public GameObject itemPrefab;
    public Transform content;
    public InventoryItemDetail itemDetail;

    private Image selectedBg;
    private Sprite selectedNormalSprite;

    [HideInInspector] public InventoryItem.ItemData selectedItem;
    [HideInInspector] public GameObject selectedCard;

    public int GetItemCount(string itemName)
    {
        if (string.IsNullOrEmpty(itemName)) return 0;
        int count = 0;
        for (int i = 0; i < savedItems.Count; i++)
        {
            if (savedItems[i] != null && savedItems[i].itemName == itemName)
            {
                count++;
            }
        }
        return count;
    }

    public InventoryItem.ItemData GetItemDataByName(string itemName)
    {
        if (string.IsNullOrEmpty(itemName)) return null;
        for (int i = 0; i < savedItems.Count; i++)
        {
            if (savedItems[i] != null && savedItems[i].itemName == itemName)
            {
                return savedItems[i];
            }
        }
        return null;
    }

    public bool HasItemWithName(string itemName)
    {
        if (string.IsNullOrEmpty(itemName)) return false;
        for (int i = 0; i < savedItems.Count; i++)
        {
            if (savedItems[i] != null && savedItems[i].itemName == itemName)
            {
                return true;
            }
        }
        return false;
    }

    public void AddItemToInventory(InventoryItem.ItemData newItem)
    {
        if (newItem == null) return;

        bool alreadyExists = HasItemWithName(newItem.itemName);
        savedItems.Add(newItem);

        if (alreadyExists)
        {
            if (selectedItem != null && selectedItem.itemName == newItem.itemName)
            {
                int count = GetItemCount(newItem.itemName);
                itemDetail?.UpdateDetail(selectedItem.thumbnail, selectedItem.itemName, selectedItem.description, count);
            }
            return;
        }

        bool isFirst = (content != null && content.childCount == 0);
        SpawnCard(newItem, isFirst);
    }

    void SpawnCard(InventoryItem.ItemData item, bool isFirst)
    {
        if (itemPrefab == null || content == null) return;

        GameObject card = Instantiate(itemPrefab, content);
        card.name = "Card_" + item.itemName;

        Image cardBg = card.GetComponent<Image>();
        Image thumb = card.transform.Find("Thumbnail")?.GetComponent<Image>();
        TMP_Text nameText = card.transform.Find("Item Name")?.GetComponent<TMP_Text>();

        if (cardBg != null) cardBg.sprite = item.cardImage;
        if (thumb != null) thumb.sprite = item.thumbnail;
        if (nameText != null) nameText.text = item.itemName;

        Button btn = card.GetComponent<Button>();
        if (btn != null) btn.transition = Selectable.Transition.None;

        EventTrigger trigger = card.AddComponent<EventTrigger>();
        Sprite normal = item.cardImage;
        Sprite hover = item.cardImageHover;
        InventoryItem.ItemData captured = item;

        if (isFirst)
        {
            selectedBg = cardBg;
            selectedNormalSprite = normal;
            if (cardBg) cardBg.sprite = hover;
            selectedItem = captured;
            selectedCard = card;
            itemDetail?.UpdateDetail(captured.thumbnail, captured.itemName, captured.description, GetItemCount(captured.itemName));
        }

        AddTrigger(trigger, EventTriggerType.PointerEnter, _ => { 
            if (cardBg && selectedBg != cardBg) cardBg.sprite = hover; 
        });
        
        AddTrigger(trigger, EventTriggerType.PointerExit,  _ => { 
            if (cardBg && selectedBg != cardBg) cardBg.sprite = normal; 
        });
        
        AddTrigger(trigger, EventTriggerType.PointerClick, _ => {
            if (selectedBg != null && selectedBg != cardBg)
                selectedBg.sprite = selectedNormalSprite;
            
            selectedBg = cardBg;
            selectedNormalSprite = normal;
            if (cardBg) cardBg.sprite = hover;

            InventoryItem.ItemData currentData = GetItemDataByName(captured.itemName);
            if (currentData != null) captured = currentData;

            selectedItem = captured;
            selectedCard = card;
            int count = GetItemCount(captured.itemName);
            itemDetail?.UpdateDetail(captured.thumbnail, captured.itemName, captured.description, count);
        });
    }

    void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    public void SelectFirstItem(GameObject cardToIgnore = null)
    {
        if (content == null || content.childCount == 0)
        {
            selectedItem = null;
            selectedCard = null;
            itemDetail?.UpdateDetail(null, "", "", 0);
            return;
        }

        GameObject firstCard = null;
        for (int i = 0; i < content.childCount; i++)
        {
            GameObject child = content.GetChild(i).gameObject;
            if (child != cardToIgnore && child.activeSelf)
            {
                firstCard = child;
                break;
            }
        }

        if (firstCard == null)
        {
            selectedItem = null;
            selectedCard = null;
            itemDetail?.UpdateDetail(null, "", "", 0);
            return;
        }

        Image cardBg = firstCard.GetComponent<Image>();

        if (selectedBg != null && selectedBg != cardBg)
            selectedBg.sprite = selectedNormalSprite;

        string cardName = firstCard.name;
        string itemName = cardName.StartsWith("Card_") ? cardName.Substring(5) : "";

        InventoryItem.ItemData data = GetItemDataByName(itemName);
        if (data == null && savedItems.Count > 0)
        {
            data = savedItems[0];
        }

        if (data == null)
        {
            selectedItem = null;
            selectedCard = null;
            itemDetail?.UpdateDetail(null, "", "", 0);
            return;
        }

        selectedBg            = cardBg;
        selectedNormalSprite  = data.cardImage;
        selectedItem          = data;
        selectedCard          = firstCard;

        if (cardBg) cardBg.sprite = data.cardImageHover;
        int count = GetItemCount(data.itemName);
        itemDetail?.UpdateDetail(data.thumbnail, data.itemName, data.description, count);
    }
}
