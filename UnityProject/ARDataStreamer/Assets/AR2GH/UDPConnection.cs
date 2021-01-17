using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace ar2gh
{
    public class UDPConnection
    {
        private const int Port = 8888;
        public const string DefaultReceiverIP = "192.168.0.233";

        public void StartSender(IPAddress ip)
        {
            _remoteEndPoint = new IPEndPoint(ip, Port);
            _client = new UdpClient {Client = {SendBufferSize = 64000}};
        }

        public void Send(byte[] data)
        {
            void SendPackage(byte[] chunkData)
            {
                try
                {
                    _client.Send(chunkData, chunkData.Length, _remoteEndPoint);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.ToString());
                }
            }

            var chunks = GenerateUDPPackages(data);
            foreach (var p in chunks)
            {
                SendPackage(p);
            }
        }

        private List<byte[]> GenerateUDPPackages(byte[] data)
        {
            var messageID = Guid.NewGuid();
            var chunkID = 0;
            var chunks = new List<byte[]>();

            var headerSize = messageID.ToByteArray().Length; // messageID
            headerSize += sizeof(int); // chunkID
            headerSize += sizeof(int); // chunkCount
            headerSize += sizeof(int); // chunkBodySize

            var maxBodySize = MaxPackageSize - headerSize;
            var chunkCount = Mathf.CeilToInt(data.Length / (maxBodySize * 1f));

            var offsetInSrc = 0;
            while (offsetInSrc < data.Length)
            {
                // generate new chunk
                var chunkData = new byte[MaxPackageSize];
                var dstOffSet = 0;
                var chunkBodySize = Mathf.Min(maxBodySize, data.Length - offsetInSrc);

                // write header
                SerializationHelper.WriteGuid(messageID, ref chunkData, ref dstOffSet);
                SerializationHelper.WriteInt(chunkID, ref chunkData, ref dstOffSet);
                SerializationHelper.WriteInt(chunkCount, ref chunkData, ref dstOffSet);
                SerializationHelper.WriteInt(chunkBodySize, ref chunkData, ref dstOffSet);

                //write body
                Buffer.BlockCopy(data, offsetInSrc, chunkData, dstOffSet, chunkBodySize);

                offsetInSrc += chunkBodySize;
                chunkID++;

                chunks.Add(chunkData);
            }

            return chunks;
        }

        private IPEndPoint _remoteEndPoint;
        private UdpClient _client;
        private const int MaxPackageSize = 1500;
    }
}