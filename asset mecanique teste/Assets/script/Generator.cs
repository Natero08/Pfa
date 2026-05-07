using UnityEngine;

public class Generator : MonoBehaviour
{
    [Header("Identité")]
    public string generatorName = "Générateur A";

    [Header("Valeurs")]
    public float minVoltage = 0f;
    public float maxVoltage = 100f;
    public float targetMin = 60f;   // plage acceptable
    public float targetMax = 80f;

    [Header("Boutons")]
    public int sliderCount = 3;
    public float[] currentValues;

    public bool isCalibrated = false;

    void Start()
    {
        // Initialise les valeurs à zéro
        currentValues = new float[sliderCount];
        for (int i = 0; i < sliderCount; i++)
            currentValues[i] = minVoltage;
    }

    public void Interact()
    {
        if (GeneratorMinigame.Instance.IsOpen())
        {
            GeneratorMinigame.Instance.Close();
        }
        else
        {
            GeneratorMinigame.Instance.Open(this);
        }
    }

    public void SaveCalibration()
    {
        float total = 0f;
        foreach (var v in currentValues)
            total += v;

        isCalibrated = total >= targetMin && total <= targetMax;
        Debug.Log($"{generatorName} calibré : {isCalibrated} ({total}V)");
    }
}