using System;
using System.Drawing;
using Rhino.Geometry;

namespace AR2GH.Parse
{
    /// <summary>
    /// A helperfor parsing different data types.
    /// </summary>
    public static class ParserHelper
    {
        public static StreamParser.StreamType ToStreamType(ref byte[] rawData, ref int startIndex)
        {
            var value = rawData[startIndex];
            startIndex += sizeof(byte);
            return (StreamParser.StreamType)value;
        }

        public static Color ToColor(byte[] data, ref int startIndex)
        {
            var r = ToByte(data, ref startIndex);
            var g = ToByte(data, ref startIndex);
            var b = ToByte(data, ref startIndex);
            return Color.FromArgb(255, r, g, b);
        }

        public static Color ToColorRGBA32(byte[] data, ref int startIndex)
        {
            var a = ToByte(data, ref startIndex);
            var r = ToByte(data, ref startIndex);
            var g = ToByte(data, ref startIndex);
            var b = ToByte(data, ref startIndex);
            return Color.FromArgb(a, r, g, b);
        }

        public static Point2d ToVector2D(byte[] data, ref int startIndex)
        {
            var x = ToFloat(data, ref startIndex);
            var y = ToFloat(data, ref startIndex);
            return new Point2d(x, y);
        }

        internal static Guid ToGuid(byte[] packageData, ref int startIndex)
        {
            var sizeOfByteArray = 16;
            byte[] guidBytes = new byte[sizeOfByteArray];

            Buffer.BlockCopy(packageData, startIndex, guidBytes, 0, sizeOfByteArray);
            startIndex += sizeOfByteArray;

            var value = new Guid(guidBytes);
            return value;
        }

        public static Point3d ToDataVector(byte[] data, ref int startIndex)
        {
            var x = ToFloat(data, ref startIndex);
            var y = ToFloat(data, ref startIndex);
            var z = ToFloat(data, ref startIndex);
            return new Point3d(x, y, z);
        }

        /// <summary>
        /// Read the data vector and flip y & z axes, to convert from Unity to Rhino coordinate system.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static Point3d ToCartesianVectorMMPrecision(byte[] data, ref int startIndex)
        {
            var x = ToShort(data, ref startIndex) * 0.001f;
            var z = ToShort(data, ref startIndex) * 0.001f;
            var y = ToShort(data, ref startIndex) * 0.001f;
            return new Point3d(x, y, z);
        }

        /// <summary>
        /// Read the data vector and flip y & z axes, to convert from Unity to Rhino coordinate system.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static Point3d ToCartesianVectorFloat(byte[] data, ref int startIndex)
        {
            var x = ToFloat(data, ref startIndex);
            var z = ToFloat(data, ref startIndex);
            var y = ToFloat(data, ref startIndex);
            return new Point3d(x, y, z);
        }

        /// <summary>
        /// Read the data vector and flip y & z axes, to convert from Unity to Rhino coordinate system.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static Point3f ToCartesianVectorCMPrecision(byte[] data, ref int startIndex)
        {
            var x = ToShort(data, ref startIndex) * 0.01f;
            var z = ToShort(data, ref startIndex) * 0.01f;
            var y = ToShort(data, ref startIndex) * 0.01f;
            return new Point3f(x, y, z);
        }



        public unsafe static short ToShort(byte[] data, ref int startIndex)
        {
            var value = BitConverter.ToInt16(data, startIndex);
            startIndex += sizeof(short);
            return value;
        }


        public unsafe static float ToFloat(byte[] data, ref int startIndex)
        {
            var value = BitConverter.ToSingle(data, startIndex);
            startIndex += sizeof(float);
            return value;

            // todo: implement faster alternative to BitConverter.
            /*fixed (byte* pbyte = &data[startIndex])
            {
                // assumes data is aligned 
                //  (*pbyte) | (*(pbyte + 1) << 8) | (*(pbyte + 2) << 16) | (*(pbyte + 3) << 24);
                //(*pbyte << 24) | (*(pbyte + 1) << 16) | (*(pbyte + 2) << 8) | (*(pbyte + 3));
                int val = *((int*)pbyte);
                startIndex += sizeof(float);
                return *(float*)&val;
            }*/
        }

        public static byte ToByte(byte[] data, ref int startIndex)
        {
            var value = data[startIndex];
            startIndex += sizeof(byte);
            return value;
        }

        public static ulong ToULong(byte[] data, ref int startIndex)
        {
            var value = BitConverter.ToUInt64(data, startIndex);
            startIndex += sizeof(ulong);
            return value;
        }

        public static int ToInt(byte[] data, ref int startIndex)
        {
            var value = BitConverter.ToInt32(data, startIndex);
            startIndex += sizeof(int);
            return value;
        }
    }
}
