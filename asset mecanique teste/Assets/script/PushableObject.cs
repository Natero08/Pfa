using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviour
{
    [Header("Poussage")]
    [SerializeField] private float pushForce = 12f;
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float maxDistance = 2.5f;
    [SerializeField] private float minDistance = 0.8f;
    [SerializeField] private float followDistance = 1.5f;
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
        Vector3 forward = Vector3.ProjectOnPlane(pusher.forward, Vector3.up).normalized;

        // Position cible : devant le joueur à followDistance
        Vector3 targetPosition = pusher.position + forward * followDistance;
        targetPosition.y = transform.position.y;

        float distance = Vector3.Distance(transform.position, targetPosition);
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Force proportionnelle à la distance, sans blocage par dot
        float forceMult = Mathf.Clamp(distance, 0.5f, 3f);
        rb.AddForce(direction * pushForce * forceMult, ForceMode.Force);

        // Limite la vitesse manuellement
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
        Collider[] nearby = Physics.OverlapSphere(transform.position, 2f);
        foreach (var col in nearby)
            if (col.CompareTag("Player"))
                return col.transform;
        return null;
    }

    bool HasArm() { return true; }
}