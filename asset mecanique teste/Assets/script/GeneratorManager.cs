using UnityEngine;

public class GeneratorManager : MonoBehaviour
{
    public static GeneratorManager Instance;

    [Header("Générateurs")]
    [SerializeField] private Generator[] generators;

    [Header("Barrière")]
    [SerializeField] private Transform barrier;
    [SerializeField] private float barrierMoveDistance = 5f;
    [SerializeField] private float barrierMoveSpeed = 2f;

    private bool barrierOpened = false;
    private Vector3 barrierTargetPosition;
    private Vector3 barrierStartPosition;

    void Awake()
    {
        Instance = this;
        
    }

    void Start()
    {
        barrierStartPosition = barrier.position;
        barrierTargetPosition = barrierStartPosition + Vector3.left * barrierMoveDistance;
    }

    void Update()
    {
        if (barrierOpened)
        {
            barrier.position = Vector3.MoveTowards(
                barrier.position,
                barrierTargetPosition,
                barrierMoveSpeed * Time.deltaTime
            );
        }
    }

    public void CheckAllGenerators()
    {
        if (barrierOpened) return;

        foreach (var g in generators)
        {
            if (!g.isCalibrated)
            {
                Debug.Log("Pas encore tous calibrés !");
                return;
            }
        }

        Debug.Log("Tous calibrés ! Barrière ouvre !");
        barrierOpened = true;
    }
}