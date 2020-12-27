using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AR2GH.Parse
{
    /// <summary>
    /// Parse the AR-Foundation human body sceleton.
    /// </summary>
    class HumanBodyParser
    {
        public static void ParseHumanBodyUpdates(byte[] rawData, int startIndex, ref ConcurrentDictionary<ulong, HumanBody> receivedBodies)
        {
            var addedCount = ParserHelper.ToInt( rawData, ref startIndex);
            for (var i = 0; i < addedCount; i++)
            {
                var b  =ParseBody( rawData, ref startIndex);
                receivedBodies.AddOrUpdate(b.ID1, id => b, (id, existingBody) => b);
            }

            var removedCount = ParserHelper.ToInt( rawData, ref startIndex);
            for (var i = 0; i < removedCount; i++)
            {
                var b = ParseBody( rawData, ref startIndex);
                var bodyRemovedFromDictionary = new HumanBody();
                receivedBodies.TryRemove(b.ID1, out bodyRemovedFromDictionary);
            }

            var updatedCount = ParserHelper.ToInt( rawData, ref startIndex);
            for (var i = 0; i < updatedCount; i++)
            {
                var b = ParseBody( rawData, ref startIndex);
                receivedBodies.AddOrUpdate(b.ID1, id => b, (id, existingBody) => b);

            }
        }

        private static HumanBody ParseBody( byte[] rawData, ref int startIndex)
        {
            var body = new HumanBody();

            body.ID1 = ParserHelper.ToULong( rawData, ref startIndex);
            body.ID2 = ParserHelper.ToULong( rawData, ref startIndex);

            var jointCount = ParserHelper.ToInt( rawData, ref startIndex);
            body.joints = new List<Joint>();
            for (var iVertex = 0; iVertex < jointCount; iVertex++)
            {
                body.joints.Add(ParseJoint( rawData, ref startIndex));
            }

            return body;
        }

        private static Joint ParseJoint( byte[] rawData, ref int startIndex)
        {
            return new Joint
            {
                ID = ParserHelper.ToInt( rawData, ref startIndex),
                Position = ParserHelper.ToCartesianVectorMMPrecision( rawData, ref startIndex),
                Rotation = ParserHelper.ToCartesianVectorMMPrecision( rawData, ref startIndex),
            };
        }
    }
}
