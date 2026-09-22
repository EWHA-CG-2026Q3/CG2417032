using UnityEngine;

public class S06_ImmediateModeTriangle_Finish : MonoBehaviour
{
    [SerializeField] private Material glMaterial;

    [SerializeField] private Vector3 vertexA = new Vector3(0.2f, 0.2f, 0f); // 좌하단
    [SerializeField] private Vector3 vertexB = new Vector3(0.8f, 0.2f, 0f); // 우하단
    [SerializeField] private Vector3 vertexC = new Vector3(0.5f, 0.8f, 0f); // 상단 중앙
    [SerializeField] private Color triangleColor = new Color(1f, 0.6f, 0.2f, 1f); // 주황색

    void OnRenderObject()
    {
        if (glMaterial == null)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            if (shader != null)
            {
                glMaterial = new Material(shader);
            }
            else
            {
                return;
            }
        }

        glMaterial.SetPass(0);

        GL.PushMatrix();
        GL.LoadOrtho();

        GL.Begin(GL.TRIANGLES);
        GL.Color(triangleColor);
        GL.Vertex3(vertexA.x, vertexA.y, vertexA.z);
        GL.Vertex3(vertexB.x, vertexB.y, vertexB.z);
        GL.Vertex3(vertexC.x, vertexC.y, vertexC.z);
        GL.End();

        GL.PopMatrix();
    }
}