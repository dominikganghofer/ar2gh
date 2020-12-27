using System.Net;
using ar2gh;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ARKitToGrasshopper : MonoBehaviour
{
    [SerializeField]
    private Toggle _lidarOrMeshToggle = default;

    [SerializeField]
    private TMP_InputField _receiverIPInputField = null;

    [SerializeField]
    private PointCloudView _pointCloudView = null;

    [SerializeField]
    private Button _connectButton = null;

    [SerializeField]
    private LidarPointCloud _lidarPointCloud = null;

    [SerializeField]
    private ARHumanBodyManager _humanBodyManager = null;

    [SerializeField]
    private ARCameraManager _cameraManager = null;

    [SerializeField]
    private ARMeshManager _meshManager = null;

    [SerializeField]
    private ARPlaneManager _planeManger = null;

//    [SerializeField]
//    private ARPointCloudManager _pointCloudManager = null;
//
//    [SerializeField]
//    private PointCloudSender _pointCloudSender = null;

    [SerializeField]
    private MeshSender _meshSender = null;

    private readonly LidarPointCloudSerializer _lidarSerializer = new LidarPointCloudSerializer();

    private UDPConnection _udpConnection;

    private void Start()
    {
        _udpConnection = new UDPConnection();
        _udpConnection.StartSender(IPAddress.Parse(UDPConnection.DefaultReceiverIP));

        _connectButton.onClick.AddListener(ConnectButtonClickedHandler);
        _receiverIPInputField.text = IPAddress.Parse(UDPConnection.DefaultReceiverIP).ToString();

        // register to ar foundation components
        _lidarPointCloud.CloudUpdateEvent += SendLidarPointCloudUpdate;
        _meshManager.meshesChanged += MeshChangedHandler;
        _planeManger.planesChanged += PlanesChangedHandler;
        //_cameraManager.frameReceived += CameraChangedHandler;
        _meshSender.DataReadyEvent += _udpConnection.Send;

        // human bodies cannot be detected simultaneously with the depth map. todo: add feature switch 
        // _humanBodyManager.humanBodiesChanged += HumanBodyChangedHandler;

        // sending of feature points is replaced with lidar point cloud
        //_pointCloudManager.pointCloudsChanged += _pointCloudSender.PointCloudChangedHandler;
        //_pointCloudSender.PackageReadyEvent += Send;
    }

    private void CameraChangedHandler(ARCameraFrameEventArgs e)
    {
        var fov = e.projectionMatrix != null
            ? 2 * Mathf.Atan(1f / e.projectionMatrix.Value.m11) * 180 / Mathf.PI
            : 0f;

        var data = CameraSerializer.SerializeCameraInfo(_cameraManager.transform, fov);
        _udpConnection.Send(data);
    }

    private void MeshChangedHandler(ARMeshesChangedEventArgs obj)
    {
        if (!_lidarOrMeshToggle.isOn)
            return;

        _meshSender.MeshesChangedHandler(obj);
    }

    private void SendLidarPointCloudUpdate(LidarPointCloud.CloudPoint[] cloud)
    {
        if (_lidarOrMeshToggle.isOn)
            return;

        _pointCloudView.RenderVertices(cloud);
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