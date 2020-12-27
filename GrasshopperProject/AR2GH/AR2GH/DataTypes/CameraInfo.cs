using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AR2GH.DataTypes
{
    public class CameraInfo// : IGH_Goo, IGH_PreviewData
    {
        public Point3d Position;
        public Vector3d RotationEuler;
        public float FoV;

        public BoundingBox ClippingBox => new BoundingBox(new List<Point3d>() { Position });

        public bool IsValid => true;

        public string IsValidWhyNot => "Is always valid";

        public string TypeName => "Cam Info";

        public string TypeDescription => "Pos, Rot and FoV";

        public bool CastFrom(object source) => false;

        public bool CastTo<T>(out T target)
        {
            target = default;
            return false;
        }

      //  public IGH_Goo Duplicate() => new CameraInfo() { Position = Position, RotationEuler = RotationEuler, FoV = FoV };

        public IGH_GooProxy EmitProxy() => null;

        public bool Read(GH_IReader reader) => false;

        public object ScriptVariable() => null;

        public bool Write(GH_IWriter writer) => false;


        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
            DrawFrustum((line, c) => args.Pipeline.DrawLine(line, c));
        }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            DrawFrustum((line, c) => args.Pipeline.DrawLine(line, c));
        }

        private void DrawFrustum(Action<Line, Color> draw)
        {
            var size = 0.4;
            var w = size * Math.Tan(FoV / 2f);
            Color c;
            Transform t;

            var z = Math.PI / 180 * RotationEuler.X;
            var x = Math.PI / 180 * RotationEuler.Y;
            var y = Math.PI / 180 * RotationEuler.Z;

            c =Color.FromArgb(100, Color.White);
            t = Transform.RotationZYX(-x, y, z);
            Draw(c, t, Position, w, size, draw);


        }

        static void Draw(Color c, Transform t, Point3d pos, double w, double size, Action<Line, Color> draw)
        {
            var p0 = pos + t * new Point3d(w, size, w);
            var p1 = pos + t * new Point3d(-w, size ,w);
            var p2 = pos + t * new Point3d(w, size ,- w);
            var p3 = pos + t * new Point3d(-w, size, - w);

            draw(new Line(p0, p1), c);
            draw(new Line(p1, p3), c);
            draw(new Line(p2, p3), c);
            draw(new Line(p2, p0), c);

            draw(new Line(pos, p0), c);
            draw(new Line(pos, p1), c);
            draw(new Line(pos, p2), c);
            draw(new Line(pos, p3), c);

        }
    }
}
