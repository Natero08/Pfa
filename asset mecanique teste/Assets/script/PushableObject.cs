using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviour
{
    [Header("Poussage")]
    [SerializeField] private float maxDistance = 3f;

    private Rigidbody rb;
    private bool isPushing = false;
    private Transform pusher;
    private PlayerController playerControllerScript;
    private Vector3 initialOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 3f;
        rb.angularDamping = 25f;
        rb.freezeRotation = true;
        gameObject.layer = LayerMask.NameToLayer("Pushable");
    }

    [Header("Restriction")]
    [SerializeField] private string lockedMessage = "C'est trop lourd...";

    public void Interact()
    {
        if (!PlayerAbilities.Instance.canPush)
        {
            HUDMessage.Instance.ShowMessage(lockedMessage);
            return;
        }

        isPushing = !isPushing;
        if (isPushing)
        {
            pusher = FindPlayer();
            if (pusher == null)
            {
                isPushing = false;
                Debug.LogWarning("⚠️ Aucun joueur trouvé pour pousser.");
                return;
            }
            playerControllerScript = pusher.GetComponent<PlayerController>();
            if (playerControllerScript == null)
            {
                isPushing = false;
                pusher = null;
                Debug.LogWarning("⚠️ Le joueur n'a pas de script PlayerController.");
                return;
            }
            initialOffset = transform.position - pusher.position;
            initialOffset.y = 0f;
            playerControllerScript.IsInteracting = true;
            Debug.Log($"🚀 DÉBUT poussage {name}");
        }
        else
        {
            StopPush();
        }
    }

    void Update()
    {
        if (!isPushing || pusher == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, pusher.position);
        if (distanceToPlayer > maxDistance)
        {
            StopPush();
            return;
        }
    }

    void FixedUpdate()
    {
        if (!isPushing || pusher == null) return;

        // ✅ Mouvement latéral bloqué, uniquement avant/arrière
        float moveZ = Input.GetAxis("Vertical");

        if (Mathf.Abs(moveZ) < 0.1f)
        {
            rb.linearVelocity = Vector3.zero;

            Vector3 targetPosition = pusher.position + initialOffset;
            targetPosition.y = transform.position.y;
            rb.MovePosition(Vector3.Lerp(rb.position, targetPosition, 0.2f));
            return;
        }

        // ✅ Uniquement la direction avant/arrière du joueur
        Vector3 inputDirection = pusher.forward * moveZ;
        inputDirection.y = 0f;
        inputDirection.Normalize();

        float speed = playerControllerScript.CurrentSpeed;

        Vector3 desiredPosition = pusher.position + initialOffset + inputDirection * speed * Time.fixedDeltaTime;
        desiredPosition.y = transform.position.y;

        rb.MovePosition(Vector3.Lerp(rb.position, desiredPosition, 0.8f));
    }

    void StopPush()
    {
        isPushing = false;
        pusher = null;
        initialOffset = Vector3.zero;
        rb.linearVelocity = Vector3.zero;

        // ✅ On débloque AVANT de mettre à null
        if (playerControllerScript != null)
            playerControllerScript.IsInteracting = false;

        playerControllerScript = null; // ✅ Mis à null EN DERNIER
        Debug.Log($"⏹️ ARRÊT {name}");
    }

    Transform FindPlayer()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, maxDistance);
        foreach (var col in nearby)
            if (col.CompareTag("Player"))
                return col.transform;
        return null;
    }
}