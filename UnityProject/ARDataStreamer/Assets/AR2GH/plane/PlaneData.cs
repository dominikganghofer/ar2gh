using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ar2gh.plane
{
    /// <summary>
    /// The relevant data for a detected plane.
    /// </summary>
    public struct PlaneData
    {
        public ulong ID;
        public List<Vector3> BoundaryWorld;
        public int PlaneClassification;
        public int PlaneAlignment;

        public static PlaneData? FromNativeARPlane(ARPlane nativePlane, ARPlaneManager planeManager)
        {
            var planeTransform = planeManager.GetPlane(nativePlane.trackableId)?.transform;
            if (planeTransform == null)
                return null;

            var worldPoints = nativePlane.boundary
                .Select(b => planeTransform.TransformPoint(new Vector3(b.x, 0f, b.y))).ToList();

            return new PlaneData()
            {
                ID = nativePlane.trackableId.subId1,
                BoundaryWorld = worldPoints,
                PlaneClassification = ClassificationToInt(nativePlane.classification),
                PlaneAlignment = AlignmentToInt(nativePlane.alignment),
            };
        }

        #region helpers

        private static int ClassificationToInt(PlaneClassification classification)
        {
            switch (classification)
            {
                case UnityEngine.XR.ARSubsystems.PlaneClassification.None:
                    return 0;
                case UnityEngine.XR.ARSubsystems.PlaneClassification.Wall:
                    return 1;
                case UnityEngine.XR.ARSubsystems.PlaneClassification.Floor:
                    return 2;
                case UnityEngine.XR.ARSubsystems.PlaneClassification.Ceiling:
                    return 3;
                case UnityEngine.XR.ARSubsystems.PlaneClassification.Table:
                    return 4;
                case UnityEngine.XR.ARSubsystems.PlaneClassification.Seat:
                    return 5;
                case UnityEngine.XR.ARSubsystems.PlaneClassification.Door:
                    return 6;
                case UnityEngine.XR.ARSubsystems.PlaneClassification.Window:
                    return 7;
                default:
                    throw new ArgumentOutOfRangeException(nameof(classification), classification, null);
            }
        }

        private static int AlignmentToInt(PlaneAlignment alignment)
        {
            switch (alignment)
            {
                case UnityEngine.XR.ARSubsystems.PlaneAlignment.None:
                    return 0;
                case UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp:
                    return 1;
                case UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalDown:
                    return 2;
                case UnityEngine.XR.ARSubsystems.PlaneAlignment.Vertical:
                    return 3;
                case UnityEngine.XR.ARSubsystems.PlaneAlignment.NotAxisAligned:
                    return 4;
                default:
                    throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null);
            }
        }

        #endregion
    }
}