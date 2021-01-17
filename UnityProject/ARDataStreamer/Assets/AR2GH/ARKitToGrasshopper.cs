using System.Net;
using ar2gh.camera;
using ar2gh.humanBody;
using ar2gh.lidar;
using ar2gh.mesh;
using ar2gh.plane;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace ar2gh
{
    /// <summary>
    /// Main Controller. Registers at ARKit components and sends updates to Grasshopper via <see cref="UDPConnection"/>.
    /// Has a toggle to decide if the lidar point cloud or the environment mesh should be send. 
    /// </summary>
    public class ARKitToGrasshopper : MonoBehaviour
    {
        [Header("UI Components")] [SerializeField]
        private Toggle _lidarOrMeshToggle = default;

        [SerializeField] private TMP_InputField _receiverIPInputField = null;

        [SerializeField] private Button _connectButton = null;

        [Header("AR Components")] [SerializeField]
        private LidarPointCloud _lidarPointCloud = null;

        [SerializeField] private ARHumanBodyManager _humanBodyManager = null;

        [SerializeField] private ARCameraManager _cameraManager = null;

        [SerializeField] private ARMeshManager _meshManager = null;

        [SerializeField] private ARPlaneManager _planeManger = null;

        [FormerlySerializedAs("_pointCloudView")] [SerializeField] private LidarPointCloudView _lidarPointCloudView = null;

        [SerializeField] private MeshSender _meshSender = null;

        private readonly LidarPointCloudSerializer _lidarSerializer = new LidarPointCloudSerializer();

        private UDPConnection _udpConnection;

        private void Start()
        {
            _udpConnection = new UDPConnection();
            _udpConnection.StartSender(IPAddress.Parse(UDPConnection.DefaultReceiverIP));

            _connectButton.onClick.AddListener(ConnectButtonClickedHandler);
            _receiverIPInputField.text = IPAddress.Parse(UDPConnection.DefaultReceiverIP).ToString();

            // register to ar foundation components
            _meshManager.meshesChanged += MeshChangedHandler;
            _planeManger.planesChanged += PlanesChangedHandler;
            _lidarPointCloud.CloudUpdateEvent += SendLidarPointCloudUpdate;
            _meshSender.DataReadyEvent += _udpConnection.Send;

            // bandwidth is too small to send all data simultaneously
            // todo: add possibility to switch between the ar components
            //_cameraManager.frameReceived += CameraChangedHandler;
            //_humanBodyManager.humanBodiesChanged += HumanBodyChangedHandler;
        }

        private void CameraChangedHandler(ARCameraFrameEventArgs e)
        {
            var isCameraInitialized = e.projectionMatrix != null;
            var fov = isCameraInitialized
                ? 2 * Mathf.Atan(1f / e.projectionMatrix.Value.m11) * 180 / Mathf.PI
                : 0f;

            var data = CameraSerializer.SerializeCameraInfo(_cameraManager.transform, fov);
            _udpConnection.Send(data);
        }

        private void MeshChangedHandler(ARMeshesChangedEventArgs e)
        {
            if (!_lidarOrMeshToggle.isOn)
                return;

            _meshSender.SendUpdate(e.added, e.updated, e.removed);
        }

        private void SendLidarPointCloudUpdate(LidarPoint[] cloud)
        {
            if (_lidarOrMeshToggle.isOn)
                return;

            _lidarPointCloudView.RenderVertices(cloud);

            var data = _lidarSerializer.Serialize(cloud);
            foreach (var package in data)
            {
                _udpConnection.Send(package);
            }
        }

        private void ConnectButtonClickedHandler()
        {
            var ip = _receiverIPInputField.text;
            _udpConnection.StartSender(IPAddress.Parse(ip));
        }

        private void HumanBodyChangedHandler(ARHumanBodiesChangedEventArgs e)
        {
            var data = HumanBodySerializer.GenerateHumanBodyUpdateData(e);
            _udpConnection.Send(data);
        }

        private void PlanesChangedHandler(ARPlanesChangedEventArgs e)
        {
            var data = PlaneSerializer.GeneratePlaneData(e, _planeManger);
            _udpConnection.Send(data);
        }
    }
}