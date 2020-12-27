using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ar2gh
{
    public static class PlaneSerializer
    {
        public static byte[] GeneratePlaneData(
            ARPlanesChangedEventArgs arPlanesChangedEventArgs,
            ARPlaneManager planeManager)
        {
            // preprocess planes
            var added = arPlanesChangedEventArgs.added
                .Select(a => SerializableARPlane.FromNativeARPlane(a, planeManager))
                .Where(plane => plane.HasValue)
                .Select(nullable => nullable.Value)
                .ToList();

            var removed = arPlanesChangedEventArgs.removed
                .Select(a => SerializableARPlane.FromNativeARPlane(a, planeManager))
                .Where(plane => plane.HasValue)
                .Select(nullable => nullable.Value)
                .ToList();

            var updated = arPlanesChangedEventArgs.updated
                .Select(a => SerializableARPlane.FromNativeARPlane(a, planeManager))
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

        private static int CalculateSizeForPlanes(List<SerializableARPlane> added)
        {
            var sizeForPlanes = 0;
            sizeForPlanes += sizeof(int); // planeCount
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

        private static void WritePlanes(List<SerializableARPlane> added, ref byte[] data, ref int dstOffSet)
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