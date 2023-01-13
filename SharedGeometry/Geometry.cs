using System.Text;
using System.Text.Json;
using g4;

namespace SharedGeometry
{


    public class Distance
    {
        public double Value { get; set; }
    }
    public class GeneratedSphere_Tetrahedron : Mesh
    {
        Mesh SphereMesh { get; set; } = new Mesh();
        // https://en.wikipedia.org/wiki/Tetrahedron#Formulas_for_a_regular_tetrahedron is source of constants
        public GeneratedSphere_Tetrahedron(double radius, double[] center, double maxEdge)
        {
            // regular tetrahedron inscribed in a radius radius sphere centered at 0.0
            // vertex at 0, 0, radius and base at z = -radius*3
            Node p0 = new Node(radius * Math.Sqrt(8.0 / 9.0), 0.0, -radius / 3.0);
            Node p1 = new Node(-radius * Math.Sqrt(2.0 / 9.0), radius * Math.Sqrt(2.0 / 3.0), -radius / 3.0);
            Node p2 = new Node(-radius * Math.Sqrt(2.0 / 9.0), -radius * Math.Sqrt(2.0 / 3.0), -radius / 3.0);
            Node p3 = new Node(0.0, 0.0, radius);
            // translate to center
            for (int nCoord = 0; nCoord < 3; nCoord++)
            {
                p0.Coordinates[nCoord] += center[nCoord];
                p1.Coordinates[nCoord] += center[nCoord];
                p2.Coordinates[nCoord] += center[nCoord];
                p3.Coordinates[nCoord] += center[nCoord];
            }
            SphereMesh.Vertices.Add(Tuple.Create(p0.Coordinates[0], p0.Coordinates[1], p0.Coordinates[2]));
            SphereMesh.Vertices.Add(Tuple.Create(p1.Coordinates[0], p1.Coordinates[1], p1.Coordinates[2]));
            SphereMesh.Vertices.Add(Tuple.Create(p2.Coordinates[0], p2.Coordinates[1], p2.Coordinates[2]));
            SphereMesh.Vertices.Add(Tuple.Create(p3.Coordinates[0], p3.Coordinates[1], p3.Coordinates[2]));
            SphereMesh.Indices.Add(Tuple.Create((ushort)1, (ushort)2, (ushort)3));
            SphereMesh.Indices.Add(Tuple.Create((ushort)2, (ushort)0, (ushort)3));
            SphereMesh.Indices.Add(Tuple.Create((ushort)0, (ushort)1, (ushort)3));
            SphereMesh.Indices.Add(Tuple.Create((ushort)0, (ushort)2, (ushort)1));
        }
        // 1, 2, 3; 0, 3, 2; 0, 1, 3; 0, 2, 1
        public Mesh GetMesh()
        {
            return SphereMesh;
        }
    }
    public class GeneratedSphere_Cube : Mesh
    {
        Mesh SphereMesh { get; set; } = new Mesh();
        public GeneratedSphere_Cube(double radius, double[] center, int numberEdgePoints)
        {
            Sphere3Generator_NormalizedCube sphereMesh = new Sphere3Generator_NormalizedCube();
            sphereMesh.Radius = radius;
            sphereMesh.EdgeVertices = 8;
            sphereMesh.Generate();
            // store vertices
            foreach (Vector3d v in sphereMesh.vertices.AsVector3d())
            {
                SphereMesh.Vertices.Add(Tuple.Create(v.x, v.y, v.z));
            }
            // store normals
            foreach (Vector3f v in sphereMesh.normals.AsVector3f())
            {
                SphereMesh.Normals.Add(Tuple.Create(v.x, v.y, v.z));
            }
            // store triangle indices
            for (int nIndex = 0; nIndex < sphereMesh.triangles.array.Length; nIndex += 3)
            {
                SphereMesh.Indices.Add(Tuple.Create((ushort)sphereMesh.triangles.array[nIndex],
                    (ushort)sphereMesh.triangles.array[nIndex + 1],
                    (ushort)sphereMesh.triangles.array[nIndex + 2]));
            }
        }
        public Mesh GetMesh()
        {
            return SphereMesh;
        }
    }
    public class GeneratedTerrain : Mesh
    {
        Mesh TerrainMesh { get; set; } = new Mesh();
        public GeneratedTerrain(double radius, double[] center, int numberEdgePoints)
        {
            Sphere3Generator_NormalizedCube terrainMesh = new Sphere3Generator_NormalizedCube();
            terrainMesh.Radius = radius * 0.3;
            terrainMesh.EdgeVertices = 4;
            terrainMesh.Generate();
            // store vertices
            foreach (Vector3d v in terrainMesh.vertices.AsVector3d())
            {
                TerrainMesh.Vertices.Add(Tuple.Create(v.x, v.y, v.z));
            }
            // store normals
            foreach (Vector3f v in terrainMesh.normals.AsVector3f())
            {
                TerrainMesh.Normals.Add(Tuple.Create(v.x, v.y, v.z));
            }
            // store triangle indices
            for (int nIndex = 0; nIndex < terrainMesh.triangles.array.Length; nIndex += 3)
            {
                TerrainMesh.Indices.Add(Tuple.Create((ushort)terrainMesh.triangles.array[nIndex],
                    (ushort)terrainMesh.triangles.array[nIndex + 1],
                    (ushort)terrainMesh.triangles.array[nIndex + 2]));
            }
        }
        public Mesh GetMesh()
        {
            return TerrainMesh;
        }
    }

    public class Node
    {
        public double[] Coordinates { get; set; } = new double[3];
        public Node(double x, double y, double z)
        {
            Coordinates[0] = x;
            Coordinates[1] = y;
            Coordinates[2] = z;
        }
        public Node(double[] coordinates)
        {
            Coordinates = coordinates;
        }
    }
    public class Mesh
    {
        // In this first implementation, it's a very 3D-graphics approach
        // It is likely to become more abstract in the future but probably keeping
        //  these interfaces for (1) ease of interface with graphics and (2) backwards compatibility.
        // Mesh operations will benefit from a more computational-geometry design.
        public string Name { get; set; } = string.Empty;
        // vertices are points - typically must be converted to floats for graphics processing
        public List<Tuple<double, double, double>> Vertices { get; set; } = new List<Tuple<double, double, double>>();
        // normals are unit vectors - float OK for precision needed
        public List<Tuple<float, float, float>> Normals { get; set; } = new List<Tuple<float, float, float>>();
        // indices are the index into the Vertices defining individual triangles
        public List<Tuple<ushort, ushort, ushort>> Indices { get; set; } = new List<Tuple<ushort, ushort, ushort>>();
        // UVs are texture coordinates mapping an image texture map to each point
        public List<Tuple<float, float>> UVs { get; set; } = new List<Tuple<float, float>>();
        // ImageUrl is location of texture map
        public string? ImageUrl { get; set; } = null;   
    }
}
#if NOMORE
namespace GeoPose
{
    public abstract class Position
    {
        
    }
    public abstract class Orientation
    {

    }
    public abstract class GeoPose
    {
        public Position? Position { get; set; } = null;
        public Orientation? Orientation { get; set; } = null;
    }
    public class SphericalPosition : Position
    {
        public double lat { get; set; } = double.NaN;
        public double lon { get; set; } = double.NaN;
        public double h { get; set; } = double.NaN;
    }
    public class ENUPosition : Position
    {
        public double East { get; set; } = double.NaN;
        public double North { get; set; } = double.NaN;
        public double Up { get; set; } = double.NaN;
    }
    public class YPRAngles : Orientation
    {
        public double yaw { get; set; } = double.NaN;
        public double pitch { get; set; } = double.NaN;
        public double roll { get; set; } = double.NaN;
    }
    public class Quaternion : Orientation
    {
        public double x { get; set; } = double.NaN;
        public double y { get; set; } = double.NaN;
        public double z { get; set; } = double.NaN;
        public double w { get; set; } = double.NaN;
    }
    public class ENUPose : GeoPose
    {
        public new ENUPosition Position { get; set; } = new ENUPosition();
        public new YPRAngles Orientation { get; set; } = new YPRAngles();
        public string ToJSON(string indent = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\r\n\t\t" + indent);
            sb.Append("\"position\": {\r\n\t\t\t" + indent + "\"east\": " + Position.East + ",\r\n\t\t\t" + indent +
                "\"north\": " + Position.North + ",\r\n\t\t\t" + indent +
                "\"up\":   " + Position.Up);
            sb.Append("\r\n\t\t" + indent + "},");
            sb.Append("\r\n\t\t" + indent);
            sb.Append("\"angles\": {\r\n\t\t\t" + indent + "\"yaw\":   " + Orientation.yaw + ",\r\n\t\t\t" + indent +
                "\"pitch\": " + Orientation.pitch + ",\r\n\t\t\t" + indent +
                "\"roll\":  " + Orientation.roll);
            sb.Append("\r\n\t\t" + indent + "}");
            sb.Append("\r\n\t" + indent + "}");
            return sb.ToString();
        }
    }
    public class BasicYPR : GeoPose
    {
        public new SphericalPosition? Position { get; set; } = new SphericalPosition();
        public new YPRAngles? Orientation { get; set; } = new YPRAngles();
        public string ToJSON(string indent = "")
        {
            StringBuilder sb = new StringBuilder();
            if (Position != null && Orientation != null)
            {
                sb.Append("{\r\n\t\t" + indent);
                sb.Append("\"position\": {\r\n\t\t\t" + indent + "\"lat\": " + Position.lat + ",\r\n\t\t\t" + indent +
                    "\"lon\": " + Position.lon + ",\r\n\t\t\t" + indent +
                    "\"h\":   " + Position.h);
                sb.Append("\r\n\t\t" + indent + "},");
                sb.Append("\r\n\t\t" + indent);
                sb.Append("\"angles\": {\r\n\t\t\t" + indent + "\"yaw\":   " + Orientation.yaw + ",\r\n\t\t\t" + indent +
                    "\"pitch\": " + Orientation.pitch + ",\r\n\t\t\t" + indent +
                    "\"roll\":  " + Orientation.roll);
                sb.Append("\r\n\t\t" + indent + "}");
                sb.Append("\r\n\t" + indent + "}");
            }
            return sb.ToString();
        }
    }
    public class FrameSpecification
    {
        public string authority { get; set; } = "";
        public string id { get; set; } = "";
        public string parameters { get; set; } = "";
    }
    public class Advanced : GeoPose
    {
        public new FrameSpecification Position { get; set; } = new FrameSpecification();
        public new Quaternion Orientation { get; set; } = new Quaternion();
        public string ToJSON()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"frameSpecification\":{\"authority\":" + Position.authority + ",\"id\":" + Position.id + ",\"parameters\":" + Position.parameters + "},");
            sb.Append("\"quaternion\":{\"x\":" + Orientation.x + ",\"y\":" + Orientation.y + ",\"z\":" + Orientation.z + ",\"w\":" + Orientation.w);
            sb.Append("}}");
            return sb.ToString();
        }
    }
}
#endif // NOMORE
