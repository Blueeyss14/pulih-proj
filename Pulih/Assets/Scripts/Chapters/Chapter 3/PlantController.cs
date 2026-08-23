using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlantController : MonoBehaviour
{
    public static PlantController activePlant;

    public RectTransform uiElement;
    public Vector3 offset;

    public GameObject mouseUi;
    public GameObject smokeEffect;
    public GameObject progressUi;
    public GameObject perfectTimingUi;
    public bool isCompleted;

    private Camera mainCamera;
    private bool isHolding = false;

    public Animator animator;

    [Header("Object")]
    public GameObject plantObj;
    public GameObject plantObjDone;

    [Header("Planting Settings")]
    public float plantingDistance = 1.5f;
    public float approachSpeed = 2f;
    public float rotationSpeed = 10f;
    public float maxDuration = 10f;
    public int perfectTimingCount = 4;
    public float failPenaltySeconds = 2f;

    private bool isApproaching = false;
    private bool isFinished = false;
    public AliceController aliceController;
    public CharacterController playerController;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Start() {
        if (progressUi != null && activePlant == null) progressUi.SetActive(false);
        if (smokeEffect != null) smokeEffect.SetActive(false);
        if (perfectTimingUi != null && activePlant == null) perfectTimingUi.SetActive(false);
        if (plantObjDone != null) plantObjDone.SetActive(false);
    }

    void OnDisable()
    {
        if (activePlant == this)
        {
            activePlant = null;
        }
    }

    void LateUpdate()
    {
        if (isFinished)
        {
            if (uiElement != null) uiElement.gameObject.SetActive(false);
            return;
        }

        if (uiElement == null) return;

        bool leftPressed = Mouse.current.leftButton.isPressed;
        bool isTargeted = CrosshairAim.currentTarget == gameObject;

        if (isTargeted && Mouse.current.leftButton.wasPressedThisFrame)
        {
            isHolding = true;
            activePlant = this;
        }

        if (!leftPressed)
        {
            if (isHolding)
            {
                StopAllCoroutines();
                isApproaching = false;
                if (aliceController != null) aliceController.enabled = true;
                if (animator != null) animator.SetFloat("Move", 0f);

                if (activePlant == this)
                {
                    if (progressUi != null) progressUi.SetActive(false);
                    if (perfectTimingUi != null) perfectTimingUi.SetActive(false);
                    activePlant = null;
                }
            }
            isHolding = false;
        }

        if (isHolding) {
            if (uiElement != null) uiElement.gameObject.SetActive(false);
            if (mouseUi != null) mouseUi.SetActive(false);

            if (aliceController != null && !isApproaching)
            {
                Vector3 flatPlayer = new Vector3(aliceController.transform.position.x, 0f, aliceController.transform.position.z);
                Vector3 flatPlant  = new Vector3(transform.position.x, 0f, transform.position.z);

                if (Vector3.Distance(flatPlayer, flatPlant) > plantingDistance)
                    StartCoroutine(ApproachAndPlant());
                else
                {
                    Vector3 dir = transform.position - aliceController.transform.position;
                    dir.y = 0f;
                    if (dir.magnitude > 0.01f)
                    {
                        Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
                        aliceController.transform.rotation = Quaternion.Slerp(
                            aliceController.transform.rotation, targetRot, Time.deltaTime * rotationSpeed
                        );
                    }

                    aliceController.enabled = false;
                    if (smokeEffect != null) smokeEffect.SetActive(true);
                    if (animator != null) animator.SetBool("Planting", true);
                    if (progressUi != null)
                    {
                        if (!progressUi.activeSelf) progressUi.SetActive(true);

                        SliderController slider = progressUi.GetComponent<SliderController>();
                        if (slider != null)
                        {
                            slider.maxDuration = maxDuration;
                            slider.perfectTimingCount = perfectTimingCount;
                            slider.failPenaltySeconds = failPenaltySeconds;
                            slider.perfectTimingUi = perfectTimingUi;

                            if (slider.sliderValue >= 1f)
                            {
                                OnPlantingComplete();
                                return;
                            }
                        }
                    }
                    Debug.Log("Planting...");
                }
            }

            return;
        } else {
            if (animator != null) animator.SetBool("Planting", false);
            if (smokeEffect != null) smokeEffect.SetActive(false);

            if (activePlant == null || activePlant == this)
            {
                if (progressUi != null) progressUi.SetActive(false);
                if (perfectTimingUi != null) perfectTimingUi.SetActive(false);
            }
        }

        if (PlayerInZone.isInZone) {
            if (isTargeted) {
                uiElement.gameObject.SetActive(false);
                if (mouseUi != null) mouseUi.SetActive(true);
            } else {
                uiElement.gameObject.SetActive(true);
                if (CrosshairAim.currentTarget == null)
                {
                    if (mouseUi != null && (activePlant == null || activePlant == this))
                        mouseUi.SetActive(false);
                }
            }
        }
        else {
            uiElement.gameObject.SetActive(false);
            if (isTargeted || CrosshairAim.currentTarget == null)
            {
                if (mouseUi != null && (activePlant == null || activePlant == this))
                    mouseUi.SetActive(false);
            }
        }

        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position + offset);
        screenPos.z = 0;
        uiElement.position = screenPos;
    }

    IEnumerator ApproachAndPlant()
    {
        isApproaching = true;
        if (aliceController != null) aliceController.enabled = false;

        while (isHolding)
        {
            Vector3 flatPlant  = new Vector3(transform.position.x, aliceController.transform.position.y, transform.position.z);
            Vector3 flatPlayer = new Vector3(aliceController.transform.position.x, aliceController.transform.position.y, aliceController.transform.position.z);
            float distance = Vector3.Distance(flatPlayer, flatPlant);

            if (distance <= plantingDistance)
                break;

            Vector3 direction = (flatPlant - flatPlayer).normalized;

            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                aliceController.transform.rotation = Quaternion.Slerp(
                    aliceController.transform.rotation, targetRot, Time.deltaTime * rotationSpeed
                );
            }

            if (playerController != null)
                playerController.Move((direction * approachSpeed + Physics.gravity) * Time.deltaTime);

            if (animator != null)
                animator.SetFloat("Move", 0.5f, 0.1f, Time.deltaTime);

            yield return null;
        }

        if (animator != null)
            animator.SetFloat("Move", 0f, 0.1f, Time.deltaTime);

        isApproaching = false;

        if (isHolding)
        {
            if (animator != null) animator.SetBool("Planting", true);
        }
        else
        {
            if (aliceController != null) aliceController.enabled = true;
        }
    }

    void OnPlantingComplete()
    {
        isFinished = true;
        isCompleted = true;
        isHolding = false;
        isApproaching = false;
        if (activePlant == this) activePlant = null;
        StopAllCoroutines();

        if (aliceController != null) aliceController.enabled = true;
        if (animator != null)
        {
            animator.SetBool("Planting", false);
            animator.SetFloat("Move", 0f);
        }
        
        if (smokeEffect != null) smokeEffect.SetActive(false);
        if (progressUi != null) progressUi.SetActive(false);
        if (perfectTimingUi != null) perfectTimingUi.SetActive(false);
        if (uiElement != null) uiElement.gameObject.SetActive(false);
        if (mouseUi != null) mouseUi.SetActive(false);

        if (plantObj != null) plantObj.SetActive(false);
        if (plantObjDone != null) plantObjDone.SetActive(true);
    }
}