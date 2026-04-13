using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviour
{
    [Header("Poussage")]
    [SerializeField] private float pushForce = 30f;
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float maxDistance = 3f;
    [SerializeField] private float minDistance = 0.8f;
    [SerializeField] private float followDistance = 1.5f;

    private Rigidbody rb;
    private bool isPushing = false;
    private Transform pusher;

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

        Push();
    }

    void Push()
    {
        // L'objet va simplement dans la direction où marche le joueur
        Vector3 playerVelocity = pusher.GetComponent<CharacterController>().velocity;
        playerVelocity.y = 0f;

        // Si le joueur ne bouge pas, on ne pousse pas
        if (playerVelocity.magnitude < 0.1f)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        rb.AddForce(playerVelocity.normalized * pushForce, ForceMode.Force);

        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    void StopPush()
    {
        isPushing = false;
        pusher = null;
        rb.linearVelocity = Vector3.zero;
        Debug.Log($"⏹️ ARRÊT {name}");
    }

    Transform FindPlayer()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, 3f);
        foreach (var col in nearby)
            if (col.CompareTag("Player"))
                return col.transform;
        return null;
    }
}