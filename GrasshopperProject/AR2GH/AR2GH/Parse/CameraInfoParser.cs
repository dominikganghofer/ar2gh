using AR2GH.DataTypes;
using Rhino.Geometry;
using System.Collections.Concurrent;

namespace AR2GH.Parse
{
    /// <summary>
    /// Generates <see cref="CameraInfo"/> from a byte array.
    /// </summary>
    public static class CameraInfoParser
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
