using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace AR2GH
{
    public class AR2GHInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "AR2GH";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("1f56f50a-279b-4b5b-b746-0a456b93bad9");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
