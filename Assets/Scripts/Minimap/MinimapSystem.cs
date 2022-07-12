using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapSystem : MonoBehaviour
{
    private int mapResolution = 512; // 128
    public RawImage Minimap;
    public RawImage Fullmap;
    [SerializeField] private Camera FullMapCam;
    [SerializeField] private Camera MiniMapCam;

    private void Awake()
    {
        Room.ResetPlayerEnter();
        Room.PlayerEnter += RefreshMinimap;
    }

    private void RefreshMinimap()
    {
        StartCoroutine(AwaitMapRefresh());

    }

    private IEnumerator AwaitMapRefresh()
    {
        yield return new WaitForSeconds(0.5f);
        MiniMapCam.transform.position = DungeonManager.currentRoom - Vector3.forward * 10;
    }

    public Texture2D FullMapTex
    {
        get
        {
            int Width = mapResolution;
            int Height = mapResolution;

            Rect R = new Rect(0, 0, Width, Height);
            RenderTexture RT = new RenderTexture(Width, Height, 24);
            Texture2D Tex = new Texture2D(Width, Height, TextureFormat.RGBA32, false);

            FullMapCam.targetTexture = RT;
            FullMapCam.Render();

            RenderTexture.active = RT;
            Tex.ReadPixels(R, 0, 0);

            FullMapCam.targetTexture = null;
            RenderTexture.active = null;

            Destroy(RT);

            Texture2D Final = new Texture2D(Width, Height);

            for (int X = 0; X < Width; X++)
            {
                for (int Y = 0; Y < Height; Y++)
                {
                    if (Tex.GetPixel(X, Y) == Color.black || Tex.GetPixel(X, Y) == Color.clear)
                    {
                        Final.SetPixel(X, Y, Color.black);
                    }
                    else
                    {
                        Final.SetPixel(X, Y, Tex.GetPixel(X, Y));

                        // if (Tex.GetPixel(X, Y) == Color.red) // current room
                        // {
                        //     Final.SetPixel(X, Y, Color.cyan);
                        // }
                        // else if (Tex.GetPixel(X, Y) == Color.blue) // doors
                        // {
                        //     Final.SetPixel(X, Y, Color.white);
                        // }
                        // else if (Tex.GetPixel(X, Y) == Color.green) // boss
                        // {
                        //     Final.SetPixel(X, Y, Color.red);
                        // }
                        // else // visited rooms (uses an off green, to save clean colors for other things)
                        // {
                        //     Final.SetPixel(X, Y, Color.grey);
                        // }
                    }
                }
            }

            Final.Apply();
            Final.filterMode = FilterMode.Point;

            return Final;
        }
    }

    private Vector2 FromInt(Vector2Int Input)
    {
        return new Vector2(Input.x, Input.y);
    }

    public void RefreshFullMap()
    {
        // set map cam position
        FullMapCam.transform.position = Vector2.Lerp(FromInt(DungeonManager.topLeft), FromInt(DungeonManager.bottomRight), 0.5f);
        FullMapCam.transform.position -= Vector3.forward * 10;

        // set map cam size
        // is the map tall, or fat?
        float mapWidth = Mathf.Abs(DungeonManager.topLeft.x - DungeonManager.bottomRight.x);
        float mapHeight = Mathf.Abs(DungeonManager.topLeft.y - DungeonManager.bottomRight.y);
        if (mapHeight > mapWidth)
        {
            FullMapCam.orthographicSize = mapHeight * 0.55f;
        }
        else
        {
            FullMapCam.orthographicSize = mapWidth * 0.55f;
        }

        Fullmap.texture = FullMapTex;
    }

    private void Update()
    {
        if (PauseMenu.MenuOpen || UnityEngine.SceneManagement.SceneManager.GetSceneByName("Combat").isLoaded || DungeonManager.darkScreen)
        {
            Minimap.enabled = false;
            return;
        }

        Minimap.enabled = true;
    }
}
