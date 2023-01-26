using GeoPoseX;
/*
{
                     "id": "http://example.com/nodes/MarsExpress/1/Passengers/Josh",
                     "position" : { "x": 2.0, "y": 0.8, "z": -6 },
                     "angles": { "yaw": 1, "pitch": 2, "roll" : 0 }
                  },
                  {
                     "id": "http://example.com/nodes/MarsExpress/1/Passengers/Jerome",
                     "position" : { "x": 2.2, "y": 0.8, "z": -7 },
                     "angles": { "yaw": -1, "pitch": 1, "roll" : 0 }
                  }

                     "id": "http://example.com/nodes/MarsExpress/1/Passengers/Steve",
                     "position" : { "x": -5, "y": 0.82, "z": 6 },
                     "angles": { "yaw": -2, "pitch": 1.5, "roll" : 0 }

  "id": "http://example.com/nodes/MarsExpress/1/Wagons/1",
               "position" : { "lat" : 30, "lon": 50, "h": 25 },
               "angles" : { "yaw": 30, "pitch": 5, "roll" : -0.5 },

            "id": "http://example.com/nodes/MarsExpress/1/Wagons/3",
               "position" : { "lat" : 29.998, "lon": 50.004, "h": 24.5 },
               "angles" : { "yaw": 31, "pitch": 5.2, "roll" : -0.4 },

 */

// maxs express in icrf2
Advanced marsExpress = new Advanced(new PoseID("http://example.com/nodes/MarsExpress/1"),
    new Extrinsic("https://www.iers.org/", "icrf3", "{\"x\": 1234567890.9876,\"y\": 2345678901.8765, \"z\": 3456789012.7654}"),
    new Quaternion(0,0,0,1));
marsExpress.validTime = new UnixTime(1674767748003);
// four wagons local to mars express
List<Local> wagons = new List<Local>();
Local wagon1 = new Local("http://example.com/nodes/MarsExpress/1/Wagons/1", new Translation( 2.2, 0.82, -7.0), new YPRAngles(0.2, 0.0, 23.0));
wagons.Add(wagon1);
Local wagon2 = new Local("http://example.com/nodes/MarsExpress/1/Wagons/2", new Translation(12.2, 0.78, -7.0), new YPRAngles(0.2, 0.0, 23.4));
wagons.Add(wagon2);
Local wagon3 = new Local("http://example.com/nodes/MarsExpress/1/Wagons/3", new Translation(22.5, 0.77, -7.0), new YPRAngles(0.2, 0.0, 21.0));
wagons.Add(wagon3);
Local wagon4 = new Local("http://example.com/nodes/MarsExpress/1/Wagons/4", new Translation(33.2, 0.74, -7.0), new YPRAngles(0.2, 0.0, 42.0));
wagons.Add(wagon4);
// passengers from the SWG in wagons 1 and 3 local to each wagon
List<Local> passengers = new List<Local>();
Local jerome = new Local("http://example.com/nodes/MarsExpress/1/Passengers/Jerome", new Translation(2.2, 0.8, -7.0), new YPRAngles(180.0, 1.0, 0.0));
passengers.Add(jerome);
Local josh = new Local("http://example.com/nodes/MarsExpress/1/Passengers/Josh", new Translation(2.0, 0.8, -6.0), new YPRAngles(180.0, 2.0, 0.0));
passengers.Add(josh);
Local steve = new Local("http://example.com/nodes/MarsExpress/1/Passengers/Steve", new Translation(-5.0, 0.82, 6.0), new YPRAngles(-2.0, 1.5, 0.0));
passengers.Add(steve);
// add wagons to mars express
wagon1.parentPoseID = marsExpress.poseID;
wagon2.parentPoseID = marsExpress.poseID;
wagon3.parentPoseID = marsExpress.poseID;
wagon4.parentPoseID = marsExpress.poseID;
// add passengers to wagons
jerome.parentPoseID = wagon1.poseID;
josh.parentPoseID = wagon1.poseID;
steve.parentPoseID = wagon3.poseID;
// show pose tree
Console.WriteLine("Mars Express at Local Clock UNIX Time " + marsExpress.validTime.timeValue);
Console.WriteLine(marsExpress.ToJSON(""));
foreach (GeoPoseX.GeoPose wagon in wagons)
{
    Console.WriteLine("Wagon: " + wagon.poseID.id.Substring(1 + wagon.poseID.id.LastIndexOf('/')));
    Console.WriteLine(wagon.ToJSON(""));
    foreach(Local passenger in passengers)
    {
        if(passenger.parentPoseID.id == wagon.poseID.id)
        {
            Console.WriteLine("Passenger: " + passenger.poseID.id.Substring(1 + passenger.poseID.id.LastIndexOf('/')));
            Console.WriteLine(passenger.ToJSON(""));
        }
    }
}


// show intermediate and final pose trees
/*
BasicYPR marsExpress_1 = new BasicYPR("http://example.com/nodes/MarsExpress/1/Wagons/1", new GeodeticPosition(30.0, 50.0, 25.0), new YPRAngles(30.0, 5.0, -0.5));


BasicYPR marsExpress_1 = new BasicYPR("http://example.com/nodes/MarsExpress/1/Wagons/1", new GeodeticPosition(30.0, 50.0, 25.0), new YPRAngles(30.0, 5.0, -0.5));

BasicYPR marsExpress_3 = new BasicYPR("http://example.com/nodes/MarsExpress/1/Wagons/3", new GeodeticPosition(29.998, 50.004, 24.5), new YPRAngles(31.0, 5.2, -0.4));
Local nodeJerome = new Local("http://example.com/nodes/MarsExpress/1/Passengers/Jerome", new Translation( 2.2, 0.8 , -7.0), new YPRAngles(-1.0, 1.0, 0.0));
nodeJerome.parentPoseID = marsExpress_1.poseID;
Local nodeJosh   = new Local("http://example.com/nodes/MarsExpress/1/Passengers/Josh"  , new Translation( 2.0, 0.8 , -6.0), new YPRAngles( 1.0, 2.0, 0.0));
nodeJosh.parentPoseID = marsExpress_1.poseID;
Local nodeSteve  = new Local("http://example.com/nodes/MarsExpress/1/Passengers/Steve" , new Translation(-5.0, 0.82,  6.0), new YPRAngles(-2.0, 1.5, 0.0));
nodeSteve.parentPoseID = marsExpress_3.poseID;

Console.WriteLine(marsExpress_1.ToJSON());
*/


