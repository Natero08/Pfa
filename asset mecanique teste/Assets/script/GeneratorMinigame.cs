using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GeneratorMinigame : MonoBehaviour
{
    public static GeneratorMinigame Instance;

    [Header("UI")]
    [SerializeField] private GameObject minigamePanel;
    [SerializeField] private Slider voltageSlider;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TextMeshProUGUI minText;
    [SerializeField] private TextMeshProUGUI maxText;
    [SerializeField] private TextMeshProUGUI targetRangeText;
    [SerializeField] private TextMeshProUGUI generatorNameText;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Auto-fermeture")]
    [SerializeField] private float autoCloseDistance = 3f;
    [SerializeField] private Transform playerTransform; // glisse ton Player ici dans l'Inspector

    private Generator currentGenerator;
    private bool isOpen = false;

    void Awake()
    {
        Instance = this;
        minigamePanel.SetActive(false);
    }

    void Start()
    {
        // Fallback si pas assigné dans l'Inspector
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (!isOpen || currentGenerator == null || playerTransform == null) return;

        float dist = Vector3.Distance(playerTransform.position, currentGenerator.transform.position);
        if (dist > autoCloseDistance)
            Close();
    }

    public void Open(Generator generator)
    {
        
        currentGenerator = generator;
        isOpen = true;
        minigamePanel.SetActive(true);

        voltageSlider.minValue = generator.minVoltage;
        voltageSlider.maxValue = generator.maxVoltage;
        voltageSlider.value = generator.currentValue;

        generatorNameText.text = generator.generatorName;
        minText.text = generator.minVoltage + " V";
        maxText.text = generator.maxVoltage + " V";
        targetRangeText.text = $"Cible : {generator.targetMin}V — {generator.targetMax}V";

        voltageSlider.onValueChanged.RemoveAllListeners();
        voltageSlider.onValueChanged.AddListener(OnSliderChanged);

        UpdateUI(voltageSlider.value);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Close()
    {
        if (!isOpen) return;

        currentGenerator.currentValue = voltageSlider.value;
        currentGenerator.SaveCalibration();

        isOpen = false;
        minigamePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnSliderChanged(float val)
    {
        UpdateUI(val);
    }

    void UpdateUI(float val)
    {
        valueText.text = val.ToString("F1") + " V";
        bool inRange = val >= currentGenerator.targetMin && val <= currentGenerator.targetMax;
        valueText.color = inRange ? Color.green : Color.red;
        statusText.text = inRange ? "Calibré" : "Hors plage";
        statusText.color = inRange ? Color.green : Color.red;
    }

    public bool IsOpen() => isOpen;
}