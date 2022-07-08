using UnityEngine;
using Alchemy.Music;

namespace Alchemy.Dungeon 
{
    [RequireComponent(typeof(MusicStarter))]
    public class SpriteSwapper : MonoBehaviour
    {
        private MusicStarter Mus;

        public bool RefreshOnAwake;
        public Sprite TargetSprite;
        [Space]
        public Texture2D Forest;
        public Texture2D MidnightDesert;
        public Texture2D Castle;
        public Texture2D Sewers;
        public Texture2D Maze;

        private void Awake()
        {
            if (RefreshOnAwake)
                Refresh();
        }

        public void Refresh()
        {
            Mus = GetComponent<MusicStarter>();

            switch (Mus.DungeonType)
            {
                case DungeonType.Forest:
                    TargetSprite.texture.LoadImage(Forest.EncodeToPNG());
                    TargetSprite.texture.Apply();
                    break;
                case DungeonType.MidnightDesert:
                    TargetSprite.texture.LoadImage(MidnightDesert.EncodeToPNG());
                    TargetSprite.texture.Apply();
                    break;
                case DungeonType.Castle:
                    TargetSprite.texture.LoadImage(Castle.EncodeToPNG());
                    TargetSprite.texture.Apply();
                    break;
                case DungeonType.Sewers:
                    TargetSprite.texture.LoadImage(Sewers.EncodeToPNG());
                    TargetSprite.texture.Apply();
                    break;
                case DungeonType.Maze:
                    TargetSprite.texture.LoadImage(Maze.EncodeToPNG());
                    TargetSprite.texture.Apply();
                    break;
            }
        }
    }
}