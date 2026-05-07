using UnityEngine;

public class Generator : MonoBehaviour
{
    [Header("Identité")]
    public string generatorName = "Générateur A";

    [Header("Valeurs")]
    public float minVoltage = 0f;
    public float maxVoltage = 100f;
    public float targetMin = 55f;
    public float targetMax = 75f;

    public float currentValue = 0f;
    public bool isCalibrated = false;

    public void Interact()
    {
        if (GeneratorMinigame.Instance.IsOpen())
            GeneratorMinigame.Instance.Close();
        else
            GeneratorMinigame.Instance.Open(this);
    }

    public void SaveCalibration()
    {
        isCalibrated = currentValue >= targetMin && currentValue <= targetMax;
        Debug.Log($"{generatorName} calibré : {isCalibrated} ({currentValue}V)");
    }
}