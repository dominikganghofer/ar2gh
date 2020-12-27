using Rhino.Geometry;
using System.Collections.Generic;

namespace AR2GH
{
    public struct HumanBody
    {
        public ulong ID1 { get; internal set; }
        public ulong ID2 { get; internal set; }
        public List<Joint> joints;
    }

    public struct Joint
    {
        public int ID;
        public Point3d Position;
        public Point3d Rotation;
    }
}
