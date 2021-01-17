using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ar2gh.mesh
{
    /// <summary>
    /// Mesh updates as <see cref="MeshData"/>.
    /// </summary>
    public class MeshDataUpdates
    {
        public MeshData[] Added;
        public MeshData[] Updated;
        public MeshData[] Removed;

        public static MeshDataUpdates Generate(List<MeshFilter> added, List<MeshFilter> updated,
            List<MeshFilter> removed)
        {
            return new MeshDataUpdates()
            {
                Added = added.Select(mf => mf.mesh).Select(MeshData.FromMesh).ToArray(),
                Updated = updated.Select(mf => mf.mesh).Select(MeshData.FromMesh).ToArray(),
                Removed = removed.Select(mf => mf.mesh).Select(MeshData.FromMesh).ToArray()
            };
        }

        public void WriteCameraImageColors(Camera camera, Texture2D camTexture)
        {
            for (var iMesh = 0; iMesh < Added.Count(); iMesh++)
            {
                var m = Added[iMesh];
                var colors = new Color?[m.Vertices.Length];
                for (var i = 0; i < m.Vertices.Length; i++)
                {
                    var vertex = m.Vertices[i];
                    colors[i] = GetColorAtWorldPosition(vertex, camTexture, camera);
                }

                Added[iMesh].Colors = colors;
            }

            for (var iMesh = 0; iMesh < Updated.Count(); iMesh++)
            {
                var m = Updated[iMesh];
                var colors = new Color?[m.Vertices.Length];
                for (var i = 0; i < m.Vertices.Length; i++)
                {
                    var vertex = m.Vertices[i];
                    colors[i] = GetColorAtWorldPosition(vertex, camTexture, camera);
                }

                Updated[iMesh].Colors = colors;
            }
        }

        private static Color? GetColorAtWorldPosition(Vector3 worldPosition, Texture2D texture, Camera camera)
        {
            var screenPosition = camera.WorldToScreenPoint(worldPosition);
            if (screenPosition.x < 0 || screenPosition.x > Screen.width)
                return null;
            if (screenPosition.y < 0 || screenPosition.y > Screen.height)
                return null;

            var wTextureToScreen = texture.width / (1f * Screen.width);
            var hTextureToScreen = texture.height / (1f * Screen.height);

            return texture.GetPixel((int) (wTextureToScreen * screenPosition.x),
                (int) (hTextureToScreen * screenPosition.y));
        }
    }

}