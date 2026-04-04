using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Mouvement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Transform playerCamera;

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
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (playerCamera != null)
        {
            defaultCameraPosition = playerCamera.localPosition;
        }
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
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        characterController.Move(move * moveSpeed * Time.deltaTime);
        characterController.Move(Vector3.down * 9.81f * Time.deltaTime);
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

        // Vérifie si le joueur bouge
        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        bool isGrounded = characterController.isGrounded;

        if (isMoving && isGrounded)
        {
            // Incrémente le timer
            headBobTimer += Time.deltaTime * headBobSpeed;

            // Calcule la nouvelle position avec un sinus
            float newY = Mathf.Sin(headBobTimer) * headBobAmount;

            // Lissage de la transition
            currentHeadBobY = Mathf.Lerp(currentHeadBobY, newY, Time.deltaTime * headBobSmoothing);
        }
        else
        {
            // Retour à la position par défaut
            headBobTimer = 0f;
            currentHeadBobY = Mathf.Lerp(currentHeadBobY, 0f, Time.deltaTime * headBobSmoothing);
        }

        // Applique le balancement à la caméra
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