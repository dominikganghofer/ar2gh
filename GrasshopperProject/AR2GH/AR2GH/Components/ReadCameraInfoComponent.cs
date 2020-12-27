using System;
using System.Collections.Generic;
using System.Linq;
using AR2GH.DataTypes;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;

namespace AR2GH
{
    public class ReadCameraInfo : GH_Component
    {
        public ReadCameraInfo()
          : base("Read Camera", "CameraInfo", "Read the camera transform detected in the enviroment.", "AR2GH", "Read")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("CameraInfo", "Cam", "Cam Info", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Position", "P", "Camera Position", GH_ParamAccess.item);
            pManager.AddVectorParameter("Rotation","R", "Rotaition in Euler Angles", GH_ParamAccess.item);
            pManager.AddNumberParameter("FoV", "FoV", "FoV", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var camInfo= new CameraInfo();
            DA.GetData(0,ref  camInfo );

            DA.SetData(0, camInfo.Position);
            DA.SetData(1, camInfo.RotationEuler);
            DA.SetData(2, camInfo.FoV);
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => Properties.Resources.cam;

        public override Guid ComponentGuid => new Guid("252aca39-2df8-4133-811e-b684759f15b2");
    }
}
