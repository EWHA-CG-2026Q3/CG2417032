using UnityEngine;

public class FillCheckerBoardCanvas : MonoBehaviour
{
    public int canvasWidth = 256;
    public int canvasHeight = 256;
    public int checkerSize = 32;
    public Color colorA = Color.black;
    public Color colorB = Color.white;

    private Texture2D canvasTexture;

    void Start()
    {
        canvasTexture = new Texture2D(canvasWidth, canvasHeight);
        canvasTexture.filterMode = FilterMode.Point; 

        FillCheckerBoard(checkerSize, colorA, colorB);
        canvasTexture.Apply(); 

        ApplyMaterial();
    }

    private void FillCheckerBoard(int size, Color colorA, Color colorB)
    {
        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                // (x / size) + (y / size)의 합이 짝수면 colorA, 홀수면 colorB
                bool isColorA = ((x / size) + (y / size)) % 2 == 0;
                Color pixelColor = isColorA ? colorA : colorB;

                canvasTexture.SetPixel(x, y, pixelColor);
            }
        }
    }

    private void ApplyMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) shader = Shader.Find("Unlit/Texture");
        if (shader == null) shader = Shader.Find("Sprites/Default");

        Material mat = new Material(shader);
        mat.mainTexture = canvasTexture;

        if (mat.HasProperty("_BaseMap"))
        {
            mat.SetTexture("_BaseMap", canvasTexture);
        }

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.sharedMaterial = mat;
        }
    }
}