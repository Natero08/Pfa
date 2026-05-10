using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Cycle")]
    [SerializeField] private float dayDuration = 120f; // durée d'un jour complet en secondes
    [SerializeField] private float timeOfDay = 0.25f;  // 0=minuit, 0.25=matin, 0.5=midi, 0.75=soir

    [Header("Couleurs lumière")]
    [SerializeField] private Gradient lightColor;
    [SerializeField] private AnimationCurve lightIntensity;

    private Light directionalLight;

    private void Start()
    {
        directionalLight = GetComponent<Light>();

        // Courbe d'intensité par défaut si non configurée
        if (lightIntensity == null || lightIntensity.keys.Length == 0)
        {
            lightIntensity = new AnimationCurve(
                new Keyframe(0f, 0f),    // minuit : éteint
                new Keyframe(0.25f, 1f), // matin : plein
                new Keyframe(0.5f, 1f),  // midi : plein
                new Keyframe(0.75f, 0.5f), // soir : demi
                new Keyframe(1f, 0f)     // minuit : éteint
            );
        }
    }

    private void Update()
    {
        if (!isActiveAndEnabled) return;

        timeOfDay += Time.deltaTime / dayDuration;
        if (timeOfDay >= 1f) timeOfDay = 0f;

        // Rotation du soleil
        transform.rotation = Quaternion.Euler(
            (timeOfDay * 360f) - 90f, // X : arc du soleil
            170f,                      // Y : direction
            0f
        );

        // Intensité et couleur
        directionalLight.intensity = lightIntensity.Evaluate(timeOfDay);
        if (lightColor != null)
            directionalLight.color = lightColor.Evaluate(timeOfDay);
    }

    public bool IsDay() => timeOfDay > 0.2f && timeOfDay < 0.8f;
}