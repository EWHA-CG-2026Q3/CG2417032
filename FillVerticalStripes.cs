using UnityEngine;

public class VerticalStripeCanvas : MonoBehaviour
{
    public int canvasWidth = 256;
    public int canvasHeight = 256;
    public int stripeWidth = 32;
    public Color colorA = Color.black;
    public Color colorB = Color.white;

    private Texture2D canvasTexture;

    void Start()
    {
        canvasTexture = new Texture2D(canvasWidth, canvasHeight);
        FillVerticalStripes(stripeWidth, colorA, colorB);
        canvasTexture.Apply(); // 텍스처 변경사항 적용

        // 머티리얼 생성 및 텍스처 연결
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        Material mat = new Material(shader);
        mat.mainTexture = canvasTexture;
        GetComponent<MeshRenderer>().sharedMaterial = mat;
    }

    private void FillVerticalStripes(int width, Color colorA, Color colorB)
    {
        for (int x = 0; x < canvasWidth; x++)
        {
            // x 좌표를 width로 나눈 몫이 짝수면 colorA, 홀수면 colorB
            bool isColorA = (x / width) % 2 == 0;
            Color stripeColor = isColorA ? colorA : colorB;

            for (int y = 0; y < canvasHeight; y++)
            {
                canvasTexture.SetPixel(x, y, stripeColor);
            }
        }
    }
}