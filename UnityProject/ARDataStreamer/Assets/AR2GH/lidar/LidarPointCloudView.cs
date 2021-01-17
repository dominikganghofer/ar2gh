using System.Linq;
using UnityEngine;

namespace ar2gh.lidar
{
    /// <summary>
    /// Renders  <see cref="LidarPoint"/>s as a mesh with vertex colors.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class LidarPointCloudView : MonoBehaviour
    {
        private Mesh _mesh;

        public void RenderVertices(LidarPoint[] cloud)
        {
            if (_mesh == null)
            {
                // generate mesh
                _mesh = new Mesh {indexFormat = UnityEngine.Rendering.IndexFormat.UInt32};
                gameObject.GetComponent<MeshFilter>().mesh = _mesh;
            }

            var vertexCountHasChanged = _mesh.vertices.Length != cloud.Length;
            if (!vertexCountHasChanged)
                return;
            
            var indices = new int[cloud.Length];
            for (var i = 0; i < cloud.Length; i++)
            {
                indices[i] = i;
            }
            
            _mesh.vertices = cloud.Where(c => c.IsValid).Select(c => c.WorldPosition).ToArray();
            _mesh.colors = cloud.Where(c => c.IsValid).Select(c => c.Color).ToArray();
            _mesh.RecalculateBounds();
            _mesh.SetIndices(indices, MeshTopology.Points, 0);
        }
    }
}