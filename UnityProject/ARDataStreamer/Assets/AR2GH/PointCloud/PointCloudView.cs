using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class PointCloudView : MonoBehaviour
{
    private Mesh _mesh;

    public void RenderVertices(LidarPointCloud.CloudPoint[] cloud)
    {
        if (_mesh == null)
        {
            // create mesh
            _mesh = new Mesh {indexFormat = UnityEngine.Rendering.IndexFormat.UInt32};
            gameObject.GetComponent<MeshFilter>().mesh = _mesh;
        }

        var vertexCountHasChanged = _mesh.vertices.Length != cloud.Length;
        _mesh.vertices = cloud.Where(c=>c.IsValid).Select(c => c.WorldPosition).ToArray();
        _mesh.colors = cloud.Where(c=>c.IsValid).Select(c => c.Color).ToArray();
        _mesh.RecalculateBounds();

        if (vertexCountHasChanged)
        {
            var indices = new int[cloud.Length];
            for (var i = 0; i < cloud.Length; i++)
                indices[i] = i;

            _mesh.SetIndices(indices, MeshTopology.Points, 0);
        }
    }
}