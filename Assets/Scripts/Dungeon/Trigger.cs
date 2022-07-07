using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Alchemy.Dungeon
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Trigger : MonoBehaviour
    {
        public UnityEvent PlayerEnter;
        public UnityEvent PlayerExit;

        protected virtual void OnPlayerEnter()
        {
            PlayerEnter.Invoke();
        }

        protected virtual void OnPlayerStay()
        {
            
        }

        public void ShowDialogueText(string Message)
        {
            DialogueManager.ShowMessage(Message);
        }

        protected virtual void OnPlayerExit()
        {
            PlayerExit.Invoke();
        }

        protected void OnTriggerEnter2D(Collider2D Other)
        {
            if (Other.CompareTag("Player"))
            {
                OnPlayerEnter();
            }
        }

        protected void OnTriggerStay2D(Collider2D Other)
        {
            if (Other.CompareTag("Player"))
            {
                OnPlayerStay();
            }
        }

        protected void OnTriggerExit2D(Collider2D Other)
        {
            if (Other.CompareTag("Player"))
            {
                OnPlayerExit();
            }
        }
    }
}