using System.Collections.Generic;
using System.Linq;
using ar2gh;
using UnityEngine;

    public class LidarPointCloudSerializer
    {
        private const float SendInterval = 1f;

        private readonly HashSet<Vector3Int> _sentPositions = new HashSet<Vector3Int>();
        private float _lastTimeSent = float.NegativeInfinity;
        private readonly CloudPointComparer _pointCloudComparer = new CloudPointComparer();

        public byte[][] Serialize(LidarPointCloud.CloudPoint[] cloud)
        {
            if (Time.time - _lastTimeSent < SendInterval)
                return new byte[0][];
            _lastTimeSent = Time.time;

            var news = cloud.Distinct(_pointCloudComparer)
                .Where(cp => !_sentPositions.Contains(cp.SnappedPositionWorld)).ToArray();
            _sentPositions.UnionWith(news.Select(cp => cp.SnappedPositionWorld));

            Debug.Log($"Lidar Cloud: {news.Length} / {cloud.Length} points are send as updates.");

            const int packageSize = 110;
            var packageCount = Mathf.RoundToInt(news.Length / (1f * packageSize));
            var startIndex = 0;

            var data = new byte[packageCount][];
            for (var i = 0; i < packageCount; i++)
            {
                data[i] = PointCloudSerializer.GenerateLidarPointCloudData(news, startIndex, packageSize);
                startIndex += packageSize;
            }

            return data;
        }

        private class CloudPointComparer : IEqualityComparer<LidarPointCloud.CloudPoint>
        {
            public bool Equals(LidarPointCloud.CloudPoint x, LidarPointCloud.CloudPoint y)
            {
                return x.SnappedPositionWorld == y.SnappedPositionWorld;
            }

            public int GetHashCode(LidarPointCloud.CloudPoint obj)
            {
                return obj.SnappedPositionWorld.GetHashCode();
            }
        }
    }
