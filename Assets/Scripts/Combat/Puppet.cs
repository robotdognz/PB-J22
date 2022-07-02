using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Combat 
{
    public class Puppet : Stats.ActorStats
    {
        public Vector3 TrackedCoords;

        protected override void Awake()
        {
            // No need to load skills for a puppet, as they use an existing actors skills
        }

        private void Update()
        {
            transform.position = TrackedCoords; // Track their coords xD
        }
    }
}