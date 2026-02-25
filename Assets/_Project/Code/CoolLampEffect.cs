using System.Collections.Generic;
using UnityEngine;

public class CoolLampEffect : MonoBehaviour
{
    [SerializeField] Color color1 = Color.red;
    [SerializeField] Color color2 = Color.yellow;
    [SerializeField] List<Light> Lights;
    [SerializeField] int materialIndex = 1;
    [SerializeField] float speed = 1f;

    Renderer renderer;
    Material materialInstance;
    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    void Start()
    {
        renderer = GetComponent<Renderer>();

        if (renderer.materials.Length <= materialIndex)
        {
            Debug.LogError("Material index out of range.");
            enabled = false;
            return;
        }
        materialInstance = renderer.materials[materialIndex];
    }

    void Update()
    {
        float lerpFactor = Mathf.PingPong(Time.time * speed, 1.0f);
        Color currentColor = Color.Lerp(color1, color2, lerpFactor);
        materialInstance.SetColor(BaseColorId, currentColor);
        for (int i = 0; i < Lights.Count; i++)
        {
            if (Lights[i] != null)
                Lights[i].color = currentColor;
        }
    }
}