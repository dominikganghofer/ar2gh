using Rhino.Geometry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;  
using System.Threading.Tasks;

namespace AR2GH
{
    public class ConcurrentPointCloud
    {
        public ConcurrentDictionary<ulong, Point3d> Points = new ConcurrentDictionary<ulong, Point3d>();
        public ConcurrentDictionary<ulong, Color> Colors = new ConcurrentDictionary<ulong, Color>();
    }

}
