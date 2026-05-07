using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GeneratorMinigame : MonoBehaviour
{
    public static GeneratorMinigame Instance;

    [Header("UI")]
    [SerializeField] private GameObject minigamePanel;
    [SerializeField] private Slider[] sliders;
    [SerializeField] private TextMeshProUGUI[] sliderValueTexts;
    [SerializeField] private TextMeshProUGUI totalVoltageText;
    [SerializeField] private TextMeshProUGUI generatorNameText;

    private Generator currentGenerator;
    private bool isOpen = false;

    void Awake()
    {
        Instance = this;
        minigamePanel.SetActive(false);
    }

    public void Open(Generator generator)
    {
        currentGenerator = generator;
        isOpen = true;
        minigamePanel.SetActive(true);

        generatorNameText.text = generator.generatorName;

        // Configure les sliders selon le générateur
        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i].minValue = generator.minVoltage;
            sliders[i].maxValue = generator.maxVoltage;
            sliders[i].value = generator.currentValues[i];

            int index = i;
            sliders[i].onValueChanged.RemoveAllListeners();
            sliders[i].onValueChanged.AddListener((val) => OnSliderChanged(index, val));

            UpdateSliderText(i);
        }

        UpdateTotalText();

        // Bloque le curseur
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Close()
    {
        if (!isOpen) return;

        // Sauvegarde les valeurs dans le générateur
        for (int i = 0; i < sliders.Length; i++)
            currentGenerator.currentValues[i] = sliders[i].value;

        currentGenerator.SaveCalibration();
        isOpen = false;
        minigamePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Vérifie si tous les générateurs atteignent 200v
        GeneratorManager.Instance.CheckAllGenerators();
    }

    void OnSliderChanged(int index, float val)
    {
        UpdateSliderText(index);
        UpdateTotalText();
    }

    void UpdateSliderText(int index)
    {
        float val = sliders[index].value;
        sliderValueTexts[index].text = val.ToString("F1") + " V";
    }

    void UpdateTotalText()
    {
        float total = 0f;
        foreach (var s in sliders)
            total += s.value;

        totalVoltageText.text = "Total : " + total.ToString("F1") + " V";

        // Couleur selon si on est dans la bonne plage
        bool calibrated = total >= currentGenerator.targetMin && total <= currentGenerator.targetMax;
        totalVoltageText.color = calibrated ? Color.green : Color.red;
    }

    public bool IsOpen() => isOpen;
}
