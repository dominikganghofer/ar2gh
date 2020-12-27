using Rhino.Geometry;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;

namespace AR2GH.Parse
{
    /// <summary>
    /// Parse AR-Foundation enviroment mesh
    /// </summary>
    class MeshParser
    {
        public static void ParseMeshUpdates(byte[] rawData, int startIndex, ref ConcurrentDictionary<int, Mesh> receivedMeshes)
        {
            // If a vertex does not have a valid color, because it lies on the edge of the camera view port, the color
            // of its closest neighbour is used. 
            Mesh UseNeighbourColorIfUndefined(Mesh existingMesh, Mesh newMesh)
            {
                for (var iNewVertex = 0; iNewVertex < newMesh.Vertices.Count; iNewVertex++)
                {
                    if (newMesh.VertexColors[iNewVertex].A > 0)
                        continue;

                    var vertexToPaint = newMesh.Vertices[iNewVertex];
                    var iNeighbour = GetIndexOfClosestColoredVertex(existingMesh, vertexToPaint);
                
                    newMesh.VertexColors[iNewVertex] = existingMesh.VertexColors[iNeighbour];
                }
                return newMesh;
            }

            var addedCount = ParserHelper.ToInt(rawData, ref startIndex);
            for (var iMesh = 0; iMesh < addedCount; iMesh++)
            {
                var id = ParserHelper.ToInt(rawData, ref startIndex);
                var m = ParseMesh(rawData, ref startIndex);
                receivedMeshes.AddOrUpdate(id, meshID => m, (meshID, existingMesh) => UseNeighbourColorIfUndefined(existingMesh, m));
            }

            var updatedCount = ParserHelper.ToInt(rawData, ref startIndex);
            for (var iMesh = 0; iMesh < updatedCount; iMesh++)
            {
                var id = ParserHelper.ToInt(rawData, ref startIndex);
                var m = ParseMesh(rawData, ref startIndex);
                receivedMeshes.AddOrUpdate(id, meshID => m, (meshID, existingMesh) => UseNeighbourColorIfUndefined(existingMesh, m));
            }

            var removedCount = ParserHelper.ToInt(rawData, ref startIndex);
            for (var iMesh = 0; iMesh < removedCount; iMesh++)
            {
                var id = ParserHelper.ToInt(rawData, ref startIndex);
                var m = ParseMesh(rawData, ref startIndex);
                receivedMeshes.TryRemove(id, out Mesh removedMesh);
            }
        }

        private static Mesh ParseMesh(byte[] rawData, ref int startIndex)
        {
            var vertexCount = ParserHelper.ToInt(rawData, ref startIndex);
            var triangleCount = ParserHelper.ToInt(rawData, ref startIndex);

            var vertices = new List<Point3d>();
            for (var iVertex = 0; iVertex < vertexCount; iVertex++)
            {
                vertices.Add(ParserHelper.ToCartesianVectorMMPrecision(rawData, ref startIndex));
            }

            var triangles = new List<int>();
            for (var iTriangle = 0; iTriangle < triangleCount; iTriangle++)
            {
                triangles.Add(ParserHelper.ToInt(rawData, ref startIndex));
            }

            var colors = new List<Color>();
            for (var iColor = 0; iColor < vertexCount; iColor++)
            {
                colors.Add(ParserHelper.ToColorRGBA32(rawData, ref startIndex));
            }

            var mesh = new Mesh();
            mesh.Vertices.AddVertices(vertices);

            for (var i = 0; i < vertices.Count; i++)
            {
                mesh.VertexColors.Add(colors[i]);
            }

            for (int i = 0; i < triangles.Count; i += 3)
            {
                mesh.Faces.AddFace(triangles[i], triangles[i + 1], triangles[i + 2]);
            }

            return mesh;
        }

        private static int GetIndexOfClosestColoredVertex(Mesh mesh, Point3f position)
        {
            (int index, double distance) searchResult = (-1, float.PositiveInfinity);
            for (var i = 0; i < mesh.Vertices.Count; i++)
            {
                //skip vertices with no color
                if (mesh.VertexColors[i].A == 0)
                    continue;

                var distance = position.DistanceTo(mesh.Vertices[i]);
                if (distance < searchResult.distance)
                    searchResult = (i, distance);
            }
            return searchResult.index;
        }
    }
}
