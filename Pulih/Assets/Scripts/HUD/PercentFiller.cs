using UnityEngine;
using UnityEngine.InputSystem;

public class PercentFiller : MonoBehaviour
{
    [Header("UI Setup")]
    public RectTransform rangeUI;
    public RectTransform thickUI;

    [Header("Skill Check Settings")]
    [Range(0f, 1f)] public float startPercent = 0f;
    [Range(0f, 1f)] public float endPercent = 0f;
    public float rangeWidth = 0.2f;
    public float speed = 1.2f;

    [Header("Owner Controller")]
    public SliderController sliderController;

    private bool isActive = false;
    private bool movingRight = true;
    private float thickPercent = 0f;

    public bool IsActive => isActive;

    void OnEnable()
    {
        UpdateRangeUI();
    }

    void OnDisable()
    {
        isActive = false;
    }

    public void StartSkillCheck()
    {
        isActive = true;

        float minRangeStart = 0.1f;
        float maxRangeStart = Mathf.Max(0.1f, 0.9f - rangeWidth);
        startPercent = Random.Range(minRangeStart, maxRangeStart);
        endPercent = startPercent + rangeWidth;

        UpdateRangeUI();

        float distToMin = startPercent - 0f;
        float distToMax = 1f - endPercent;

        if (distToMin >= distToMax)
        {
            thickPercent = 0f;
            movingRight = true;
        }
        else
        {
            thickPercent = 1f;
            movingRight = false;
        }

        UpdateThickUI();
    }

    void Update()
    {
        if (!isActive) return;

        if (movingRight)
        {
            thickPercent += speed * Time.deltaTime;
        }
        else
        {
            thickPercent -= speed * Time.deltaTime;
        }

        UpdateThickUI();

        if (IsSpacePressed())
        {
            if (thickPercent >= startPercent && thickPercent <= endPercent)
            {
                OnSkillCheckSuccess();
            }
            else
            {
                OnSkillCheckFail();
            }
            return;
        }

        if (movingRight && thickPercent > endPercent)
        {
            OnSkillCheckFail();
        }
        else if (!movingRight && thickPercent < startPercent)
        {
            OnSkillCheckFail();
        }
    }

    bool IsSpacePressed()
    {
        return Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
    }

    void OnSkillCheckSuccess()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    void OnSkillCheckFail()
    {
        isActive = false;
        if (sliderController != null)
        {
            sliderController.ApplyPenalty(sliderController.failPenaltySeconds);
        }
        gameObject.SetActive(false);
    }

    void UpdateRangeUI()
    {
        if (rangeUI != null)
        {
            rangeUI.anchorMin = new Vector2(startPercent, rangeUI.anchorMin.y);
            rangeUI.anchorMax = new Vector2(endPercent, rangeUI.anchorMax.y);

            rangeUI.offsetMin = new Vector2(0f, rangeUI.offsetMin.y);
            rangeUI.offsetMax = new Vector2(0f, rangeUI.offsetMax.y);
        }
    }

    void UpdateThickUI()
    {
        if (thickUI != null)
        {
            float clampedPercent = Mathf.Clamp01(thickPercent);

            thickUI.anchorMin = new Vector2(clampedPercent, thickUI.anchorMin.y);
            thickUI.anchorMax = new Vector2(clampedPercent, thickUI.anchorMax.y);

            thickUI.anchoredPosition = new Vector2(0f, thickUI.anchoredPosition.y);
        }
    }

    void OnValidate()
    {
        UpdateRangeUI();
    }
}