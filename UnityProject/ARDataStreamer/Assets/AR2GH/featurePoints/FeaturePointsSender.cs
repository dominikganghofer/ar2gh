using System;
using System.Collections.Generic;
using System.Linq;
using ar2gh.camera;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ar2gh.featurePoints
{
    /// <summary>
    /// Uses the camera current camera image to generate a colored point cloud from feature points and sends it to Grasshopper.
    /// </summary>
    public class FeaturePointsSender : MonoBehaviour
    {
        public event Action<byte[]> PackageReadyEvent;

        [SerializeField] private CameraImageReceiver _cameraImageReceiver = null;

        [SerializeField] private Camera _camera = null;

        public class FeaturePoint
        {
            public ulong ID;
            public Vector3 Position;
            public Color? Color;
        }

        public void SendFeaturePoints(ARPointCloudChangedEventArgs e)
        {
            var updates = ExtractPointCloudUpdates(e.updated);
            _cameraImageReceiver.TryGetLatestCameraImage(texture2D => OnCameraImageReceived(updates, texture2D));
        }

        private void OnCameraImageReceived(List<FeaturePoint> updates, Texture2D camTexture)
        {
            var updatesWithColor = GenerateColoredPointCloud(updates, camTexture, _camera);
            var updatesWithinFrustum = updatesWithColor.Where(u => u.Color.HasValue).ToList();
            UpdateDebugView(updatesWithinFrustum);

            var cameraTransform = _camera.transform;
            var data = FeaturePointsSerializer.GeneratePointCloudData(
                updatesWithinFrustum,
                cameraTransform.position,
                cameraTransform.rotation.eulerAngles,
                Input.mousePosition,
                Vector2.zero);

            PackageReadyEvent?.Invoke(data);
        }

        private void UpdateDebugView(List<FeaturePoint> updates)
        {
            var mesh = new Mesh();
            var vertices = new List<Vector3>();
            var indices = new List<int>();
            var colors = new List<Color>();

            for (var i = 0; i < updates.Count; i++)
            {
                indices.Add(i);
                vertices.Add(updates[i].Position);
                colors.Add(updates[i].Color.Value);
            }

            // assign the array of colors to the Mesh.
            mesh.vertices = vertices.ToArray();
            mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);
            mesh.colors = colors.ToArray();
            GetComponent<MeshFilter>().mesh = mesh;
        }

        #region helpers

        private static List<FeaturePoint> ExtractPointCloudUpdates(List<ARPointCloud> updates)
        {
            var pointCloudUpdate = new List<FeaturePoint>();
            foreach (var updatedPatch in updates.Where(patch => patch.positions.HasValue))
            {
                for (var i = 0; i < updatedPatch.identifiers.Value.Length; i++)
                {
                    var id = updatedPatch.identifiers.Value[i];
                    var pos = updatedPatch.positions.Value[i];
                    pointCloudUpdate.Add(
                        new FeaturePoint()
                        {
                            ID = id,
                            Position = pos,
                        }
                    );
                }
            }
            return pointCloudUpdate;
        }

        private static List<FeaturePoint> GenerateColoredPointCloud(List<FeaturePoint> updates, Texture2D cameraImage,
            Camera camera)
        {
            foreach (var u in updates)
            {
                u.Color = GetColorAtWorldPosition(u.Position, cameraImage, camera);
            }
            return updates;
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

        #endregion
    }
}