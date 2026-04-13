using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color interactColor = Color.yellow;

    public void SetInteractable(bool canInteract)
    {
        crosshairImage.color = canInteract ? interactColor : defaultColor;
    }
}