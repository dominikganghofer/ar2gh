using System.Linq;

namespace ar2gh.mesh
{
    /// <summary>
    /// Serializes a colored environment mesh.
    /// </summary>
    public static class MeshSerializer
    {
        public static byte[] GenerateMeshData(MeshDataUpdates meshes)
        {
            var streamSize = sizeof(byte); // streamType
            streamSize += CalculateSizeForMeshes(meshes.Added);
            streamSize += CalculateSizeForMeshes(meshes.Updated);
            streamSize += CalculateSizeForMeshes(meshes.Removed);

            var data = new byte[streamSize];
            var dstOffSet = 0;

            SerializationHelper.WriteStreamType(SerializationHelper.StreamType.Mesh, ref data, ref dstOffSet);
            WriteMeshArray(meshes.Added, ref data, ref dstOffSet);
            WriteMeshArray(meshes.Updated, ref data, ref dstOffSet);
            WriteMeshArray(meshes.Removed, ref data, ref dstOffSet);

            return data;
        }

        private static void WriteMeshArray(MeshData[] meshes, ref byte[] data, ref int dstOffSet)
        {
            var meshCount = meshes.Length;
            SerializationHelper.WriteInt(meshCount, ref data, ref dstOffSet);
            foreach (var m in meshes)
            {
                WriteMesh(m, ref data, ref dstOffSet);
            }
        }

        private static void WriteMesh(MeshData meshData, ref byte[] data, ref int dstOffSet)
        {
            SerializationHelper.WriteInt(meshData.ID, ref data, ref dstOffSet);
            SerializationHelper.WriteInt(meshData.Vertices.Length, ref data, ref dstOffSet);
            SerializationHelper.WriteInt(meshData.Triangles.Length, ref data, ref dstOffSet);

            //write vertices 
            foreach (var v in meshData.Vertices)
            {
                SerializationHelper.WriteVector3AsMMInt(v, ref data, ref dstOffSet);
            }

            //write triangles
            foreach (var t in meshData.Triangles)
            {
                SerializationHelper.WriteInt(t, ref data, ref dstOffSet);
            }

            //write colors
            foreach (var c in meshData.Colors)
            {
                SerializationHelper.WriteNullableColor32(c, ref data, ref dstOffSet);
            }
        }

        private static int CalculateSizeForMeshes(MeshData[] meshes)
        {
            var size = sizeof(int); //mesh count

            foreach (var m in meshes)
            {
                size += sizeof(int); //mesh id
                size += sizeof(int); //vertex count
                size += sizeof(int); //triangle count
                size += m.Vertices.Sum(v => 3 * sizeof(float));
                size += m.Vertices.Sum(v => 3 * sizeof(float)); //colors
                size += m.Triangles.Sum(t => sizeof(int));
            }

            return size;
        }
    }
}