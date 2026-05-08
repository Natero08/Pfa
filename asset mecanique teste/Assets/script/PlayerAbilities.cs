using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    public static PlayerAbilities Instance;

    [Header("Capacités")]
    public bool canInteractGenerators = false;
    public bool canJump = false;
    public bool canPush = false;
    public bool canCarry = false;

    [Header("Messages de restriction")]
    public string generatorLockedMessage = "Trouvez la télécommande d'abord !";
    public string jumpLockedMessage = "Vous ne pouvez pas encore sauter !";
    public string pushLockedMessage = "Vous ne pouvez pas encore pousser !";
    public string carryLockedMessage = "Vous ne pouvez pas encore porter d'objets !";

    void Awake()
    {
        Instance = this;
    }
}