using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpriteSwapWindow : EditorWindow
{
    [MenuItem("Alchemical Dreams/Palette Manager")]
    public static void OpenWindow()
    {
        GetWindow<SpriteSwapWindow>("Palette Manager");
    }

    private static void Setup()
    {
        Tiles = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/Environment/MainTiles.png");
        Doors = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/Environment/DoorsMain.png");

        TForest = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Sprites/Environment/ForestTiles.png");
        DForest = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Sprites/Environment/DoorsForest.png");

        TMidnightDesert = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Sprites/Environment/MidnightDesertTiles.png");
        DMidnightDesert = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Sprites/Environment/DoorsMidnightDesert.png");

        TCastle = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Sprites/Environment/CastleTiles.png");
        DCastle = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Sprites/Environment/DoorsCastle.png");

        TSewers = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Sprites/Environment/SewerTiles.png");
        DSewers = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Sprites/Environment/DoorsSewer.png");
    }

    static Sprite Tiles;
    static Sprite Doors;

    static Texture2D TForest;
    static Texture2D TMidnightDesert;
    static Texture2D TCastle;
    static Texture2D TSewers;

    static Texture2D DForest;
    static Texture2D DMidnightDesert;
    static Texture2D DCastle;
    static Texture2D DSewers;

    private void OnGUI()
    {
        Setup();

        GUILayout.Label($"Palette Manager");
        EditorGUILayout.Separator();
        if (GUILayout.Button("Load Forest Tileset"))
        {
            Tiles.texture.LoadImage(TForest.EncodeToPNG());
            Doors.texture.LoadImage(DForest.EncodeToPNG());
        }
        if (GUILayout.Button("Load Midnight Desert Tileset"))
        {
            Tiles.texture.LoadImage(TMidnightDesert.EncodeToPNG());
            Doors.texture.LoadImage(DMidnightDesert.EncodeToPNG());
        }
        if (GUILayout.Button("Load Castle Tileset")) 
        {
            Tiles.texture.LoadImage(TCastle.EncodeToPNG());
            Doors.texture.LoadImage(DCastle.EncodeToPNG());
        }
        if (GUILayout.Button("Load Sewers Tileset"))
        {
            Tiles.texture.LoadImage(TSewers.EncodeToPNG());
            Doors.texture.LoadImage(DSewers.EncodeToPNG());
        }
    }
}
