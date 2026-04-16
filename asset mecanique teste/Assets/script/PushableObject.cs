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

    // ✅ Offset initial entre le joueur et le cube au moment de l'interaction
    private Vector3 initialOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 3f;
        rb.angularDamping = 25f;
        rb.freezeRotation = true;
        gameObject.layer = LayerMask.NameToLayer("Pushable");
    }

    public void Interact()
    {
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

            // ✅ On mémorise l'offset cube -> joueur au moment de l'interaction
            initialOffset = transform.position - pusher.position;
            initialOffset.y = 0f;

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

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        if (Mathf.Abs(moveX) < 0.1f && Mathf.Abs(moveZ) < 0.1f)
        {
            rb.linearVelocity = Vector3.zero;

            // ✅ Quand le joueur s'arrête, on recentre le cube sur son offset initial
            Vector3 targetPosition = pusher.position + initialOffset;
            targetPosition.y = transform.position.y;
            rb.MovePosition(Vector3.Lerp(rb.position, targetPosition, 0.2f));
            return;
        }

        Vector3 inputDirection = pusher.right * moveX + pusher.forward * moveZ;
        inputDirection.y = 0f;
        inputDirection.Normalize();

        float speed = playerControllerScript.CurrentSpeed;

        // ✅ On déplace le cube ET on corrige progressivement le drift latéral
        Vector3 desiredPosition = pusher.position + initialOffset + inputDirection * speed * Time.fixedDeltaTime;
        desiredPosition.y = transform.position.y;

        rb.MovePosition(Vector3.Lerp(rb.position, desiredPosition, 0.8f));
    }

    void StopPush()
    {
        isPushing = false;
        pusher = null;
        playerControllerScript = null;
        initialOffset = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
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