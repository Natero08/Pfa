using UnityEngine;
using UnityEngine.Playables;

public class BarrierInteraction : MonoBehaviour
{
    [Header("Cinématique")]
    [SerializeField] private PlayableDirector timeline; // sera assigné quand tu auras ta cinématique Blender

    [Header("Téléportation")]
    [SerializeField] private Transform teleportDestination; // point de destination dans la ville basse

    [Header("Joueur")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private CharacterController characterController;

    private bool hasBeenUsed = false;

    private void Start()
    {
        // Récupère le joueur automatiquement si pas assigné
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                characterController = player.GetComponent<CharacterController>();
            }
        }
    }

    public void Interact()
    {
        if (hasBeenUsed) return;
        hasBeenUsed = true;

        if (timeline != null)
        {
            // Lance la cinématique puis téléporte
            timeline.Play();
            timeline.stopped += OnCinematicFinished;
        }
        else
        {
            // Pas encore de cinématique, téléporte directement
            TeleportPlayer();
        }
    }

    private void OnCinematicFinished(PlayableDirector director)
    {
        timeline.stopped -= OnCinematicFinished;
        TeleportPlayer();
    }

    private void TeleportPlayer()
    {
        if (playerTransform == null || teleportDestination == null) return;

        // Désactive le CharacterController le temps de la téléportation
        if (characterController != null)
            characterController.enabled = false;

        playerTransform.position = teleportDestination.position;
        playerTransform.rotation = teleportDestination.rotation;

        if (characterController != null)
            characterController.enabled = true;

        Debug.Log("Joueur téléporté à " + teleportDestination.position);
    }
}