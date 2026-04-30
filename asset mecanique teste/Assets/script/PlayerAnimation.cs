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
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.Z))

        {
            anim.SetBool("saut", true);
        }
        else
        {
            anim.SetBool("saut", false);
        }
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.W))
        {
            anim.SetBool("walking", true);
        }
        else
        {
            anim.SetBool("walking", false);
        }
        if (Input.GetKey(KeyCode.S))
        {
            anim.SetBool("walking_back", true);
        }
        else
        {
            anim.SetBool("walking_back", false);
        }
        if (Input.GetKey(KeyCode.E))
        {
            anim.SetBool("push", true);
        }
        else
        {
            anim.SetBool("push", false);
        }
    }
}