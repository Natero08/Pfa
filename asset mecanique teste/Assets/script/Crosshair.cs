using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color interactColor = Color.yellow;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite interactSprite;

    public void SetInteractable(bool canInteract)
    {
        crosshairImage.color = canInteract ? interactColor : defaultColor;

        if (canInteract && interactSprite != null)
            crosshairImage.sprite = interactSprite;
        else if (!canInteract && defaultSprite != null)
            crosshairImage.sprite = defaultSprite;
    }
}