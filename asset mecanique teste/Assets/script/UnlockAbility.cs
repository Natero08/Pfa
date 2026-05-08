using UnityEngine;

public class UnlockAbility : MonoBehaviour
{
    public enum AbilityType { InteractGenerators, Jump, Push, Carry }

    [SerializeField] private AbilityType abilityToUnlock;
    [SerializeField] private string unlockMessage = "Capacité débloquée !";
    [SerializeField] private bool destroyAfterUse = false;

    public void Interact()
    {
        switch (abilityToUnlock)
        {
            case AbilityType.InteractGenerators:
                PlayerAbilities.Instance.canInteractGenerators = true; break;
            case AbilityType.Jump:
                PlayerAbilities.Instance.canJump = true; break;
            case AbilityType.Push:
                PlayerAbilities.Instance.canPush = true; break;
            case AbilityType.Carry:
                PlayerAbilities.Instance.canCarry = true; break;
        }

        HUDMessage.Instance.ShowMessage(unlockMessage);

        if (destroyAfterUse)
            Destroy(gameObject);
    }
}