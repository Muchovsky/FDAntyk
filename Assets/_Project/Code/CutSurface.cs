using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSurface : MonoBehaviour
{
    [SerializeField] Sprite surfaceSprite;
    [SerializeField] Texture2D surfaceTexture;

    void Start()
    {
        surfaceTexture = surfaceSprite.texture;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Cut();
        }
    }

    void Cut()
    {
        var mask = BuildMask(surfaceTexture);



     

        Debug.Log("asd");
    }

    bool[,] BuildMask(Texture2D tex, float alphaThreshold = 0.1f)
    {
        int w = tex.width / 10;
        int h = tex.height / 10;

        bool[,] mask = new bool[w, h];
        var pixels = tex.GetPixels32();

        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++)
        {
            var p = pixels[y * w + x];
            Debug.Log(p.a > alphaThreshold);

            mask[x, y] = p.a > alphaThreshold;
        }

        return mask;
    }

   

}