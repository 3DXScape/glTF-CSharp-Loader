using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            pcs = fac.CreateFromWkt(WKT) as ProjectedCoordinateSystem;

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
        }
    }
}
