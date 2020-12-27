using AR2GH.DataTypes;
using Rhino.Geometry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR2GH.Parse
{
    public class CameraInfoParser
    {
        public static void ParseCameraInfo(byte[] rawData, int startIndex, ref ConcurrentStack<CameraInfo> cameraInfo)
        {
            var info = new CameraInfo()
            {
                Position = ParserHelper.ToCartesianVectorFloat(rawData, ref startIndex),
                RotationEuler = new Vector3d(ParserHelper.ToDataVector(rawData, ref startIndex)),
                FoV = ParserHelper.ToFloat(rawData, ref startIndex),
            };
            
            cameraInfo.Push(info);
        }
    }

    
}
