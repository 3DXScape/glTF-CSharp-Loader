using System;
using System.IO;
using System.Net.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using NetTopologySuite.Geometries;
using GeoAPI.Geometries;
using Util3857;
namespace TerrainComponents
{
    public class Vec2
    {
        public Vec2()
        {

        }
        public Vec2(Coordinate p)
        {
            this.Components[0] = (float)p.X;
            this.Components[1] = (float)p.Y;
        }
        public Vec2(float u, float v)
        {
            Components[0] = u;
            Components[1] = v;
        }
        public float[] Components { get; set; } = new float[2];
    }
    public class Vec3
    {
        public Vec3()
        {

        }
        public Vec3(Coordinate p)
        {
            this.Components[0] = (float)p.X;
            this.Components[1] = (float)p.Y;
            this.Components[2] = (float)p.Z;
        }
        public Vec3(float x, float y, float z)
        {
            Components[0] = x;
            Components[1] = y;
            Components[2] = z;  
        }
        public float[] Components { get; set; } = new float[3];
        public static Vec3 operator -(Vec3 a, Vec3 b)
        {
            return new Vec3(a.Components[0] - b.Components[0], a.Components[1] - b.Components[1], a.Components[2] - b.Components[2]);
        }
        // get a new normalized copy
        public static Vec3 GetNormlized(Vec3 a)
        {
            double n2 = a.Components[0] * a.Components[0] + a.Components[1] * a.Components[1] + a.Components[2] * a.Components[2];
            if ((float)n2 != 0.0)
            {
                float v = (float)Math.Sqrt(n2);
                Vec3 unitVec3 = new Vec3((float)a.Components[0] / v, a.Components[1] / v, a.Components[2] / v);
                return unitVec3;
            }
            return new Vec3((float)0.0, (float)0.0, (float)0.0);
        }
        // normalize in place
        public void Normalize()
        {
            double n2 = this.Components[0] * this.Components[0] + this.Components[1] * this.Components[1] + this.Components[2] * this.Components[2];
            if ((float)n2 != 0.0)
            {
                float v = (float)Math.Sqrt(n2);
                this.Components[0] /= v;
                this.Components[1] /= v;
                this.Components[2] /= v;
            }
        }
        public static Vec3 GetCross(Vec3 a, Vec3 b)
        {
            double i = a.Components[1] * b.Components[2] - b.Components[1] * a.Components[2];
            double j = b.Components[0] * a.Components[2] - a.Components[0] * b.Components[2];
            double k = a.Components[0] * b.Components[1] - b.Components[0] * a.Components[1];
            return new Vec3((float)i, (float)j, (float)k);
        }
        public static Vec3 GetAverage(Vec3[] vecArray)
        {
            Vec3 result = new Vec3((float)0.0, (float)0.0, (float)0.0);
            int n = vecArray.Length;
            if (n > 0)
            {
                float dn = (float)n;
                for (int nV = 0; nV < vecArray.Length; nV++)
                {
                    result.Components[0] += vecArray[nV].Components[0]/dn;
                    result.Components[1] += vecArray[nV].Components[1]/dn;
                    result.Components[2] += vecArray[nV].Components[2]/dn;
                }
            }
            return result;
        }
    }
    public class Vec3Store
    {
        private List<Vec3> _theList = new List<Vec3>();
        public int Add(Coordinate p)
        {
            Vec3 vec3 = new Vec3();
            vec3.Components[0] = (float)p.X;
            vec3.Components[1] = (float)p.Y;
            vec3.Components[2] = (float)p.Z;
            return Add(vec3);
        }
        public int Add(Vec3 vec3)
        {
            int nVec3;
            // is this already in store>
            for (nVec3 = 0; nVec3 < _theList.Count; nVec3++)
            {
                if (vec3.Components[0] == _theList[nVec3].Components[0] &&
                    vec3.Components[1] == _theList[nVec3].Components[1] &&
                    vec3.Components[2] == _theList[nVec3].Components[2])
                {
                    return nVec3;
                }
            }
            nVec3 = _theList.Count;
            _theList.Add(vec3);
            return nVec3;
        }
        public Vec3[] Vec3Array
        {
            get
            {
                return _theList.ToArray();
            }
        }
        public int Length
        {
            get
            {
                return _theList.Count;
            }
        }
    }
    public class TerrainInfo
    {

        // groups of three east, north, up coordinates
        public Vec3[] Vertices { get; set; } = new Vec3[0];
        // groups of three i, j, k components of 3D unit vectors
        public Vec3[] Normals  { get; set; } = new Vec3[0];
        // groups of three triangle indices into the Vertices
        public int[] Indices { get; set; } = new int[0];
        // groups of two u, v values for texture coordinates
        public Vec2[] UVCoord { get; set; } = new Vec2[0];
        public string TerrainTextureFile { get; set; } = string.Empty;
    }
    public static class TerrainComponents
    {
        public static async Task<TerrainInfo> GetTerrainComponents(string name, string baseDir, double cLat, double cLon, double cH, double radius)
        {
            TerrainInfo results = new TerrainInfo();
            results.Vertices = new Vec3[0];
            results.Normals = new Vec3[0];
            results.Indices = new int[0];
            results.UVCoord = new Vec2[0];
            // should return
            // 1. mesh for terrain
            //    a. vertices
            //    b. indices
            //    c. normals
            //    d. uv coord
            //    e. texture map in png file
            //  2. list of semantic objects
            // create directories if not already present
            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }
            string baseImagesDir = baseDir + "/" + "Terrain4";
            if (!Directory.Exists(baseImagesDir))
            {
                Directory.CreateDirectory(baseImagesDir);
            }
            string baseOSMDir = baseDir + "/" + "OSM";
            if (!Directory.Exists(baseOSMDir))
            {
                Directory.CreateDirectory(baseOSMDir);
            }
            double north, east, up, latMin, lonMin, latMax, lonMax, aH;
            // get rectangular bounds corresponding to circle
            LTP_ENU.LTP_ENU.GeodeticToEnu(cLat, cLon, cH, cLat, cLon, cH, out east, out north, out up);
            LTP_ENU.LTP_ENU.EnuToGeodetic(east + radius, north + radius, up, cLat, cLon, cH, out latMax, out lonMax, out aH);
            LTP_ENU.LTP_ENU.EnuToGeodetic(east - radius, north - radius, up, cLat, cLon, cH, out latMin, out lonMin, out aH);
            GlobalMercator gm = new GlobalMercator();
            CoordPair botLeft = gm.LatLonToMeters(latMin, lonMin);
            IntCoordPair iCoord = gm.MetersToPixels(botLeft.X, botLeft.Y, 20);
            IntCoordPair rCoord = gm.PixelsToRaster(iCoord.X, iCoord.Y, 20);

            // need the LTP-ENU coordinates for center and corners of rectangle
            CoordPair[] LTP_ENUAnchors = new CoordPair[5];
            LTP_ENUAnchors[0] = new CoordPair(east - radius, north - radius);
            LTP_ENUAnchors[1] = new CoordPair(east + radius, north - radius);
            LTP_ENUAnchors[2] = new CoordPair(east + radius, north + radius);
            LTP_ENUAnchors[3] = new CoordPair(east - radius, north + radius);
            LTP_ENUAnchors[4] = new CoordPair(east, north);

            // need the WGS84 EPSG 4326 positions
            CoordPair[] GeodeticPositions = new CoordPair[5];
            GeodeticPositions[0] = new CoordPair(lonMin, latMin);
            GeodeticPositions[1] = new CoordPair(lonMax, latMin);
            GeodeticPositions[2] = new CoordPair(lonMax, latMax);
            GeodeticPositions[3] = new CoordPair(lonMin, latMax);
            GeodeticPositions[4] = new CoordPair(cLon, cLat);
            Console.WriteLine("LL: " + GeodeticPositions[0].X.ToString("f8") + " " + GeodeticPositions[0].Y.ToString("f8"));
            Console.WriteLine("LR: " + GeodeticPositions[1].X.ToString("f8") + " " + GeodeticPositions[1].Y.ToString("f8"));
            Console.WriteLine("UR: " + GeodeticPositions[2].X.ToString("f8") + " " + GeodeticPositions[2].Y.ToString("f8"));
            Console.WriteLine("UL: " + GeodeticPositions[3].X.ToString("f8") + " " + GeodeticPositions[3].Y.ToString("f8"));
            Console.WriteLine("CT: " + GeodeticPositions[4].X.ToString("f8") + " " + GeodeticPositions[4].Y.ToString("f8"));

            // need the raster coordinates for center and corners of rectangle
            IntCoordPair[] RasterAnchors = new IntCoordPair[5];
            RasterAnchors[0] = rCoord;
            for (int nPair = 0; nPair < 5; nPair++)
            {
                CoordPair aPoint = gm.LatLonToMeters(GeodeticPositions[nPair].Y, GeodeticPositions[nPair].X);
                IntCoordPair anICoord = gm.MetersToPixels(aPoint.X, aPoint.Y, 20);
                IntCoordPair anRCoord = gm.PixelsToRaster(anICoord.X, anICoord.Y, 20);
                RasterAnchors[nPair] = anRCoord;
            }

            int pmDeltaX = (int)(RasterAnchors[1].X - RasterAnchors[0].X);
            int pmDeltaY = (int)(RasterAnchors[0].Y - RasterAnchors[3].Y);
            int imageSize = 512;
            uint zoom = 20;
            double overlap = 0.2;
            double jumpSize = 1.0 - overlap;
            // 
            int shiftLines = (int)(0.5 + (double)imageSize * jumpSize);
            // start 256 lines down from top of final image and 256 pixels to the right of the left edge of the final image
            HttpClient client = new HttpClient();
            int usableImageSize = (int)((double)imageSize * jumpSize);
            int imageWidth = RasterAnchors[1].X - RasterAnchors[0].X + 1;
            int imageHeight = RasterAnchors[0].Y - RasterAnchors[3].Y + 1; ;
            Image<Rgba32> finalImage = new Image<Rgba32>(imageWidth, imageHeight);

            int nImages = 0;
            for (int tileLeft = RasterAnchors[0].X; tileLeft < RasterAnchors[1].X; tileLeft += shiftLines)
            {
                int tileRight = tileLeft + shiftLines - 1;
                int tileCol = (tileLeft + tileRight) / 2;
                for (int tileTop = RasterAnchors[3].Y; tileTop < RasterAnchors[0].Y; tileTop += shiftLines)
                {
                    nImages++;
                    int tileBot = tileTop + shiftLines - 1;
                    int tileRow = (tileTop + tileBot) / 2;
                    IntCoordPair cPixel = gm.PixelsToRaster(tileCol, tileRow, (int)zoom);
                    CoordPair cMeters = gm.PixelsToMeters(cPixel.X, cPixel.Y, (int)zoom);
                    CoordPair cGeodetic = gm.MetersToLatLon(cMeters.X, cMeters.Y);
                    string gmRC = tileRow.ToString() + "." + tileCol.ToString();
                    string latlon = cGeodetic.Y.ToString("f7") + "," + cGeodetic.X.ToString("f7");

                    string fileName = baseImagesDir + "/20." + gmRC + ".png";
                    if (!File.Exists(fileName))
                    {

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
                    int maxDistSquared = (imageHeight * imageHeight + 3) / 4;
                    const int mysteryOffset = 128;  // probably 640 - imageSize [512] - should investigate further sometime
                    Image<Rgba32> tileBitmap = Image.Load<Rgba32>(fileName);
                    for (int tRow = 0; tRow < imageSize; tRow++)
                    {
                        int finalTopRow = tileTop - RasterAnchors[3].Y;
                        // what row is this in the finalImage?
                        int outRow = tRow + finalTopRow - mysteryOffset;                        // ((nRows - 1) - nRow) * (int)(imageSize * jumpSize);
                        if (outRow >= 0 && outRow < imageHeight)
                        {
                            for (int tCol = 0; tCol < imageSize; tCol++)
                            {
                                int finalLeftCol = tileLeft - RasterAnchors[0].X;
                                // what col is this in the finalImage?
                                int outCol = tCol + finalLeftCol - mysteryOffset;                    // nCol * (int)(imageSize * jumpSize);
                                if (outCol >= 0 && outCol < imageWidth)
                                {
                                    int cOutRow = outRow - imageHeight / 2;
                                    int cOutCol = outCol - imageWidth / 2;
                                    Rgba32 c = tileBitmap[tCol, tRow];
                                    if (cOutRow * cOutRow + cOutCol * cOutCol > maxDistSquared)
                                    {
                                        c.A = 64;
                                        //c.R  = 0;
                                        //c.G /= 2;
                                        //c.B /= 2;
                                    }
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
            int resizedImageSize = (int)(lowerSize + 0.5);
            if(lowerDelta > upperDelta)
            {
                resizedImageSize = (int)(upperSize + 0.5);
            }
            // resize image to lowerSize x lowerSize
            outImage = new Image<Rgba32>(resizedImageSize, resizedImageSize);
            finalImage.Mutate(x => x.Resize(resizedImageSize, resizedImageSize));
            finalImage.Save(finalFileName);
            results.TerrainTextureFile = new string(finalFileName);

            // get OSM Data
            string osmBB = "(" + GeodeticPositions[0].Y.ToString("f8") + "," + GeodeticPositions[0].X.ToString("f8") + "," +
                GeodeticPositions[2].Y.ToString("f8") + "," + GeodeticPositions[2].X.ToString("f8") + ")";
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
            List<GeoAPI.Geometries.Coordinate> icc = new List<Coordinate>();
            List<Tuple<int, int, int>> triIndices = new List<Tuple<int, int, int>>();
            //    take fourLTP-ENU corners plus points on radius circle

            Coordinate ll = new Coordinate(LTP_ENUAnchors[0].X, LTP_ENUAnchors[0].Y, 0.0);
            Coordinate ul = new Coordinate(LTP_ENUAnchors[1].X, LTP_ENUAnchors[1].Y, 0.0);
            Coordinate ur = new Coordinate(LTP_ENUAnchors[2].X, LTP_ENUAnchors[2].Y, 0.0);
            Coordinate lr = new Coordinate(LTP_ENUAnchors[3].X, LTP_ENUAnchors[3].Y, 0.0);
            Coordinate ct = new Coordinate(LTP_ENUAnchors[4].X, LTP_ENUAnchors[4].Y, 0.0);
            icc.Add(ll);
            icc.Add(lr);
            icc.Add(ur);
            icc.Add(ul);
            icc.Add(ct);
            //    triangulate them
            NetTopologySuite.Triangulate.DelaunayTriangulationBuilder tb = new NetTopologySuite.Triangulate.DelaunayTriangulationBuilder();
            GeometryFactory gf = new GeometryFactory();
            tb.SetSites(icc);
            NetTopologySuite.Triangulate.QuadEdge.QuadEdgeSubdivision qes = tb.GetSubdivision();
            NetTopologySuite.Geometries.GeometryCollection triangles = (NetTopologySuite.Geometries.GeometryCollection)qes.GetTriangles(new GeometryFactory());
            Vec3Store vec3Store = new Vec3Store();
            // get triangles that do not include frame
            //   store vertices in vertex list
            //   make an index triple for triangle and store in index triple list
            List<Vec3> triangleNormalList = new List<Vec3>();   
            foreach (Polygon poly in triangles.Geometries)
            {
                if (poly.NumPoints == 4)
                {
                    //Polygon triangle = (Polygon)geometry;
                    Vec3 p0 = new Vec3(poly.Coordinates[0]);
                    Vec3 p1 = new Vec3(poly.Coordinates[1]);
                    Vec3 p2 = new Vec3(poly.Coordinates[2]);
                    int v0 = vec3Store.Add(p0);
                    int v1 = vec3Store.Add(p1);
                    int v2 = vec3Store.Add(p2);
                    // normal = normalize (p1-p0 x p2-p0)
                    Vec3 n1 = p1 - p0;
                    Vec3 n2 = p2 - p0;
                    Vec3 cross = Vec3.GetCross(n1, n2);
                    cross.Normalize();
                    triangleNormalList.Add(cross);
                    triIndices.Add(new Tuple<int, int, int>(v0, v1, v2));
                    ////triangleNormalList.Add(normal);
                }
            }
            //foreach
            Vec3[] vertices = vec3Store.Vec3Array;
            Vec2[] uvCoord = new Vec2[vec3Store.Length];
            Vec3[] normals = new Vec3[vec3Store.Length];
            int[] indices = new int[triIndices.Count * 3];
            double uMin = ll.X;
            double vMin = ll.Y;
            double uMax = ur.X;
            double vMax = ur.Y;
            for (int nV = 0; nV < vec3Store.Length; nV++)
            {
                // calculate uvs using LTP-ENU corners
                double u = (vertices[nV].Components[0] - uMin) / (uMax - uMin);
                double v = (vertices[nV].Components[1] - vMin) / (vMax - vMin);
                uvCoord[nV] = new Vec2((float)u, (float)v);
                List<Vec3> normalList = new List<Vec3>();   
                for(int nT = 0; nT < triangleNormalList.Count; nT++)
                {
                    Tuple<int, int, int> t = triIndices[nT];
                    if (t.Item1 == nV || t.Item2 == nV || t.Item3 == nV)
                    {
                        normalList.Add(triangleNormalList[nT]); 
                    }
                }
                Vec3[] normalArray = normalList.ToArray();
                normals[nV] = Vec3.GetAverage(normalArray);
            }
            for (int nT = 0; nT < triangleNormalList.Count; nT++)
            {
                indices[nT * 3] = triIndices[nT].Item1;
                indices[nT * 3 + 1] = triIndices[nT].Item2;
                indices[nT * 3 + 2] = triIndices[nT].Item3;
            }

            results.Normals = normals;
            results.UVCoord = uvCoord;
            results.Indices = indices;
            results.Vertices = vertices;
            //    attach to terrain object
            // then go back to loader and create a snow globe with textured terrain
            // then add buildings from OSM - push up terrain inside footprints
            // then clean up and refactor
            return results;
        }
    }
}
