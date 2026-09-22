using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class S07_Clipping : MonoBehaviour
{
    [SerializeField] private int canvasWidth = 256;
    [SerializeField] private int canvasHeight = 256;
    [SerializeField] private int clipMargin = 40;

    // [과제 조건] 다각형이 회색 여백선(왼쪽 x=40, 위쪽 y=216) 두 방향 이상을 넘어가도록 좌표 설계
    [SerializeField]
    private List<Vector2> polygon = new List<Vector2> {
        new Vector2(10, 130),   
        new Vector2(130, 250),  
        new Vector2(230, 60)    
    };

    [SerializeField] private Color fillColor = new Color(1f, 0.6f, 0.2f, 1f);

    private Texture2D canvasTexture;
    private RawImage targetImage;

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

        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                canvasTexture.SetPixel(x, y, Color.black);
            }
        }

        DrawMarginOutline();

        List<Vector2> clipped = polygon;
        clipped = ClipLeft(clipped, clipMargin);
        clipped = ClipRight(clipped, canvasWidth - clipMargin);
        clipped = ClipBottom(clipped, clipMargin);
        clipped = ClipTop(clipped, canvasHeight - clipMargin);

        FillPolygon(clipped, fillColor);

        canvasTexture.Apply();
        targetImage.texture = canvasTexture;
    }

    private void DrawMarginOutline()
    {
        Color outlineColor = Color.gray;
        int minX = clipMargin;
        int maxX = canvasWidth - clipMargin;
        int minY = clipMargin;
        int maxY = canvasHeight - clipMargin;

        for (int x = minX; x <= maxX; x++)
        {
            canvasTexture.SetPixel(x, minY, outlineColor);
            canvasTexture.SetPixel(x, maxY, outlineColor);
        }
        for (int y = minY; y <= maxY; y++)
        {
            canvasTexture.SetPixel(minX, y, outlineColor);
            canvasTexture.SetPixel(maxX, y, outlineColor);
        }
    }

    private List<Vector2> ClipLeft(List<Vector2> input, float boundary)
    {
        List<Vector2> output = new List<Vector2>();
        for (int i = 0; i < input.Count; i++)
        {
            Vector2 current = input[i];
            Vector2 previous = input[(i - 1 + input.Count) % input.Count];

            bool currentInside = current.x >= boundary;
            bool previousInside = previous.x >= boundary;

            if (currentInside)
            {
                if (!previousInside) output.Add(GetIntersectionX(previous, current, boundary));
                output.Add(current);
            }
            else if (previousInside)
            {
                output.Add(GetIntersectionX(previous, current, boundary));
            }
        }
        return output;
    }

    private List<Vector2> ClipRight(List<Vector2> input, float boundary)
    {
        List<Vector2> output = new List<Vector2>();
        for (int i = 0; i < input.Count; i++)
        {
            Vector2 current = input[i];
            Vector2 previous = input[(i - 1 + input.Count) % input.Count];

            bool currentInside = current.x <= boundary;
            bool previousInside = previous.x <= boundary;

            if (currentInside)
            {
                if (!previousInside) output.Add(GetIntersectionX(previous, current, boundary));
                output.Add(current);
            }
            else if (previousInside)
            {
                output.Add(GetIntersectionX(previous, current, boundary));
            }
        }
        return output;
    }

    private List<Vector2> ClipBottom(List<Vector2> input, float boundary)
    {
        List<Vector2> output = new List<Vector2>();
        for (int i = 0; i < input.Count; i++)
        {
            Vector2 current = input[i];
            Vector2 previous = input[(i - 1 + input.Count) % input.Count];

            bool currentInside = current.y >= boundary;
            bool previousInside = previous.y >= boundary;

            if (currentInside)
            {
                if (!previousInside) output.Add(GetIntersectionY(previous, current, boundary));
                output.Add(current);
            }
            else if (previousInside)
            {
                output.Add(GetIntersectionY(previous, current, boundary));
            }
        }
        return output;
    }

    private List<Vector2> ClipTop(List<Vector2> input, float boundary)
    {
        List<Vector2> output = new List<Vector2>();
        for (int i = 0; i < input.Count; i++)
        {
            Vector2 current = input[i];
            Vector2 previous = input[(i - 1 + input.Count) % input.Count];

            bool currentInside = current.y <= boundary;
            bool previousInside = previous.y <= boundary;

            if (currentInside)
            {
                if (!previousInside) output.Add(GetIntersectionY(previous, current, boundary));
                output.Add(current);
            }
            else if (previousInside)
            {
                output.Add(GetIntersectionY(previous, current, boundary));
            }
        }
        return output;
    }

    private Vector2 GetIntersectionX(Vector2 p1, Vector2 p2, float boundaryX)
    {
        float t = (boundaryX - p1.x) / (p2.x - p1.x);
        return new Vector2(boundaryX, p1.y + t * (p2.y - p1.y));
    }

    private Vector2 GetIntersectionY(Vector2 p1, Vector2 p2, float boundaryY)
    {
        float t = (boundaryY - p1.y) / (p2.y - p1.y);
        return new Vector2(p1.x + t * (p2.x - p1.x), boundaryY);
    }

    private void FillPolygon(List<Vector2> poly, Color color)
    {
        if (poly.Count < 3) return;
        for (int i = 1; i < poly.Count - 1; i++)
        {
            DrawTriangle(poly[0], poly[i], poly[i + 1], color);
        }
    }

    private void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
    {
        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                if (IsInsideTriangle(p, a, b, c))
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