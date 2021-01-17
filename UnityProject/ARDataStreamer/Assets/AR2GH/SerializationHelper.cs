using System;
using UnityEngine;

namespace ar2gh
{
    public static class SerializationHelper
    {
        public enum StreamType
        {
            CameraInfo = 0,
            Planes = 1,
            HumanBody = 2,
            Mesh = 3,
            LidarPointCloud = 4,
        }

        public static void WriteVector3(Vector3 item, ref byte[] data, ref int dstOffSet)
        {
            WriteFloat(item.x, ref data, ref dstOffSet);
            WriteFloat(item.y, ref data, ref dstOffSet);
            WriteFloat(item.z, ref data, ref dstOffSet);
        }

        public static void WriteVector3Int(Vector3Int item, ref byte[] data, ref int dstOffSet)
        {
            WriteShort(IntToShort(item.x), ref data, ref dstOffSet);
            WriteShort(IntToShort(item.y), ref data, ref dstOffSet);
            WriteShort(IntToShort(item.z), ref data, ref dstOffSet);
        }

        public static void WriteVector3AsMMInt(Vector3 item, ref byte[] data, ref int dstOffSet)
        {
            WriteShort(IntToShort(FloatToMMInt(item.x)), ref data, ref dstOffSet);
            WriteShort(IntToShort(FloatToMMInt(item.y)), ref data, ref dstOffSet);
            WriteShort(IntToShort(FloatToMMInt(item.z)), ref data, ref dstOffSet);
        }

        private static int FloatToMMInt(float value)
        {
            return Mathf.RoundToInt(value * 1000f);
        }

        private static short IntToShort(int value)
        {
            if (value > short.MaxValue)
            {
                Debug.Log($"{value} > {short.MaxValue}");
                value = short.MaxValue;
            }

            if (value < short.MinValue)
            {
                Debug.Log($"{value} < {short.MinValue}");
                value = short.MinValue;
            }

            return Convert.ToInt16(value);
        }

        public static void WriteColor(Color item, ref byte[] data, ref int dstOffSet)
        {
            WriteByte(ColorFloatToByte(item.r), ref data, ref dstOffSet);
            WriteByte(ColorFloatToByte(item.g), ref data, ref dstOffSet);
            WriteByte(ColorFloatToByte(item.b), ref data, ref dstOffSet);
        }

        private static byte ColorFloatToByte(float value)
        {
            var asInt = Mathf.RoundToInt(value * 255);
            var asByte = Convert.ToByte(asInt);
            //   Debug.Log($"{value}=>int{asInt}=>byte{asByte}");
            return asByte;
        }

        public static void WriteNullableColor32(Color? item, ref byte[] data, ref int dstOffSet)
        {
            if (item.HasValue)
            {
                var color = item.Value;
                WriteByte(255, ref data, ref dstOffSet);
                WriteByte((byte) (color.r * 255), ref data, ref dstOffSet);
                WriteByte((byte) (color.g * 255), ref data, ref dstOffSet);
                WriteByte((byte) (color.b * 255), ref data, ref dstOffSet);
            }
            else
            {
                WriteByte(0, ref data, ref dstOffSet);
                WriteByte(0, ref data, ref dstOffSet);
                WriteByte(0, ref data, ref dstOffSet);
                WriteByte(0, ref data, ref dstOffSet);
            }
        }

        private static void WriteByte(byte item, ref byte[] data, ref int dstOffSet)
        {
            data[dstOffSet] = item;
            dstOffSet += sizeof(byte);
        }

        public static void WriteVector2(Vector2 item, ref byte[] data, ref int dstOffSet)
        {
            WriteFloat(item.x, ref data, ref dstOffSet);
            WriteFloat(item.y, ref data, ref dstOffSet);
        }

        public static void WriteFloat(float item, ref byte[] data, ref int dstOffSet)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(item), 0, data, dstOffSet, sizeof(float));
            dstOffSet += sizeof(float);
        }

        public static void WriteULong(ulong item, ref byte[] data, ref int dstOffSet)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(item), 0, data, dstOffSet, sizeof(ulong));
            dstOffSet += sizeof(ulong);
        }

        public static void WriteInt(int item, ref byte[] data, ref int dstOffSet)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(item), 0, data, dstOffSet, sizeof(int));
            dstOffSet += sizeof(int);
        }

        public static void WriteShort(short item, ref byte[] data, ref int dstOffSet)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(item), 0, data, dstOffSet, sizeof(short));
            dstOffSet += sizeof(short);
        }

        public static void WriteStreamType(StreamType item, ref byte[] data, ref int dstOffSet)
        {
            Buffer.BlockCopy(BitConverter.GetBytes((byte) item), 0, data, dstOffSet, sizeof(byte));
            dstOffSet += sizeof(byte);
        }

        public static void WriteGuid(Guid item, ref byte[] data, ref int dstOffSet)
        {
            var size = item.ToByteArray().Length;
            Buffer.BlockCopy(item.ToByteArray(), 0, data, dstOffSet, size);
            dstOffSet += size;
        }
    }
}