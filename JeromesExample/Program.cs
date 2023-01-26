using GeoPoseX;
// Create Mars Express in the current International Celestial Reference Frame ICRF2 
Advanced marsExpress = new Advanced(new PoseID("http://example.com/nodes/MarsExpress/1"),
    new Extrinsic("https://www.iers.org/", "icrf3", "{\"x\": 1234567890.9876,\"y\": 2345678901.8765, \"z\": 3456789012.7654}"),
    new Quaternion(0,0,0,1));
marsExpress.validTime = new UnixTime(1674767748003);

// Create four wagons in frames local to Mars Express and remember them in a wagon list
List<Local> wagons = new List<Local>();
Local wagon1 = new Local("http://example.com/nodes/MarsExpress/1/Wagons/1", new Translation( 2.2, 0.82, -7.0), new YPRAngles(0.2, 0.0, 23.0));
wagons.Add(wagon1);
Local wagon2 = new Local("http://example.com/nodes/MarsExpress/1/Wagons/2", new Translation(12.2, 0.78, -7.0), new YPRAngles(0.2, 0.0, 23.4));
wagons.Add(wagon2);
Local wagon3 = new Local("http://example.com/nodes/MarsExpress/1/Wagons/3", new Translation(22.5, 0.77, -7.0), new YPRAngles(0.2, 0.0, 21.0));
wagons.Add(wagon3);
Local wagon4 = new Local("http://example.com/nodes/MarsExpress/1/Wagons/4", new Translation(33.2, 0.74, -7.0), new YPRAngles(0.2, 0.0, 42.0));
wagons.Add(wagon4);

// Create passengers from the SWG in wagons 1 and 3 in local frames local to specific wagons and remember them in a passenger list
List<GeoPoseX.GeoPose> passengers = new List<GeoPoseX.GeoPose>();
//  - Jerome is a clever thinker who has many questions and good ideas
Local jerome = new Local("http://example.com/nodes/MarsExpress/1/Passengers/Jerome", new Translation(2.2, 0.8, -7.0), new YPRAngles(180.0, 1.0, 0.0));
passengers.Add(jerome);
//  - Josh is a nice fellow who guided us towrd the frame transform in the early days
Local josh = new Local("http://example.com/nodes/MarsExpress/1/Passengers/Josh", new Translation(2.0, 0.8, -6.0), new YPRAngles(180.0, 2.0, 0.0));
passengers.Add(josh);
//  - Steve thinks that the Local GeoPose is needed and should be added to version 1.1.0
Local steve = new Local("http://example.com/nodes/MarsExpress/1/Passengers/Steve", new Translation(-5.0, 0.82, 6.0), new YPRAngles(-2.0, 1.5, 0.0));
passengers.Add(steve);
//  - Carl is one of Steve's multiple personalities who does not believe in using any GeoPose not in the 1.0.0 standard
Advanced carl =
    new Advanced(new PoseID("http://carlsmyth.com"),
    new Extrinsic(
        "https://ogc.org",
        "PROJCRS[\"GeoPose Local\",+GEOGCS[\"None)\"]+CS[Cartesian,3],+AXIS[\"x\",,ORDER[1],LENGTHUNIT[\"metre\",1]],+AXIS[\"y\",,ORDER[2],LENGTHUNIT[\"metre\",1]],+AXIS[\"z\",,ORDER[3],LENGTHUNIT[\"metre\",1]]+USAGE[AREA[\"+/-1000 m\"],BBOX[-1000,-1000,1000,1000],ID[\"GeoPose\",Local]]",
        "{\"x\": 1234567890.9876,\"y\": 2345678901.8765, \"z\": 3456789012.7654}"),
    new Quaternion(0.0174509, 0.0130876, -0.0002284, 0.9997621));
passengers.Add(carl);

// Reference the wagons to mars express
wagon1.parentPoseID = marsExpress.poseID;
wagon2.parentPoseID = marsExpress.poseID;
wagon3.parentPoseID = marsExpress.poseID;
wagon4.parentPoseID = marsExpress.poseID;

// Reference the passengers to their respective wagons
jerome.parentPoseID = wagon1.poseID;
josh.parentPoseID = wagon1.poseID;
steve.parentPoseID = wagon3.poseID;
carl.parentPoseID = wagon3.poseID;

// Display pose tree
Console.WriteLine("Mars Express at Local Clock UNIX Time " + marsExpress.validTime.timeValue);
Console.WriteLine(marsExpress.ToJSON(""));
foreach (GeoPoseX.GeoPose wagon in wagons)
{
    Console.WriteLine("Wagon: " + wagon.poseID.id.Substring(1 + wagon.poseID.id.LastIndexOf('/')));
    Console.WriteLine(wagon.ToJSON(""));
    foreach(GeoPoseX.GeoPose passenger in passengers)
    {
        if(passenger.parentPoseID.id == wagon.poseID.id)
        {
            Console.WriteLine("Passenger: " + passenger.poseID.id.Substring(1 + passenger.poseID.id.LastIndexOf('/')));
            Console.WriteLine(passenger.ToJSON(""));
        }
    }
}

// After a minute, Carl decides that he must split from Steve and moves to wagon 4
marsExpress.validTime = new UnixTime(1674767748003 + 60*1000);
carl.parentPoseID = wagon4.poseID;

// Display new pose tree
Console.WriteLine("Mars Express at Local Clock UNIX Time " + marsExpress.validTime.timeValue);
Console.WriteLine(marsExpress.ToJSON(""));
foreach (GeoPoseX.GeoPose wagon in wagons)
{
    Console.WriteLine("Wagon: " + wagon.poseID.id.Substring(1 + wagon.poseID.id.LastIndexOf('/')));
    Console.WriteLine(wagon.ToJSON(""));
    foreach (GeoPoseX.GeoPose passenger in passengers)
    {
        if (passenger.parentPoseID.id == wagon.poseID.id)
        {
            Console.WriteLine("Passenger: " + passenger.poseID.id.Substring(1 + passenger.poseID.id.LastIndexOf('/')));
            Console.WriteLine(passenger.ToJSON(""));
        }
    }
}


