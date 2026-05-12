using UnityEngine;

public class RestrictionZone : MonoBehaviour
{
    [Header("Restrictions")]
    [SerializeField] private bool restrictJump = true;
    [SerializeField] private bool restrictCarry = true;
    [SerializeField] private bool restrictPush = true;
    [SerializeField] private bool restrictGenerators = true;

    [Header("Lighting")]
    [SerializeField] private DayNightCycle dayNightCycle;
    [SerializeField] private NightLight nightLight;
    [SerializeField] private Light lightZoneHaut;
    [SerializeField] private Light lightZoneBas;
    [SerializeField] private float transitionSpeed = 2f;

    [Header("Intensités cibles")]
    [SerializeField] private float intensityHaut = 0.1f;   // nuit sombre
    [SerializeField] private float intensityBas = 1f;      // jour/nuit habituel

    [Header("Couleurs")]
    [SerializeField] private Color colorZoneHaut = new Color(0.1f, 0.1f, 0.18f); // bleu nuit
    [SerializeField] private Color colorZoneBas = new Color(1f, 0.95f, 0.88f);   // blanc chaud

    [Header("Fog")]
    [SerializeField] private bool useFog = true;
    [SerializeField] private Color fogColorHaut = new Color(0.05f, 0.05f, 0.1f);
    [SerializeField] private Color fogColorBas = new Color(0.7f, 0.75f, 0.8f);
    [SerializeField] private float fogDensityHaut = 0.02f;
    [SerializeField] private float fogDensityBas = 0.005f;

    private bool isInZoneBas = false;

    private void Start()
    {
        // état initial : zone haut active
        if (lightZoneHaut != null)
        {
            lightZoneHaut.intensity = intensityHaut;
            lightZoneHaut.color = colorZoneHaut;
        }
        if (lightZoneBas != null)
        {
            lightZoneBas.intensity = 0f;
            lightZoneBas.color = colorZoneBas;
        }

        if (useFog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = fogColorHaut;
            RenderSettings.fogDensity = fogDensityHaut;
        }
    }

    private void Update()
    {
        if (lightZoneHaut == null || lightZoneBas == null) return;

        if (isInZoneBas)
        {
            // transition vers zone bas
            lightZoneHaut.intensity = Mathf.Lerp(lightZoneHaut.intensity, 0f, Time.deltaTime * transitionSpeed);
            lightZoneBas.intensity = Mathf.Lerp(lightZoneBas.intensity, intensityBas, Time.deltaTime * transitionSpeed);
            lightZoneHaut.color = Color.Lerp(lightZoneHaut.color, colorZoneHaut, Time.deltaTime * transitionSpeed);
            lightZoneBas.color = Color.Lerp(lightZoneBas.color, colorZoneBas, Time.deltaTime * transitionSpeed);

            if (useFog)
            {
                RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, fogColorBas, Time.deltaTime * transitionSpeed);
                RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, fogDensityBas, Time.deltaTime * transitionSpeed);
            }
        }
        else
        {
            // transition vers zone haut
            lightZoneHaut.intensity = Mathf.Lerp(lightZoneHaut.intensity, intensityHaut, Time.deltaTime * transitionSpeed);
            lightZoneBas.intensity = Mathf.Lerp(lightZoneBas.intensity, 0f, Time.deltaTime * transitionSpeed);
            lightZoneHaut.color = Color.Lerp(lightZoneHaut.color, colorZoneHaut, Time.deltaTime * transitionSpeed);
            lightZoneBas.color = Color.Lerp(lightZoneBas.color, colorZoneBas, Time.deltaTime * transitionSpeed);

            if (useFog)
            {
                RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, fogColorHaut, Time.deltaTime * transitionSpeed);
                RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, fogDensityHaut, Time.deltaTime * transitionSpeed);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isInZoneBas = true;

        // On ne restreint que si la capacité n'est pas débloquée de façon permanente
        if (restrictJump && !PlayerAbilities.Instance.unlockedJump)
            PlayerAbilities.Instance.canJump = false;

        if (restrictCarry && !PlayerAbilities.Instance.unlockedCarry)
            PlayerAbilities.Instance.canCarry = false;

        if (restrictPush && !PlayerAbilities.Instance.unlockedPush)
            PlayerAbilities.Instance.canPush = false;

        if (restrictGenerators && !PlayerAbilities.Instance.unlockedGenerators)
            PlayerAbilities.Instance.canInteractGenerators = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isInZoneBas = false;

        // En sortant tout redevient accessible
        if (restrictJump) PlayerAbilities.Instance.canJump = true;
        if (restrictCarry) PlayerAbilities.Instance.canCarry = true;
        if (restrictPush) PlayerAbilities.Instance.canPush = true;
        if (restrictGenerators) PlayerAbilities.Instance.canInteractGenerators = true;
    }
}