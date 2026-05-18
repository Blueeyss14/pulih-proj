using UnityEngine;

public class TrashCanDump : MonoBehaviour
{
    public Chapter1Mission chapter1Mission;
    public SmokeAnimation smokeAnimation;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trash Can"))
        {
            if (smokeAnimation != null)
            {
                smokeAnimation.PlaySmoke();
            }

            if (chapter1Mission != null && chapter1Mission.hasReachedRequiredTrash)
            {
                chapter1Mission.OnTrashDumped();
            }
            
            Destroy(other.gameObject);
        }
    }
}