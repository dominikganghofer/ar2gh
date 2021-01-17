using UnityEngine;

namespace ar2gh.mesh
{
    /// <summary>
    /// The relevant data for environment mesh.
    /// </summary>
    public struct MeshData
    {
        public Vector3[] Vertices;
        public int[] Triangles;
        public Color?[] Colors;
        public int ID;

        public static MeshData FromMesh(Mesh m)
        {
            return new MeshData()
            {
                ID = m.GetInstanceID(),
                Vertices = m.vertices,
                Triangles = m.GetIndices(0),
            };
        }
    }
}