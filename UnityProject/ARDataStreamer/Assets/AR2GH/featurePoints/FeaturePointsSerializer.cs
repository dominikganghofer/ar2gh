using System.Collections.Generic;
using UnityEngine;

namespace ar2gh.featurePoints
{
    /// <summary>
    /// Serializer for Colored Feature Point Cloud 
    /// </summary>
    public static class FeaturePointsSerializer
    {
        public static byte[] GeneratePointCloudData(List<FeaturePointsSender.FeaturePoint> updates,
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

        private static void WritePointCloudItem(FeaturePointsSender.FeaturePoint item, ref byte[] data,
            ref int dstOffSet)
        {
            SerializationHelper.WriteULong(item.ID, ref data, ref dstOffSet);
            SerializationHelper.WriteVector3AsMMInt(item.Position, ref data, ref dstOffSet);
            SerializationHelper.WriteColor(item.Color.Value, ref data, ref dstOffSet);
        }
    }
}