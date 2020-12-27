using System;
using System.Collections.Generic;
using System.Linq;
using ar2gh;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;

public class MeshSender : MonoBehaviour
{
    public event Action<byte[]> DataReadyEvent;

    [FormerlySerializedAs("_mockMesh")]
    [SerializeField]
    private List<MeshFilter> _mockMeshes;

    private Texture2D _rbgaTexture;

    [SerializeField]
    private CameraImageReceiver _cameraImageReceiver = null;

    [SerializeField]
    private Camera _camera = null;

    public void MeshesChangedHandler(ARMeshesChangedEventArgs arMeshesChangedEventArgs)
    {
        var serializableUpdate = SerializableMeshUpdate.FromUpdateEvent(arMeshesChangedEventArgs);
        _cameraImageReceiver.TryGetLatestCameraImage(texture2D => OnCameraImageReceived(serializableUpdate, texture2D));
    }

    public struct SerializableMesh
    {
        public Vector3[] Vertices;
        public int[] Triangles;
        public Color?[] Colors;
        public int ID;

        public static SerializableMesh FromMesh(Mesh m)
        {
            return new SerializableMesh()
            {
                ID = m.GetInstanceID(),
                Vertices = m.vertices,
                Triangles = m.GetIndices(0),
            };
        }
    }

    public class SerializableMeshUpdate
    {
        public SerializableMesh[] Added;
        public SerializableMesh[] Updated;
        public SerializableMesh[] Removed;

        public static SerializableMeshUpdate FromUpdateEvent(ARMeshesChangedEventArgs e)
        {
            return new SerializableMeshUpdate()
            {
                Added = e.added.Select(mf => mf.mesh).Select(SerializableMesh.FromMesh).ToArray(),
                Updated = e.updated.Select(mf => mf.mesh).Select(SerializableMesh.FromMesh).ToArray(),
                Removed = e.removed.Select(mf => mf.mesh).Select(SerializableMesh.FromMesh).ToArray()
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
    }

    private void OnCameraImageReceived(SerializableMeshUpdate meshes, Texture2D camTexture)
    {
        meshes.WriteCameraImageColors(_camera, camTexture);
        var data = MeshSerializer.GenerateMeshData(meshes);
        DataReadyEvent?.Invoke(data);
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