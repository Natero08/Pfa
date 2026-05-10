using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    public static PlayerAbilities Instance;

    [Header("Déblocages permanents")]
    public bool unlockedJump = false;
    public bool unlockedCarry = false;
    public bool unlockedPush = false;
    public bool unlockedGenerators = false;

    [Header("Capacités actives")]
    public bool canJump = true;
    public bool canCarry = true;
    public bool canPush = true;
    public bool canInteractGenerators = true;

    [Header("Messages de restriction")]
    public string jumpLockedMessage = "Je ne peux pas sauter ici !";
    public string carryLockedMessage = "Je ne peux pas porter ça...";
    public string pushLockedMessage = "C'est trop lourd...";
    public string generatorLockedMessage = "Je ne comprends pas comment ça marche...";

    void Awake()
    {
        Instance = this;
    }
}