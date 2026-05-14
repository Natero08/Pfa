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
    [SerializeField] private float transitionSpeed = 2f;

    [Header("Fog")]
    [SerializeField] private bool useFog = true;
    [SerializeField] private Color fogColorHaut = new Color(0.05f, 0.05f, 0.1f);
    [SerializeField] private Color fogColorBas = new Color(0.7f, 0.75f, 0.8f);
    [SerializeField] private float fogDensityHaut = 0.02f;
    [SerializeField] private float fogDensityBas = 0.005f;

    [Header("Mesh Bras")]
    [SerializeField] private GameObject brasNormal;
    [SerializeField] private GameObject brasDebloque;

    private bool isInZoneBas = false;

    private void Start()
    {
        if (dayNightCycle != null) dayNightCycle.enabled = false;
        if (nightLight != null) nightLight.enabled = true;

        if (useFog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = fogColorHaut;
            RenderSettings.fogDensity = fogDensityHaut;
        }

        // Bras normal visible au démarrage
        if (brasNormal != null) brasNormal.SetActive(true);
        if (brasDebloque != null) brasDebloque.SetActive(false);
    }

    private void Update()
    {
        if (!useFog) return;

        if (isInZoneBas)
        {
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, fogColorBas, Time.deltaTime * transitionSpeed);
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, fogDensityBas, Time.deltaTime * transitionSpeed);
        }
        else
        {
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, fogColorHaut, Time.deltaTime * transitionSpeed);
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, fogDensityHaut, Time.deltaTime * transitionSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isInZoneBas = true;

        // Lighting
        if (dayNightCycle != null) dayNightCycle.enabled = true;
        if (nightLight != null) nightLight.enabled = false;

        // Restrictions
        if (restrictJump && !PlayerAbilities.Instance.unlockedJump)
            PlayerAbilities.Instance.canJump = false;
        if (restrictCarry && !PlayerAbilities.Instance.unlockedCarry)
            PlayerAbilities.Instance.canCarry = false;
        if (restrictPush && !PlayerAbilities.Instance.unlockedPush)
            PlayerAbilities.Instance.canPush = false;
        if (restrictGenerators && !PlayerAbilities.Instance.unlockedGenerators)
            PlayerAbilities.Instance.canInteractGenerators = false;

        // Gestion du bras selon l'état du déblocage
        bool brasDebloqué = PlayerAbilities.Instance.unlockedCarry
                         || PlayerAbilities.Instance.unlockedPush;

        if (brasDebloqué)
        {
            // Capacité déjà débloquée, on montre le bon bras
            if (brasNormal != null) brasNormal.SetActive(false);
            if (brasDebloque != null) brasDebloque.SetActive(true);
        }
        else
        {
            // Pas encore débloqué, on cache tout
            if (brasNormal != null) brasNormal.SetActive(false);
            if (brasDebloque != null) brasDebloque.SetActive(false);
        }
    }


}