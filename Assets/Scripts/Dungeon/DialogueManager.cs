using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager Instance;

    public GameObject DialogueScreen;
    public float CharacterDelay = 0.05f;

    private bool AwaitSubmit;

    private void Awake()
    {
        Instance = this;
    }

    public static void ShowMessage(string Message)
    {
        Instance.ShowDialogue(Message);
    }

    public void ShowDialogue(string Message)
    {
        DialogueScreen.SetActive(true);
        StartCoroutine(Show(Message));
    }

    public static UnityAction OnDialogueClose = new UnityAction(() => { });

    private void Update()
    {
        if (AwaitSubmit)
        {
            if (Input.GetButtonDown("Back"))
            {
                OnDialogueClose.Invoke();
                OnDialogueClose = new UnityAction(() => { });
                DialogueScreen.SetActive(false);
                AwaitSubmit = false;
            }
        }
    }

    public AudioSource TickSound;

    private IEnumerator Show(string Message)
    {
        Text T = DialogueScreen.GetComponentInChildren<Text>();

        T.text = "";

        for (int i = 0; i < Message.Length; i++)
        {
            T.text += Message[i];
            if (Message[i] != ' ')
            {
                if (TickSound)
                {
                    TickSound.pitch = Random.Range(0.95f, 1.05f);
                    TickSound.Play();
                }
                yield return new WaitForSecondsRealtime(CharacterDelay);
            }
        }

        yield return new WaitForSecondsRealtime(0.5f);
        T.text += "\n\nPress [TAB] or (B) to close the dialogue...";

        AwaitSubmit = true;
    }
}
