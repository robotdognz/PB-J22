using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MinimapDrawType
{
    Minimap,
    FullMap
}

public class MinimapSystem : MonoBehaviour
{
    private int mapResolution = 256; // 128
    public RawImage Minimap;
    public RawImage Fullmap;
    [SerializeField] private Camera Cam;

    private void Awake()
    {
        Room.PlayerEnter += RefreshMinimap;
    }

    private void RefreshMinimap()
    {
        DrawMap(MinimapDrawType.Minimap);
    }

    public Texture2D MinimapTex
    {
        get
        {
            int Width = mapResolution;
            int Height = mapResolution;

            Rect R = new Rect(0, 0, Width, Height);
            RenderTexture RT = new RenderTexture(Width, Height, 24);
            Texture2D Tex = new Texture2D(Width, Height, TextureFormat.RGBA32, false);

            Cam.targetTexture = RT;
            Cam.Render();

            RenderTexture.active = RT;
            Tex.ReadPixels(R, 0, 0);

            Cam.targetTexture = null;
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
                        if (Tex.GetPixel(X, Y) == Color.red) // current room
                        {
                            Final.SetPixel(X, Y, Color.cyan);
                        }
                        else if (Tex.GetPixel(X, Y) == Color.blue) // doors
                        {
                            Final.SetPixel(X, Y, Color.white);
                        }
                        else if (Tex.GetPixel(X, Y) == Color.green) // boss
                        {
                            Final.SetPixel(X, Y, Color.red);
                        }
                        else // visited rooms (uses an off green, to save clean colors for other things)
                        {
                            Final.SetPixel(X, Y, Color.grey);
                        }
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
        DrawMap(MinimapDrawType.FullMap);
    }

    public void DrawMap(MinimapDrawType DrawMode)
    {
        DungeonManager DM = FindObjectOfType<DungeonManager>();

        switch (DrawMode)
        {
            case MinimapDrawType.Minimap:
                Cam.transform.position = Camera.main.transform.position;
                Cam.orthographicSize = Camera.main.orthographicSize * 10;
                Minimap.texture = MinimapTex;
                break;
            case MinimapDrawType.FullMap:
                Cam.transform.position = Vector2.Lerp(FromInt(DungeonManager.topLeft), FromInt(DungeonManager.bottomRight), 0.5f);
                Cam.transform.position -= Vector3.forward * 10;

                // is the map tall, or fat?
                float mapWidth = Mathf.Abs(DungeonManager.topLeft.x - DungeonManager.bottomRight.x);
                float mapHeight = Mathf.Abs(DungeonManager.topLeft.y - DungeonManager.bottomRight.y);
                if (mapHeight > mapWidth)
                {
                    Cam.orthographicSize = mapHeight * 0.55f;
                }
                else
                {
                    Cam.orthographicSize = mapWidth * 0.55f;
                }
                
                Fullmap.texture = MinimapTex;
                break;
        }
    }

    private void Update()
    {
        if (PauseMenu.MenuOpen || UnityEngine.SceneManagement.SceneManager.GetSceneByName("Combat").isLoaded)
        {
            Minimap.enabled = false;
            return;
        }

        if (InputManager.GetButtonDown("Minimap"))
            DrawMap(MinimapDrawType.Minimap);

        if (InputManager.GetButton("Minimap"))
        {
            Minimap.enabled = true;
            Time.timeScale = 0;
        }

        if (InputManager.GetButtonUp("Minimap"))
        {
            Minimap.enabled = false;
            Time.timeScale = 1;
        }
    }
}
