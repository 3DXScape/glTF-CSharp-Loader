using System;
using System.Diagnostics;

namespace LTP_ENU
{
    // This is an implementation of EPSG method 9837 using sections 4.1.1 and 4.1.2 of https://www.iogp.org/wp-content/uploads/2019/09/373-07-02.pdf.
    public class LTP_ENU
    {
        // WGS-84 geodetic constants
        const double a = 6378137.0;         // WGS-84 Earth semimajor axis (m)

        const double b = 6356752.314245;     // Derived Earth semiminor axis (m)
        const double f = (a - b) / a;           // Ellipsoid Flatness
        const double f_inv = 1.0 / f;       // Inverse flattening

        //const double f_inv = 298.257223563; // WGS-84 Flattening Factor of the Earth 
        //const double b = a - a / f_inv;
        //const double f = 1.0 / f_inv;

        const double a_sq = a * a;
        const double b_sq = b * b;
        const double e_sq = f * (2 - f);    // Square of Eccentricity

        // Converts WGS-84 Geodetic point (lat, lon, h) to the 
        // Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z).
        public static void GeodeticToEcef(double lat, double lon, double h,
                                            out double x, out double y, out double z)
        {
            // Convert to radians in notation consistent with the paper:
            var lambda = DegreesToRadians(lat);
            var phi = DegreesToRadians(lon);
            var s = Math.Sin(lambda);
            var N = a / Math.Sqrt(1 - e_sq * s * s);

            var sin_lambda = Math.Sin(lambda);
            var cos_lambda = Math.Cos(lambda);
            var cos_phi = Math.Cos(phi);
            var sin_phi = Math.Sin(phi);

            x = (h + N) * cos_lambda * cos_phi;
            y = (h + N) * cos_lambda * sin_phi;
            z = (h + (1 - e_sq) * N) * sin_lambda;
        }

        // Converts the Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z) to 
        // (WGS-84) Geodetic point (lat, lon, h).
        public static void EcefToGeodetic(double x, double y, double z,
                                            out double lat, out double lon, out double h)
        {
            var eps = e_sq / (1.0 - e_sq);
            var p = Math.Sqrt(x * x + y * y);
            var q = Math.Atan2((z * a), (p * b));
            var sin_q = Math.Sin(q);
            var cos_q = Math.Cos(q);
            var sin_q_3 = sin_q * sin_q * sin_q;
            var cos_q_3 = cos_q * cos_q * cos_q;
            var phi = Math.Atan2((z + eps * b * sin_q_3), (p - e_sq * a * cos_q_3));
            var lambda = Math.Atan2(y, x);
            var v = a / Math.Sqrt(1.0 - e_sq * Math.Sin(phi) * Math.Sin(phi));
            h = (p / Math.Cos(phi)) - v;

            lat = RadiansToDegrees(phi);
            lon = RadiansToDegrees(lambda);
        }

        // Converts the Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z) to 
        // East-North-Up coordinates in a Local Tangent Plane that is centered at the 
        // (WGS-84) Geodetic point (lat0, lon0, h0).
        public static void EcefToEnu(double x, double y, double z,
                                        double lat0, double lon0, double h0,
                                        out double xEast, out double yNorth, out double zUp)
        {
            // Convert to radians in notation consistent with the paper:
            var lambda = DegreesToRadians(lat0);
            var phi = DegreesToRadians(lon0);
            var s = Math.Sin(lambda);
            var N = a / Math.Sqrt(1 - e_sq * s * s);

            var sin_lambda = Math.Sin(lambda);
            var cos_lambda = Math.Cos(lambda);
            var cos_phi = Math.Cos(phi);
            var sin_phi = Math.Sin(phi);

            double x0 = (h0 + N) * cos_lambda * cos_phi;
            double y0 = (h0 + N) * cos_lambda * sin_phi;
            double z0 = (h0 + (1 - e_sq) * N) * sin_lambda;

            double xd, yd, zd;
            xd = x - x0;
            yd = y - y0;
            zd = z - z0;

            // This is the matrix multiplication
            xEast = -sin_phi * xd + cos_phi * yd;
            yNorth = -cos_phi * sin_lambda * xd - sin_lambda * sin_phi * yd + cos_lambda * zd;
            zUp = cos_lambda * cos_phi * xd + cos_lambda * sin_phi * yd + sin_lambda * zd;
        }

        // Inverse of EcefToEnu. Converts East-North-Up coordinates (xEast, yNorth, zUp) in a
        // Local Tangent Plane that is centered at the (WGS-84) Geodetic point (lat0, lon0, h0)
        // to the Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z).
        public static void EnuToEcef(double xEast, double yNorth, double zUp,
                                        double lat0, double lon0, double h0,
                                        out double x, out double y, out double z)
        {
            // Convert to radians in notation consistent with the paper:
            var lambda = DegreesToRadians(lat0);
            var phi = DegreesToRadians(lon0);
            var s = Math.Sin(lambda);
            var N = a / Math.Sqrt(1 - e_sq * s * s);

            var sin_lambda = Math.Sin(lambda);
            var cos_lambda = Math.Cos(lambda);
            var cos_phi = Math.Cos(phi);
            var sin_phi = Math.Sin(phi);

            double x0 = (h0 + N) * cos_lambda * cos_phi;
            double y0 = (h0 + N) * cos_lambda * sin_phi;
            double z0 = (h0 + (1 - e_sq) * N) * sin_lambda;

            double xd = -sin_phi * xEast - cos_phi * sin_lambda * yNorth + cos_lambda * cos_phi * zUp;
            double yd = cos_phi * xEast - sin_lambda * sin_phi * yNorth + cos_lambda * sin_phi * zUp;
            double zd = cos_lambda * yNorth + sin_lambda * zUp;

            x = xd + x0;
            y = yd + y0;
            z = zd + z0;
        }

        // Converts the geodetic WGS-84 coordinated (lat, lon, h) to 
        // East-North-Up coordinates in a Local Tangent Plane that is centered at the 
        // (WGS-84) Geodetic point (lat0, lon0, h0).
        public static void GeodeticToEnu(double lat, double lon, double h,
                                            double lat0, double lon0, double h0,
                                            out double xEast, out double yNorth, out double zUp)
        {
            double x, y, z;
            GeodeticToEcef(lat, lon, h, out x, out y, out z);
            EcefToEnu(x, y, z, lat0, lon0, h0, out xEast, out yNorth, out zUp);
        }
        public static void EnuToGeodetic(double xEast, double yNorth, double zUp,
                                             double lat0, double lon0, double h0,
                                            out double lat, out double lon, out double h
                                            )
        {
            double x, y, z;
            EnuToEcef(xEast, yNorth, zUp, lat0, lon0, h0, out x, out y, out z);
            EcefToGeodetic(x, y, z, out lat, out lon, out h);
        }

        public static void Test()
        {
            var latLA = 34.00000048;
            var lonLA = -117.3335693;
            var hLA = 251.702;

            double x0, y0, z0;
            GeodeticToEcef(latLA, lonLA, hLA, out x0, out y0, out z0);

            System.Diagnostics.Debug.Assert(AreClose(-2430601.8, x0));
            Debug.Assert(AreClose(-4702442.7, y0));
            Debug.Assert(AreClose(3546587.4, z0));

            // Checks to read out the matrix entries, to compare to the paper
            double x, y, z;
            double xEast, yNorth, zUp;

            // First column
            x = x0 + 1;
            y = y0;
            z = z0;
            EcefToEnu(x, y, z, latLA, lonLA, hLA, out xEast, out yNorth, out zUp);
            Debug.Assert(AreClose(0.88834836, xEast));
            Debug.Assert(AreClose(0.25676467, yNorth));
            Debug.Assert(AreClose(-0.38066927, zUp));

            x = x0;
            y = y0 + 1;
            z = z0;
            EcefToEnu(x, y, z, latLA, lonLA, hLA, out xEast, out yNorth, out zUp);
            Debug.Assert(AreClose(-0.45917011, xEast));
            Debug.Assert(AreClose(0.49675810, yNorth));
            Debug.Assert(AreClose(-0.73647416, zUp));

            x = x0;
            y = y0;
            z = z0 + 1;
            EcefToEnu(x, y, z, latLA, lonLA, hLA, out xEast, out yNorth, out zUp);
            Debug.Assert(AreClose(0.00000000, xEast));
            Debug.Assert(AreClose(0.82903757, yNorth));
            Debug.Assert(AreClose(0.55919291, zUp));

        }

        public static void Test2()
        {
            var latLA = 34.00000048;
            var lonLA = -117.3335693;
            var hLA = 251.702;

            double x0, y0, z0;
            GeodeticToEcef(latLA, lonLA, hLA, out x0, out y0, out z0);

            Debug.Assert(AreClose(-2430601.8, x0));
            Debug.Assert(AreClose(-4702442.7, y0));
            Debug.Assert(AreClose(3546587.4, z0));

            EcefToEnu(x0, y0, z0, latLA, lonLA, hLA, out double xEast, out double yNorth, out double zUp);

            Debug.Assert(AreClose(0, xEast));
            Debug.Assert(AreClose(0, yNorth));
            Debug.Assert(AreClose(0, zUp));

            EnuToEcef(xEast, yNorth, zUp, latLA, lonLA, hLA, out double xTest, out double yTest, out double zTest);
            EcefToGeodetic(xTest, yTest, zTest, out double latTest, out double lonTest, out double hTest);

            Debug.Assert(AreClose(latLA, latTest));
            Debug.Assert(AreClose(lonLA, lonTest));
            Debug.Assert(AreClose(hTest, hLA));

        }

        static bool AreClose(double x0, double x1)
        {
            var d = x1 - x0;
            return (d * d) < 0.1;
        }


        static double DegreesToRadians(double degrees)
        {
            return Math.PI / 180.0 * degrees;
        }

        static double RadiansToDegrees(double radians)
        {
            return 180.0 / Math.PI * radians;
        }
    }
}
