using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] [Range(0.1f, 1f)] float refreshTimer = 0.1f;
    float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= refreshTimer)
        {
            text.text = Time.time.ToString("F2");
            timer = 0f;
        }
    }
}