using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ar2gh
{
    public class PointCloudSerializer
    {
        public static byte[] GenerateLidarPointCloudData(LidarPointCloud.CloudPoint[] cloud, int startIndex, int count)
        {
            if (startIndex + count > cloud.Length)
                count = cloud.Length - startIndex;

            var streamSize = sizeof(byte);
            streamSize += sizeof(int); //pointCount
            streamSize += count * (sizeof(short) * 3 + sizeof(byte) * 3); //pointCloud

            var data = new byte[streamSize];
            var dstOffSet = 0;

            SerializationHelper.WriteStreamType(SerializationHelper.StreamType.LidarPointCloud, ref data,
                ref dstOffSet);

            SerializationHelper.WriteInt(cloud.Count(), ref data, ref dstOffSet);
            for (var i = startIndex; i < startIndex + count; i++)
            {
                SerializationHelper.WriteVector3Int(cloud[i].SnappedPositionWorld, ref data, ref dstOffSet);
                SerializationHelper.WriteColor(cloud[i].Color, ref data, ref dstOffSet);
            }

            return data;
        }

        public static byte[] GeneratePointCloudData(List<PointCloudSender.PointCloudEntry> updates,
                                                    Vector3 camPosition,
                                                    Vector3 camRotationEulerAngles,
                                                    Vector3 mousePosition,
                                                    Vector2 touchDelta
        )
        {
            var streamSize = sizeof(byte);
            streamSize += 6 * sizeof(float); //camPos + camRot
            streamSize += 4 * sizeof(float); //mousePos + touchDelta
            streamSize += sizeof(int); //pointCount
            streamSize += updates.Count * (sizeof(float) * 6 + sizeof(ulong)); //pointCloud

            var data = new byte[streamSize];
            var dstOffSet = 0;

            //   SerializationHelper.WriteStreamType(SerializationHelper.StreamType.CameraInfo, ref data, ref dstOffSet);
            //todo: create new enum item, if this method will ever be used again 
            
            // write input state
            SerializationHelper.WriteVector3AsMMInt(camPosition, ref data, ref dstOffSet);
            SerializationHelper.WriteVector3AsMMInt(camRotationEulerAngles, ref data, ref dstOffSet);
            SerializationHelper.WriteVector2(mousePosition, ref data, ref dstOffSet);
            SerializationHelper.WriteVector2(touchDelta, ref data, ref dstOffSet);

            // write updates

            SerializationHelper.WriteInt(updates.Count, ref data, ref dstOffSet);
            foreach (var u in updates)
            {
                WritePointCloudItem(u, ref data, ref dstOffSet);
            }

            return data;
        }

        private static void WritePointCloudItem(PointCloudSender.PointCloudEntry item, ref byte[] data,
                                                ref int dstOffSet)
        {
            SerializationHelper.WriteULong(item.ID, ref data, ref dstOffSet);
            SerializationHelper.WriteVector3AsMMInt(item.Position, ref data, ref dstOffSet);
            SerializationHelper.WriteColor(item.Color.Value, ref data, ref dstOffSet);
        }

    }
}