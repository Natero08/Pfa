using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

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
    [SerializeField] private GeneratorManager myManager;
    [Header("Restriction")]
    public string lockedMessage = "Je ne comprends pas comment ça marche...";
    public void Interact()
    {
        if (!PlayerAbilities.Instance.canInteractGenerators)
        {
            HUDMessage.Instance.ShowMessage(lockedMessage);
            return;
        }
        if (!PlayerAbilities.Instance.canInteractGenerators)
        {
            HUDMessage.Instance.ShowMessage(PlayerAbilities.Instance.generatorLockedMessage);
            return;
        }
        if (GeneratorMinigame.Instance.IsOpen())
            GeneratorMinigame.Instance.Close();
        else
            GeneratorMinigame.Instance.Open(this);
    }

    public void SaveCalibration()
    {
        {
            isCalibrated = currentValue >= targetMin && currentValue <= targetMax;
            myManager.CheckAllGenerators(); // utilise myManager au lieu de Instance 

            // Met à jour le compteur automatiquement
            GeneratorCounter counter = FindFirstObjectByType<GeneratorCounter>();
            if (counter != null)
                counter.UpdateDisplay();
        }
    }

}