using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSeeker : MonoBehaviour
{
    public float InteractRadius;
    public GameObject InteractPrompt;
    public AudioSource InteractHelper;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, InteractRadius);
    }

    private bool PlayedInteractSound = false;

    private void Update()
    {
        if (PlayerMovement.Instance.isDisabled)
        {
            InteractPrompt.SetActive(false);
            return;
        }

        InteractiveObject Interaction = null;

        foreach (Collider2D Col in Physics2D.OverlapCircleAll(transform.position, InteractRadius))
        {
            if (Col.GetComponent<InteractiveObject>() && Col.GetComponent<InteractiveObject>().ReadyToInteract)
            {
                if (Interaction == null || Vector3.Distance(transform.position, Col.transform.position) < Vector3.Distance(transform.position, Col.transform.position))
                {
                    Interaction = Col.GetComponent<InteractiveObject>();
                }
            }
        }

        InteractPrompt.SetActive(Interaction);

        if (Interaction)
        {
            if (!PlayedInteractSound)
            {
                InteractHelper.Play();
                PlayedInteractSound = true;
            }

            if (Input.GetButtonDown("Interact"))
            {
                if (!PauseMenu.MenuOpen)
                {
                    Interaction.Interact();
                }
            }
        }
        else
            PlayedInteractSound = false;
    }    
}
