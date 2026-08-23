using UnityEngine;
using System.Collections.Generic;

public class SliderController : MonoBehaviour
{
    [Header("UI Setup")]
    public RectTransform rangeUI;
    public PercentFiller percentFiller;
    public GameObject perfectTimingUi;

    [Header("Planting Settings (Inspector Configurable)")]
    public float maxDuration = 10f;
    public int perfectTimingCount = 4;
    public float failPenaltySeconds = 2f;

    [Header("Progress State")]
    [Range(0f, 1f)] public float sliderValue = 0f;

    private List<float> triggerPoints = new List<float>();
    private List<bool> triggeredFlags = new List<bool>();

    void Awake()
    {
        FindReferences();
    }

    void OnEnable()
    {
        FindReferences();
        ResetPlanting();
    }

    void FindReferences()
    {
        if (percentFiller == null)
        {
            percentFiller = GetComponentInChildren<PercentFiller>();
            if (percentFiller == null)
            {
                percentFiller = FindObjectOfType<PercentFiller>(true);
            }
        }

        if (percentFiller != null)
        {
            percentFiller.sliderController = this;
            if (perfectTimingUi == null)
            {
                perfectTimingUi = percentFiller.gameObject;
            }
        }
    }

    public void ResetPlanting()
    {
        sliderValue = 0f;
        UpdateUI();
        GenerateTriggerPoints();
    }

    void GenerateTriggerPoints()
    {
        triggerPoints.Clear();
        triggeredFlags.Clear();

        if (perfectTimingCount <= 0) return;

        float startMargin = 0.10f;
        float endMargin = 0.88f;
        float interval = (endMargin - startMargin) / perfectTimingCount;

        for (int i = 0; i < perfectTimingCount; i++)
        {
            float minPoint = startMargin + (i * interval);
            float maxPoint = minPoint + (interval * 0.8f);
            float randomPoint = Random.Range(minPoint, maxPoint);
            triggerPoints.Add(randomPoint);
            triggeredFlags.Add(false);
        }
    }

    void Update()
    {
        if (sliderValue < 1f)
        {
            sliderValue += Time.deltaTime / Mathf.Max(0.1f, maxDuration);
            sliderValue = Mathf.Clamp01(sliderValue);
            UpdateUI();

            CheckForSkillCheck();
        }
    }

    void CheckForSkillCheck()
    {
        if (percentFiller == null) return;
        if (percentFiller.IsActive) return;

        for (int i = 0; i < triggerPoints.Count; i++)
        {
            if (!triggeredFlags[i] && sliderValue >= triggerPoints[i])
            {
                triggeredFlags[i] = true;
                if (perfectTimingUi != null)
                {
                    perfectTimingUi.SetActive(true);
                }
                percentFiller.StartSkillCheck();
                break;
            }
        }
    }

    public void ApplyPenalty(float seconds)
    {
        float penaltyPercent = seconds / Mathf.Max(0.1f, maxDuration);
        sliderValue = Mathf.Max(0f, sliderValue - penaltyPercent);
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (rangeUI != null)
        {
            rangeUI.anchorMin = new Vector2(0f, 0f);
            rangeUI.anchorMax = new Vector2(sliderValue, 1f);
            rangeUI.offsetMin = Vector2.zero;
            rangeUI.offsetMax = Vector2.zero;
        }
    }

    void OnValidate()
    {
        UpdateUI();
    }
}