using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveObject : MonoBehaviour
{
    public bool ReadyToInteract = true;
    public UnityEvent OnInteract;

    public void ShowDialogueMessage(string Text)
    {
        DialogueManager.ShowMessage(Text);
    }

    public virtual void Interact()
    {
        OnInteract.Invoke();
    }
}
