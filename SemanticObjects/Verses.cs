using System.Collections;
namespace Verses
/// <summary>
/// The Omniverse is the source of all externally defined information and universal relationships and interactions between entities
/// there are Oracles that provide information and Animators that alter conditions in the real world. Both operate without any explanation or known mechanism.
/// </summary>
{
    public class OutsideOfAnyWorld
    {
        public DateTime Now { get { return DateTime.UtcNow; } }

    }
    public class World
    {
        public string Name { get; set; } = "";
        public string ReferenceFrame { get; set; } = "Default";
        public GeoPose.GeoPose? FramePose { get; set; }
        public GeoPose.Position[] Center {get;set;}
        public Geometry.Distance Width { get; set; }
        public List<Entities.Entity> Entities = new List<Entities.Entity>();
        public void SaveAsJSON(StreamWriter jsw)
        {

        }
        public void ListElementsAsMarkDown(StreamWriter sw)
        {
            // name
            //sw.WriteLine("|" + Name);
            // reference frame
            //sw.WriteLine("\r\n \r\nReferenceFrame: " + ReferenceFrame);
            // entities
            foreach(Entities.Entity e in Entities)
            {
                string poseString = (e.Pose.GetType().ToString() == "GeoPose.Basic") ? ((GeoPose.Basic)(e.Pose)).ToJSON() : ((GeoPose.Advanced)(e.Pose)).ToJSON();
                sw.WriteLine("|" + e.Name + "|" + e.ID + "|" + e.SemanticEntityClass.GetType().Name + "|" + poseString + "|");
                //sw.WriteLine("\r\nEntity ID:  " + e.ID + "\r\n ");
                //sw.WriteLine("\r\nSemantic Class: " + e.SemanticEntityClass.GetType().Name + "\r\n ");
            }
        }
        public void AddEntity(Entities.Entity newEntity)
        {
            Entities.Add(newEntity);
        }

    }
    public class StaticWorld : World
    {
        public const string Moniker  = "static"; 
    }

    public class DynamicWorld : World
    {
        public const string Moniker = "dynamic";
    }
    public class VirtualWorld : World
    {
        public const string Moniker = "virtual";
        public SemanticClasses.Sign SignOverRide { get; set; } = new SemanticClasses.Sign();
    }

    public class PlanetLike : StaticWorld
    {

    }

    public class UrbanPatch : PlanetLike
    {

    }
    public class IntegratedWorld
    {
        public IntegratedWorld(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; } = "";
        public string ReferenceFrame { get; set; } = "Default";
        public GeoPose.GeoPose Pose { get; set; }
        public OutsideOfAnyWorld OmniVerse { get; set; } = new OutsideOfAnyWorld();

        public List<Verses.World> WorldSet = new List<World>();

        public void AddWorld(World aWorld)
        {
            WorldSet.Add(aWorld);   
        }

        public void SaveAsJSON()
        {
            string fileName = "c:/temp/models/integrated.json";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            StreamWriter sw = new StreamWriter(fileName);
            sw.WriteLine("{\r\n\"Integrated-World\": " + "\"" + Name + "\"");
            sw.WriteLine("Created: " + OmniVerse.Now.ToString());
            sw.WriteLine("\"Worlds\":\r\n[\r\n");
            // loop through worlds
            foreach(World w in WorldSet)
            {
                sw.WriteLine("\r\n# " + w.Name + ": " + w.GetType().ToString());
            }
            //sw.WriteLine("\r\n# Background World\r\n");
            //Background.SaveAsJSON(sw);
            //sw.WriteLine("\r\n# Foreground World\r\n");
            //Foreground.SaveAsJSON(sw);
            //sw.WriteLine("\r\n# Virtual World\r\n");
            //VirtualParts.SaveAsJSON(sw);
            sw.Close();

        }

        /*
         * 
# Integrated World: Use Case 1
Created: 11/23/2022 11:54:10 PM UTC

---

## Integrated Sub-Worlds

---

**World Name**: \"Background\": 

**World Type**: Verses.StaticWorld

**ReferenceFrame**:  Default

**Contained Entities**:

| Name | ID | Semantic Class |GeoPose |
| ----------- | ----------- | -------- | ----- |
| Planet Surface | 33 | LandSurface | {\"position\": {\"lat\": 48,\"lon\":-122,\"h\": 0},\"quaternion\":{"x":0.00774503,"y":0.00451,"z":0.86391,"w":-0.50356  }}
  |||||

---

         * 
         */
        public void ListElementsAsMarkDown(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            StreamWriter sw = new StreamWriter(fileName);
            sw.WriteLine("# Integrated World: \"" + Name + "\"");
            sw.WriteLine("Created: " + OmniVerse.Now.ToString() + " UTC");
            sw.WriteLine("\r\n---\r\n");
            sw.WriteLine("## Component Worlds");
            sw.WriteLine("\r\n---\r\n");
            // loop through worlds
            foreach (World w in WorldSet)
            {
                sw.WriteLine("\r\n**Name:** " + w.Name);
                sw.WriteLine("\r\n**Type:** " + w.GetType().ToString());
                sw.WriteLine("\r\n**Reference Frame:** " + w.ReferenceFrame);
                string frameType = (w.FramePose != null) ? w.FramePose.GetType().Name : "Unspecified";
                if (w.FramePose != null && frameType == "Basic")
                {
                    sw.WriteLine("\r\n**Frame Pose:** " + ((GeoPose.Basic)w.FramePose).ToJSON());
                }
                else if (w.FramePose != null && frameType == "Advanced")
                {
                    sw.WriteLine("\r\n**Frame Pose:** " + ((GeoPose.Advanced)w.FramePose).ToJSON());
                }
                else
                {
                    sw.WriteLine("\r\n**Frame Pose:** " + "\"unrecognize type\"" + frameType); ;
                }
                sw.WriteLine("\r\n**Contained Entities:** ");
                sw.WriteLine("\r\n| Name | ID | Semantic Class |GeoPose |");
                sw.WriteLine("| ----------- | ----------- | ----------- | ----------- |");
                w.ListElementsAsMarkDown(sw);
                sw.WriteLine("\r\n---\r\n");
            }
            //sw.WriteLine("\r\n# Background World\r\n");
            //Background.ListElementsAsMarkDown(sw);
            //sw.WriteLine("\r\n# Foreground World\r\n");
            //Foreground.ListElementsAsMarkDown(sw);
            //sw.WriteLine("\r\n# Virtual World\r\n");
            //VirtualParts.ListElementsAsMarkDown(sw);
            sw.Close();
        }
        public void ListElementsAsJSON(string fileName)
        {
            string header = " {" + " {\r\n";
            string footer = "}\r\n";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            StreamWriter sw = new StreamWriter(fileName);
            sw.Write(header);
            /*
            sw.WriteLine("# Integrated World: \"" + Name + "\"");
            sw.WriteLine("Created: " + OmniVerse.Now.ToString() + " UTC");
            sw.WriteLine("\r\n---\r\n");
            sw.WriteLine("## Component Worlds");
            sw.WriteLine("\r\n---\r\n");
            // loop through worlds
            foreach (World w in WorldSet)
            {
                sw.WriteLine("\r\n**Name:** " + w.Name);
                sw.WriteLine("\r\n**Type:** " + w.GetType().ToString());
                sw.WriteLine("\r\n**Reference Frame:** " + w.ReferenceFrame);
                string frameType = (w.FramePose != null) ? w.FramePose.GetType().Name : "Unspecified";
                if (w.FramePose != null && frameType == "Basic")
                {
                    sw.WriteLine("\r\n**Frame Pose:** " + ((GeoPose.Basic)w.FramePose).ToJSON());
                }
                else if (w.FramePose != null && frameType == "Advanced")
                {
                    sw.WriteLine("\r\n**Frame Pose:** " + ((GeoPose.Advanced)w.FramePose).ToJSON());
                }
                else
                {
                    sw.WriteLine("\r\n**Frame Pose:** " + "\"unrecognize type\"" + frameType); ;
                }
                sw.WriteLine("\r\n**Contained Entities:** ");
                sw.WriteLine("\r\n| Name | ID | Semantic Class |GeoPose |");
                sw.WriteLine("| ----------- | ----------- | ----------- | ----------- |");
                w.ListElementsAsMarkDown(sw);
                sw.WriteLine("\r\n---\r\n");
            }
            //sw.WriteLine("\r\n# Background World\r\n");
            //Background.ListElementsAsMarkDown(sw);
            //sw.WriteLine("\r\n# Foreground World\r\n");
            //Foreground.ListElementsAsMarkDown(sw);
            //sw.WriteLine("\r\n# Virtual World\r\n");
            //VirtualParts.ListElementsAsMarkDown(sw);
            */
            sw.Write(footer);
            sw.Close();
        }
        /*
         * 
 {
	"extensionsUsed": [
		"OGC_Geo_Semantic_Replica"
	],
	"extensionsRequired": [
		"OGC_Geo_Semantic_Replica"
	],
	"asset": {
		"generator": "SGR00",
		"version": "2.0"
	},

         * 
         */
        public void GenerateglTF(string fileName)
        {
            string header = " {" + " {\r\n" +
                "\"extensionsUsed\": [" + " {\r\n" +
                "   \"OGC_Geo_Semantic_Replica\"" + " {\r\n" +
                "]," + " {\r\n" +
                "\"extensionsRequired\": [" + " {\r\n" +
                "    \"OGC_Geo_Semantic_Replica\"" + " {\r\n" +
                "]," + " {\r\n" +
                "\"asset\": {" + " {\r\n" +
                "            \"generator\": \"SGR00\"," + " {\r\n" +
                "	\"version\": \"2.0\"" + " {\r\n" +
                "}\r\n";
            string footer = "}\r\n";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            StreamWriter sw = new StreamWriter(fileName);
            sw.Write(header);
            /*
            sw.WriteLine("Created: " + OmniVerse.Now.ToString() + " UTC");
            sw.WriteLine("\r\n---\r\n");
            sw.WriteLine("## Component Worlds");
            sw.WriteLine("\r\n---\r\n");
            // loop through worlds
            foreach (World w in WorldSet)
            {
                sw.WriteLine("\r\n**Name:** " + w.Name);
                sw.WriteLine("\r\n**Type:** " + w.GetType().ToString());
                sw.WriteLine("\r\n**Reference Frame:** " + w.ReferenceFrame);
                string frameType = (w.FramePose != null) ? w.FramePose.GetType().Name : "Unspecified";
                if (w.FramePose != null && frameType == "Basic")
                {
                    sw.WriteLine("\r\n**Frame Pose:** " + ((GeoPose.Basic)w.FramePose).ToJSON());
                }
                else if (w.FramePose != null && frameType == "Advanced")
                {
                    sw.WriteLine("\r\n**Frame Pose:** " + ((GeoPose.Advanced)w.FramePose).ToJSON());
                }
                else
                {
                    sw.WriteLine("\r\n**Frame Pose:** " + "\"unrecognize type\"" + frameType); ;
                }
                sw.WriteLine("\r\n**Contained Entities:** ");
                sw.WriteLine("\r\n| Name | ID | Semantic Class |GeoPose |");
                sw.WriteLine("| ----------- | ----------- | ----------- | ----------- |");
                w.ListElementsAsMarkDown(sw);
                sw.WriteLine("\r\n---\r\n");
            }
            //sw.WriteLine("\r\n# Background World\r\n");
            //Background.ListElementsAsMarkDown(sw);
            //sw.WriteLine("\r\n# Foreground World\r\n");
            //Foreground.ListElementsAsMarkDown(sw);
            //sw.WriteLine("\r\n# Virtual World\r\n");
            //VirtualParts.ListElementsAsMarkDown(sw);
            */
            sw.WriteLine(footer);
            sw.Close();
        }
    }
}