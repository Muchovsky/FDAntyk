using UnityEngine;

public class ScribbleSurface : MonoBehaviour
{
    [SerializeField] Camera tableCamera;
    [SerializeField] LayerMask InteractionLayer;
    [SerializeField] Color BackgroundColor = new(0, 0, 0, 0);
    [SerializeField] bool ClearTextureOnStart = true;
    Sprite surfaceSprite;
    Texture2D surfaceTexture;

    Vector2 lastDragPosition;
    Color[] baseColors;
    Color32[] currentPixelColors;
    bool wasMouseDownLastFrame = false;
    bool skipDrawingThisDrag = false;
    bool isInteractable = false;

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
        if (!isInteractable)
            return;

        bool isMouseDown = Input.GetMouseButton(0);
        if (isMouseDown && !skipDrawingThisDrag)
        {
            Ray mouseRay = tableCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, Mathf.Infinity, InteractionLayer.value))
            {
                DrawLine(hitInfo.point);
                surfaceTexture.SetPixels32(currentPixelColors);
                surfaceTexture.Apply(false);
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

    public void SetIsInteractable(bool status)
    {
        isInteractable = status;
    }

    void DrawLine(Vector3 worldPosition)
    {
        Vector3 pixelCoords = TransformToPixelCoordinates(worldPosition);
        currentPixelColors = surfaceTexture.GetPixels32();
        if (lastDragPosition == Vector2.zero)
            FillPixels(pixelCoords, 3, Color.black);
        else
            DrawLineSegment(lastDragPosition, pixelCoords, 5, Color.black);

        lastDragPosition = pixelCoords;
    }

    void DrawLineSegment(Vector2 startPoint, Vector2 endPoint, int thickness, Color drawColor)
    {
        float distance = Vector2.Distance(startPoint, endPoint);
        int steps = Mathf.CeilToInt(distance);

        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 point = Vector2.Lerp(startPoint, endPoint, t);
            FillPixels(point, thickness, drawColor);
        }
    }

    void PreparePixels(Vector2 center, int radius, Color color)
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

    void MarkPixelForUpdate(int x, int y, Color color)
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

    void FillPixels(Vector2 center, int radius, Color32 color)
    {
        int width = (int)surfaceSprite.rect.width;
        int height = (int)surfaceSprite.rect.height;

        int centerX = (int)center.x;
        int centerY = (int)center.y;

        for (int x = centerX - radius; x <= centerX + radius; x++)
        {
            if (x < 0 || x >= width) continue;

            for (int y = centerY - radius; y <= centerY + radius; y++)
            {
                if (y < 0 || y >= height) continue;

                int index = y * width + x;
                currentPixelColors[index] = color;
            }
        }
    }

    Vector3 TransformToPixelCoordinates(Vector3 worldPosition)
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

    void ClearSurface()
    {
        surfaceTexture.SetPixels(baseColors);
        surfaceTexture.Apply();
    }
}