using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel ;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;

namespace AR2GH
{
    public class ReadPlaneComponent : GH_Component
    {
        public ReadPlaneComponent()
          : base("Read Planes", "Planes", "Read the planes detected in the enviroment.", "AR2GH", "Read")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("PlaneData", "P", "PlaneData", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("None", "None", "None", GH_ParamAccess.list);
            pManager.AddBrepParameter("Wall", "Wall", "Wall", GH_ParamAccess.list);
            pManager.AddBrepParameter("Floor", "Floor", "Floor", GH_ParamAccess.list);
            pManager.AddBrepParameter("Ceiling", "Ceiling", "Ceiling", GH_ParamAccess.list);
            pManager.AddBrepParameter("Table", "Table", "Table", GH_ParamAccess.list);
            pManager.AddBrepParameter("Seat", "Seat", "Seat", GH_ParamAccess.list);
            pManager.AddBrepParameter("Door", "Door", "Door", GH_ParamAccess.list);
            pManager.AddBrepParameter("Window", "Window", "Window", GH_ParamAccess.list);
        }

        private  Brep PlaneToBrep(Plane plane)
        {
            var boundary = plane.Boundary;
            if (boundary.Count == 0)
                return null;

            var polyline = Curve.CreateControlPointCurve(boundary, 1);
            var tolerance = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;

            var breps = Brep.CreatePlanarBreps(polyline, tolerance);
            if (breps == null)
                return null;

            if (breps.Length != 1)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Created more or less than one brep.");
            }
            return breps[0];
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var planeData = new List<Plane>();
            DA.GetDataList(0, planeData);

            var none = planeData.Where(p => p.PlaneClassification == Plane.Classification.None).Select(PlaneToBrep).ToList();
            var wall = planeData.Where(p => p.PlaneClassification == Plane.Classification.Wall).Select(PlaneToBrep).ToList();
            var floor = planeData.Where(p => p.PlaneClassification == Plane.Classification.Floor).Select(PlaneToBrep).ToList();
            var ceiling = planeData.Where(p => p.PlaneClassification == Plane.Classification.Ceiling).Select(PlaneToBrep).ToList();
            var table = planeData.Where(p => p.PlaneClassification == Plane.Classification.Table).Select(PlaneToBrep).ToList();
            var seat = planeData.Where(p => p.PlaneClassification == Plane.Classification.Seat).Select(PlaneToBrep).ToList();
            var door = planeData.Where(p => p.PlaneClassification == Plane.Classification.Door).Select(PlaneToBrep).ToList();
            var window = planeData.Where(p => p.PlaneClassification == Plane.Classification.Window).Select(PlaneToBrep).ToList();

            DA.SetDataList(0, none);
            DA.SetDataList(1, wall);
            DA.SetDataList(2, floor);
            DA.SetDataList(3, ceiling);
            DA.SetDataList(4, table);
            DA.SetDataList(5, seat);
            DA.SetDataList(6, door);
            DA.SetDataList(7, window);

        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => Properties.Resources.planes;

        public override Guid ComponentGuid => new Guid("252aba39-2df8-4133-811e-b684759f15b2");
    }
}
