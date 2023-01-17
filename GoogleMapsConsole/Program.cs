using System;
using System.IO;
using System.Net.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using NetTopologySuite.Geometries;
using GeoAPI.Geometries;
using Util3857;

namespace GoogleMapsConsole
{
    /*
     * 
        public static void GeodeticToEnu(double lat, double lon, double h,
                                            double lat0, double lon0, double h0,
                                            out double xEast, out double yNorth, out double zUp)
        
        public static void EnuToGeodetic(double xEast, double yNorth, double zUp,
                                             double lat0, double lon0, double h0,
                                            out double lat, out double lon, out double h
     * 
     */
    internal class Program
    {
        // check/create working directories
        // define a center in wgs 84
        // define a radius in m
        // get corners of bb based on center and radius
        // get texture from Google
        // get feature info from OSM
        // make a point set with center and circular boundary with 1 + 256 points
        // build roads, paths, water, and buildings/footprints from feature info
        // triangulate points of the area features plus 
        // add 
        static async Task Main(string[] args)
        {
            string baseDir = "c:/temp/models/world";
            // create directories if not already present
            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }
            string baseImagesDir = baseDir + "/" + "Terrain";
            if (!Directory.Exists(baseImagesDir))
            {
                Directory.CreateDirectory(baseImagesDir);
            }
            string baseOSMDir = baseDir + "/" + "OSM";
            if (!Directory.Exists(baseOSMDir))
            {
                Directory.CreateDirectory(baseOSMDir);
            }

            double cLat = 50.9374713795844;
            double cLon = -1.4696387314938;
            double cH = 19.08;
            double radius = 220.0;

            double north, east, up, latMin, lonMin, latMax, lonMax, aH;
            // get rectangular bounds corresponding to circle
            LTP_ENU.LTP_ENU.GeodeticToEnu(cLat, cLon, cH, cLat, cLon, cH, out east, out north, out up);
            LTP_ENU.LTP_ENU.EnuToGeodetic(east + radius, north + radius, up, cLat, cLon, cH, out latMax, out lonMax, out aH);
            LTP_ENU.LTP_ENU.EnuToGeodetic(east - radius, north - radius, up, cLat, cLon, cH, out latMin, out lonMin, out aH);
            // get pseudo mercator for lat/lon min/max
            uint pmXMin = lonToX(lonMin, 20);
            uint pmXMax = lonToX(lonMax, 20);
            uint pMYMin = latToY(latMin, 20);
            uint pmYMax = latToY(latMax, 20);
            GlobalMercator gm = new GlobalMercator();
            CoordPair botLeft = gm.LatLonToMeters(latMin, lonMin);
            IntCoordPair iCoord = gm.MetersToPixels(botLeft.X, botLeft.Y, 20);
            IntCoordPair rCoord = gm.PixelsToRaster(iCoord.X, iCoord.Y, 20);

            int pmDeltaX = (int)(pmXMax - pmXMin);
            int pmDeltaY = (int)(pmYMax - pMYMin);
            // also the WGS-84 50.93559071, -1.47291916;50.93558776, -1.46687911;50.93939992, -1.46688010;50.93939699, -1.47292183
            // get centers of images to cover rectangle
            int imageSize = 512;
            uint zoom = 20;
            // Overlap is the fractional duplication of adjacent images 
            double overlap = 0.2;
            double jumpSize = 1.0 - overlap;
            double shiftLines = (double)imageSize * jumpSize;
            Console.Write("Rows per degree: " + RowsPerLatDegree(latMin, latMax, zoom).ToString("f8"));
            Console.WriteLine("  Cols per degree: " + ColsPerLonDegree(lonMin, lonMax, zoom).ToString("f8"));
            // what is the height in pseudo mercator northings? half of imageSize
            // what is the width in pseudo mercator eastings? half of imageSize
            // what is the height of an image in degrees? imageSize/RowsPerLatDegree(latMin, latMax, zoom)
            double imageSizeLatDegrees = (double)imageSize / RowsPerLatDegree(latMin, latMax, zoom);
            double imageSizeLonDegrees = (double)imageSize / ColsPerLonDegree(latMin, latMax, zoom);
            // what is the lat, lon of the lower left image?
            // also the WGS-84 50.93559071, -1.47291916;50.93558776, -1.46687911;50.93939992, -1.46688010;50.93939699, -1.47292183
            double latLL = latMin + imageSizeLatDegrees * jumpSize * 0.5;
            double lonLL = lonMin + imageSizeLonDegrees * jumpSize * 0.5;
            double latUR = latMax - imageSizeLatDegrees * jumpSize * 0.5;
            double lonUR = lonMax - imageSizeLonDegrees * jumpSize * 0.5;
            int nRows = (int)((latUR - latLL + imageSizeLatDegrees * 0.5) / (imageSizeLatDegrees * jumpSize));
            int nCols = (int)((lonUR - lonLL + imageSizeLonDegrees * 0.5) / (imageSizeLonDegrees * jumpSize));
            double latIncrement = shiftLines / RowsPerLatDegree(latMin, latMax, zoom);
            double lonIncrement = shiftLines / ColsPerLonDegree(lonMin, lonMax, zoom);
            HttpClient client = new HttpClient();
            for (int nCol = 0; nCol < nCols; nCol++)
            {
                double colLon = lonLL + nCol * lonIncrement;
                for (int nRow = 0; nRow < nRows; nRow++)
                {
                    double rowLat = latLL + nRow * latIncrement;
                    string gmRC = latToY(rowLat, (uint)zoom).ToString() + "." + lonToX(colLon, (uint)zoom).ToString();
                    string latlon = rowLat.ToString("f7") + "," + colLon.ToString("f7");
                    string colrow = nCol.ToString("d2") + "." + nRow.ToString("d2");
                    string fileName = baseImagesDir + "/20." + gmRC + ".png";// "c:\\temp\\models\\world\\satimages\\20." + gmRC + ".png";
                    if (File.Exists(fileName))
                    {
                        continue;
                    }
                    Uri uri = new Uri("https://maps.googleapis.com/maps/api/staticmap?center=<<latlon>>&format=png&maptype=satellite&zoom=20&size=640x640&key=AIzaSyDdzK4fey4N9dfCFDY78s02ICM3AyJ27Xk"
                        .Replace("<<latlon>>", latlon));
                    //client.BaseAddress = uri;
                    try
                    {

                        var imageContent = await client.GetByteArrayAsync(uri);

                        using (var imageBuffer = new MemoryStream(imageContent))
                        {
                            var image = Image.Load(imageBuffer);
                            image.Save(fileName);
                            Thread.Sleep(1000);
                            //Do something with image
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                }
            }
            int usableImageSize = (int)((double)imageSize * jumpSize);
            int imageWidth = nCols * usableImageSize;
            imageWidth = ((imageWidth + 3) / 4) * 4;
            int imageHeight = nRows * usableImageSize;
            imageHeight = ((imageHeight + 3) / 4) * 4;
            Image<Rgba32> finalImage = new Image<Rgba32>(imageWidth, imageHeight);
#if TESTPATTERN
            for (int nRow = 0; nRow < imageHeight; nRow++)
            {
                for(int nCol = 0; nCol < imageWidth; nCol++)
                {
                    int r = nRow % 256;
                    int g = nCol % 256;
                    int b = (nRow + nCol) % 256;
                    Rgba32 c = new Rgba32(r, g, b, 240);
                    finalImage[nCol, nRow] = c;
                }
            }
#endif // TESTPATTERN
            for (int nCol = 0; nCol < nCols; nCol++)
            {
                double colLon = lonLL + nCol * lonIncrement;
                for (int nRow = 0; nRow < nRows; nRow++)
                {
                    double rowLat = latLL + nRow * latIncrement;
                    string gmRC = latToY(rowLat, (uint)zoom).ToString() + "." + lonToX(colLon, (uint)zoom).ToString();
                    string latlon = rowLat.ToString("f7") + "," + colLon.ToString("f7");
                    string colrow = nCol.ToString("d2") + "." + nRow.ToString("d2");
                    string fileName = baseImagesDir + "/20." + gmRC + ".png";
                    if (!File.Exists(fileName))
                    {
                        continue;
                    }
                    Image<Rgba32> tileBitmap = Image.Load<Rgba32>(fileName);
                    for (int tRow = 0; tRow < imageSize; tRow++)
                    {
                        int outRow = tRow + ((nRows - 1) - nRow) * (int)(imageSize * jumpSize);
                        if (outRow >= 0 && outRow < imageHeight)
                        {
                            for (int tCol = 0; tCol < imageSize; tCol++)
                            {
                                int outCol = tCol + nCol * (int)(imageSize * jumpSize);
                                if (outCol >= 0 && outCol < imageWidth)
                                {
                                    Rgba32 c = tileBitmap[tCol, tRow];
                                    finalImage[outCol, outRow] = c;
                                }
                            }
                        }
                    }
                }
            }
            string finalFileName = baseImagesDir + "/Terrain.png";
            if (File.Exists(finalFileName))
            {
                File.Delete(finalFileName);
            }
            finalImage.Save(finalFileName);
            // get closest power of two size
            int lowerPowerOfTwo = (int)Math.Log2((double)imageHeight);
            int upperPowerOfTwo = lowerPowerOfTwo + 1;
            double lowerSize = Math.Pow(2, lowerPowerOfTwo);
            double upperSize = Math.Pow(2, upperPowerOfTwo);
            double lowerCount = lowerSize * lowerSize;
            double actualCount = (double)imageHeight * (double)imageHeight;
            double upperCount = upperSize * upperSize;
            double lowerDelta = actualCount - lowerCount;
            double upperDelta = upperCount - actualCount;
            finalFileName = finalFileName.Replace(".png", "." + lowerSize.ToString() + ".png");
            if (File.Exists(finalFileName))
            {
                File.Delete(finalFileName);
            }
            Image<Rgba32> outImage;
            if (lowerDelta < upperDelta)
            {
                // resize image to lowerSize x lowerSize
                outImage = new Image<Rgba32>((int)(lowerSize + 0.5), (int)(lowerSize + 0.5));
                finalImage.Mutate(x => x.Resize((int)(lowerSize + 0.5), (int)(lowerSize + 0.5)));
            }
            else
            {
                // resize image to upperSize x upperSize
                outImage = new Image<Rgba32>((int)(upperSize + 0.5), (int)(upperSize + 0.5));
                finalImage.Mutate(x => x.Resize((int)(upperSize + 0.5), (int)(upperSize + 0.5)));
            }
            finalImage.Save(finalFileName);
            // need pseudo mercator coordinates of LL, LR, UR, and UL
            // also the WGS-84 50.93559071, -1.47291916;50.93558776, -1.46687911;50.93939992, -1.46688010;50.93939699, -1.47292183
            // x,y: -163964.604,6609908.363;-163292.235,6609907.842;-163292.345,6610581.262;-163964.908,6610580.744
            // LTP-ENU equivalents

            // get OSM Data
            string osmBB = "(" + latLL.ToString("f8") + "," + lonLL.ToString("f8") + "," + latUR.ToString("f8") + "," + lonUR.ToString("f8") + ")";
            string OSMBaseUri = "https://overpass-api.de/api/interpreter?data=[out:json];";
            string OSMUriTail = osmBB + ";%20out%20body;";
            string[] OSMTypes = new string[3] { "node", "way", "relation" };
            // https://overpass-api.de/api/interpreter?data=[out:json];node(50.93545,-1.4727869,50.93946,-1.46737964);%20out%20body;
            // https://overpass-api.de/api/interpreter?data=[out:json];way(50.93545,-1.4727869,50.93946,-1.46737964);%20out%20body;
            // https://overpass-api.de/api/interpreter?data=[out:json];relation(50.93545,-1.4727869,50.93946,-1.46737964);%20out%20body;
            for (int nUri = 0; nUri < 3; nUri++)
            {
                string uri = OSMBaseUri + OSMTypes[nUri] + OSMUriTail;
                string osmFileName = baseOSMDir + "/" + OSMTypes[nUri] + "s.txt";
                if (File.Exists(osmFileName))
                {
                    continue;
                }
                try
                {

                    var osmContent = await client.GetByteArrayAsync(uri);

                    using (var osmBuffer = new MemoryStream(osmContent))
                    {
                        using (var reader = new StreamReader(osmBuffer))
                        {
                            string strContent = reader.ReadToEnd();
                            StreamWriter sw = new StreamWriter(osmFileName);
                            sw.Write(strContent);
                            sw.Close();
                            Thread.Sleep(1000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception getting OSM data: " + ex.ToString());
                }

            }
            // then make a mesh for the rectangular area and compute the texture coordinates
            //    take four corners plus points on radius circle
            /*
                        NetTopologySuite.Triangulate.DelaunayTriangulationBuilder tb = new NetTopologySuite.Triangulate.DelaunayTriangulationBuilder();
                        NetTopologySuite.Triangulate.VoronoiDiagramBuilder vb = new NetTopologySuite.Triangulate.VoronoiDiagramBuilder();
                        List<Coordinate> icc = new List<Coordinate>();
                        IList<AddressPoint> pointList = pointQTIndex.QueryAll();
                        foreach (AddressPoint sap in pointList)
                        {
                            //NetTopologySuite.Geometries.Point p = new NetTopologySuite.Geometries.Point(sap.position.X, sap.position.Y);
                            icc.Add(sap.position.Coordinate);
                        }
                        vb.SetSites(icc);
                        GeometryFactory gf = new GeometryFactory();
                        GeometryCollection gc = vb.GetDiagram(gf);
                        int nPoly = gc.Count;
                        if(nPoly < 1)
                        {
                            return false;
                        }
                        int nMaxOver = (int)(nPoly * 0.01 * maxOverPercent);
                        //int nMaxOver = (int)(nPoly * 0.075);
                        double threshold = 100.0;
                        //int bnOver = 0;
                        while (true)
                        {
                            int nOver = 0;
                            foreach (Polygon poly in gc)
             */

            //    triangulate them
            NetTopologySuite.Triangulate.DelaunayTriangulationBuilder tb = new NetTopologySuite.Triangulate.DelaunayTriangulationBuilder();
            //NetTopologySuite.Triangulate.VoronoiDiagramBuilder vb = new NetTopologySuite.Triangulate.VoronoiDiagramBuilder();
            List<GeoAPI.Geometries.Coordinate> icc = new List<Coordinate>();
            Coordinate ll = new Coordinate(0.0, 0.0);
            Coordinate ul = new Coordinate(0.0, 1.0);
            Coordinate ur = new Coordinate(1.0, 1.0);
            Coordinate lr = new Coordinate(1.0, 0.0);
            icc.Add(ll);
            icc.Add(lr);
            icc.Add(ur);
            icc.Add(ul);
            GeometryFactory gf = new GeometryFactory();
            tb.SetSites(icc);
            NetTopologySuite.Triangulate.QuadEdge.QuadEdgeSubdivision qes = tb.GetSubdivision();
            //icc.Add()
            //IList<AddressPoint> pointList = pointQTIndex.QueryAll();

            //    attach to terrain object
            // then package as function
            // then go back to loader and create a snow globe with textured terrain
            // then add buildings from OSM - push up terrain inside footprints
            // then clean up and refactor
        }

        static double RowsPerLatDegree(double minLat, double maxLat, uint zoom)
        {
            // get the delta lat per row for the given range;
            double deltaLat = maxLat - minLat;
            double minY = latToY(minLat, zoom);
            double maxY = latToY(maxLat, zoom);
            double deltaY = maxY - minY;
            double result = deltaY / deltaLat;
            return -result;
        }
        static double ColsPerLonDegree(double minLon, double maxLon, uint zoom)
        {
            // get the delta lon per row for the given range;
            double deltaX = lonToX(maxLon, zoom) - lonToX(minLon, zoom);
            double result = deltaX / (maxLon - minLon);
            return result;
        }
        static public uint lonToX(double lon, uint zoom)
        {
            uint offset = 256u << ((int)zoom - 1); // one pi worth of longitude
            return (uint)Math.Round(offset + (offset * lon / 180));
        }

        static public uint latToY(double lat, uint zoom)
        {
            uint offset = 256u << ((int)zoom - 1);
            return (uint)Math.Round(offset - offset / Math.PI * Math.Log((1.0 + Math.Sin(lat * Math.PI / 180.0)) / (1.0 - Math.Sin(lat * Math.PI / 180.0))) / 2.0);
        }
    }
}


