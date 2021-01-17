using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR.ARFoundation;

namespace ar2gh.plane
{
    /// <summary>
    /// Serializes detected planes
    /// </summary>
    public static class PlaneSerializer
    {
        public static byte[] GeneratePlaneData(
            ARPlanesChangedEventArgs arPlanesChangedEventArgs,
            ARPlaneManager planeManager)
        {
            // preprocess planes
            var added = arPlanesChangedEventArgs.added
                .Select(a => PlaneData.FromNativeARPlane(a, planeManager))
                .Where(plane => plane.HasValue)
                .Select(nullable => nullable.Value)
                .ToList();

            var removed = arPlanesChangedEventArgs.removed
                .Select(a => PlaneData.FromNativeARPlane(a, planeManager))
                .Where(plane => plane.HasValue)
                .Select(nullable => nullable.Value)
                .ToList();

            var updated = arPlanesChangedEventArgs.updated
                .Select(a => PlaneData.FromNativeARPlane(a, planeManager))
                .Where(plane => plane.HasValue)
                .Select(nullable => nullable.Value)
                .ToList();

            // calculate stream size
            var streamSize = sizeof(byte); // streamType
            streamSize += CalculateSizeForPlanes(added);
            streamSize += CalculateSizeForPlanes(removed);
            streamSize += CalculateSizeForPlanes(updated);

            var data = new byte[streamSize];
            var dstOffSet = 0;

            SerializationHelper.WriteStreamType(SerializationHelper.StreamType.Planes, ref data, ref dstOffSet);

            WritePlanes(added, ref data, ref dstOffSet);
            WritePlanes(removed, ref data, ref dstOffSet);
            WritePlanes(updated, ref data, ref dstOffSet);

            return data;
        }

        private static int CalculateSizeForPlanes(List<PlaneData> added)
        {
            var sizeForPlanes = sizeof(int); // planeCount
            foreach (var p in added)
            {
                sizeForPlanes += sizeof(ulong); // planeID
                sizeForPlanes += sizeof(int); // boundaryCount
                sizeForPlanes += 3 * sizeof(float) * p.BoundaryWorld.Count; // boundaryCount
                sizeForPlanes += sizeof(int); // plane classification
                sizeForPlanes += sizeof(int); // plane alignment
            }

            return sizeForPlanes;
        }

        private static void WritePlanes(List<PlaneData> added, ref byte[] data, ref int dstOffSet)
        {
            SerializationHelper.WriteInt(added.Count, ref data, ref dstOffSet);
            foreach (var a in added)
            {
                SerializationHelper.WriteULong(a.ID, ref data, ref dstOffSet);
                SerializationHelper.WriteInt(a.BoundaryWorld.Count, ref data, ref dstOffSet);
                foreach (var p in a.BoundaryWorld)
                {
                    SerializationHelper.WriteVector3AsMMInt(p, ref data, ref dstOffSet);
                }

                SerializationHelper.WriteInt(a.PlaneClassification, ref data, ref dstOffSet);
                SerializationHelper.WriteInt(a.PlaneAlignment, ref data, ref dstOffSet);
            }
        }
    }
}