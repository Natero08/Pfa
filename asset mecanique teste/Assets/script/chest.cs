using UnityEngine;

public class Chest : MonoBehaviour
{
    private bool isOpened = false;

    public void Interact()
    {
        if (isOpened)
        {
            Debug.Log("Coffre déjà ouvert !");
            return;
        }

        isOpened = true;
        Debug.Log("Coffre ouvert !");

        // Ouvre le lid
        Transform lid = transform.Find("ChestLid");
        if (lid != null)
        {
            lid.localRotation = Quaternion.Euler(110, 0, 0);
        }
    }
}