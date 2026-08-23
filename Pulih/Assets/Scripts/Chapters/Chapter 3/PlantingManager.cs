using System.Collections.Generic;
using UnityEngine;

public class PlantingManager : MonoBehaviour
{
    [Header("Planting Requirements")]
    public List<PlantController> requiredPlants = new List<PlantController>();

    public bool isAreaCompleted = false;
    private int lastCompletedCount = -1;

    void Update()
    {
        CheckPlantingProgress();
    }

    private void CheckPlantingProgress()
    {
        if (isAreaCompleted) return;

        int completedCount = 0;
        foreach (PlantController plant in requiredPlants)
        {
            if (plant != null && plant.isCompleted)
            {
                completedCount++;
            }
        }

        if (completedCount != lastCompletedCount)
        {
            lastCompletedCount = completedCount;
            Debug.Log($"Progress: {completedCount}/{requiredPlants.Count}");

            if (requiredPlants.Count > 0 && completedCount == requiredPlants.Count)
            {
                isAreaCompleted = true;
                Debug.Log($"Progress: {completedCount}/{requiredPlants.Count}");
            }
        }
    }
}
