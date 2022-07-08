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
    public RawImage Minimap;
    public RawImage Fullmap;
    [SerializeField] private Camera Cam;

    public Texture2D MinimapTex
    {
        get
        {
            int Width = 128;
            int Height = 128;

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
                        if (Tex.GetPixel(X, Y) != Color.red && Tex.GetPixel(X, Y) != Color.blue)
                        {
                            Final.SetPixel(X, Y, Color.grey);
                        }
                        else if (Tex.GetPixel(X, Y) == Color.red) // current room
                        {
                            Final.SetPixel(X, Y, Color.cyan);
                        }
                        else // doors
                        {
                            Final.SetPixel(X, Y, Color.white);
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
                Cam.transform.position = Vector2.Lerp(FromInt(DM.topLeft), FromInt(DM.bottomRight), 0.5f);
                Cam.transform.position -= Vector3.forward * 10;
                Cam.orthographicSize = Vector2Int.Distance(DM.topLeft, DM.bottomRight);
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

        if (Input.GetButtonDown("Minimap"))
            DrawMap(MinimapDrawType.Minimap);

        if (Input.GetButton("Minimap"))
        {
            Minimap.enabled = true;
            Time.timeScale = 0;
        }

        if (Input.GetButtonUp("Minimap"))
        {
            Minimap.enabled = false;
            Time.timeScale = 1;
        }
    }
}
