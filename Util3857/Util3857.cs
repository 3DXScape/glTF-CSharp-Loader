using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util3857
{
    public class CoordPair
    {
        public CoordPair()
        {

        }
        public CoordPair(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        public double X { get; set; }
        public double Y { get; set; }
    }
    public class IntCoordPair
    {
        public IntCoordPair()
        {

        }
        public IntCoordPair(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }
    }
    public class BBox
    {
        public CoordPair lLeft { get; set; }
        public CoordPair uRight { get; set; }
    }
    public class GlobalMercator
    {
        /*
         * """
        TMS Global Mercator Profile
        ---------------------------

        Functions necessary for generation of tiles in Spherical Mercator projection,
        EPSG:900913 (EPSG:gOOglE, Google Maps Global Mercator), EPSG:3785, OSGEO:41001.

        Such tiles are compatible with Google Maps, Microsoft Virtual Earth, Yahoo Maps,
        UK Ordnance Survey OpenSpace API, ...
        and you can overlay them on top of base maps of those web mapping applications.
	
        Pixel and tile coordinates are in TMS notation (origin [0,0] in bottom-left).

        What coordinate conversions do we need for TMS Global Mercator tiles::

             LatLon      <->       Meters      <->     Pixels    <->       Tile     

         WGS84 coordinates   Spherical Mercator  Pixels in pyramid  Tiles in pyramid
             lat/lon            XY in metres     XY pixels Z zoom      XYZ from TMS 
            EPSG:4326           EPSG:900913                                         
             .----.              ---------               --                TMS      
            /      \     <->     |       |     <->     /----/    <->      Google    
            \      /             |       |           /--------/          QuadTree   
             -----               ---------         /------------/                   
           KML, public         WebMapService         Web Clients      TileMapService

        What is the coordinate extent of Earth in EPSG:900913?

          [-20037508.342789244, -20037508.342789244, 20037508.342789244, 20037508.342789244]
          Constant 20037508.342789244 comes from the circumference of the Earth in meters,
          which is 40 thousand kilometers, the coordinate origin is in the middle of extent.
          In fact you can calculate the constant as: 2 * System.Math.PI * 6378137 / 2.0
          $ echo 180 85 | gdaltransform -s_srs EPSG:4326 -t_srs EPSG:900913
          Polar areas with abs(latitude) bigger then 85.05112878 are clipped off.

        What are zoom level constants (pixels/meter) for pyramid with EPSG:900913?

          whole region is on top of pyramid (zoom=0) covered by 256x256 pixels tile,
          every lower zoom level resolution is always divided by two
          initialResolution = 20037508.342789244 * 2 / 256 = 156543.03392804062

        What is the difference between TMS and Google Maps/QuadTree tile name convention?

          The tile raster itthis.is the same (equal extent, projection, pixel size),
          there is just different identification of the same raster tile.
          Tiles in TMS are counted from [0,0] in the bottom-left corner, id is XYZ.
          Google placed the origin [0,0] to the top-left corner, reference is XYZ.
          Microsoft is referencing tiles by a QuadTree name, defined on the website:
          http://msdn2.microsoft.com/en-us/library/bb259689.aspx

        The lat/lon coordinates are using WGS84 datum, yeh?

          Yes, all lat/lon we are mentioning should use WGS84 Geodetic Datum.
          Well, the web clients like Google Maps are projecting those coordinates by
          Spherical Mercator, so in fact lat/lon coordinates on sphere are treated as if
          the were on the WGS84 ellipsoid.
	 
          From MSDN documentation:
          To simplify the calculations, we use the spherical form of projection, not
          the ellipsoidal form. Since the projection is used only for map display,
          and not for displaying numeric coordinates, we don't need the extra precision
          of an ellipsoidal projection. The spherical projection causes approximately
          0.33 percent scale distortion in the Y direction, which is not visually noticable.

        How do I create a raster in EPSG:900913 and convert coordinates with PROJ.4?

          You can use standard GIS tools like gdalwarp, cs2cs or gdaltransform.
          All of the tools supports -t_srs 'epsg:900913'.

          For other GIS programs check the exact definition of the projection:
          More info at http://spatialreference.org/ref/user/google-projection/
          The same projection is degined as EPSG:3785. WKT definition is in the official
          EPSG database.

          Proj4 Text:
            +proj=merc +a=6378137 +b=6378137 +lat_ts=0.0 +lon_0=0.0 +x_0=0.0 +y_0=0
            +k=1.0 +units=m +nadgrids=@null +no_defs

          Human readable WKT format of EPGS:900913:
             PROJCS["Google Maps Global Mercator",
                 GEOGCS["WGS 84",
                     DATUM["WGS_1984",
                         SPHEROID["WGS 84",6378137,298.2572235630016,
                             AUTHORITY["EPSG","7030"]],
                         AUTHORITY["EPSG","6326"]],
                     PRIMEM["Greenwich",0],
                     UNIT["degree",0.0174532925199433],
                     AUTHORITY["EPSG","4326"]],
                 PROJECTION["Mercator_1SP"],
                 PARAMETER["central_meridian",0],
                 PARAMETER["scale_factor",1],
                 PARAMETER["false_easting",0],
                 PARAMETER["false_northing",0],
                 UNIT["metre",1,
                     AUTHORITY["EPSG","9001"]]]268435456
        """
    */
        private int tileSize { get; set; }
        private double initialResolution { get; set; }
        private double originShift { get; set; }
        public GlobalMercator(int tileSize = 256)
        {
            //"Initialize the TMS Global Mercator pyramid"
            this.tileSize = tileSize;
            this.initialResolution = 2.0 * Math.PI * 6378137.0 / (double)this.tileSize;
            //# 156543.03392804062 for tileSize 256 pixels
            this.originShift = 2.0 * Math.PI * 6378137.0 / 2.0;
            //# 20037508.342789244
        }

        public CoordPair LatLonToMeters(double lat, double lon)
        {
            //"Converts given lat/lon in WGS84 Datum to XY in Spherical Mercator EPSG:900913"
            CoordPair m = new CoordPair();
            m.X = lon * this.originShift / 180.0;
            m.Y = Math.Log(Math.Tan((90 + lat) * Math.PI / 360.0)) / (Math.PI / 180.0);
            m.Y = m.Y * originShift / 180.0;
            return m;
        }

        public CoordPair MetersToLatLon(double mx, double my)
        {
            //"Converts XY point from Spherical Mercator EPSG:900913 to lat/lon in WGS84 Datum"
            CoordPair latLon = new CoordPair();
            latLon.X = (mx / this.originShift) * 180.0;
            double tLat = (my / this.originShift) * 180.0;
            //lon = (m.X / this.originShift) * 180.0
            //lat = (m.Y / this.originShift) * 180.0

            latLon.Y = 180 / System.Math.PI * (2 * System.Math.Atan(System.Math.Exp(tLat * System.Math.PI / 180.0)) - System.Math.PI / 2.0);
            return latLon;
        }

        public CoordPair PixelsToMeters(int px, int py, int zoom)
        {
            //"Converts pixel coordinates in given zoom level of pyramid to EPSG:900913"

            double res = Resolution(zoom);
            CoordPair m = new CoordPair();
            m.X = ((double)px * res - this.originShift);
            m.Y = ((double)py * res - this.originShift);
            return m;
        }

        public IntCoordPair MetersToPixels(double mx, double my, int zoom)
        {
            //"Converts EPSG:900913 to pyramid pixel coordinates in given zoom level"

            double res = Resolution(zoom);
            IntCoordPair p = new IntCoordPair();
            p.X = (int)((mx + this.originShift) / res);
            p.Y = (int)((my + this.originShift) / res);
            return p;
        }

        public IntCoordPair PixelsToTile(int px, int py)
        {
            //"Returns a tile covering region in given pixel coordinates"
            IntCoordPair t = new IntCoordPair(); ;
            t.X = (int)(System.Math.Ceiling(px / (double)(this.tileSize)) - 1);
            t.Y = (int)(System.Math.Ceiling(py / (double)(this.tileSize)) - 1);
            return t;
        }
        public IntCoordPair PixelsToRaster(int px, int py, int zoom)
        {
            //"Move the origin of pixel coordinates to top-left corner"

            int mapSize = tileSize << zoom;
            IntCoordPair p = new IntCoordPair();
            p.X = px;
            p.Y = mapSize - py;
            return p;
        }

        public IntCoordPair MetersToTile(double mx, double my, int zoom)
        {
            //"Returns tile for given mercator coordinates"
            IntCoordPair p = this.MetersToPixels(mx, my, zoom);
            return PixelsToTile(p.X, p.Y);
        }

        public BBox TileBounds(int tx, int ty, int zoom)
        {
            //"Returns bounds of the given tile in EPSG:900913 coordinates"
            BBox b = new BBox(); ;
            //CoordPair lLeft;
            b.lLeft = this.PixelsToMeters(tx * this.tileSize, ty * this.tileSize, zoom);
            b.uRight = this.PixelsToMeters((tx + 1) * this.tileSize, (ty + 1) * this.tileSize, zoom);
            //minx, miny = this.PixelsToMeters( tx*this.tileSize, ty*this.tileSize, zoom );
            //maxx, maxy = this.PixelsToMeters( (tx+1)*this.tileSize, (ty+1)*this.tileSize, zoom )
            return b;//( minx, miny, maxx, maxy )
        }
        public BBox TileLatLonBounds(int tx, int ty, int zoom)
        {
            //"Returns bounds of the given tile in latutude/longitude using WGS84 datum"
            BBox b = new BBox();
            BBox tb = this.TileBounds(tx, ty, zoom);
            b.lLeft = this.MetersToLatLon(tb.lLeft.X, tb.lLeft.Y);
            b.uRight = this.MetersToLatLon(tb.uRight.X, tb.uRight.Y);
            //minLat, minLon = this.MetersToLatLon(bounds[0], bounds[1])
            //maxLat, maxLon = this.MetersToLatLon(bounds[2], bounds[3])
            return b;
        }

        public double Resolution(int zoom)
        {
            //"Resolution (meters/pixel) for given zoom level (measured at Equator)"

            //# return (2 * System.Math.PI * 6378137) / (this.tileSize * 2**zoom)
            //return this.initialResolution / (2**zoom);
            return initialResolution / Math.Pow(2.0, (double)zoom);
        }

        public int ZoomForPixelSize(double pixelSize)
        {
            //"Maximal scaledown zoom of the pyramid closest to the pixelSize."

            //for i in range(30):
            for (int i = 0; i <= 30; i++)
            {
                if (pixelSize > Resolution(i))
                {
                    if (i != 0)
                    {
                        //return i - 1;
                        return i - 2;
                    }
                    else
                    {
                        return 0;
                    }
                }
                //return i-1 if i!=0 else 0 # We don't want to scale up
            }
            return 30;
        }

        public IntCoordPair GoogleTile(int tx, int ty, int zoom)
        {
            //"Converts TMS tile coordinates to Google Tile coordinates"
            //# coordinate origin is moved from bottom-left to top-left corner of the extent
            IntCoordPair t = new IntCoordPair();
            t.X = tx;
            t.Y = ((int)Math.Pow(2.0, (double)zoom) - 1) - ty;
            return t;
        }
        private char[] digits = { '0', '1', '2', '3', '4', 'X' };
        public string QuadTree(int tx, int ty, int zoom)
        {
            //"Converts TMS tile coordinates to Microsoft QuadTree"
            StringBuilder quadKey = new StringBuilder();
            ty = ((int)Math.Pow(2.0, (double)zoom) - 1) - ty;
            //for i in range(zoom, 0, -1):
            for (int i = zoom; i >= 0; i--)
            {
                int digit = 0;
                uint mask = (uint)(1 << (i - 1));
                if ((tx & mask) != 0)
                {
                    digit += 1;
                }
                if ((ty & mask) != 0)
                {
                    digit += 2;
                }
                quadKey.Append(digits[digit]);
            }
            return quadKey.ToString();
        }

        public class GlobalGeodetic
        {
            /*
            """
            TMS Global Geodetic Profile
            ---------------------------

            Functions necessary for generation of global tiles in Plate Carre projection,
            EPSG:4326, "unprojected profile".

            Such tiles are compatible with Google Earth (as any other EPSG:4326 rasters)
            and you can overlay the tiles on top of OpenLayers base map.
	
            Pixel and tile coordinates are in TMS notation (origin [0,0] in bottom-left).

            What coordinate conversions do we need for TMS Global Geodetic tiles?

              Global Geodetic tiles are using geodetic coordinates (latitude,longitude)
              directly as planar coordinates XY (it is also called Unprojected or Plate
              Carre). We need only scaling to pixel pyramid and cutting to tiles.
              Pyramid has on top level two tiles, so it is not square but rectangle.
              Area [-180,-90,180,90] is scaled to 512x256 pixels.
              TMS has coordinate origin (for pixels and tiles) in bottom-left corner.
              Rasters are in EPSG:4326 and therefore are compatible with Google Earth.

                 LatLon      <->      Pixels      <->     Tiles     

             WGS84 coordinates   Pixels in pyramid  Tiles in pyramid
                 lat/lon         XY pixels Z zoom      XYZ from TMS 
                EPSG:4326                                           
                 .----.                ----                         
                /      \     <->    /--------/    <->      TMS      
                \      /         /--------------/                   
                 -----        /--------------------/                
               WMS, KML    Web Clients, Google Earth  TileMapService
            """
        */
            public GlobalGeodetic(int tileSize = 256)
            {
                this.tileSize = tileSize;
            }
            public int tileSize { get; set; }
            public IntCoordPair LatLonToPixels(double lat, double lon, int zoom)
            {
                //"Converts lat/lon to pixel coordinates in given zoom of the EPSG:4326 pyramid"
                IntCoordPair p = new IntCoordPair(); ;
                double res = (180 / 256.0 / Math.Pow(2, zoom));
                p.X = (int)((180.0 + lat) / res);
                p.Y = (int)((90.0 + lon) / res);
                return p;
            }

            public CoordPair PixelsToTile(int px, int py)
            {
                //"Returns coordinates of the tile covering region in pixel coordinates"
                CoordPair t = new CoordPair(); ;
                t.X = (int)(Math.Ceiling(px / (double)(this.tileSize)) - 1);
                t.Y = (int)(Math.Ceiling(py / (double)(this.tileSize)) - 1);
                return t;
            }

            public double Resolution(int zoom)
            {
                //"Resolution (arc/pixel) for given zoom level (measured at Equator)"
                return 180 / 256.0 / Math.Pow(2, zoom);
            }

            public BBox TileBounds(int tx, int ty, int zoom)
            {
                //"Returns bounds of the given tile"
                double res = 180 / 256.0 / Math.Pow(2, zoom);
                BBox b = new BBox();
                b.lLeft.X = tx * 256 * res - 180;
                b.lLeft.Y = ty * 256 * res - 90;
                b.uRight.X = (tx + 1) * 256 * res - 180;
                b.uRight.X = (ty + 1) * 256 * res - 90;
                return b;
            }

#if NOT
from __future__ import division
import math
MERCATOR_RANGE = 256

def  bound(value, opt_min, opt_max):
  if (opt_min != None): 
    value = max(value, opt_min)
  if (opt_max != None): 
    value = min(value, opt_max)
  return value


def  degreesToRadians(deg) :
  return deg * (math.pi / 180)


def  radiansToDegrees(rad) :
  return rad / (math.pi / 180)


class G_Point :
    def __init__(self,x=0, y=0):
        self.x = x
        self.y = y



class G_LatLng :
    def __init__(self,lt, ln):
        self.lat = lt
        self.lng = ln


class MercatorProjection :


    def __init__(self) :
      self.pixelOrigin_ =  G_Point( MERCATOR_RANGE / 2, MERCATOR_RANGE / 2)
      self.pixelsPerLonDegree_ = MERCATOR_RANGE / 360
      self.pixelsPerLonRadian_ = MERCATOR_RANGE / (2 * math.pi)


    def fromLatLngToPoint(self, latLng, opt_point=None) :
      point = opt_point if opt_point is not None else G_Point(0,0)
      origin = self.pixelOrigin_
      point.x = origin.x + latLng.lng * self.pixelsPerLonDegree_
//# NOTE(appleton): Truncating to 0.9999 effectively limits latitude to
//# 89.189.  This is about a third of a tile past the edge of the world tile.
      siny = bound(math.sin(degreesToRadians(latLng.lat)), -0.9999, 0.9999)
      point.y = origin.y + 0.5 * math.log((1 + siny) / (1 - siny)) * -     self.pixelsPerLonRadian_
      return point


def fromPointToLatLng(self,point) :
      origin = self.pixelOrigin_
      lng = (point.x - origin.x) / self.pixelsPerLonDegree_
      latRadians = (point.y - origin.y) / -self.pixelsPerLonRadian_
      lat = radiansToDegrees(2 * math.atan(math.exp(latRadians)) - math.pi / 2)
      return G_LatLng(lat, lng)

//# pixelCoordinate = worldCoordinate * pow(2,zoomLevel)

def getCorners(center, zoom, mapWidth, mapHeight):
    scale = 2**zoom
    proj = MercatorProjection()
    centerPx = proj.fromLatLngToPoint(center)
    SWPoint = G_Point(centerPx.x-(mapWidth/2)/scale, centerPx.y+(mapHeight/2)/scale)
    SWLatLon = proj.fromPointToLatLng(SWPoint)
    NEPoint = G_Point(centerPx.x+(mapWidth/2)/scale, centerPx.y-(mapHeight/2)/scale)
    NELatLon = proj.fromPointToLatLng(NEPoint)
    return {
        'N' : NELatLon.lat,
        'E' : NELatLon.lng,
        'S' : SWLatLon.lat,
        'W' : SWLatLon.lng,
    }
Usage :
#endif // NOT

        }
    }
}
