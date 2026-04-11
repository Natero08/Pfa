using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviour
{
    [Header("Poussage")]
    [SerializeField] private float pushForce = 12f;
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float maxDistance = 2.5f;
    [SerializeField] private float minDistance = 0.3f; // ← ajoute cette ligne
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
        if (!isPushing || pusher == null) return;

        float distance = Vector3.Distance(transform.position, pusher.position);

        if (distance > maxDistance)
        {
            StopPush(); // trop loin
            return;
        }

        if (distance < minDistance)
        {
            rb.linearVelocity = Vector3.zero; // stoppe l'objet
            return;
        }

        Push();
    }

    void Push()
    {
        Vector3 pushDirection = Vector3.ProjectOnPlane(pusher.forward, Vector3.up).normalized;

        if (rb.linearVelocity.magnitude < maxSpeed)
            rb.AddForce(pushDirection * pushForce, ForceMode.Force);
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