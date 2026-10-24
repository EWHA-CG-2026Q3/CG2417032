using System.Collections.Generic;
using UnityEngine;

// S12 study implementation: perspective-projected wireframe canvas beside Unity's Game view.
// Attach to an EMPTY GameObject called VirtualCamera, not to Main Camera.
[ExecuteAlways]
public class S12_VirtualCamera_Finish : MonoBehaviour
{
    [Range(10f, 150f)] public float fieldOfView = 90f;
    [Min(0.01f)] public float near = 0.3f;
    [Min(0.1f)] public float far = 20f;
    public GameObject[] targets;
    public int width = 400;
    public int height = 300;
    public bool showCanvas = true;

    private Texture2D canvas;
    private Color32[] pixels;
    private Matrix4x4 projection;

    void OnEnable() { CreateCanvas(); }
    void OnDisable() { if (canvas != null) { if (Application.isPlaying) Destroy(canvas); else DestroyImmediate(canvas); canvas = null; } }

    void CreateCanvas()
    {
        width = Mathf.Clamp(width, 64, 1024);
        height = Mathf.Clamp(height, 64, 1024);
        if (canvas != null && canvas.width == width && canvas.height == height) return;
        if (canvas != null) { if (Application.isPlaying) Destroy(canvas); else DestroyImmediate(canvas); }
        canvas = new Texture2D(width, height, TextureFormat.RGBA32, false);
        canvas.filterMode = FilterMode.Point;
        pixels = new Color32[width * height];
    }

    // Based on the projection matrix shown in the S12 slides.
    Matrix4x4 BuildProjectionMatrix()
    {
        float aspect = 1f; // For the assignment, temporarily change this to 1f.
        float t = Mathf.Tan(fieldOfView * 0.5f * Mathf.Deg2Rad);
        Matrix4x4 P = Matrix4x4.zero;
        P.m00 = 1f / (t * aspect);
        P.m11 = 1f / t;
        P.m32 = 1f; // w = camera-space z
        return P;
    }

    bool ToPixel(Vector3 p, out Vector2 pixel)
    {
        pixel = Vector2.zero;
        if (p.z < near || p.z > far) return false;
        Vector4 clip = projection * new Vector4(p.x, p.y, p.z, 1f);
        if (Mathf.Abs(clip.w) < 0.00001f) return false;
        Vector2 ndc = new Vector2(clip.x / clip.w, clip.y / clip.w);
        pixel.x = width * 0.5f + ndc.x * width * 0.5f;
        pixel.y = height * 0.5f + ndc.y * height * 0.5f;
        return true;
    }

    void Update()
    {
        if (!showCanvas) return;
        CreateCanvas();
        projection = BuildProjectionMatrix();
        for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color32(22, 27, 36, 255);
        if (targets != null)
            foreach (GameObject target in targets)
                if (target != null && target.activeInHierarchy) DrawTarget(target);
        canvas.SetPixels32(pixels);
        canvas.Apply(false);
    }

    void DrawTarget(GameObject target)
    {
        MeshFilter[] filters = target.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter filter in filters)
        {
            Mesh mesh = filter.sharedMesh;
            if (mesh == null) continue;
            Vector3[] verts = mesh.vertices;
            int[] triangles = mesh.triangles;
            Matrix4x4 VM = transform.worldToLocalMatrix * filter.transform.localToWorldMatrix;
            HashSet<ulong> seen = new HashSet<ulong>();
            for (int i = 0; i + 2 < triangles.Length; i += 3)
            {
                DrawEdgeUnique(triangles[i], triangles[i + 1], verts, VM, seen);
                DrawEdgeUnique(triangles[i + 1], triangles[i + 2], verts, VM, seen);
                DrawEdgeUnique(triangles[i + 2], triangles[i], verts, VM, seen);
            }
        }
    }

    void DrawEdgeUnique(int a, int b, Vector3[] vertices, Matrix4x4 VM, HashSet<ulong> seen)
    {
        if (a < 0 || b < 0 || a >= vertices.Length || b >= vertices.Length || a == b) return;
        uint lo = (uint)Mathf.Min(a, b), hi = (uint)Mathf.Max(a, b);
        ulong key = ((ulong)lo << 32) | hi;
        if (!seen.Add(key)) return;
        Vector3 A = VM.MultiplyPoint3x4(vertices[a]);
        Vector3 B = VM.MultiplyPoint3x4(vertices[b]);
        // Clip against near/far planes, rather than dropping the entire edge.
        if (!ClipDepth(ref A, ref B)) return;
        Vector2 pa, pb;
        if (ToPixel(A, out pa) && ToPixel(B, out pb)) DrawLine(pa, pb);
    }

    bool ClipDepth(ref Vector3 a, ref Vector3 b)
    {
        if (a.z < near && b.z < near || a.z > far && b.z > far) return false;
        Vector3 origA = a, origB = b;
        float dz = origB.z - origA.z;
        if (Mathf.Abs(dz) < 0.000001f) return a.z >= near && a.z <= far;
        float t0 = Mathf.Clamp01((near - origA.z) / dz);
        float t1 = Mathf.Clamp01((far - origA.z) / dz);
        float enter = Mathf.Max(0f, Mathf.Min(t0, t1));
        float exit = Mathf.Min(1f, Mathf.Max(t0, t1));
        if (origA.z >= near && origA.z <= far) enter = 0f;
        if (origB.z >= near && origB.z <= far) exit = 1f;
        if (enter > exit) return false;
        a = Vector3.Lerp(origA, origB, enter);
        b = Vector3.Lerp(origA, origB, exit);
        return true;
    }

    void DrawLine(Vector2 a, Vector2 b)
    {
        // Clamp very large screen projections to avoid excessive work.
        int steps = Mathf.CeilToInt(Mathf.Max(Mathf.Abs(b.x - a.x), Mathf.Abs(b.y - a.y)));
        steps = Mathf.Clamp(steps, 1, 4000);
        Color32 color = new Color32(105, 225, 255, 255);
        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            int x = Mathf.RoundToInt(Mathf.Lerp(a.x, b.x, t));
            int y = Mathf.RoundToInt(Mathf.Lerp(a.y, b.y, t));
            if (x >= 0 && x < width && y >= 0 && y < height) pixels[y * width + x] = color;
        }
    }

    void OnGUI()
    {
        if (!showCanvas || canvas == null) return;
        GUI.Box(new Rect(8, 8, width + 16, height + 38), "VirtualCamera canvas (wireframe)");
        GUI.DrawTexture(new Rect(16, 34, width, height), canvas, ScaleMode.StretchToFill);
    }
}
