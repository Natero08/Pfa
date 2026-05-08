using UnityEngine;
using TMPro;
using System.Collections;

public class HUDMessage : MonoBehaviour
{
    public static HUDMessage Instance;

    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float displayDuration = 3f;

    void Awake()
    {
        Instance = this;
        messageText.text = "";
    }

    public void ShowMessage(string message)
    {
        StopAllCoroutines();
        StartCoroutine(DisplayMessage(message));
    }

    private IEnumerator DisplayMessage(string message)
    {
        messageText.text = message;
        yield return new WaitForSeconds(displayDuration);
        messageText.text = "";
    }
}