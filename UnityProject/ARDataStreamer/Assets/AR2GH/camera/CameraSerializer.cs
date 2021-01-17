using UnityEngine;

namespace ar2gh.camera
{
    /// <summary>
    /// Serialize info on the ar camera
    /// </summary>
    public static class CameraSerializer
    {
        public static byte[] SerializeCameraInfo(Transform camTransform, float fov)
        {
            var streamSize = sizeof(byte); // streamType
            streamSize += 3 * sizeof(float); //position
            streamSize += 3 * sizeof(float); //rotation
            streamSize += sizeof(float); //fov

            var data = new byte[streamSize];
            var dstOffSet = 0;

            SerializationHelper.WriteStreamType(SerializationHelper.StreamType.CameraInfo, ref data, ref dstOffSet);
            SerializationHelper.WriteVector3(camTransform.position, ref data, ref dstOffSet);
            SerializationHelper.WriteVector3(camTransform.rotation.eulerAngles, ref data, ref dstOffSet);
            SerializationHelper.WriteFloat(fov, ref data, ref dstOffSet);
            
            return data;
        }
    }
}