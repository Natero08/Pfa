using JetBrains.Annotations;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        // Récupère le composant Animator attaché au mesh
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Exemple : si j'appuie sur une touche, j'active l'animation
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.W))
        {
            anim.SetBool("walking", true);
        }
        else
        {
            anim.SetBool("walking", false);
        }
    }
}