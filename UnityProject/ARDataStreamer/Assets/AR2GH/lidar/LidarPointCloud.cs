using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Random = UnityEngine.Random;

namespace ar2gh.lidar
{
    /// <summary>
    /// Generates a colored point cloud of the environment using the depth texture from <see cref="AROcclusionManager"/>
    /// and the camera image from <see cref="ARCameraManager"/>.
    /// </summary>
    public  class LidarPointCloud : MonoBehaviour
    {
        public event Action<LidarPoint[]> CloudUpdateEvent;

        [SerializeField] private ARCameraManager _cameraManager = null;
        [SerializeField] private AROcclusionManager _occlusionManager = null;
        [SerializeField] private DebugViews _debugViews = default;
       
        private const float Near = 0.2f;
        private const float Far = 3f;
        
        private float _cx, _cy, _fx, _fy;

        private Texture2D _cameraTexture;
        private Texture2D _depthTextureFloat;
        private Texture2D _depthTextureDebug;
        private Texture2D _confidenceTexR8;
        private Texture2D _confidenceTexDebug;
        
        private LidarPoint[] _cloud;
        private float _lastTimeGenerated = float.NegativeInfinity;

        private void OnEnable()
        {
            _cameraManager.frameReceived += OnCameraFrameReceived;
        }

        private void OnDisable()
        {
            _cameraManager.frameReceived -= OnCameraFrameReceived;
        }

        private unsafe void UpdateCameraImage()
        {
            if (!_cameraManager.TryAcquireLatestCpuImage(out var image))
            {
                CloudUpdateEvent?.Invoke(GenerateDebugCloud());
                return;
            }

            using (image)
            {
                if (_cameraTexture == null || !HasEqualDimensions(_cameraTexture, image))
                    _cameraTexture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);

                var conversionParams =
                    new XRCpuImage.ConversionParams(image, TextureFormat.RGBA32, XRCpuImage.Transformation.MirrorY);
                var textureRaw = _cameraTexture.GetRawTextureData<byte>();
                image.Convert(conversionParams, new IntPtr(textureRaw.GetUnsafePtr()), textureRaw.Length);

                _cameraTexture.Apply();
                _debugViews.Camera.texture = _cameraTexture;
            }
        }

        private void UpdateEnvironmentDepthImage()
        {
            if (!_occlusionManager.TryAcquireEnvironmentDepthCpuImage(out var image))
                return;

            using (image)
            {
                // initialize textures
                if (_depthTextureFloat == null || !HasEqualDimensions(_depthTextureFloat, image))
                {
                    _depthTextureFloat =
                        new Texture2D(image.width, image.height, image.format.AsTextureFormat(), false);
                    _depthTextureDebug = new Texture2D(image.width, image.height, TextureFormat.BGRA32, false);
                }

                //Acquire Depth Image (RFloat format). Depth pixels are stored with meter unit.
                UpdateRawImage(_depthTextureFloat, image);

                //Convert RFloat into Grayscale Image between near and far clip area.
                ConvertFloatToGrayScale(_depthTextureFloat, _depthTextureDebug);

                //Visualize near~far depth.
                _debugViews.Depth.texture = _depthTextureDebug;
            }
        }

        private void UpdateEnvironmentConfidenceImage()
        {
            if (!_occlusionManager.TryAcquireEnvironmentDepthConfidenceCpuImage(out var image))
                return;

            using (image)
            {
                if (_confidenceTexR8 == null || HasEqualDimensions(_confidenceTexR8, image))
                {
                    _confidenceTexR8 = new Texture2D(image.width, image.height, image.format.AsTextureFormat(), false);
                    _confidenceTexDebug = new Texture2D(image.width, image.height, TextureFormat.BGRA32, false);
                }

                UpdateRawImage(_confidenceTexR8, image);
                ConvertR8ToConfidenceMap(_confidenceTexR8, _confidenceTexDebug);
                _debugViews.Confidence.texture = _confidenceTexDebug;
            }
        }

        private void ReprojectPointCloud()
        {
            if (_depthTextureFloat == null)
                return;

            var widthDepth = _depthTextureFloat.width;
            var heightDepth = _depthTextureFloat.height;
            var widthCamera = _cameraTexture.width;

            if (_cloud == null || _cloud.Length != widthDepth * heightDepth)
            {
                _cloud = new LidarPoint[widthDepth * heightDepth];

                _cameraManager.TryGetIntrinsics(out var intrinsic);

                var ratio = (float) widthDepth / (float) widthCamera;
                _fx = intrinsic.focalLength.x * ratio;
                _fy = intrinsic.focalLength.y * ratio;

                _cx = intrinsic.principalPoint.x * ratio;
                _cy = intrinsic.principalPoint.y * ratio;
            }

            var depthPixels = _depthTextureFloat.GetPixels();
            var confidenceMap = _confidenceTexR8.GetPixels32();

            for (var iY = 0; iY < heightDepth; iY++)
            {
                for (var iX = 0; iX < widthDepth; iX++)
                {
                    var iPoint = iY * widthDepth + iX;
                    _cloud[iPoint].Color = _cameraTexture.GetPixelBilinear((float) iX / (widthDepth),
                        (float) iY / (heightDepth));

                    var depth = depthPixels[iPoint].r;

                    _cloud[iPoint].IsValid = depth > Near && depth < Far && confidenceMap[iPoint].r == 2;
                    if (!_cloud[iPoint].IsValid)
                        continue;

                    _cloud[iPoint].PositionCameraSpace.x = -depth * (iX - _cx) / _fx;
                    _cloud[iPoint].PositionCameraSpace.y = -depth * (iY - _cy) / _fy;
                    _cloud[iPoint].PositionCameraSpace.z = depth;

                    var worldPosition = _cameraManager.transform.TransformPoint(_cloud[iPoint].PositionCameraSpace);
                    _cloud[iPoint].WorldPosition = worldPosition;
                    _cloud[iPoint].SnappedPositionWorld.x = Mathf.RoundToInt(100 * worldPosition.x);
                    _cloud[iPoint].SnappedPositionWorld.y = Mathf.RoundToInt(100 * worldPosition.y);
                    _cloud[iPoint].SnappedPositionWorld.z = Mathf.RoundToInt(100 * worldPosition.z);
                }
            }

            CloudUpdateEvent?.Invoke(_cloud);
        }
        
        private void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
        {
            if (Time.time - _lastTimeGenerated < 4f)
                return;
            _lastTimeGenerated = Time.time;

            UpdateCameraImage();
            UpdateEnvironmentDepthImage();
            UpdateEnvironmentConfidenceImage();
            ReprojectPointCloud();
        }

        #region Helpers

        private static void UpdateRawImage(Texture2D texture, XRCpuImage cpuImage)
        {
            var conversionParams = new XRCpuImage.ConversionParams(cpuImage, cpuImage.format.AsTextureFormat(),
                XRCpuImage.Transformation.MirrorY);
            var rawTextureData = texture.GetRawTextureData<byte>();

            Debug.Assert(
                rawTextureData.Length ==
                cpuImage.GetConvertedDataSize(conversionParams.outputDimensions, conversionParams.outputFormat),
                "The Texture2D is not the same size as the converted data.");

            cpuImage.Convert(conversionParams, rawTextureData);
            texture.Apply();
        }

        private static void ConvertFloatToGrayScale(Texture2D txFloat, Texture2D txGray)
        {
            var length = txGray.width * txGray.height;
            var depthPixels = txFloat.GetPixels();
            var colorPixels = txGray.GetPixels();

            for (var index = 0; index < length; index++)
            {
                var value = (depthPixels[index].r - Near) / (Far - Near);

                colorPixels[index].r = value;
                colorPixels[index].g = value;
                colorPixels[index].b = value;
                colorPixels[index].a = 1;
            }

            txGray.SetPixels(colorPixels);
            txGray.Apply();
        }

        private static void ConvertR8ToConfidenceMap(Texture2D txR8, Texture2D txRGBA)
        {
            var r8 = txR8.GetPixels32();
            var rgba = txRGBA.GetPixels32();
            for (var i = 0; i < r8.Length; i++)
            {
                switch (r8[i].r)
                {
                    case 0:
                        rgba[i].r = 255;
                        rgba[i].g = 0;
                        rgba[i].b = 0;
                        rgba[i].a = 255;
                        break;
                    case 1:
                        rgba[i].r = 0;
                        rgba[i].g = 255;
                        rgba[i].b = 0;
                        rgba[i].a = 255;
                        break;
                    case 2:
                        rgba[i].r = 0;
                        rgba[i].g = 0;
                        rgba[i].b = 255;
                        rgba[i].a = 255;
                        break;
                }
            }

            txRGBA.SetPixels32(rgba);
            txRGBA.Apply();
        }

        private static bool HasEqualDimensions(Texture tex, XRCpuImage image)
        {
            return tex.width == image.width || tex.height == image.height;
        }

        private static LidarPoint[] GenerateDebugCloud()
        {
            const int count = 10000;
            var cloud = new LidarPoint[count];
            for (var i = 0; i < count; i++)
            {
                var t = 1f * i / count;
                var randomPosition = 10 * Random.insideUnitSphere;
                cloud[i] = new LidarPoint()
                {
                    SnappedPositionWorld = new Vector3Int(Mathf.RoundToInt(randomPosition.x),
                        Mathf.RoundToInt(randomPosition.y),
                        Mathf.RoundToInt(randomPosition.z)),
                    WorldPosition = randomPosition,
                    PositionCameraSpace = randomPosition,
                    Color = Color.blue * (1 - t) + Color.green * t,
                    IsValid = true,
                };
            }

            return cloud;
        }

        #endregion
         
        [Serializable]
        public struct DebugViews
        {
            public RawImage Camera;
            public RawImage Depth;
            public RawImage Confidence;
        }

    }
}