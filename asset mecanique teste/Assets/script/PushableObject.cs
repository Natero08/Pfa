using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviour
{
    [Header("Poussage")]
    [SerializeField] private float pushForce = 12f;
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float maxDistance = 2.5f;

    private Rigidbody rb;
    private bool isPushing = false;  // Toggle on/off
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
        if (!HasArm())
        {
            Debug.Log("❌ Pas de bras !");
            return;
        }

        // ⭐ TOGGLE : E = start/stop
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
        if (isPushing && pusher != null && Vector3.Distance(transform.position, pusher.position) < maxDistance)
        {
            Push();
        }
        else if (isPushing)
        {
            StopPush(); // Trop loin
        }
    }

    void Push()
    {
        Vector3 direction = (pusher.position - transform.position);

        // ⭐ REPOUSSE si trop proche
        float distance = Vector3.Distance(transform.position, pusher.position);
        if (distance < 1.2f) // Trop proche
        {
            direction = (transform.position - pusher.position).normalized;
            rb.AddForce(direction * pushForce * 1.5f, ForceMode.Force);
            return;
        }

        direction = Vector3.ProjectOnPlane(direction, Vector3.up).normalized;

        if (rb.linearVelocity.magnitude < maxSpeed)
            rb.AddForce(direction * pushForce);
    }

    void StopPush()
    {
        isPushing = false;
        pusher = null;
        Debug.Log($"⏹️ ARRÊT {name}");
    }

    Transform FindPlayer()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, 2f);
        foreach (var col in nearby)
            if (col.CompareTag("Player"))
                return col.transform;
        return null;
    }

    bool HasArm() { return true; }
}