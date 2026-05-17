using UnityEngine;

public class TrashCanDump : MonoBehaviour
{
    public Chapter1Mission chapter1Mission;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Dumpster")) return;

        if (chapter1Mission != null && !chapter1Mission.trashFull) return;

        if (chapter1Mission != null)
        {
            chapter1Mission.OnTrashDumped();
        }

        Destroy(gameObject);
    }
}