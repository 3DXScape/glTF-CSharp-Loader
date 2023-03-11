// See https://aka.ms/new-console-template for more information
using System;

using Affordances;
using Verses;
using Entities;
using GeoPose;
using g4;
// 50.93813907737537, -1.4706584849810118
//TerrainComponents.TerrainInfo results = await TerrainComponents.TerrainComponents.GetTerrainComponents("OS Southampton HQ", "c:/temp/models/world", 50.93765028067923, -1.4696398272318714, 19.08, 256.0);

const int nCars = 4;
const int nPersons = 8;
const int nBuildings = 16;
const int nStreets = 8;
const int nSignals = 7;
const double lat = 50.93813907737537;
const double lon = -1.47065848498101186;
const double h = -114.0;
const double yaw = 0.0;
const double pitch = -90.0;
const double roll = 0.0;
const double size = 256.0;

IntegratedWorld myWorld = new IntegratedWorld("OS Southampton HQ");
myWorld.Uri = "c:/temp/models/world";

GeoPose.BasicYPR myIntegratedFrame = new GeoPose.BasicYPR();
myIntegratedFrame.Position.lat = 50.93765028067923;
myIntegratedFrame.Position.lon = -1.4696398272318714;
myIntegratedFrame.Position.h = 19.08;
myIntegratedFrame.Orientation.yaw = yaw;
myIntegratedFrame.Orientation.pitch = pitch;
myIntegratedFrame.Orientation.roll = roll;
myWorld.OmniVerse = new OutsideOfAnyWorld();
myWorld.FramePose = myIntegratedFrame;
myWorld.Size = new SharedGeometry.Distance();
myWorld.Size.Value = size;
// add interfaces to OmniVerse

StaticWorld myBackground = new StaticWorld();
myBackground.Name = "Background";
SharedGeometry.Distance wSize = new SharedGeometry.Distance();
wSize.Value = myWorld.Size.Value;
myBackground.Size = wSize;

GeoPose.BasicYPR myBackgroundFrame = new GeoPose.BasicYPR();
myBackgroundFrame.Position.lat = myIntegratedFrame.Position.lat;
myBackgroundFrame.Position.lon = myIntegratedFrame.Position.lon;
myBackgroundFrame.Position.h = myIntegratedFrame.Position.h;
myBackground.FramePose = myBackgroundFrame;
// Temporary placeholder for ENUPose
GeoPose.ENUPose pose;

// add entity to world, with name, ID, parent entity, semantic class, template, parameters

// add entities to background
Entity boundingSphere = new Entity(myBackground, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, "Bounding Sphere", new SemanticClasses.BoundingSphere());
boundingSphere.Material = boundingSphere.SemanticEntityClass.Material;
boundingSphere.Meshes.Add(SemanticClasses.BoundingSphere.Generate(new Tuple<double, double, double>(0.0, 0.0, 0.0), myBackground.Size.Value));
myBackground.AddEntity(boundingSphere);

Entity earthSurface = new Entity(myBackground, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, "Terrain", new SemanticClasses.LandSurface());
earthSurface.Material = earthSurface.SemanticEntityClass.Material;
SharedGeometry.Mesh aMesh = await SemanticClasses.LandSurface.Generate(new Tuple<double, double, double>(0.0, 0.0, 0.0), myBackground.Size.Value);
if (aMesh != null)
{
    earthSurface.Meshes.Add(aMesh);
}
earthSurface.SemanticEntityClass = new SemanticClasses.LandSurface();
myBackground.AddEntity(earthSurface);

for (int nBuilding = 0; nBuilding < nBuildings; nBuilding++)
{
    Entity aBuilding = new Entity();
    aBuilding.Name = "Building " + (nBuilding + 0).ToString();

    pose = new GeoPose.ENUPose();
    pose.Position.East = 0.0;
    pose.Position.North = 0.0;
    pose.Position.Up = 0.0;
    pose.Orientation.yaw = 0.0;
    pose.Orientation.pitch = 0.0;
    pose.Orientation.roll = 0.0;
    aBuilding.Pose = pose;

    aBuilding.SemanticEntityClass = new SemanticClasses.Building();
    myBackground.AddEntity(aBuilding);
}
for (int nStreet = 0; nStreet < nStreets; nStreet++)
{
    // add streets
    Entity aStreet = new Entity();
    aStreet.Name = "Street " + (nStreet + 1).ToString();

    pose = new GeoPose.ENUPose();
    pose.Position.East = 0.0;
    pose.Position.North = 0.0;
    pose.Position.Up = 0.0;
    pose.Orientation.yaw = 0.0;
    pose.Orientation.pitch = 0.0;
    pose.Orientation.roll = 0.0;
    aStreet.Pose = pose;

    // add sidewalks
    aStreet.SemanticEntityClass = new SemanticClasses.Road();
    myBackground.AddEntity(aStreet);
    Entity aSidewalk = new Entity();
    aSidewalk.Name = "Walkway " + (nStreet*2 + 0).ToString();

    pose = new GeoPose.ENUPose();
    pose.Position.East = 0.0;
    pose.Position.North = 0.0;
    pose.Position.Up = 0.0;
    pose.Orientation.yaw = 0.0;
    pose.Orientation.pitch = 0.0;
    pose.Orientation.roll = 0.0;
    aSidewalk.Pose = pose;

    aSidewalk.SemanticEntityClass = new SemanticClasses.WalkWay();
    myBackground.AddEntity(aSidewalk);
    aSidewalk = new Entity();
    aSidewalk.Name = "Walkway " + (nStreet*2 + 1).ToString();

    pose = new GeoPose.ENUPose();
    pose.Position.East = 0.0;
    pose.Position.North = 0.0;
    pose.Position.Up = 0.0;
    pose.Orientation.yaw = 0.0;
    pose.Orientation.pitch = 0.0;
    pose.Orientation.roll = 0.0;
    aSidewalk.Pose = pose;
    aSidewalk.SemanticEntityClass = new SemanticClasses.WalkWay();
    myBackground.AddEntity(aSidewalk);
}
// add signals
for (int nSignal = 0; nSignal < nSignals; nSignal++)
{
    Entity aSignal = new Entity();
    aSignal.Name = "Signal " + (nSignal + 1).ToString();

    pose = new GeoPose.ENUPose();
    pose.Position.East = 0.0;
    pose.Position.North = 0.0;
    pose.Position.Up = 0.0;
    pose.Orientation.yaw = 0.0;
    pose.Orientation.pitch = 0.0;
    pose.Orientation.roll = 0.0;
    aSignal.Pose = pose;

    aSignal.SemanticEntityClass = new SemanticClasses.Signal();
    myBackground.AddEntity(aSignal);
}
myWorld.AddWorld(myBackground);

// ====== add foreground world
DynamicWorld myForeground = new DynamicWorld();
myForeground.Name = "Foreground";

GeoPose.BasicYPR myForegroundFrame = new GeoPose.BasicYPR();
myForegroundFrame.Position.lat = lat;
myForegroundFrame.Position.lon = lon;
myForegroundFrame.Position.h = h;
myForeground.FramePose = myForegroundFrame;
// === add rider
Entity riderPerson = new Entity();
riderPerson.Name = "Rider";

pose = new GeoPose.ENUPose();
pose.Position.East = 0.0;
pose.Position.North = 0.0;
pose.Position.Up = 0.0;
pose.Orientation.yaw = 0.0;
pose.Orientation.pitch = 0.0;
pose.Orientation.roll = 0.0;
riderPerson.Pose = pose;

riderPerson.SemanticEntityClass = new SemanticClasses.Person();
myForeground.AddEntity(riderPerson);

// === add driver
Entity driverPerson = new Entity();
driverPerson.Name = "Driver";

pose = new GeoPose.ENUPose();
pose.Position.East = 0.0;
pose.Position.North = 0.0;
pose.Position.Up = 0.0;
pose.Orientation.yaw = 0.0;
pose.Orientation.pitch = 0.0;
pose.Orientation.roll = 0.0;
driverPerson.Pose = pose;

driverPerson.SemanticEntityClass = new SemanticClasses.Person();
myForeground.AddEntity(driverPerson);
// === add random people
for (int nPerson = 0; nPerson < nPersons; nPerson++)
{
    Entity aPerson = new Entity();
    aPerson.Name = "Non-participant Person " + (nPerson + 1).ToString();

    pose = new GeoPose.ENUPose();
    pose.Position.East = 0.0;
    pose.Position.North = 0.0;
    pose.Position.Up = 0.0;
    pose.Orientation.yaw = 0.0;
    pose.Orientation.pitch = 0.0;
    pose.Orientation.roll = 0.0;
    aPerson.Pose = pose;

    aPerson.SemanticEntityClass = new SemanticClasses.Person();
    myForeground.AddEntity(aPerson);
}
// === add cars
Entity rideCar = new Entity();
rideCar.Name = "Ride Car";

pose = new GeoPose.ENUPose();
pose.Position.East = 0.0;
pose.Position.North = 0.0;
pose.Position.Up = 0.0;
pose.Orientation.yaw = 0.0;
pose.Orientation.pitch = 0.0;
pose.Orientation.roll = 0.0;
rideCar.Pose = pose;

rideCar.SemanticEntityClass = new SemanticClasses.Car();
myForeground.AddEntity(rideCar);

for(int nCar = 0;nCar < nCars; nCar++)
{
    Entity aCar = new Entity();
    aCar.Name = "Non-participant Car " + (nCar+1).ToString();

    pose = new GeoPose.ENUPose();
    pose.Position.East = 0.0;
    pose.Position.North = 0.0;
    pose.Position.Up = 0.0;
    pose.Orientation.yaw = 0.0;
    pose.Orientation.pitch = 0.0;
    pose.Orientation.roll = 0.0;
    aCar.Pose = pose;

    aCar.SemanticEntityClass = new SemanticClasses.Car();
    myForeground.AddEntity(aCar);
}
myWorld.AddWorld(myForeground);

// ====== add earth inertial world - NASA SPICE J2000
DynamicWorld myEarthCenteredInertial = new DynamicWorld();
myEarthCenteredInertial.Name = "EarthCenteredInertial";

GeoPose.Advanced myEarthCenteredInertialFrame = new GeoPose.Advanced();
myEarthCenteredInertialFrame.Position.authority = "https://naif.jpl.nasa.gov/naif/";
myEarthCenteredInertialFrame.Position.id = "J2000";
myEarthCenteredInertialFrame.Position.parameters = "";
myEarthCenteredInertial.FramePose = myEarthCenteredInertialFrame;
myWorld.AddWorld(myEarthCenteredInertial);

// ====== add virtual world for virtual props
VirtualWorld myVirtualParts = new VirtualWorld();
myVirtualParts.Name = "Virtual";

GeoPose.BasicYPR myVirtualPartsFrame = new GeoPose.BasicYPR();
myVirtualPartsFrame.Position.lat = lat;
myVirtualPartsFrame.Position.lon = lon;
myVirtualPartsFrame.Position.h = h;
myVirtualParts.FramePose = myVirtualPartsFrame;

Entity carSign = new Entity();
carSign.Name = "Virtual Sign Over Ride Car";

pose = new GeoPose.ENUPose();
pose.Position.East = 0.0;
pose.Position.North = 0.0;
pose.Position.Up = 0.0;
pose.Orientation.yaw = 0.0;
pose.Orientation.pitch = 0.0;
pose.Orientation.roll = 0.0;
carSign.Pose = pose;

carSign.SemanticEntityClass = new SemanticClasses.Sign();
myVirtualParts.AddEntity(carSign);
myWorld.AddWorld(myVirtualParts);
DateTime now = DateTime.Now;
string basePath = "c:/temp/models/world/";

string instanceID = now.Year.ToString("d4") + "." + now.Month.ToString("d2") + "." + now.Day.ToString("d2") + "." +
    now.Hour.ToString("d2") + "." + now.Minute.ToString("d2") + "." + now.Second.ToString("d2");
string mdName = basePath + instanceID + ".md";
string jsonName = basePath + instanceID + ".json";
string gltfName = basePath + instanceID + ".gltf";
myWorld.ListElementsAsMarkDown(mdName);
myWorld.SaveAsJSON(jsonName);
myWorld.GenerateglTF(basePath, instanceID);
//myWorld.Render2glTF("r_" + gltfName);
