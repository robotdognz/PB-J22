using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Experimental.Rendering;

public class CandyTrailManager : MonoBehaviour
{
    [SerializeField] Texture2D[] wrapperTextures;
    [SerializeField] Material wrapperMaterial;

    public Texture2D atlasTexture;

    public static Rect[] uvs;

    // Start is called before the first frame update
    void Awake()
    {
        atlasTexture = new Texture2D(512, 512); //, TextureFormat.ARGB32, false);

        Texture2D[] fixedTextures = new Texture2D[wrapperTextures.Length];
        for (int i = 0; i < wrapperTextures.Length; i++)
        {
            fixedTextures[i] = duplicateTexture(wrapperTextures[i]);
        }


        // Debug.Log("Sweet jesus: " + atlasTexture.isReadable);
        if (fixedTextures != null && wrapperMaterial != null)
        {
            uvs = atlasTexture.PackTextures(fixedTextures, 0, 512, false);
            wrapperMaterial.SetTexture("_MainTex", atlasTexture);
            Debug.Log("Built texture atlas");
        }

        // foreach(Rect rect in uvs)
        // {
        //     Debug.Log("UV- " + rect + ", All- yMin:" + rect.yMin + " yMax:" + rect.yMax + "- xMin:" + rect.xMin + " xMax:" + rect.xMax);
            
        // }
    }

    Texture2D duplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }

}
