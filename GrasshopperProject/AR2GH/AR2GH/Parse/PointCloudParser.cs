using AR2GH.DataTypes;
using Rhino.Geometry;
using System.Collections.Concurrent;
using System.Drawing;

namespace AR2GH.Parse
{
    /// <summary>
    /// Generates entries of <see cref="ConcurrentPointCloud"/> from a byte array.
    /// </summary>
    public static class PointCloudParser
    {
        /// <summary>
        /// Parse the AR-Foundation feature point cloud. 
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="startIndex"></param>
        /// <param name="receivedPointCloud"></param>
        public static void ParseFeaturePointCloud(byte[] rawData, int startIndex, ref ConcurrentPointCloud receivedPointCloud)
        {
            // read header
            var camPosition = ParserHelper.ToCartesianVectorMMPrecision(rawData, ref startIndex);
            var camRotationEulerAngles = ParserHelper.ToCartesianVectorMMPrecision(rawData, ref startIndex);
            var touchPosition = ParserHelper.ToVector2D(rawData, ref startIndex);
            var touchDelta = ParserHelper.ToVector2D(rawData, ref startIndex);
            var pointCount = ParserHelper.ToInt(rawData, ref startIndex);

            //read points
            for (var i = 0; i < pointCount; i++)
            {
                var id = ParserHelper.ToULong(rawData, ref startIndex);
                var p = ParserHelper.ToCartesianVectorMMPrecision(rawData, ref startIndex);
                var color = ParserHelper.ToColor(rawData, ref startIndex);
                if (!receivedPointCloud.Points.ContainsKey(id))
                    receivedPointCloud.Points.TryAdd(id, p);
                else
                    receivedPointCloud.Points[id] = p;

                if (!receivedPointCloud.Colors.ContainsKey(id))
                    receivedPointCloud.Colors.TryAdd(id, color);
                else
                    receivedPointCloud.Colors[id] = color;
            }
        }

        /// <summary>
        /// Parse the point cloud created via the AR-Foundation depth map.  
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="startIndex"></param>
        /// <param name="lidarPointCloud"></param>
        public static void ParseDepthMapPointCloud(
            byte[] rawData,
            int startIndex,
            ref ConcurrentQueue<(Point3d, Color)> lidarPointCloud)
        {
            var pointCount = ParserHelper.ToInt(rawData, ref startIndex);
            for (var i = 0; i < pointCount; i++)
            {
                var p = ParserHelper.ToCartesianVectorCMPrecision(rawData, ref startIndex);
                var color = ParserHelper.ToColor(rawData, ref startIndex);
                lidarPointCloud.Enqueue((p, color));
            }
        }
    }
}
