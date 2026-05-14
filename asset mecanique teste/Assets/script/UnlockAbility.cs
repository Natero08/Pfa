using UnityEngine;

public class UnlockAbility : MonoBehaviour
{
    public enum AbilityType { InteractGenerators, Jump, Push, Carry }

    [SerializeField] private AbilityType abilityToUnlock;
    [SerializeField] private string unlockMessage = "Capacité débloquée !";
    [SerializeField] private bool destroyAfterUse = false;

    [Header("Swap de mesh")]
    [SerializeField] private GameObject meshADesactiver;
    [SerializeField] private GameObject meshAActiver;

    public void Interact()
    {
        switch (abilityToUnlock)
        {
            case AbilityType.InteractGenerators:
                PlayerAbilities.Instance.unlockedGenerators = true;
                PlayerAbilities.Instance.canInteractGenerators = true;
                break;

            case AbilityType.Jump:
                PlayerAbilities.Instance.unlockedJump = true;
                PlayerAbilities.Instance.canJump = true;
                break;

            case AbilityType.Push:
                PlayerAbilities.Instance.unlockedPush = true;
                PlayerAbilities.Instance.canPush = true;
                // Swap mesh bras
                if (meshADesactiver != null) meshADesactiver.SetActive(false);
                if (meshAActiver != null) meshAActiver.SetActive(true);
                break;

            case AbilityType.Carry:
                PlayerAbilities.Instance.unlockedCarry = true;
                PlayerAbilities.Instance.canCarry = true;
                // Swap mesh bras
                if (meshADesactiver != null) meshADesactiver.SetActive(false);
                if (meshAActiver != null) meshAActiver.SetActive(true);
                break;
        }

        HUDMessage.Instance.ShowMessage(unlockMessage);

        if (destroyAfterUse)
            Destroy(gameObject);
    }
}