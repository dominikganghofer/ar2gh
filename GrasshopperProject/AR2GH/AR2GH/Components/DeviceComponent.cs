using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using AR2GH.Parse;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace AR2GH
{
    public class DeviceComponent : GH_Component
    {
        private Thread _receiveThread;
        private UdpClient _client;
        private int _port = 8888;
        private StreamParser _receivedData = new StreamParser();

        public DeviceComponent()
          : base("IPadConnection", "IPad", "Receives Data Stream from AR Device via udp.", "AR2GH", "iPad")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("CloudData", "C", "Removed Mesh Patches", GH_ParamAccess.item);
            pManager.AddGenericParameter("PlaneData", "P", "Received Planes", GH_ParamAccess.list);
            pManager.AddGenericParameter("BodyData", "B", "Received Human Bodies", GH_ParamAccess.list);
            pManager.AddGenericParameter("Mesh", "M", "Received Enviroment Mesh", GH_ParamAccess.list);
           // pManager.AddPointParameter("CamPosition", "CP", "Received Cam Position", GH_ParamAccess.item);
            // pManager.AddGenericParameter("CameraInfo", "Cam", "Received Enviroment Mesh", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var hasNotBeenInitialized = _receiveThread == null;
            if (hasNotBeenInitialized)
                StartReceiver();

            if (_receivedData == null)
                return;


            while (!_receivedData.LidarPointCloud.IsEmpty)
            {
                if (_receivedData.LidarPointCloud.TryDequeue(out var item))
                    _pc.Add(item.Item1, item.Item2);
            }
            DA.SetData(0, _pc);
            DA.SetDataList(1, _receivedData.ReceivedPlanes.Values);
            DA.SetDataList(2, _receivedData.ReceivedBodies.Values);
            DA.SetDataList(3, _receivedData.ReceivedMeshes.Values);



        //    if (_receivedData.CameraInfo.TryPop(out var cameraInfo))
          //      DA.SetData(4, cameraInfo.Position);
            //     DA.SetData(4, cameraInfo);

            //clean up stack
            //_receivedData.CameraInfo.Clear();
        }

        private PointCloud _pc = new PointCloud();
        private void StartReceiver()
        {
            _receiveThread = new Thread(new ThreadStart(ReceiveData)) { IsBackground = true };
            _receiveThread.Start();
            _client = new UdpClient(_port);
            _client.Client.ReceiveBufferSize = 655360;
        }

        private void ReceiveData()
        {
            while (true)
            {
                try
                {
                    var anyIp = new IPEndPoint(IPAddress.Any, 0);
                    var data = _client.Receive(ref anyIp);
                    _receivedData.AddUDPPackage(data);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.ToString());
                }
            }
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.udp;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("152aba39-2df8-4133-811e-b684749f15b2"); }
        }
    }
}
