using UnityEngine;

// S09 과제: 높이(y)에 비례해 x축 방향으로 기울이기
// 교수님 S09_TRS_Raw_Finish.cs의 4x4 행렬 계산 방식을 활용했습니다.
[ExecuteAlways]
[RequireComponent(typeof(DiamondMesh))]
public class S09_Shear : MonoBehaviour
{
    [SerializeField] float k = 0.6f; // 학번 끝자리 2인 경우. Inspector에서 변경 가능

    DiamondMesh diamondMesh;

    void OnEnable()
    {
        diamondMesh = GetComponent<DiamondMesh>();
        ApplyShearAndLog();
    }

    void OnValidate()
    {
        diamondMesh = GetComponent<DiamondMesh>();
        ApplyShearAndLog();
    }

    void Update()
    {
        if (diamondMesh == null) diamondMesh = GetComponent<DiamondMesh>();
        if (diamondMesh == null || diamondMesh.BaseVertices == null) return;
        diamondMesh.SetVertices(ApplyShearRaw(diamondMesh.BaseVertices, k));
    }

    // 4x4 Shear 행렬: (x, y, z) -> (x + k*y, y, z)
    float[,] ShearMatrixRaw(float k)
    {
        return new float[,]
        {
            { 1f, k,  0f, 0f },
            { 0f, 1f, 0f, 0f },
            { 0f, 0f, 1f, 0f },
            { 0f, 0f, 0f, 1f }
        };
    }

    Vector4 ToHomogeneous(Vector3 v)
    {
        return new Vector4(v.x, v.y, v.z, 1f);
    }

    Vector3 FromHomogeneous(Vector4 h)
    {
        return new Vector3(h.x, h.y, h.z);
    }

    Vector4 MultiplyMatrixVectorRaw(float[,] M, Vector4 v)
    {
        float[] input = { v.x, v.y, v.z, v.w };
        float[] result = new float[4];
        for (int row = 0; row < 4; row++)
            for (int col = 0; col < 4; col++)
                result[row] += M[row, col] * input[col];
        return new Vector4(result[0], result[1], result[2], result[3]);
    }

    Vector3[] ApplyShearRaw(Vector3[] baseVertices, float amount)
    {
        float[,] M = ShearMatrixRaw(amount);
        Vector3[] verts = new Vector3[baseVertices.Length];
        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector4 h = ToHomogeneous(baseVertices[i]);
            verts[i] = FromHomogeneous(MultiplyMatrixVectorRaw(M, h));
        }
        return verts;
    }

    void ApplyShearAndLog()
    {
        if (diamondMesh == null || diamondMesh.BaseVertices == null) return;
        diamondMesh.SetVertices(ApplyShearRaw(diamondMesh.BaseVertices, k));

        Vector3 top = new Vector3(0.5f, 1f, 0.5f);
        Vector3 result = FromHomogeneous(
            MultiplyMatrixVectorRaw(ShearMatrixRaw(k), ToHomogeneous(top)));
        Debug.Log($"[{gameObject.name}] k = {k:F2}, top = ({result.x:F2}, {result.y:F2}, {result.z:F2})", this);
    }
}
