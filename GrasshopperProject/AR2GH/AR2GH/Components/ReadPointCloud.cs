using System;
using Grasshopper.Kernel;

namespace AR2GH.Components
{
    /// <summary>
    /// Translates the point cloud detected in the enviroment into GH data types.
    /// </summary>
    public class ReadPointCloud : GH_Component
    {
        public ReadPointCloud()
          : base("Read Point Cloud", "Point Cloud", "Read the point cloud detected in the enviroment.", "AR2GH", "Read")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Point Cloud Data", "C", "Point cloud data from device", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "P", "Points of the cloud", GH_ParamAccess.list);
            pManager.AddColourParameter("Colors", "C", "Colors of the cloud", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ConcurrentPointCloud pointCloud = new ConcurrentPointCloud();
            DA.GetData(0,ref pointCloud);

            DA.SetDataList(0, pointCloud.Points.Values);
            DA.SetDataList(1, pointCloud.Colors.Values);
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => Properties.Resources.cloud;

        public override Guid ComponentGuid => new Guid("152aba39-2df8-4133-111e-b684749f15b2");
    }
}
