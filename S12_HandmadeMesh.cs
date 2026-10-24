using UnityEngine;

// S03-S04 style: manually authored vertices and triangle winding.
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class S12_HandmadeMesh : MonoBehaviour
{
    public enum Shape { Pyramid, TriangularPrism }
    public Shape shape = Shape.Pyramid;

    void OnEnable() { Build(); }
    void OnValidate() { if (isActiveAndEnabled) Build(); }

    void Build()
    {
        Vector3[] v;
        int[] t;
        if (shape == Shape.Pyramid)
        {
            v = new [] {new Vector3(-.5f,0,-.5f),new Vector3(.5f,0,-.5f),new Vector3(.5f,0,.5f),new Vector3(-.5f,0,.5f),new Vector3(0,1,0)};
            t = new [] {0,2,1, 0,3,2, 0,1,4, 1,2,4, 2,3,4, 3,0,4};
        }
        else
        {
            v = new [] {new Vector3(-.5f,0,-.4f),new Vector3(.5f,0,-.4f),new Vector3(0,.9f,-.4f),new Vector3(-.5f,0,.4f),new Vector3(.5f,0,.4f),new Vector3(0,.9f,.4f)};
            t = new [] {0,2,1, 3,4,5, 0,1,4, 0,4,3, 1,2,5, 1,5,4, 2,0,3, 2,3,5};
        }
        Mesh mesh = new Mesh { name = "S12_Handmade_" + shape };
        mesh.vertices = v;
        mesh.triangles = t;
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
