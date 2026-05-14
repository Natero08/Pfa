using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        bool avancer = Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.W);
        bool reculer = Input.GetKey(KeyCode.S);
        bool interagir = Input.GetKey(KeyCode.E);

        // Saut
        if (Input.GetKey(KeyCode.Space))
            anim.SetBool("saut", true);
        else
            anim.SetBool("saut", false);

        // Marche avant
        anim.SetBool("walking", avancer && !interagir);

        // Marche arrière
        anim.SetBool("walking_back", reculer && !interagir);

        // Push : E + avancer
        if (interagir && avancer)
        {
            anim.SetBool("push", true);
            anim.SetBool("tirer", false);
        }
        // Tirer : E + reculer
        else if (interagir && reculer)
        {
            anim.SetBool("tirer", true);
            anim.SetBool("push", false);
        }
        // Interaction simple : E seul (ouverture de porte etc)
        else if (Input.GetKeyDown(KeyCode.E) && !avancer && !reculer)
        {
            anim.SetTrigger("interaction");
            anim.SetBool("push", false);
            anim.SetBool("tirer", false);
        }
        else
        {
            anim.SetBool("push", false);
            anim.SetBool("tirer", false);
        }
    }
}