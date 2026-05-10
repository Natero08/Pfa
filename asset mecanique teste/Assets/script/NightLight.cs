using UnityEngine;

public class NightLight : MonoBehaviour
{
    [Header("Nuit fixe")]
    [SerializeField] private float nightIntensity = 0.15f;
    [SerializeField] private Color nightColor = new Color(0.1f, 0.15f, 0.3f);

    private void Start()
    {
        Light l = GetComponent<Light>();
        l.intensity = nightIntensity;
        l.color = nightColor;
        transform.rotation = Quaternion.Euler(30f, 170f, 0f);
    }
}