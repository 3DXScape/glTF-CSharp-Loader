using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ProjNet;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using GeoAPI.CoordinateSystems.Transformations;

namespace ProjNetConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CoordinateSystemFactory csFactory = new CoordinateSystemFactory();
            const string sourceCsWkt = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]]";
            CoordinateSystem sourceCs = csFactory.CreateFromWkt(sourceCsWkt);
            ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory ctfac = new CoordinateTransformationFactory();


            var fac = new CoordinateSystemFactory();
            var wkt = "PROJCS[\"OSGB 1936 / British National Grid\"," +
                 "GEOGCS[\"OSGB 1936\"," +
                 "DATUM[\"OSGB_1936\"," +
                     "SPHEROID[\"Airy 1830\",6377563.396,299.3249646,AUTHORITY[\"EPSG\",\"7001\"]]," +
                     "AUTHORITY[\"EPSG\",\"6277\"]]," +
                     "PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]]," +
                     "UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]]," +
                     "AUTHORITY[\"EPSG\",\"4277\"]]," +
                 "UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]]," +
                 "PROJECTION[\"Transverse_Mercator\"]," +
                 "PARAMETER[\"latitude_of_origin\",49]," +
                 "PARAMETER[\"central_meridian\",-2]," +
                 "PARAMETER[\"scale_factor\",0.9996012717]," +
                 "PARAMETER[\"false_easting\",400000]," +
                 "PARAMETER[\"false_northing\",-100000]," +
                 "AUTHORITY[\"EPSG\",\"27700\"]," +
                 "AXIS[\"Easting\",EAST]," +
                 "AXIS[\"Northing\",NORTH]]";
            ProjNet.CoordinateSystems.ProjectedCoordinateSystem pcs = fac.CreateFromWkt(wkt) as ProjectedCoordinateSystem;


            //ProjectedCoordinateSystem pcs = CoordinateSystemWktReader.Parse(wkt) as ProjectedCoordinateSystem;
            //ProjNet.CoordinateSystems.Transformations ntfac = new CoordinateTransformationFactory();
            string WKT = "PROJCRS[\"EPSG topocentric example A\"," +
            "    BASEGEOGCRS[\"WGS 84\"," +
            "        DATUM[\"World Geodetic System 1984\"," +
            "            ELLIPSOID[\"WGS 84\",6378137,298.257223563," +
            "                LENGTHUNIT[\"metre\",1]]]," +
            "        PRIMEM[\"Greenwich\",0," +
            "            ANGLEUNIT[\"degree\",0.0174532925199433]]," +
"        ID[\"EPSG\",4979]]," +
"    CONVERSION[\"EPSG topocentric example A\"," +
"        METHOD[\"Geographic/topocentric conversions\"," +
"            ID[\"EPSG\",9837]]," +
"        PARAMETER[\"Latitude of topocentric origin\",55," +
"            ANGLEUNIT[\"degree\",0.0174532925199433]," +
"            ID[\"EPSG\",8834]]," +
"        PARAMETER[\"Longitude of topocentric origin\",5," +
"            ANGLEUNIT[\"degree\",0.0174532925199433]," +
"            ID[\"EPSG\",8835]]," +
"        PARAMETER[\"Ellipsoidal height of topocentric origin\",0," +
"            LENGTHUNIT[\"metre\",1]," +
"            ID[\"EPSG\",8836]]]," +
"    CS[Cartesian,3]," +
"        AXIS[\"topocentric East (U)\",east," +
"            ORDER[1]," +
"            LENGTHUNIT[\"metre\",1]]," +
"        AXIS[\"topocentric North (V)\",north," +
"            ORDER[2]," +
"            LENGTHUNIT[\"metre\",1]]," +
"        AXIS[\"topocentric height (W)\",up," +
"            ORDER[3]," +
"            LENGTHUNIT[\"metre\",1]]," +
"    USAGE[" +
"        SCOPE[\"unknown\"]," +
"        AREA[\"To be specified\"]," +
"        BBOX[-90,-180,90,180]]," +
"    ID[\"EPSG\",5819]]";
            //pcs = fac.CreateFromWkt(WKT) as ProjectedCoordinateSystem;

            var from = ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84;
            var to = ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WGS84_UTM(30, true);
            var too = GeocentricCoordinateSystem.WGS84;

            // convert points from one coordinate system to another
            ProjNet.CoordinateSystems.Transformations.ICoordinateTransformation trans = ctfac.CreateFromCoordinateSystems(from, too);

            ProjNet.Geometries.XYZ businessCoordinate = new ProjNet.Geometries.XYZ(-0.127758, 51.50735, 10.0);
            ProjNet.Geometries.XYZ searchLocationCoordinate = new ProjNet.Geometries.XYZ(-0.142500, 51.539188, 10.0);

            MathTransform mathTransform = trans.MathTransform;
            var businessLocation = mathTransform.Transform(businessCoordinate.X, businessCoordinate.Y, businessCoordinate.Z);
            var searchLocation = mathTransform.Transform(searchLocationCoordinate.X, searchLocationCoordinate.Y, searchLocationCoordinate.Z);

            double[] xyz = new double[3] { -1.471256, 50.937951, 10.0 };
            //double[] xyz = new double[3] { 51.50735, -0.127758, 10.0 };
            var cf = new CoordinateSystemFactory();
            var f = new CoordinateTransformationFactory();

            string utm30N = "PROJCS[\"WGS 84 / UTM zone 30N\"," +
                " GEOGCS[\"WGS 84\"," +
                " DATUM[\"WGS_1984\"," +
                " SPHEROID[\"WGS 84\",6378137,298.257223563," +
                " AUTHORITY[\"EPSG\",\"7030\"]]," +
                " AUTHORITY[\"EPSG\",\"6326\"]]," +
                " PRIMEM[\"Greenwich\",0," +
                " AUTHORITY[\"EPSG\",\"8901\"]]," +
                " UNIT[\"degree\",0.0174532925199433," +
                " AUTHORITY[\"EPSG\",\"9122\"]]," +
                " AUTHORITY[\"EPSG\",\"4326\"]]," +
                " PROJECTION[\"Transverse_Mercator\"]," +
                " PARAMETER[\"latitude_of_origin\",0]," +
                " PARAMETER[\"central_meridian\",-3]," +
                " PARAMETER[\"scale_factor\",0.9996]," +
                " PARAMETER[\"false_easting\",500000]," +
                " PARAMETER[\"false_northing\",0]," +
                " UNIT[\"metre\",1," +
                " AUTHORITY[\"EPSG\",\"9001\"]]," +
                " AXIS[\"Easting\",EAST]," +
                " AXIS[\"Northing\",NORTH]," +
                " AUTHORITY[\"EPSG\",\"32630\"]]";
            string wkt4326 = "GEOGCS[\"WGS 84\"," +
                " DATUM[\"WGS_1984\"," +
                " SPHEROID[\"WGS 84\",6378137,298.257223563," +
                " AUTHORITY[\"EPSG\",\"7030\"]]," +
                " AUTHORITY[\"EPSG\",\"6326\"]]," +
                " PRIMEM[\"Greenwich\",0," +
                " AUTHORITY[\"EPSG\",\"8901\"]]," +
                " UNIT[\"degree\",0.0174532925199433," +
                " AUTHORITY[\"EPSG\",\"9122\"]]," +
                " AUTHORITY[\"EPSG\",\"4326\"]]";
            string wkt5819 = "PROJCRS[\"EPSG topocentric example A\"," +
                "   BASEGEOGCRS[\"WGS 84\"," +
                "     DATUM[\"World Geodetic System 1984\"," +
                "       ELLIPSOID[\"WGS 84\",6378137,298.257223563," +
                "       LENGTHUNIT[\"metre\",1]]]," +
                "   PRIMEM[\"Greenwich\",0," +
                "     ANGLEUNIT[\"degree\",0.0174532925199433]]," +
                "     ID[\"EPSG\",4979]]," +
                "   CONVERSION[\"EPSG topocentric example A\"," +
                "     METHOD[\"Geographic/topocentric conversions\"," +
                "     ID[\"EPSG\",9837]]," +
                "   PARAMETER[\"Latitude of topocentric origin\",51.5003," +
                "     ANGLEUNIT[\"degree\",0.0174532925199433]," +
                "     ID[\"EPSG\",8834]]," +
                "   PARAMETER[\"Longitude of topocentric origin\",-1.2," +
                "     ANGLEUNIT[\"degree\",0.0174532925199433]," +
                "     ID[\"EPSG\",8835]]," +
                "   PARAMETER[\"Ellipsoidal height of topocentric origin\",0," +
                "     LENGTHUNIT[\"metre\",1]," +
                "     ID[\"EPSG\",8836]]]," +
                "   CS[Cartesian,3]," +
                "   AXIS[\"topocentric East (U)\",east," +
                "     ORDER[1]," +
                "     LENGTHUNIT[\"metre\",1]]," +
                "   AXIS[\"topocentric North (V)\",north," +
                "     ORDER[2]," +
                "     LENGTHUNIT[\"metre\",1]]," +
                "   AXIS[\"topocentric height (W)\",up," +
                "     ORDER[3]," +
                "     LENGTHUNIT[\"metre\",1]]," +
                "   USAGE[" +
                "     SCOPE[\"unknown\"]," +
                "     AREA[\"To be specified\"]," +
                "     BBOX[-90,-180,90,180]]," +
                "   ID[\"EPSG\",5819]]";
            string wkt25831 = "PROJCS[\"ETRS89 / UTM zone 30N\",GEOGCS[\"ETRS89\",DATUM[\"European_Terrestrial_Reference_System_1989\",SPHEROID[\"GRS 1980\",6378137,298.257222101,AUTHORITY[\"EPSG\",\"7019\"]],TOWGS84[0,0,0,0,0,0,0],AUTHORITY[\"EPSG\",\"6258\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4258\"]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",3],PARAMETER[\"scale_factor\",0.9996],PARAMETER[\"false_easting\",500000],PARAMETER[\"false_northing\",0],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],AXIS[\"Easting\",EAST],AXIS[\"Northing\",NORTH],AUTHORITY[\"EPSG\",\"25831\"]]";
            string wkt3857 = "PROJCS[\"WGS 84 / Pseudo-Mercator\"," +
"GEOGCS[\"WGS 84\"," +
"DATUM[\"WGS_1984\"," +
"SPHEROID[\"WGS 84\",6378137,298.257223563," +
"AUTHORITY[\"EPSG\",\"7030\"]]," +
"AUTHORITY[\"EPSG\",\"6326\"]]," +
"PRIMEM[\"Greenwich\",0," +
"AUTHORITY[\"EPSG\",\"8901\"]]," +
"UNIT[\"degree\",0.0174532925199433," +
"AUTHORITY[\"EPSG\",\"9122\"]]," +
"AUTHORITY[\"EPSG\",\"4326\"]]," +
"PROJECTION[\"Mercator_1SP\"]," +
"PARAMETER[\"central_meridian\",0]," +
"PARAMETER[\"scale_factor\",1]," +
"PARAMETER[\"false_easting\",0]," +
"PARAMETER[\"false_northing\",0]," +
"UNIT[\"metre\",1," +
"AUTHORITY[\"EPSG\",\"9001\"]]," +
"AXIS[\"Easting\",EAST]," +
"AXIS[\"Northing\",NORTH]," +
"EXTENSION[\"PROJ4\",\"+proj=merc +a=6378137 +b=6378137 +lat_ts=0 +lon_0=0 +x_0=0 +y_0=0 +k=1 +units=m +nadgrids=@null +wktext +no_defs\"]," +
"AUTHORITY[\"EPSG\",\"3857\"]]";
            string from4326ToUTM30N = wkt4326 + "=>" + utm30N;
            string idString =  wkt5819; // from4326ToUTM30N; // 
            string parameterString = "{\"lat\": 51.5,\"lon\": -1.2,\"h\": 11.3}";
            string myNumber = GetEPSGNumber(wkt3857);
            string fromCRS = "";
            string toCRS = "";

            if (IsDerivedCRS(idString))
            {
                //
                string epsgNumber = GetEPSGNumber(idString);
                if (epsgNumber == null || epsgNumber == "")
                {
                    // no transformation
                }
                else if (epsgNumber == "5819")
                {
                    // get lat0, lon0, h0
                    double[] origin = new double[3];
                    if (GetOriginParameters(idString, ref origin))
                    {
                        Positions.GeodeticPosition point = GetPositionFromParameters(parameterString);
                        Positions.GeodeticPosition tangentPoint = new Positions.GeodeticPosition(origin[0], origin[1], origin[2]);
                        Positions.CartesianPosition outPoint = Support.LTP_ENU.GeodeticToEnu(point, tangentPoint);
                    }

                }
                else
                {
                    // not found
                }
            }
            else if (IsFromAndToCRS(idString))
            {
                GetFromAndToCRS(idString, out fromCRS, out toCRS);
            }
            else
            {
                // unrecognized form
            }

            CoordinateSystem csIn = null;
            try
            {
                csIn = cf.CreateFromWkt(fromCRS);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Bad, unrecognized, or unsupported outer coordinate system: " + ex.Message);
            }
            CoordinateSystem csOut = null;
            try
            {
                csOut = cf.CreateFromWkt(toCRS);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Bad, unrecognized, or unsupported outer coordinate system: " + ex.Message);
            }
            //var cs3857 = cf.CreateFromWkt(wkt3857);
            ProjNet.CoordinateSystems.Transformations.ICoordinateTransformation transform = null;
            if (csIn != null && csOut != null)
            {
                transform = f.CreateFromCoordinateSystems(csIn, csOut);
                double[] XYZ = new double[3];
                double[] ret = transform.MathTransform.Transform(xyz);
                if (ret.Length == 2)
                {
                    XYZ[2] = xyz[2];
                }
                XYZ[0] = ret[0];
                XYZ[1] = ret[1];
                Console.WriteLine("Coordinate transformation: " +
                    xyz[0].ToString() + ", " + xyz[1].ToString() + ", " + xyz[2].ToString() + "=>" +
                    XYZ[0].ToString() + ", " + XYZ[1].ToString() + ", " + XYZ[2].ToString());
            }
            else
            {
                Console.WriteLine("Coordinate transformation failed: ");
            }
        }
        public static bool IsDerivedCRS(string idString)
        {
            return idString.ToLower().Contains("conversion[");
        }
        public static bool IsFromAndToCRS(string idString)
        {
            return idString.Contains("=>");
        }
        public static bool GetFromAndToCRS(string idString, out string fromCRS, out string toCRS)
        {
            fromCRS = "";
            toCRS = "";
            // Split at =>
            int arrowIndex = idString.IndexOf("=>");
            if(arrowIndex < 1)
            {
                return false;
            }
            fromCRS = idString.Substring(0, arrowIndex);
            toCRS = idString.Substring(arrowIndex + 2);
            return false;
        }
        public static string GetEPSGNumber(string wktString)
        {
            string epsgNumber = "";
            // look at end of WKT for WKT1 or WKT2 ID
            // "ID\["EPSG",\d+\]\]$" or "AUTHORITY\["EPSG",\"\d+\"\]\]$"
            Regex reID = new Regex("(id\\[\\\"epsg\\\",\\d+\\]\\]$)");
            Regex reAuthority = new Regex("(authority\\[\\\"epsg\\\",\\\"\\d+\\\"\\]\\]$)");
            string thisMatch = "";
            MatchCollection matches;
            if ((matches = reID.Matches(wktString.ToLower())).Count > 0)
            {
                thisMatch = matches[0].Value;
            }
            else if ((matches = reAuthority.Matches(wktString.ToLower())).Count > 0)
            {
                thisMatch = matches[0].Value;
            }
            if (thisMatch != "")
            {
                Regex reNumber = new Regex("\\d+");
                matches = reNumber.Matches(thisMatch);
                if (matches.Count > 0)
                {
                    epsgNumber = matches[0].Value;
                }
            }
            return epsgNumber;
        }
        public static bool GetOriginParameters(string wktString, ref double[] origin)
        {
            // PARAMETER[\"Latitude of topocentric origin\",55,
            origin[0] = GetSignedDoubleInRe("parameter\\[\\\"latitude.+,-?\\d+\\.?\\d*", wktString);
            origin[1] = GetSignedDoubleInRe("parameter\\[\\\"longitude.+,-?\\d+\\.?\\d*", wktString);
            origin[2] = GetSignedDoubleInRe("parameter\\[\\\"[^\\]]*height.+,-?\\d+\\.?\\d*", wktString);
            return (!double.IsNaN(origin[0]) && !double.IsNaN(origin[1]) && !double.IsNaN(origin[2]));
        }
        public static Positions.GeodeticPosition GetOriginParameters(string wktString)
        {
            // PARAMETER[\"Latitude of topocentric origin\",55,
            double lat = GetSignedDoubleInRe("parameter\\[\\\"latitude.+,-?\\d+\\.?\\d*", wktString);
            double lon = GetSignedDoubleInRe("parameter\\[\\\"longitude.+,-?\\d+\\.?\\d*", wktString);
            double h = GetSignedDoubleInRe("parameter\\[\\\"[^\\]]*height.+,-?\\d+\\.?\\d*", wktString);
            if(!double.IsNaN(lat) && !double.IsNaN(lon) && !double.IsNaN(h))
            {
                return new Positions.GeodeticPosition(lat, lon, h);
            }
            return new Positions.NoPosition();
        }
        public static double GetSignedDoubleInRe(string reString, string inputString)
        {
            double result = double.NaN;
            Regex re = new Regex(reString);
            MatchCollection matches = re.Matches(inputString.ToLower());
            if (matches.Count > 0)
            {
                string thisMatch = matches[0].Value;
                re = new Regex("-?\\d+\\.?\\d*");
                matches = re.Matches(thisMatch);
                if (matches.Count > 0)
                {
                    result = double.Parse(matches[0].Value);
                }
            }
            return result;
        }
        public static Positions.GeodeticPosition GetPositionFromParameters(string paramString)
        {
            // JSON encoded: {"lat": 12.345, "lon": -22.54, "h": 11.22}
            double lat = GetSignedDoubleInRe("\\\"lat\\\"\\s*:\\s*-?\\d+(\\.\\d*)?", paramString);
            double lon = GetSignedDoubleInRe("\\\"lon\\\"\\s*:\\s*-?\\d+(\\.\\d*)?", paramString);
            double h   = GetSignedDoubleInRe("\\\"h\\\"\\s*:\\s*-?\\d+(\\.\\d*)?", paramString);
            if (!double.IsNaN(lat) && !double.IsNaN(lon) && !double.IsNaN(h))
            {
                return new Positions.GeodeticPosition(lat, lon, h);
            }
            return new Positions.NoPosition();
        }
    }
}
