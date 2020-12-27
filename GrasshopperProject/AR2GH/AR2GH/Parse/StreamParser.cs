using AR2GH.DataTypes;
using Rhino.Geometry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AR2GH.Parse
{
    /// <summary>
    /// Parse UDP byte packages depending on its <see cref="StreamType"/>. 
    /// </summary>
    public class StreamParser
    {
        public enum StreamType { CameraInfo = 0, Planes = 1, HumanBody = 2, Mesh = 3, LidarPointCloud = 4 }

        public ConcurrentPointCloud ReceivedPointCloud = new ConcurrentPointCloud();
        public ConcurrentDictionary<ulong, Plane> ReceivedPlanes = new ConcurrentDictionary<ulong, Plane>();
        public ConcurrentDictionary<ulong, HumanBody> ReceivedBodies = new ConcurrentDictionary<ulong, HumanBody>();
        public ConcurrentDictionary<int, Mesh> ReceivedMeshes = new ConcurrentDictionary<int, Mesh>();
        public ConcurrentQueue<(Point3d, Color)> LidarPointCloud = new ConcurrentQueue<(Point3d, Color)>();
        public ConcurrentStack<CameraInfo> CameraInfo = new ConcurrentStack<CameraInfo>();
        private Dictionary<Guid, Message> _receivedMessages = new Dictionary<Guid, Message>();

        /// <summary>
        /// Add raw data package, that was received via the network. If a message consists of multiple chunks, those are stored
        /// in <see cref="_receivedMessages"/> and parsed when all chunks were received.
        /// </summary>
        /// <param name="packageData"></param>
        public void AddUDPPackage(byte[] packageData)
        {
            var startIndex = 0;

            // read header
            var messageID = ParserHelper.ToGuid(packageData, ref startIndex);
            var chunkID = ParserHelper.ToInt(packageData, ref startIndex);
            var chunkCount = ParserHelper.ToInt(packageData, ref startIndex);
            var chunkBodySize = ParserHelper.ToInt(packageData, ref startIndex);

            // get data package
            var chunkBody = new byte[chunkBodySize];
            Buffer.BlockCopy(packageData, startIndex, chunkBody, 0, chunkBodySize);

            if (!_receivedMessages.ContainsKey(messageID))
                _receivedMessages.Add(messageID, new Message(chunkCount));

            _receivedMessages[messageID].AddChunk(chunkID, chunkBody);

            if (_receivedMessages[messageID].HasAllChunks())
            {
                var completedMessage = _receivedMessages[messageID].GetMessage();
                _receivedMessages.Remove(messageID);
                ParseCompleteMessage(completedMessage);
            }
        }

        /// <summary>
        /// Parse message according to its <see cref="StreamType"/>.
        /// </summary>
        /// <param name="rawData"></param>
        private void ParseCompleteMessage(byte[] rawData)
        {
            var startIndex = 0;
            var streamType = ParserHelper.ToStreamType(ref rawData, ref startIndex);

            switch (streamType)
            {
                case StreamType.CameraInfo:
                    CameraInfoParser.ParseCameraInfo(rawData, startIndex, ref CameraInfo);
                    break;
                case StreamType.Planes:
                    PlaneParser.ParsePlaneUpdates(rawData, startIndex, ref ReceivedPlanes);
                    break;
                case StreamType.HumanBody:
                    HumanBodyParser.ParseHumanBodyUpdates(rawData, startIndex, ref ReceivedBodies);
                    break;
                case StreamType.Mesh:
                    MeshParser.ParseMeshUpdates(rawData, startIndex, ref ReceivedMeshes);
                    break;
                case StreamType.LidarPointCloud:
                    PointCloudParser.ParseDepthMapPointCloud(rawData, startIndex, ref LidarPointCloud);
                    break;
            }
        }

        /// <summary>
        /// Stored the data chunks that make up a message received via network.
        /// </summary>
        private class Message
        {
            private int _chunkCount;
            private readonly List<byte[]> _chunks;

            public Message(int chunkCount)
            {
                _chunkCount = chunkCount;
                _chunks = new List<byte[]>(chunkCount);
            }

            public void AddChunk(int id, byte[] data)
            {
                _chunks.Insert(id, data);
            }

            public bool HasAllChunks()
            {
                return _chunks.Count == _chunkCount;
            }

            public byte[] GetMessage()
            {
                var messageLength = _chunks.Select(c => c.Length).Sum();
                var message = new byte[messageLength];
                var dstOffset = 0;

                foreach (var c in _chunks)
                {
                    Buffer.BlockCopy(c, 0, message, dstOffset, c.Length);
                    dstOffset += c.Length;
                }

                return message.ToArray();
            }
        }

    }
}
