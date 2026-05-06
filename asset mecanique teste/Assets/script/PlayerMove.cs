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
    [SerializeField] private float jumpCooldown = 0.3f; // ← AJOUT

    [Header("Head Bob (Balancement)")]
    [SerializeField] private bool enableHeadBob = true;
    [SerializeField] private float headBobSpeed = 10f;
    [SerializeField] private float headBobAmount = 0.1f;
    [SerializeField] private float headBobSmoothing = 10f;

    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private LayerMask interactableLayer;

    [Header("Port d'objet")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float holdDistance = 1.5f;
    [SerializeField] private KeyCode dropKey = KeyCode.G;

    private GameObject heldObject = null;
    private Rigidbody heldRigidbody = null;

    private CharacterController characterController;
    private float xRotation = 0f;
    private GameObject currentInteractable;

    public float CurrentSpeed => Input.GetKey(sprintKey) ? sprintSpeed : moveSpeed;

    private float headBobTimer = 0f;
    private Vector3 defaultCameraPosition;
    private float currentHeadBobY = 0f;

    private Vector3 velocity;
    private float timeOnGround;
    private bool isGrounded;

    private Vector3 lastPosition;
    public Vector3 Velocity { get; private set; }
    public bool IsInteracting { get; set; } = false;

    private bool isJumping = false;
    private float lastJumpTime = -1f; // ← AJOUT

    private void Start()
    {
        lastPosition = transform.position;
        characterController = GetComponent<CharacterController>();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        if (playerCamera != null)
            defaultCameraPosition = playerCamera.localPosition;
        else
            defaultCameraPosition = Vector3.zero;

        velocity = Vector3.zero;
    }

    private void Update()
    {
        Velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;

        HandleMovement();
        HandleMouseLook();
        HandleHeadBob();
        HandleInteraction();
    }

    private void HandleMovement()
    {
        if (characterController == null)
            return;

        Vector3 feetPosition = transform.position + characterController.center
                               - Vector3.up * (characterController.height / 2f);
        float edgeOffset = characterController.radius * 0.85f;
        isGrounded = Physics.Raycast(feetPosition, Vector3.down, groundCheckDistance, groundLayer)
                  || Physics.Raycast(feetPosition + transform.right * edgeOffset, Vector3.down, groundCheckDistance, groundLayer)
                  || Physics.Raycast(feetPosition - transform.right * edgeOffset, Vector3.down, groundCheckDistance, groundLayer)
                  || Physics.Raycast(feetPosition + transform.forward * edgeOffset, Vector3.down, groundCheckDistance, groundLayer)
                  || Physics.Raycast(feetPosition - transform.forward * edgeOffset, Vector3.down, groundCheckDistance, groundLayer);

        if (isGrounded && velocity.y <= 0f)
        {
            timeOnGround = groundTime;
            if (velocity.y < 0f)
                velocity.y = -2f;
            isJumping = false;
        }
        else
        {
            timeOnGround -= Time.deltaTime;
        }

        bool isSprinting = Input.GetKey(sprintKey);
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        float moveX = IsInteracting ? 0f : Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        characterController.Move(move * currentSpeed * Time.deltaTime);

        // ← SAUT avec cooldown
        if (Input.GetKeyDown(jumpKey) && timeOnGround > 0f && !isJumping && Time.time - lastJumpTime > jumpCooldown)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            timeOnGround = 0f;
            isJumping = true;
            lastJumpTime = Time.time; // ← AJOUT
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        if (playerCamera == null)
            return;

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

        bool isMoving = IsInteracting
            ? Input.GetAxis("Vertical") != 0
            : Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        if (isMoving && isGrounded)
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
        if (playerCamera == null)
            return;

        if (Input.GetKeyDown(dropKey) && heldObject != null)
        {
            DropObject();
            return;
        }

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.SphereCast(ray, 0.1f, out hit, interactionDistance, interactableLayer))
        {
            currentInteractable = hit.collider.gameObject;

            if (Input.GetKeyDown(interactKey))
            {
                Pickable pickable = currentInteractable.GetComponent<Pickable>();
                if (pickable != null)
                {
                    if (heldObject != null) DropObject();
                    PickUpObject(currentInteractable);
                }
                else
                {
                    currentInteractable.SendMessage("Interact", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
        else
        {
            currentInteractable = null;
        }

        if (heldObject != null && heldRigidbody != null)
        {
            Vector3 target = holdPoint != null
      ? holdPoint.position
      : playerCamera.position + playerCamera.forward * holdDistance;
            heldRigidbody.linearVelocity = (target - heldObject.transform.position) * 15f;
            heldRigidbody.angularVelocity = Vector3.zero;
        }
    }

    private void PickUpObject(GameObject obj)
    {
        if (obj == null)
            return;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
            return;

        heldObject = obj;
        heldRigidbody = rb;
        heldRigidbody.useGravity = false;
        heldRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void DropObject()
    {
        if (heldRigidbody != null)
        {
            heldRigidbody.useGravity = true;
            heldRigidbody.constraints = RigidbodyConstraints.None;
        }
        heldObject = null;
        heldRigidbody = null;
    }
}