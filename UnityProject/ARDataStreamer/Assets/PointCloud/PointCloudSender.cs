using System;
using System.Collections.Generic;
using System.Linq;
using ar2gh;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PointCloudSender : MonoBehaviour
{
    public event Action<byte[]> PackageReadyEvent;

    [SerializeField]
    private CameraImageReceiver _cameraImageReceiver = null;

    [SerializeField]
    private Camera _camera = null;

    public class PointCloudEntry
    {
        public ulong ID;
        public Vector3 Position;
        public Color? Color;
    }

    public void PointCloudChangedHandler(ARPointCloudChangedEventArgs e)
    {
        var updates = ExtractPointCloudUpdates(e.updated);
        _cameraImageReceiver.TryGetLatestCameraImage(texture2D => OnCameraImageReceived(updates, texture2D));
    }

    private void OnCameraImageReceived(List<PointCloudEntry> updates, Texture2D camTexture)
    {
        var t = _camera.transform;

        var updatesWithColor = GeneratePointCloud(updates, camTexture, _camera);
        var updatesWithinFrustum = updatesWithColor.Where(u => u.Color.HasValue).ToList();
        DrawUpdatesDebug(updatesWithinFrustum);

        var data = PointCloudSerializer.GeneratePointCloudData(
            updatesWithinFrustum,
            t.position,
            t.rotation.eulerAngles,
            Input.mousePosition,
            Vector2.zero);

        PackageReadyEvent?.Invoke(data);
    }

    private void DrawUpdatesDebug(List<PointCloudEntry> updates)
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

    private static List<PointCloudEntry> ExtractPointCloudUpdates(List<ARPointCloud> updates)
    {
        var pointCloudUpdate = new List<PointCloudEntry>();
        foreach (var updatedPatch in updates.Where(patch => patch.positions.HasValue))
        {
            for (var i = 0; i < updatedPatch.identifiers.Value.Length; i++)
            {
                var id = updatedPatch.identifiers.Value[i];
                var pos = updatedPatch.positions.Value[i];
                pointCloudUpdate.Add(
                    new PointCloudEntry()
                    {
                        ID = id,
                        Position = pos,
                    }
                );
            }
        }

        return pointCloudUpdate;
    }

    private static List<PointCloudEntry> GeneratePointCloud(List<PointCloudEntry> updates, Texture2D cameraImage,
                                                            Camera camera)
    {
        // parse event args
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
}