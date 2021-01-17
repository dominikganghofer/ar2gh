using Rhino.Geometry;
using System.Collections.Generic;

namespace AR2GH.DataTypes
{
    /// <summary>
    /// Data structure for a human body that consists of a list of <see cref="Joint"/>s.
    /// </summary>
    public struct HumanBody
    {
        public ulong ID1 { get; internal set; }
        public ulong ID2 { get; internal set; }
        public List<Joint> joints;
    }

    /// <summary>
    /// Data structure the joint of a <see cref="HumanBody"/> with positon and rotation.
    /// </summary>
    public struct Joint
    {
        public int ID;
        public Point3d Position;
        public Point3d Rotation;
    }
}
