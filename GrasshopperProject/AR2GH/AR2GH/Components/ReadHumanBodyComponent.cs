using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using System;
using System.Collections.Generic;

namespace AR2GH.Components
{
    /// <summary>
    /// Translates the received human bodies into GH data types.
    /// </summary>
    public class ReadHumanBodyComponent : GH_Component
    {
        public ReadHumanBodyComponent()
          : base("Read Human Body", "Human Body", "Read the humans detected in the enviroment.", "AR2GH", "Read")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("HumanBodyData", "B", "Input of the human body data, received by the device.", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Skeleton", "S", "The whole skeleton as a list of vertices.", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var bodyData = new List<HumanBody>();
            DA.GetDataList(0, bodyData);

            var jointTree = new DataTree<System.Object>();
            for (var i = 0; i < bodyData.Count; i++)
            {
                var body = bodyData[i];
                var path = new GH_Path(i);
                for (var j = 0; j < body.joints.Count; j++)
                {
                    jointTree.Insert(body.joints[j].Position, path, j);
                }
            }

            DA.SetDataTree(0, jointTree);
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => Properties.Resources.users;

        public override Guid ComponentGuid => new Guid("352aba39-2df8-4133-911e-b684749f15b2");
    }
}


