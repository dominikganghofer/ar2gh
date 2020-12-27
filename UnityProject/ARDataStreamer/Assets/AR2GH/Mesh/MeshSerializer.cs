using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ar2gh;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public static class MeshSerializer
{
    public static byte[] GenerateMeshData(MeshSender.SerializableMeshUpdate meshes)
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

    private static void WriteMeshArray(MeshSender.SerializableMesh[] meshes, ref byte[] data, ref int dstOffSet)
    {
        var meshCount = meshes.Length;
        SerializationHelper.WriteInt(meshCount, ref data, ref dstOffSet);
        foreach (var m in meshes)
        {
            WriteMesh(m, ref data, ref dstOffSet);
        }
    }
    
    
    private static void WriteMesh(MeshSender.SerializableMesh mesh, ref byte[] data, ref int dstOffSet)
    {
        SerializationHelper.WriteInt(mesh.ID, ref data, ref dstOffSet);
        SerializationHelper.WriteInt(mesh.Vertices.Length, ref data, ref dstOffSet);
        SerializationHelper.WriteInt(mesh.Triangles.Length, ref data, ref dstOffSet);

        //write vertices 
        foreach (var v in mesh.Vertices)
        {
            SerializationHelper.WriteVector3AsMMInt(v, ref data, ref dstOffSet);
        }

        //write triangles
        foreach (var t in mesh.Triangles)
        {
            SerializationHelper.WriteInt(t, ref data, ref dstOffSet);
        }

        //write colors
        foreach (var c in mesh.Colors)
        {
            SerializationHelper.WriteNullableColor32(c, ref data, ref dstOffSet);
        }
    }

    private static int CalculateSizeForMeshes(MeshSender.SerializableMesh[] meshes)
    {
        var size = 0;
        size += sizeof(int); //mesh count

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