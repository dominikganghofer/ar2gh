using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ar2gh.lidar
{
    /// <summary>
    /// Serializes the colored lidar point cloud.
    /// </summary>
    public class LidarPointCloudSerializer
    {
        private readonly HashSet<Vector3Int> _sentPositions = new HashSet<Vector3Int>();
        private readonly LidarPointComparer _pointLidarComparer = new LidarPointComparer();

        private const float SendInterval = 1f;
        private float _lastTimeSent = float.NegativeInfinity;

        public IEnumerable<byte[]> Serialize(LidarPoint[] cloud)
        {
            if (Time.time - _lastTimeSent < SendInterval)
                return new byte[0][];
            _lastTimeSent = Time.time;

            var news = cloud.Distinct(_pointLidarComparer)
                .Where(cp => !_sentPositions.Contains(cp.SnappedPositionWorld)).ToArray();
            _sentPositions.UnionWith(news.Select(cp => cp.SnappedPositionWorld));

            Debug.Log($"Lidar Cloud: {news.Length} / {cloud.Length} points are send as updates.");

            const int packageSize = 110;
            var packageCount = Mathf.RoundToInt(news.Length / (1f * packageSize));
            var startIndex = 0;

            var data = new byte[packageCount][];
            for (var i = 0; i < packageCount; i++)
            {
                data[i] = GenerateLidarPointCloudData(news, startIndex, packageSize);
                startIndex += packageSize;
            }

            return data;
        }

        private static byte[] GenerateLidarPointCloudData(LidarPoint[] cloud, int startIndex, int count)
        {
            if (startIndex + count > cloud.Length)
                count = cloud.Length - startIndex;

            var streamSize = sizeof(byte);
            streamSize += sizeof(int); //pointCount
            streamSize += count * (sizeof(short) * 3 + sizeof(byte) * 3); //pointCloud

            var data = new byte[streamSize];
            var dstOffSet = 0;

            SerializationHelper.WriteStreamType(SerializationHelper.StreamType.LidarPointCloud, ref data,
                ref dstOffSet);

            SerializationHelper.WriteInt(cloud.Count(), ref data, ref dstOffSet);
            for (var i = startIndex; i < startIndex + count; i++)
            {
                SerializationHelper.WriteVector3Int(cloud[i].SnappedPositionWorld, ref data, ref dstOffSet);
                SerializationHelper.WriteColor(cloud[i].Color, ref data, ref dstOffSet);
            }

            return data;
        }

        /// <summary>
        /// Two <see cref="LidarPoint"/>s are equal if they have the same snapped position.
        /// </summary>
        private class LidarPointComparer : IEqualityComparer<LidarPoint>
        {
            public bool Equals(LidarPoint x, LidarPoint y)
            {
                return x.SnappedPositionWorld == y.SnappedPositionWorld;
            }

            public int GetHashCode(LidarPoint obj)
            {
                return obj.SnappedPositionWorld.GetHashCode();
            }
        }
    }
}