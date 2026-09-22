using UnityEngine;
using UnityEngine.UI;

public class S06_SoftwareRasterizer_Finish : MonoBehaviour
{
    [SerializeField] private int canvasWidth = 256;
    [SerializeField] private int canvasHeight = 256;

    [SerializeField] private Vector2 vertexA = new Vector2(128, 230);
    [SerializeField] private Vector2 vertexB = new Vector2(30, 40);
    [SerializeField] private Vector2 vertexC = new Vector2(220, 40);

    [SerializeField] private Color fillColor = new Color(0.1f, 0.8f, 0.7f, 1f);
    [SerializeField] private Color backgroundColor = new Color(0.05f, 0.05f, 0.05f, 1f);

    private Texture2D canvasTexture;
    private RawImage targetImage;

    void Start()
    {
        targetImage = GetComponent<RawImage>();
        canvasTexture = new Texture2D(canvasWidth, canvasHeight);
        canvasTexture.filterMode = FilterMode.Point;

        DrawTriangle(vertexA, vertexB, vertexC, fillColor);
        canvasTexture.Apply();

        if (targetImage != null)
        {
            targetImage.texture = canvasTexture;
        }
    }

    private void FillBackground(Color color)
    {
        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                canvasTexture.SetPixel(x, y, color);
            }
        }
    }

    private void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
    {
        FillBackground(backgroundColor);

        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                Vector2 pixelCenter = new Vector2(x + 0.5f, y + 0.5f);
                if (IsInsideTriangle(pixelCenter, a, b, c))
                {
                    canvasTexture.SetPixel(x, y, color);
                }
            }
        }
    }

    private bool IsInsideTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float denom = a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y);
        if (Mathf.Approximately(denom, 0f)) return false;

        float w1 = (p.x * (b.y - c.y) + b.x * (c.y - p.y) + c.x * (p.y - b.y)) / denom;
        float w2 = (a.x * (p.y - c.y) + p.x * (c.y - a.y) + c.x * (a.y - p.y)) / denom;
        float w3 = 1f - w1 - w2;

        return w1 >= 0f && w2 >= 0f && w3 >= 0f;
    }
}