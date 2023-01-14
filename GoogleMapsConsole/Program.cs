using System;
using System.IO;
using System.Net.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

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
            // https://overpass-api.de/api/interpreter?data=[out:json];node(50.93545,-1.4727869,50.93946,-1.46737964);%20out%20body;
            // https://overpass-api.de/api/interpreter?data=[out:json];way(50.93545,-1.4727869,50.93946,-1.46737964);%20out%20body;
            // https://overpass-api.de/api/interpreter?data=[out:json];relation(50.93545,-1.4727869,50.93946,-1.46737964);%20out%20body;

            string baseDir = "c:/temp/models/world";
            // create directories if not already present
            if(!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }
            string baseImagesDir = baseDir + "/" + "TerrainImages";
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
            double cH   = 19.08;
            double radius = 220.0;

            double north, east, up, latMin, lonMin, latMax, lonMax, aH;
            // get rectangular bounds corresponding to circle
            LTP_ENU.LTP_ENU.GeodeticToEnu(cLat, cLon, cH, cLat, cLon, cH, out east, out north, out up);
            LTP_ENU.LTP_ENU.EnuToGeodetic(east + radius, north + radius, up, cLat, cLon, cH, out latMax, out lonMax, out aH);
            LTP_ENU.LTP_ENU.EnuToGeodetic(east - radius, north - radius, up, cLat, cLon, cH, out latMin, out lonMin, out aH);
            // get centers of images to cover rectangle
            int imageSize = 512;
            uint zoom = 20;
            double overlap = 0.8;
            double shiftLines = (double)imageSize * overlap;
            Console.Write("Rows per degree: " + RowsPerLatDegree(latMin, latMax, zoom).ToString("f8"));
            Console.WriteLine("  Cols per degree: " + ColsPerLonDegree(lonMin, lonMax, zoom).ToString("f8"));
            // what is the height in pseudo mercator northings? half of imageSize
            // what is the width in pseudo mercator eastings? half of imageSize
            // what is the height of an image in degrees? imageSize/RowsPerLatDegree(latMin, latMax, zoom)
            double imageSizeLatDegrees = (double)imageSize / RowsPerLatDegree(latMin, latMax, zoom);
            double imageSizeLonDegrees = (double)imageSize / ColsPerLonDegree(latMin, latMax, zoom);
            // what is the lat, lon of the lower left image?
            double latLL = latMin + imageSizeLatDegrees * overlap * 0.5;
            double lonLL = lonMin + imageSizeLonDegrees * overlap * 0.5;
            double latUR = latMax - imageSizeLatDegrees * overlap * 0.5;
            double lonUR = lonMax - imageSizeLonDegrees * overlap * 0.5;
            int nRows = (int)((latUR - latLL + imageSizeLatDegrees * 0.5) / (imageSizeLatDegrees * overlap));
            int nCols = (int)((lonUR - lonLL + imageSizeLonDegrees * 0.5) / (imageSizeLonDegrees * overlap));
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
                    if(File.Exists(fileName))
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
            int usableImageSize = (int)((double)imageSize * overlap);
            int imageWidth = nCols * usableImageSize;
            imageWidth = ((imageWidth + 3) / 4) * 4;
            int imageHeight = nRows * usableImageSize;
            imageHeight = ((imageHeight + 3) / 4) * 4;
            Image<Rgba32> finalImage = new Image<Rgba32>(imageWidth, imageHeight);
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
                    for(int tRow = 0; tRow < imageSize; tRow++)
                    {
                        int outRow = tRow + ((nRows-1) - nRow) * (int)(imageSize * overlap);
                        if (outRow >= 0 && outRow < imageHeight)
                        {
                            for (int tCol = 0; tCol < imageSize; tCol++)
                            {
                                int outCol = tCol + nCol * (int)(imageSize * overlap);
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
            if(File.Exists(finalFileName))
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
            // need pseudo mercator coordinates of LL and UR
            // also the WGS-84 and LTP-ENU equivalents

            // get OSM Data
            string osmBB = "(" + latLL.ToString("f8") + "," + lonLL.ToString("f8") + "," + latUR.ToString("f8") + "," + lonUR.ToString("f8") + ")";
            string OSMBaseUri = "https://overpass-api.de/api/interpreter?data=[out:json];";
            string OSMUriTail = osmBB + ";%20out%20body;";
            string[] OSMTypes = new string[3] { "node", "way", "relation" };
            //string[] OSMUris = new string[3] {
            //    "https://overpass-api.de/api/interpreter?data=[out:json];node" + osmBB + ";%20out%20body",
            //    "https://overpass-api.de/api/interpreter?data=[out:json];way" + osmBB + ";%20out%20body",
            //    "https://overpass-api.de/api/interpreter?data=[out:json];relation" + osmBB + ";%20out%20body" };
            for(int nUri = 0; nUri < 3; nUri++) 
            {
                string uri = OSMBaseUri + OSMTypes[nUri] + OSMUriTail;
                string osmFileName = baseOSMDir + "/" + OSMTypes[nUri] + "s.txt";
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

                        //Do something with image
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
            // https://overpass-api.de/api/interpreter?data=[out:json];way(50.93545,-1.4727869,50.93946,-1.46737964);%20out%20body;
            // https://overpass-api.de/api/interpreter?data=[out:json];relation(50.93545,-1.4727869,50.93946,-1.46737964);%20out%20body;
            // https://overpass-api.de/api/interpreter?data=[out:json];node(50.93545,-1.4727869,50.93946,-1.46737964);%20out%20body;
            // https://overpass-api.de/api/interpreter?data=[out:json];way(50.93545,-1.4727869,50.93946,-1.46737964);%20out%20body;
            // https://overpass-api.de/api/interpreter?data=[out:json];relation(50.93545,-1.4727869,50.93946,-1.46737964);%20out%20body;

            // then make a mesh for the rectangular area and compute the texture coordinates
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
            double result =  deltaY / deltaLat;
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
