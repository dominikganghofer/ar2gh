using System;
using AR2GH.DataTypes;
using Grasshopper.Kernel;

namespace AR2GH.Components
{
    /// <summary>
    /// Translates the received ar camera info into GH data types.
    /// </summary>
    public class ReadCameraInfo : GH_Component
    {
        public ReadCameraInfo()
          : base("Read Camera", "CameraInfo", "Read the ar camera transform.", "AR2GH", "Read")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("CameraInfo", "Cam", "Cam Info", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Position", "P", "Camera Position", GH_ParamAccess.item);
            pManager.AddVectorParameter("Rotation", "R", "Rotaition in Euler Angles", GH_ParamAccess.item);
            pManager.AddNumberParameter("FoV", "FoV", "FoV", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var camInfo = new CameraInfo();
            DA.GetData(0, ref camInfo);

            DA.SetData(0, camInfo.Position);
            DA.SetData(1, camInfo.RotationEuler);
            DA.SetData(2, camInfo.FoV);
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        public override Guid ComponentGuid => new Guid("252aca39-2df8-4133-811e-b684759f15b2");

        protected override System.Drawing.Bitmap Icon => Properties.Resources.cam;
    }
}
