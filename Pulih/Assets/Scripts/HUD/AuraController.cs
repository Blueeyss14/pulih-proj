using UnityEngine;
using TMPro; 
using UnityEngine.UI;

public class AuraController : MonoBehaviour
{
    public Slider auraSlider;
    public TextMeshProUGUI auraText;
    public TextMeshProUGUI auraLevelText;

    public float currentAura;
    public int auraLevel;
    
    private float auraPerLevel = 500f;

    private void OnValidate()
    {
        if (auraSlider != null && auraText != null && auraLevelText != null)
        {
            UpdateUI();
        }
    }

    void Start()
    {
        UpdateUI();
    }

    public void UpdateAura(float amount)
    {
        currentAura = amount;
        UpdateUI();
    }

    public void AddAura(float amount)
    {
        currentAura += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        auraLevel = Mathf.FloorToInt(currentAura / auraPerLevel) + 1;
        float displayAura = currentAura % auraPerLevel;

        if (currentAura > 0 && displayAura == 0) 
        {
            auraLevel -= 1;
            displayAura = auraPerLevel;
        }

        auraSlider.maxValue = auraPerLevel;
        auraSlider.value = displayAura;

        auraText.text = "Aura " + Mathf.RoundToInt(displayAura) + "/" + auraPerLevel;
        auraLevelText.text = auraLevel.ToString();
    }
}