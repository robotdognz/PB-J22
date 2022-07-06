using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsSystem : MonoBehaviour
{
    [SerializeField] private AudioSource Source;
    [SerializeField] private AudioClip[] Footsteps;

    public void PlayFootstepSounds()
    {
        Source.PlayOneShot(Footsteps[Random.Range(0, Footsteps.Length)]);
    }
}
