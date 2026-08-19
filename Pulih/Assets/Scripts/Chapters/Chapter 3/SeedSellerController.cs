using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class SeedSellerController : MonoBehaviour
{
    [System.Serializable]
    public class ItemData
    {
        public GameObject item;
        // public Sprite thumbnail;
        public string name;
        public Sprite cardImage;
        public Sprite cardImageHover;
    }

    [Header("Item Data")]
    public List<ItemData> items = new List<ItemData>();
    public TMP_Text itemName;

    [Header("UI")]
    public GameObject interactUi;
    public GameObject seedSellerUi;

    private ChapterManager chapterManager;
    private ItemData selectedItem = null;

    void Start()
    {
        if (interactUi != null)   interactUi.SetActive(false);
        if (seedSellerUi != null) seedSellerUi.SetActive(false);
        chapterManager = FindObjectOfType<ChapterManager>();

        for (int i = 0; i < items.Count; i++)
        {
            SetupCard(items[i]);
        }

        if (items.Count > 0 && itemName != null)
            itemName.text = items[0].name;
    }

    void SetupCard(ItemData data)
    {
        if (data.item == null) return;

        Image cardBg = data.item.GetComponent<Image>();

        if (cardBg != null) cardBg.sprite = data.cardImage;

        EventTrigger trigger = data.item.GetComponent<EventTrigger>() ?? data.item.AddComponent<EventTrigger>();

        AddTrigger(trigger, EventTriggerType.PointerEnter, _ =>
        {
            if (cardBg != null) cardBg.sprite = data.cardImageHover;
            if (itemName != null) itemName.text = data.name;
        });

        AddTrigger(trigger, EventTriggerType.PointerExit, _ =>
        {
            if (selectedItem == data) return;
            if (cardBg != null) cardBg.sprite = data.cardImage;
        });

        AddTrigger(trigger, EventTriggerType.PointerClick, _ =>
        {
            if (selectedItem != null && selectedItem != data)
            {
                Image prevCardBg = selectedItem.item.GetComponent<Image>();
                if (prevCardBg != null) prevCardBg.sprite = selectedItem.cardImage;
            }

            selectedItem = data;
            if (cardBg != null) cardBg.sprite = data.cardImageHover;
            if (itemName != null) itemName.text = data.name;
        });
    }

    void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    void Update()
    {
        if (interactUi == null) return;

        if (chapterManager.currentChapter == 3)
        {
            bool isAimed = CrosshairAim.currentTarget == gameObject;
            interactUi.SetActive(isAimed);

            if (isAimed && Keyboard.current.eKey.wasPressedThisFrame)
            {
                isAimed = false;
                OpenMenu();
            }
        }
    }

    private void OpenMenu()
    {
        if (seedSellerUi != null) seedSellerUi.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseMenu()
    {
        foreach (var item in items)
        {
            if (item.item == null) continue;
            Image cardBg = item.item.GetComponent<Image>();
            if (cardBg != null) cardBg.sprite = item.cardImage;
        }
        selectedItem = null;

        if (seedSellerUi != null) seedSellerUi.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
