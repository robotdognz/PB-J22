using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsSystem : MonoBehaviour
{
    [Range(-3, 3)] public float PitchFactor = 1;
    public bool Mute;
    [SerializeField] private AudioSource Source;
    [SerializeField] private AudioClip[] Footsteps;
    [SerializeField] private float wrapperProbability = 0.75f;

    public void PlayFootstepSounds()
    {
        if (!Mute)
        {
            Source.pitch = PitchFactor * Random.Range(0.9f, 1.1f);
            Source.PlayOneShot(Footsteps[Random.Range(0, Footsteps.Length)]);
        }

        // drop candy
        if (wrapperProbability > Random.value)
        {
            DungeonManager.DropWrappers();
        }
    }
}
