using UnityEngine;
using UnityEngine.InputSystem;

public class AliceController : MonoBehaviour
{
    [SerializeField] private Transform cam;

    CharacterController playerController;
    Animator playerAnime;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    [Header("Angle Velocity")]
    [SerializeField] private float calmTime;
    private float turnVelocity;

    [Header("Jump")]
    [SerializeField] private float jumpVelocity;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpValue;

    // [Header("Footstep")]
    // [SerializeField] private AudioSource footstepSource;
    // [SerializeField] private AudioClip[] footstepClips;
    // [SerializeField] private float walkStepInterval = 0.5f;
    // [SerializeField] private float runStepInterval = 0.35f;

    // [Range(0f, 100f)]
    // [SerializeField] private float Volume = 100f;

    private float footstepTimer;

    private bool isJumping;
    private bool isGrounded;
    private bool isFallin;

    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        playerController = GetComponent<CharacterController>();
        playerAnime = GetComponent<Animator>();
    }

    void Update()
    {
        PlayerMoveFn();
        PlayerJump();

        playerController.Move(moveDirection * Time.deltaTime);
    }

    private void PlayerMoveFn()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) input.y += 1;
            if (Keyboard.current.sKey.isPressed) input.y -= 1;
            if (Keyboard.current.dKey.isPressed) input.x += 1;
            if (Keyboard.current.aKey.isPressed) input.x -= 1;
        }

        float horizontalMove = input.x;
        float verticalMove = input.y;

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 inputDirection = forward * verticalMove + right * horizontalMove;

        if (inputDirection.magnitude >= 0.1f)
        {
            inputDirection.Normalize();

            bool isRunning = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;

            if (isRunning)
                PlayerRun();
            else
                PlayerWalk();

            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;

            float angle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref turnVelocity,
                calmTime
            );

            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        else
        {
            PlayerIdle();
        }

        moveDirection.x = inputDirection.x * moveSpeed;
        moveDirection.z = inputDirection.z * moveSpeed;
    }

    private void PlayerIdle()
    {
        playerAnime.SetFloat("Move", 0f, 0.1f, Time.deltaTime);
        moveSpeed = 0f;
        footstepTimer = 0f;
    }

    private void PlayerWalk()
    {
        playerAnime.SetFloat("Move", 0.5f, 0.1f, Time.deltaTime);
        moveSpeed = walkSpeed;
        // PlayFootstep(walkStepInterval);
    }

    private void PlayerRun()
    {
        playerAnime.SetFloat("Move", 1f, 0.1f, Time.deltaTime);
        moveSpeed = runSpeed;
        // PlayFootstep(runStepInterval);
    }

    // private void PlayFootstep(float interval)
    // {
    //     if (!playerController.isGrounded) return;

    //     footstepTimer -= Time.deltaTime;
    //     if (footstepTimer > 0f) return;

    //     AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
    //     footstepSource.pitch = Random.Range(0.95f, 1.05f);
    //     footstepSource.volume = Volume / 100f;
    //     footstepSource.PlayOneShot(clip);

    //     footstepTimer = interval;
    // }

    private void PlayerJump()
    {
        if (playerController.isGrounded)
        {
            if (!isGrounded)
            {
                isJumping = false;
                isFallin = false;
            }

            isGrounded = true;
            jumpVelocity = -0.1f;

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                jumpVelocity = jumpValue;
                isJumping = true;
                isGrounded = false;
                playerAnime.SetBool("isJumping", true);
            }
        }
        else
        {
            isGrounded = false;

            if (jumpVelocity < 0 && !isFallin)
            {
                isFallin = true;
            }
        }

        jumpVelocity += gravity * Time.deltaTime;
        moveDirection.y = jumpVelocity;
    }
}