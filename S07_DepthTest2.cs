using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class S07_DepthTest2 : MonoBehaviour
{
    [SerializeField] private int canvasWidth = 256;
    [SerializeField] private int canvasHeight = 256;

    // 삼각형 1 (파란색): 상단 정점이 가깝고(z=0.2), 하단 정점이 멂(z=0.8)
    [SerializeField] private Vector3 vertexA1 = new Vector3(128, 220, 0.2f);
    [SerializeField] private Vector3 vertexB1 = new Vector3(80, 40, 0.8f);
    [SerializeField] private Vector3 vertexC1 = new Vector3(220, 40, 0.8f);
    [SerializeField] private Color color1 = new Color(0.2f, 0.5f, 1f, 1f);

    // 삼각형 2 (주황색): 상단 정점이 멀고(z=0.8), 하단 정점이 가까움(z=0.2)
    [SerializeField] private Vector3 vertexA2 = new Vector3(100, 180, 0.8f);
    [SerializeField] private Vector3 vertexB2 = new Vector3(30, 60, 0.2f);
    [SerializeField] private Vector3 vertexC2 = new Vector3(170, 60, 0.2f);
    [SerializeField] private Color color2 = new Color(1f, 0.5f, 0.2f, 1f);

    private Texture2D canvasTexture;
    private RawImage targetImage;
    private float[,] depthBuffer;

    private void OnEnable() { RedrawAll(); }
    private void OnValidate() { RedrawAll(); }

    private void RedrawAll()
    {
        targetImage = GetComponent<RawImage>();
        if (targetImage == null) return;

        if (canvasTexture == null || canvasTexture.width != canvasWidth || canvasTexture.height != canvasHeight)
        {
            canvasTexture = new Texture2D(canvasWidth, canvasHeight);
            canvasTexture.filterMode = FilterMode.Point;
        }

        if (depthBuffer == null || depthBuffer.GetLength(0) != canvasWidth || depthBuffer.GetLength(1) != canvasHeight)
        {
            depthBuffer = new float[canvasWidth, canvasHeight];
        }

        // 깊이 버퍼 및 캔버스 초기화
        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                canvasTexture.SetPixel(x, y, Color.black);
                depthBuffer[x, y] = float.MaxValue;
            }
        }

        DrawTriangle(vertexA1, vertexB1, vertexC1, color1);
        DrawTriangle(vertexA2, vertexB2, vertexC2, color2);

        canvasTexture.Apply();
        targetImage.texture = canvasTexture;
    }

    private void DrawTriangle(Vector3 a, Vector3 b, Vector3 c, Color color)
    {
        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                if (CalculateBarycentric(p, a, b, c, out float w1, out float w2, out float w3))
                {
                    float interpolatedZ = w1 * a.z + w2 * b.z + w3 * c.z;

                    if (interpolatedZ < depthBuffer[x, y])
                    {
                        canvasTexture.SetPixel(x, y, color);
                        depthBuffer[x, y] = interpolatedZ;
                    }
                }
            }
        }
    }

    private bool CalculateBarycentric(Vector2 p, Vector3 a, Vector3 b, Vector3 c, out float w1, out float w2, out float w3)
    {
        float denom = a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y);
        if (Mathf.Approximately(denom, 0f))
        {
            w1 = w2 = w3 = 0;
            return false;
        }

        w1 = (p.x * (b.y - c.y) + b.x * (c.y - p.y) + c.x * (p.y - b.y)) / denom;
        w2 = (a.x * (p.y - c.y) + p.x * (c.y - a.y) + c.x * (a.y - p.y)) / denom;
        w3 = 1f - w1 - w2;

        return w1 >= 0f && w2 >= 0f && w3 >= 0f;
    }
}