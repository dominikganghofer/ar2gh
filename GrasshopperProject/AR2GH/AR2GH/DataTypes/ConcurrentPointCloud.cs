using Rhino.Geometry;
using System.Collections.Concurrent;
using System.Drawing;

namespace AR2GH.DataTypes
{
    /// <summary>
    /// Concurrent storage of a point cloud. This is necessary for the asynchronous retrieval and rendering of the point cloud.
    /// </summary>
    public class ConcurrentPointCloud
    {
        public ConcurrentDictionary<ulong, Point3d> Points = new ConcurrentDictionary<ulong, Point3d>();
        public ConcurrentDictionary<ulong, Color> Colors = new ConcurrentDictionary<ulong, Color>();
    }
}
