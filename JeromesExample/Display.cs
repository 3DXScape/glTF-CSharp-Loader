using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    internal static class Display
    {
        public static void Output(GeoPose.GeoPose marsExpress, List<Local> wagons, List<GeoPose.GeoPose> passengers)
        {
            Console.WriteLine("\r\n========== Mars Express at Local Clock UNIX Time " + marsExpress.validTime.timeValue + "==========\r\n");
            Console.WriteLine(marsExpress.ToJSON(""));
            foreach (GeoPose.GeoPose wagon in wagons)
            {
                Console.WriteLine("=-=-=-=-=- Wagon -=-=-=-=-=: " + wagon.poseID.id.Substring(1 + wagon.poseID.id.LastIndexOf('/')) + "\r\n");
                Console.WriteLine(wagon.ToJSON(""));
                foreach (GeoPose.GeoPose passenger in passengers)
                {
                    if (passenger.parentPoseID.id == wagon.poseID.id)
                    {
                        Console.WriteLine("---------- Passenger ----------: " + passenger.poseID.id.Substring(1 + passenger.poseID.id.LastIndexOf('/')) + "\r\n");
                        Console.WriteLine(passenger.ToJSON(""));
                    }

                }
            }
        }
    }
}
