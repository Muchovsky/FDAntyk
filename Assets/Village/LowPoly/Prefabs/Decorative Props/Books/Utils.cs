using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    private List<Texture2D> textures = new List<Texture2D>();

    public int textureWidth = 1024;
    public int textureHeight = 1024;
    
    void Start()
    {
        StartCoroutine(AddTexturesToMemory());
    }
    
    IEnumerator AddTexturesToMemory()
    {
        while (true)
        {
            Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, false);
            
            Color[] colors = new Color[textureWidth * textureHeight];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color(Random.value, Random.value, Random.value);
            }
            texture.SetPixels(colors);
            texture.Apply();
            
            textures.Add(texture);
            
            yield return new WaitForSeconds(3);
        }
    }
}
