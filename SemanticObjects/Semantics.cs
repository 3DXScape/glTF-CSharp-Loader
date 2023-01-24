using SharedGeometry;
using g4;
/*
 * 
 * Use Case: Urban Person-Vehicle Rendezvous
This is a slightly generalized form of the "shared ride" rendezvous example.

Begin Real World
===================
Universe: there are Oracles that provide information and Animators that alter conditions in the real world. Both operate without any explanation or known mechanism.

Environment: streets, buildings, pedestrian walkways

The [static real] Environment is an urban space with a few streets, buildings, and pedestrian walkways bounding the buildings and bordering the streets. The buildings are stereotypical, with at least four levels and outer walls containing windows and ground-level doors. The streets are sterotypical, with at least one lane of vehicle traffic in each direction, following the right-hand driving convention. The streets have marked pedestrian crossings, traffic signals at some junctions, and signs. The entire horizontal surface of the Environment is partitioned into non-overlapping buildings, pedestrian walkways, and streets.

Actors: vehicles, people, signal lights

The active real environment contains walking Persons [people] and moving Vehicles. The walking Persons are located on the Pedestrian-Walkways or Pedestrian-Crossings. 
The moving Vehicles are located on the Street surface. The moving Vehicles are capable of containing Persons [people]. A Person can Teleport to/from a Vehicle either from/to a Pedestrian-Walkway or from/to a Pedestrian-Crossing if and only if the Vehicle is touching or intersecting a Pedestrian-Crossing or Pedestrian-Walkway and Close to the Person. 
Signal lights have a state controlled by an Oracle. The possible and mutually exclusive signal light states are red, yellow, green. Signal light state may have a visual manifestation.

Sensors: the topmost part of every Person has a pair of Cameras.

=================
End Real World

Begin Virtual World

====================
Virtual Elements: visible ID Tag

An Actor may have a visible ID Tag located above it, visible in all directions, but possibly occluded from view by elements of the Environment or Actors.

 * 
 * The [static real] Environment is an urban space with a few streets, buildings, and pedestrian walkways bounding the buildings and bordering the streets. 
 * The buildings are stereotypical, with at least four levels and outer walls containing windows and ground-level doors. 
 * The streets are sterotypical, with at least one lane of vehicle traffic in each direction, following the right-hand driving convention.
 * The streets have marked pedestrian crossings, traffic signals at some junctions, and signs. 
 * The entire horizontal surface of the Environment is partitioned into non-overlapping buildings, pedestrian walkways, and streets.
 * 
 */

///
namespace SemanticClasses
{


    public abstract class SemanticClass
    {
        // ID - globally unique
        public string ID { get; set; } = string.Empty;
        // name
        public string Name { get; set; } = string.Empty;
        // version
        public string Version { get; set; } = string.Empty;
        // description
        public string Description { get; set; } = string.Empty;

        // class
        // parent class
        // parent (object, not classes)
        public SemanticClass? ParentClass { get; set; } = null;

        // children (objects, nott classes)
        public List<SemanticClass> ChildClasses { get; set; } = null;
        // constructors

        // behaviors
        //     affordances used including settling on support, rotational alignment, verticality
        //     interfaces used
        public List<Object>? Behaviors { get; set; } = null;

        // affordances
        public List<Object>? Affordances { get; set; } = null;
        // appearance
        //    default material
        public Entities.Material Material { get; set; } = new Entities.GenericMaterial();
        //    default texture
        // physics
    }
    public class BoundingSphere : SemanticClass
    {
        public BoundingSphere()
        {
            this.Material = new Entities.BoundingSphereMaterial();  
        }
        public static Mesh Generate(Tuple<double, double, double> center, double radius)
        {
            return new SharedGeometry.GeneratedSphere_Cube(radius, new double[3] { center.Item1, center.Item2, center.Item3 }, 8).GetMesh();
        }

    }
    public class Generic : SemanticClass
    {

    }
 
    public class Verse : SemanticClass
    {
        public List<SemanticClass>? Contents { get; set; } = null;

    }
    public class LandSurface : SemanticClass
    {
        public LandSurface()
        {
            this.Material = new Entities.TerrainMaterial();
        }
        public static async Task<Mesh> Generate(Tuple<double, double, double> center, double radius)
        {
            // recipe: make a circle at center(from bounding sphere) and with radius size*1.25
            //    make a random point set inside circle; get elevations: this is prior elevation model
            //    add shorelines (from water objects), breaklines(from this terrain), minima(from this terrain), maxima(from this terrain)
            //    add road and path centerlines and edges using profiles (from road/path objects), 
            //    add blended road junctions(from road/path objects)
            //    add building footprints(from building objects)
            //    add graded/paved areas(from road/path objects)
            //    triangulate the set of non-random points: this is the constrained elevation model
            //    make new point set: constrained points plus random points inside traingles not inside constrated points.
            //    triangulate this point set: this is the terrain mesh

            TerrainComponents.TerrainInfo results = await TerrainComponents.TerrainComponents.GetTerrainComponents("OS Southampton HQ", "c:/temp/models/world", 50.93765028067923, -1.4696398272318714, 19.08, 256.0);
            Mesh terrainMesh = new Mesh();
            for(int nV = 0; nV < results.Vertices.Length; nV++)
            {
                Tuple<double, double, double> d3 = new Tuple<double, double, double>(results.Vertices[nV].Components[0], results.Vertices[nV].Components[1], results.Vertices[nV].Components[2] );
                terrainMesh.Vertices.Add(d3);
                Tuple<float, float, float> f3 = new Tuple<float, float, float>(results.Normals[nV].Components[0], results.Normals[nV].Components[1], results.Normals[nV].Components[2]);
                terrainMesh.Normals.Add(f3);
                Tuple<float, float> f2 = new Tuple<float, float>(results.UVCoord[nV].Components[0], results.UVCoord[nV].Components[1]);
                terrainMesh.UVs.Add(f2);
            }
            for(int nIndex = 0; nIndex < results.Indices.Length; nIndex += 3)
            {
                Tuple<ushort, ushort, ushort> u3 = new Tuple<ushort, ushort, ushort>((ushort)results.Indices[nIndex], (ushort)results.Indices[nIndex+1], (ushort)results.Indices[nIndex+2]);
                terrainMesh.Indices.Add(u3);
            }
            Mesh retval = terrainMesh; // new SharedGeometry.GeneratedTerrain(radius, new double[3] { center.Item1, center.Item2, center.Item3 }, 2).GetMesh();
            return retval;
        }

    }

    /// <summary>
    /// The streets are sterotypical, with at least one lane of vehicle traffic in each direction, following the right-hand driving convention.
    /// The streets have marked pedestrian crossings, traffic signals at some junctions, and signs.    
    /// The streets are sterotypical, with at least one lane of vehicle traffic in each direction, following the right-hand driving convention.
    /// </summary>
    public class Road : SemanticClass
    {
        // ways
        RoadMarking[] RoadMarkings { get; set; } = null;
        Signal[] Signals { get; set; } = null;

    }
    public abstract class Way : SemanticClass
    {
        // a way has a beginning and an end
        // properties may be referred to as left and right as viewed from start to end
        // properties may be ordinally numbered from left to right as viewed from start to end
        // material
    }
    public class RoadWay : Way
    {
        // position, width, direction
    }
    public class RoadMarking : SemanticClass
    {

    }

    public class RoadFurniture : SemanticClass
    {

    }

    public class Signal : RoadFurniture
    {

    }

    public class Sign : RoadFurniture
    {
        // GeoPose
        // height
        // width
        // image
        // text
    }

    public class Building : SemanticClass
    {

    }

    public class ExternalDoor : SemanticClass
    {

    }

    public class ExternalWindow : SemanticClass
    {

    }

    public class WalkWay : Way
    {

    }

    public class Car : SemanticClass
    {
    

    }

    public class RideCar : Car
    {
        public bool IsAvailable { get; set; } = true;
    }

    public class Person : SemanticClass
    {
        public Sensors.Binocular BothEyes { get; set; } = new Sensors.Binocular();

    }
}
