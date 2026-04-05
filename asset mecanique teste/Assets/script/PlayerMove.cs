using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Mouvement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Saut")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private LayerMask groundLayer = 1;
    [SerializeField] private float groundCheckDistance = 1.1f;
    [SerializeField] private float groundTime = 0.1f;

    [Header("Head Bob (Balancement)")]
    [SerializeField] private bool enableHeadBob = true;
    [SerializeField] private float headBobSpeed = 10f;
    [SerializeField] private float headBobAmount = 0.1f;
    [SerializeField] private float headBobSmoothing = 10f;

    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private LayerMask interactableLayer;

    private CharacterController characterController;
    private float xRotation = 0f;
    private GameObject currentInteractable;

    private float headBobTimer = 0f;
    private Vector3 defaultCameraPosition;
    private float currentHeadBobY = 0f;

    // anti-glissade
    private Vector3 velocity;
    private float timeOnGround;
    private bool isGrounded;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCamera != null)
        {
            defaultCameraPosition = playerCamera.localPosition;
        }
        velocity = Vector3.zero;
    }

    private void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleHeadBob();
        HandleInteraction();
    }

    private void HandleMovement()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        if (isGrounded)
        {
            timeOnGround = groundTime;
        }
        else
        {
            timeOnGround -= Time.deltaTime;
        }

        bool isSprinting = Input.GetKey(sprintKey);
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        characterController.Move(move * currentSpeed * Time.deltaTime);

        // ← SAUT
        if (Input.GetKeyDown(jumpKey) && timeOnGround > 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleHeadBob()
    {
        if (!enableHeadBob || playerCamera == null) return;

        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        bool isGroundedForBob = characterController.isGrounded;

        if (isMoving && isGroundedForBob)
        {
            headBobTimer += Time.deltaTime * headBobSpeed;
            float newY = Mathf.Sin(headBobTimer) * headBobAmount;
            currentHeadBobY = Mathf.Lerp(currentHeadBobY, newY, Time.deltaTime * headBobSmoothing);
        }
        else
        {
            headBobTimer = 0f;
            currentHeadBobY = Mathf.Lerp(currentHeadBobY, 0f, Time.deltaTime * headBobSmoothing);
        }

        playerCamera.localPosition = new Vector3(
            defaultCameraPosition.x,
            defaultCameraPosition.y + currentHeadBobY,
            defaultCameraPosition.z
        );
    }

    private void HandleInteraction()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            currentInteractable = hit.collider.gameObject;

            if (Input.GetKeyDown(interactKey))
            {
                currentInteractable.SendMessage("Interact");
            }
        }
        else
        {
            currentInteractable = null;
        }
    }
}