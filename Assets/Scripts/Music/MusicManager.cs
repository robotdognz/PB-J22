using UnityEngine;
using UnityEngine.SceneManagement;

namespace Alchemy.Music
{
    public enum Track
    {
        Title,
        Explore,
        Battle,
        Victory,
        GameOver
    }

    public enum DungeonType
    {
        Forest,
        MidnightDesert,
        Castle,
        Sewers,
        Maze
    }

    public class MusicManager : MonoBehaviour
    {
        private static bool Initialized;
        public static void Initialize()
        {
            if (!SceneManager.GetSceneByName("MusicManager").isLoaded)
            {
                SceneManager.LoadScene("MusicManager", LoadSceneMode.Additive);
                Instance = FindObjectOfType<MusicManager>();
            }

            Initialized = true;
        }

        public Track MusicTrack;
        public DungeonType DungeonType;

        public static void SetTrack(Track MusicTrack)
        {
            if (!Initialized)
            {
                Initialize();
            }

            SetTrack(MusicTrack, Instance.DungeonType);
        }

        public static void SetTrack(Track MusicTrack, DungeonType Type)
        {
            if (!Initialized)
            {
                Initialize();
            }

            Instance.MusicTrack = MusicTrack;
            Instance.DungeonType = Type;

            foreach(AudioSource Src in Instance.Sources)
            {
                Src.Play(); // Reset the AudioSources to play the music from the beginning
            }
        }

        public static MusicManager Instance;

        public AudioSource[] Sources
        {
            get
            {
                return GetComponentsInChildren<AudioSource>();
            }
        }

        public int TrackNumber
        {
            get
            {
                return (int)MusicTrack < 1 ? (int)MusicTrack : MusicTrack == Track.Explore ? (int)MusicTrack + (int)DungeonType : (int)MusicTrack + 4;
            }
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            for (int i = 0; i < Sources.Length; i++)
            {
                if (i == TrackNumber)
                {
                    Sources[i].volume += Time.unscaledDeltaTime * 4;
                }
                else
                {
                    Sources[i].volume -= Time.unscaledDeltaTime * 8;
                }
            }
        }
    }
}