using System;
using Rhino;
using Rhino.Commands;

namespace AR2GH
{
    public class GH_ConcurrentPointCloud : Command
    {
        static GH_ConcurrentPointCloud _instance;
        public GH_ConcurrentPointCloud()
        {
            _instance = this;
        }

        ///<summary>The only instance of the GH_PointCloud command.</summary>
        public static GH_ConcurrentPointCloud Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "GH_PointCloud"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: complete command.
            return Result.Success;
        }
    }
}