using Rhino.Geometry;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AR2GH.Parse
{
    public class PlaneParser
    {
        public static void ParsePlaneUpdates(byte[] rawData, int startIndex, ref ConcurrentDictionary<ulong, Plane> receivedPlanes)
        {
            var addedCount = ParserHelper.ToInt( rawData, ref startIndex);
            for (var iPlane = 0; iPlane < addedCount; iPlane++)
            {
                var addedPlane = ParsePlane(rawData, ref startIndex);
                receivedPlanes.AddOrUpdate(addedPlane.ID, id => addedPlane, (id, existingPlane) => addedPlane);
            }

            var removedCount = ParserHelper.ToInt(rawData, ref startIndex);
            for (var iPlane = 0; iPlane < removedCount; iPlane++)
            {
                var removedPlane = ParsePlane(rawData, ref startIndex);
                var planeRemovedFromDictionary = new Plane();
                receivedPlanes.TryRemove(removedPlane.ID, out planeRemovedFromDictionary);
            }

            var updatedCount = ParserHelper.ToInt(rawData, ref startIndex);
            for (var iPlane = 0; iPlane < updatedCount; iPlane++)
            {
                var updatedPlane = ParsePlane(rawData, ref startIndex);
                receivedPlanes.AddOrUpdate(updatedPlane.ID, id => updatedPlane, (id, existingPlane) => updatedPlane);
            }
        }

        private static Plane ParsePlane( byte[] rawData, ref int startIndex)
        {
            var plane = new Plane();

            plane.ID = ParserHelper.ToULong(rawData, ref startIndex);
            var vertexCount = ParserHelper.ToInt(rawData, ref startIndex);
            var vertices = new List<Point3d>();
            for (var iVertex = 0; iVertex < vertexCount; iVertex++)
            {
                vertices.Add(ParserHelper.ToCartesianVectorMMPrecision(rawData, ref startIndex));
            }

            // close loop in boundary
            vertices.Add(vertices[0]);
            plane.Boundary = vertices;
            plane.PlaneClassification = Plane.ClassificationFromInt(ParserHelper.ToInt(rawData, ref startIndex));
            plane.PlaneAlignment = Plane.AlignmentFromInt(ParserHelper.ToInt(rawData, ref startIndex));
            return plane;
        }

    }
}
