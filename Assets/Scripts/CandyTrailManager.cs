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
    public static float dropProbability = 0.85f;

    // Start is called before the first frame update
    void Awake()
    {
        atlasTexture = new Texture2D(512, 512);

        Texture2D[] fixedTextures = new Texture2D[wrapperTextures.Length];
        for (int i = 0; i < wrapperTextures.Length; i++)
        {
            fixedTextures[i] = duplicateTexture(wrapperTextures[i]);
        }

        if (fixedTextures != null && wrapperMaterial != null)
        {
            uvs = atlasTexture.PackTextures(fixedTextures, 0, 512, false);
            wrapperMaterial.SetTexture("_MainTex", atlasTexture);
            Debug.Log("Built candy wrapper texture atlas");
        }
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
