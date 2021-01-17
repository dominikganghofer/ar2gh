using System;
using System.Collections.Generic;
using UnityEngine;
using ar2gh.camera;

namespace ar2gh.mesh
{
    /// <summary>
    /// Uses <see cref="CameraImageReceiver"/> to generate vertex colors for a environment mesh and sends it to Grasshopper.
    /// </summary>
    public class MeshSender : MonoBehaviour
    {
        public event Action<byte[]> DataReadyEvent;

        [SerializeField] private List<MeshFilter> _mockMeshes;
        [SerializeField] private CameraImageReceiver _cameraImageReceiver = null;
        [SerializeField] private Camera _camera = null;

        private Texture2D _rbgaTexture;

        public void SendUpdate(List<MeshFilter> added, List<MeshFilter> updated, List<MeshFilter> removed)
        {
            var serializableUpdate = MeshDataUpdates.Generate(added, updated, removed);
            _cameraImageReceiver.TryGetLatestCameraImage(texture2D =>
                OnCameraImageReceived(serializableUpdate, texture2D));
        }

        private void OnCameraImageReceived(MeshDataUpdates meshes, Texture2D camTexture)
        {
            meshes.WriteCameraImageColors(_camera, camTexture);
            var data = MeshSerializer.GenerateMeshData(meshes);
            DataReadyEvent?.Invoke(data);
        }
    }
}