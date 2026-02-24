using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ScribbleSurface : MonoBehaviour
{
    public LayerMask InteractionLayer;
    public Color BackgroundColor = new(0, 0, 0, 0);

    public bool ClearTextureOnStart = true;
    Sprite surfaceSprite;
    Texture2D surfaceTexture;

    Vector2 lastDragPosition;
    Color[] baseColors;
    Color clearColor;
    Color32[] currentPixelColors;
    bool wasMouseDownLastFrame = false;
    bool skipDrawingThisDrag = false;
    [SerializeField] Camera tableCamera;


    void Start()
    {
        surfaceSprite = GetComponent<SpriteRenderer>().sprite;
        surfaceTexture = surfaceSprite.texture;

        baseColors = new Color[(int)surfaceSprite.rect.width * (int)surfaceSprite.rect.height];
        for (int i = 0; i < baseColors.Length; i++)
            baseColors[i] = BackgroundColor;

        if (ClearTextureOnStart)
            ClearSurface();
    }

    void Update()
    {
        bool isMouseDown = Input.GetMouseButton(0);
        if (isMouseDown && !skipDrawingThisDrag)
        {
            Ray mouseRay = tableCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, Mathf.Infinity, InteractionLayer.value))
            {
                DrawLine(hitInfo.point);
            }
            else
            {
                lastDragPosition = Vector2.zero;
                if (!wasMouseDownLastFrame)
                {
                    skipDrawingThisDrag = true;
                }
            }
        }
        else if (!isMouseDown)
        {
            lastDragPosition = Vector2.zero;
            skipDrawingThisDrag = false;
        }

        wasMouseDownLastFrame = isMouseDown;

#if UNITY_EDITOR
        //Debug
        if (Input.GetKeyDown(KeyCode.X))
        {
            ClearSurface();
        }
#endif
    }

    void DrawLine(Vector3 worldPosition)
    {
        Vector3 pixelCoords = TransformToPixelCoordinates(worldPosition);
        currentPixelColors = surfaceTexture.GetPixels32();
        if (lastDragPosition == Vector2.zero)
            FillPixels(pixelCoords, 3, Color.black);
        else
            DrawLineSegment(lastDragPosition, pixelCoords, 3, Color.green);

        lastDragPosition = pixelCoords;
    }

    void DrawLineSegment(Vector2 startPoint, Vector2 endPoint, int thickness, Color drawColor)
    {
        float segmentLength = Vector2.Distance(startPoint, endPoint);

        Vector2 interpolatedPosition = startPoint;
        float interpolationSteps = 1 / segmentLength;

        for (float t = 0; t <= 1; t += interpolationSteps)
        {
            interpolatedPosition = Vector2.Lerp(startPoint, endPoint, t);
            FillPixels(interpolatedPosition, thickness, drawColor);
        }
    }

    public void PreparePixels(Vector2 center, int radius, Color color)
    {
        int centerX = (int)center.x;
        int centerY = (int)center.y;

        for (int x = centerX - radius; x <= centerX + radius; x++)
        {
            if (x >= (int)surfaceSprite.rect.width || x < 0)
                continue;

            for (int y = centerY - radius; y <= centerY + radius; y++)
            {
                MarkPixelForUpdate(x, y, color);
            }
        }
    }

    public void MarkPixelForUpdate(int x, int y, Color color)
    {
        int pixelIndex = y * (int)surfaceSprite.rect.width + x;

        if (pixelIndex > currentPixelColors.Length || pixelIndex < 0)
            return;

        currentPixelColors[pixelIndex] = color;
    }

    public void ApplyPixelUpdates()
    {
        surfaceTexture.SetPixels32(currentPixelColors);
        surfaceTexture.Apply();
    }

    void FillPixels(Vector2 center, int radius, Color color)
    {
        int centerX = (int)center.x;
        int centerY = (int)center.y;

        for (int x = centerX - radius; x <= centerX + radius; x++)
        {
            for (int y = centerY - radius; y <= centerY + radius; y++)
            {
                surfaceTexture.SetPixel(x, y, color);
            }
        }

        surfaceTexture.Apply();
    }

    public Vector3 TransformToPixelCoordinates(Vector3 worldPosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);

        float textureWidth = surfaceSprite.rect.width;
        float textureHeight = surfaceSprite.rect.height;
        float scaleFactor = textureWidth / surfaceSprite.bounds.size.x * transform.localScale.x;

        float adjustedX = localPosition.x * scaleFactor + textureWidth / 2;
        float adjustedY = localPosition.y * scaleFactor + textureHeight / 2;

        Vector2 pixelCoords = new(Mathf.RoundToInt(adjustedX), Mathf.RoundToInt(adjustedY));

        return pixelCoords;
    }

    public void ClearSurface()
    {
        surfaceTexture.SetPixels(baseColors);
        surfaceTexture.Apply();
    }
}