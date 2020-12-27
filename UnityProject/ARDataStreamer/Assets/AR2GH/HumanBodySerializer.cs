using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ar2gh
{
    public class HumanBodySerializer
    {
        public static byte[] GenerateHumanBodyUpdateData(ARHumanBodiesChangedEventArgs e)
        {
            var streamSize = sizeof(byte); // streamType
            streamSize += CalculateSizeForBodyUpdates(e.added);
            streamSize += CalculateSizeForBodyUpdates(e.removed);
            streamSize += CalculateSizeForBodyUpdates(e.updated);

            var data = new byte[streamSize];
            var dstOffSet = 0;
            
            SerializationHelper.WriteStreamType(SerializationHelper.StreamType.HumanBody, ref data, ref dstOffSet);
            WriteHumanBodyUpdates(e.added, ref data, ref dstOffSet);
            WriteHumanBodyUpdates(e.removed, ref data, ref dstOffSet);
            WriteHumanBodyUpdates(e.updated, ref data, ref dstOffSet);

            return data;
        }

        private static int CalculateSizeForBodyUpdates(List<ARHumanBody> updates)
        {
            var size = 0;
            size += sizeof(int); //body count
            foreach (var b in updates)
            {
                size += sizeof(ulong); //id1
                size += sizeof(ulong); //id2
                size += sizeof(int); //joint count

                foreach (var j in b.joints)
                {
                    size += sizeof(int); //id
                    size += 3 * sizeof(float); //pos
                    size += 3 * sizeof(float); //rot
                }
            }

            return size;
        }

        private static void WriteHumanBodyUpdates(List<ARHumanBody> bodies, ref byte[] data, ref int dstOffSet)
        {
            SerializationHelper.WriteInt(bodies.Count, ref data, ref dstOffSet);
            foreach (var b in bodies)
            {
                SerializationHelper.WriteULong(b.trackableId.subId1, ref data, ref dstOffSet);
                SerializationHelper.WriteULong(b.trackableId.subId2, ref data, ref dstOffSet);
                SerializationHelper.WriteInt(b.joints.Length, ref data, ref dstOffSet);

                foreach (var j in b.joints)
                {
                    var pos = b.transform.TransformPoint(j.anchorPose.position);
                    var rot = b.transform.rotation * j.anchorPose.rotation;

                    SerializationHelper.WriteInt(j.index, ref data, ref dstOffSet);
                    SerializationHelper.WriteVector3AsMMInt(pos, ref data, ref dstOffSet);
                    SerializationHelper.WriteVector3AsMMInt(rot.eulerAngles, ref data, ref dstOffSet);
                }
            }
        }
    }
}