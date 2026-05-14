using UnityEngine;

public class GeneratorManager : MonoBehaviour
{
    [Header("Générateurs")]
    [SerializeField] private Generator[] generators;

    [Header("Barrières")]
    [SerializeField] private Transform[] barriers;  // tableau au lieu d'une seule
    [SerializeField] private float barrierMoveDistance = 5f;
    [SerializeField] private float barrierMoveSpeed = 2f;

    private bool barrierOpened = false;
    private Vector3[] barrierTargetPositions;
    private Vector3[] barrierStartPositions;

    void Start()
    {
        barrierStartPositions = new Vector3[barriers.Length];
        barrierTargetPositions = new Vector3[barriers.Length];

        for (int i = 0; i < barriers.Length; i++)
        {
            barrierStartPositions[i] = barriers[i].position;
            barrierTargetPositions[i] = barrierStartPositions[i] + Vector3.left * barrierMoveDistance;
        }
    }

    void Update()
    {
        if (!barrierOpened) return;

        for (int i = 0; i < barriers.Length; i++)
        {
            barriers[i].position = Vector3.MoveTowards(
                barriers[i].position,
                barrierTargetPositions[i],
                barrierMoveSpeed * Time.deltaTime
            );
        }
    }

    public void CheckAllGenerators()
    {
        if (barrierOpened) return;

        foreach (var g in generators)
            if (!g.isCalibrated) return;

        Debug.Log("Tous calibrés ! Barrières ouvrent !");
        barrierOpened = true;
    }

    public int GetCalibratedCount()
    {
        int count = 0;
        foreach (var g in generators)
            if (g.isCalibrated) count++;
        return count;
    }

    public int GetTotalCount() => generators.Length;

    public string[] GetGeneratorStatus()
    {
        string[] lines = new string[generators.Length];
        for (int i = 0; i < generators.Length; i++)
        {
            string state = generators[i].isCalibrated ? "[V]" : "[X]";
            lines[i] = $"{state} {generators[i].generatorName}";
        }
        return lines;
    }

}
