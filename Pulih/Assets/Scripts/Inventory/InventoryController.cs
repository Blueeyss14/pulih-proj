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
    private bool isFirstItem = true;

    [HideInInspector] public InventoryItem.ItemData selectedItem;
    [HideInInspector] public GameObject selectedCard;

    public void AddItemToInventory(InventoryItem.ItemData newItem)
    {
        savedItems.Add(newItem);
        SpawnCard(newItem, isFirstItem);
        isFirstItem = false;
    }

    void SpawnCard(InventoryItem.ItemData item, bool isFirst)
    {
        GameObject card = Instantiate(itemPrefab, content);

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
            itemDetail?.UpdateDetail(captured.thumbnail, captured.itemName, captured.description);
            selectedItem = captured;
            selectedCard = card;
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

            selectedItem = captured;
            selectedCard = card;
            itemDetail?.UpdateDetail(captured.thumbnail, captured.itemName, captured.description);
        });
    }

    void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    public void SelectFirstItem()
    {
        if (content == null || content.childCount == 0) return;

        GameObject firstCard = content.GetChild(0).gameObject;
        Image cardBg = firstCard.GetComponent<Image>();

        if (selectedBg != null && selectedBg != cardBg)
            selectedBg.sprite = selectedNormalSprite;

        int index = 0;
        for (int i = 0; i < content.childCount; i++)
        {
            if (content.GetChild(i).gameObject == firstCard) { index = i; break; }
        }

        if (index >= savedItems.Count) return;

        InventoryItem.ItemData data = savedItems[index];

        selectedBg            = cardBg;
        selectedNormalSprite  = data.cardImage;
        selectedItem          = data;
        selectedCard          = firstCard;

        if (cardBg) cardBg.sprite = data.cardImageHover;
        itemDetail?.UpdateDetail(data.thumbnail, data.itemName, data.description);
    }
}
