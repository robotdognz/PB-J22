using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Music
{
    public class MusicStarter : MonoBehaviour
    {
        public static MusicStarter Instance;
        public Track StartingTrack;
        public DungeonType DungeonType;

        public void RefreshGraphics()
        {
            GetComponent<Dungeon.SpriteSwapper>().Refresh();

            foreach (Door D in FindObjectsOfType<Door>())
            {
                D.Refresh();
            }
        }

        private IEnumerator Start()
        {
            Instance = this;
            MusicManager.Initialize();

            while (MusicManager.Instance == null)
                yield return null;

            MusicManager.SetTrack(StartingTrack, DungeonType);
        }
    }
}