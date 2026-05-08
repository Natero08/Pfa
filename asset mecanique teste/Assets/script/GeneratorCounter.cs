using UnityEngine;
using TMPro;
public class GeneratorCounter : MonoBehaviour
{
    [SerializeField] private GeneratorManager myManager;
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private TextMeshProUGUI statusText;
    private void Start()
    {
        UpdateDisplay();
    }
    public void Interact()
    {
        UpdateDisplay();
    }
    public void UpdateDisplay()
    {
        int calibrated = myManager.GetCalibratedCount();
        int total = myManager.GetTotalCount();
        counterText.text = $"{calibrated} / {total}";
        counterText.color = calibrated == total ? Color.green : Color.red;
        statusText.text = "";
        foreach (string line in myManager.GetGeneratorStatus())
        {
            statusText.text += line + "\n";
        }
    }
}