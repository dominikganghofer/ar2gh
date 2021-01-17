using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace AR2GH.DataTypes
{
    /// <summary>
    /// Data structure for a detected ar plane.
    /// </summary>
    public struct ARPlane
    {
        public ulong ID;
        public List<Point3d> Boundary;
        public Alignment PlaneAlignment;
        public Classification PlaneClassification;

        public enum Alignment
        {
            None, HorizontalUp, HorizontalDown, Vertical, NotAxisAligned
        }

        public static Alignment AlignmentFromInt(int value)
        {
            switch (value)
            {
                case 0:
                    return Alignment.None;
                case 1:
                    return Alignment.HorizontalUp;
                case 2:
                    return Alignment.HorizontalDown;
                case 3:
                    return Alignment.Vertical;
                case 4:
                    return Alignment.NotAxisAligned;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Alignment), value, null);
            }
        }

        public enum Classification
        {
            None, Wall, Floor, Ceiling, Table, Seat, Door, Window
        }

        public static Classification ClassificationFromInt(int value)
        {
            switch (value)
            {
                case 0:
                    return Classification.None;
                case 1:
                    return Classification.Wall;
                case 2:
                    return Classification.Floor;
                case 3:
                    return Classification.Ceiling;
                case 4:
                    return Classification.Table;
                case 5:
                    return Classification.Seat;
                case 6:
                    return Classification.Door;
                case 7:
                    return Classification.Window;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Classification), value, null);
            }
        }
    }
}
