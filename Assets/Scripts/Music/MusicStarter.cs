using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Music
{
    public class MusicStarter : MonoBehaviour
    {
        public Track StartingTrack;
        public DungeonType DungeonType;

        private IEnumerator Start()
        {
            MusicManager.Initialize();

            while (MusicManager.Instance == null)
                yield return null;

            MusicManager.SetTrack(StartingTrack, DungeonType);
        }
    }
}