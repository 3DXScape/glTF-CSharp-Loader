﻿using System.Numerics;

using GSR00;
using OpenTopo;
namespace XFormConsole
{
    internal class Program
    {
        // set name, valid date/time: start and end, topoFrame, and boundary

        // create relief

        // add water

        // add transportation

        static async Task Main(string[] args)
        {
            double lat = 50.936735;
            double lon = -1.470217;
            double height = 17.0;
            GSR00.Ellipsoid ellipsoid = new Ellipsoid(6378137.0, 1.0 / 298.257223563);
            GSR00.EPSG4327 tangentPoint = new EPSG4327(lat, lon, height);
            GSR00.TopocentricFrame topoFrame = new TopocentricFrame(ellipsoid, tangentPoint);

            // tangent point in ecef
            EPSG4978 ecefPosition;
            GSR00.TopocentricFrame.EPSG4327ToEPSG4978(ellipsoid, new EPSG4327(lat, lon, height), out ecefPosition);
            /*
             * 
             * X : 4026183  4026182.68 m 
             * Y : -103335  -103335.04 m
             * Z : 4929126  4929125.59 m
             * 
             */
            // test point in ecef
            double deltaLat = 0.00234 / 1.11111;
            double deltaLon = (0.00125 / Math.Cos(lat * TopocentricFrame.DegToRadians)) / 1.11111;
            double deltaH = 43.21;
            double deltaX, deltaY;
            double expectedX, expectedY, expectedZ;
            GSR00.EPSG4327 geoPosition = new EPSG4327(lat + deltaLat, lon + deltaLon, height + deltaH);
            GSR00.EPSG4978 geocentricPosition;
            GSR00.TopocentricFrame.EPSG4327ToEPSG4978(ellipsoid, geoPosition, out geocentricPosition);
            GSR00.EPSG4327 geoNewPosition;
            GSR00.TopocentricFrame.EPSG4978ToEPSG4327(ellipsoid, geocentricPosition, out geoNewPosition);
            // convert test point to enu
            EPSG4979 enuPosition;
            GSR00.TopocentricFrame.EPSG4978ToEPSG4979(topoFrame, ecefPosition, out enuPosition);
            // convert test point back to ecef
            EPSG4979 enuPositionClone = new EPSG4979(enuPosition);
            EPSG4978 ecefNewPosition;
            GSR00.TopocentricFrame.EPSG4979ToEPSG4978(topoFrame, enuPositionClone, out ecefNewPosition);
            // Convert test point back to EPSG4327
            GSR00.EPSG4327 geoNewNewPosition;
            GSR00.TopocentricFrame.EPSG4978ToEPSG4327(ellipsoid, ecefNewPosition, out geoNewNewPosition);
            expectedZ = deltaH;
            double xScale = 111556.57;
            double yScale = 111242.75;
            double bestXScale = xScale;
            double bestYScale = yScale;
            double aveDeltaX = 100.0;
            double aveDeltaY = 100.0;
            double aveDeltaH = 100.0;
            double totalDeltaX = 0.0;
            double totalDeltaY = 0.0;
            double totalDeltaH = 0.0;
            int nSamples = 0;
            int nIterations = 0;
            int nDeviations = 100000;
            int bestNDeviations = 100000;
            while (nIterations < 100 && nDeviations > 0)
            {
                var rand = new Random();
                nSamples = 0;
                totalDeltaX = 0.0;
                totalDeltaY = 0.0;
                totalDeltaH = 0.0;
                nDeviations = 0;
                // get random north
                for (nSamples = 0; nSamples < 100000; nSamples++)
                {
                    int mRow = rand.Next(1002) - 500;
                    deltaY = mRow;// * 10.0;
                    expectedY = deltaY;
                    deltaLat = deltaY / yScale;

                    // get random east

                    int mCol = rand.Next(1002) - 500;
                    deltaX = mCol;// * 10.0;
                    expectedX = deltaX;
                    deltaLon = (deltaX / Math.Cos(lat * TopocentricFrame.DegToRadians)) / xScale;
                    // convert test point to ECEF
                    EPSG4327 nextPosition = new EPSG4327(lat + deltaLat, lon + deltaLon, height + deltaH);
                    GSR00.TopocentricFrame.EPSG4327ToEPSG4978(ellipsoid, nextPosition, out ecefPosition);
                    // convert ECEF to ENU
                    GSR00.TopocentricFrame.EPSG4978ToEPSG4979(topoFrame, ecefPosition, out enuPosition);
                    totalDeltaX += (enuPosition.east - expectedX);
                    totalDeltaY += (enuPosition.north - expectedY);
                    totalDeltaH += (enuPosition.up - expectedZ);
                    double difX = Math.Sqrt((enuPosition.east - expectedX) * (enuPosition.east - expectedX));
                    double difY = Math.Sqrt((enuPosition.north - expectedY) * (enuPosition.north - expectedY));
                    double difH = 0.0;// Math.Sqrt((enuPosition.up - expectedZ) * (enuPosition.up - expectedZ));
                    double dif = Math.Sqrt((enuPosition.east - expectedX) * (enuPosition.east - expectedX) +
                        (enuPosition.north - expectedY) * (enuPosition.north - expectedY) /* +
                        (enuPosition.up - expectedZ) * (enuPosition.up - expectedZ) */ );

                    if (difX > 0.1 || difY > 0.1 || difH > 0.1 || dif > 0.1)
                    {
                        nDeviations++;
                    }
                }
                aveDeltaX = totalDeltaX / (double)nSamples;
                aveDeltaY = totalDeltaY / (double)nSamples;
                aveDeltaH = totalDeltaH / (double)nSamples;

                if (nDeviations < bestNDeviations)
                {
                    bestNDeviations = nDeviations;
                    bestXScale = xScale;
                    bestYScale = yScale;
                }
                else
                {
                    break;
                }

                xScale += aveDeltaX * 5.0;
                yScale += aveDeltaY * 5.0;
            }
            Elevation[] path = new Elevation[2];
            await OpenTopo.ElevationRequests.RequestElevationOnPath("eudem25m", path);
        }
    }
}