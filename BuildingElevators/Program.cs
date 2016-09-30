using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingElevators
{
    class Program
    {
        static void Main(string[] args)
        {
            Building building = new Building(20, 2);

            const int requestFloor = 20;
            TimeSpan waitTime = new TimeSpan(0, 0, 0, 3);
            building.RequestElevator(requestFloor, true);

            Elevator myElevator = building.WaitForElevator(requestFloor, waitTime);
            Console.WriteLine(String.Format("Arrived at floor {0}", myElevator.GetCurrentFloor()));

            int firstDestinationAdded = 1;
            int secondDestinationAdded = 15;
            int thirdDestinationAdded = 9;

            // Add destinations in different order than the elevator should stop
            myElevator.AddDestination(firstDestinationAdded);
            myElevator.AddDestination(secondDestinationAdded);
            myElevator.AddDestination(thirdDestinationAdded);

            myElevator.WaitDestination(secondDestinationAdded, waitTime);
            Console.WriteLine(String.Format("Arrived at floor {0}", myElevator.GetCurrentFloor()));

            myElevator.WaitDestination(thirdDestinationAdded, waitTime);
            Console.WriteLine(String.Format("Arrived at floor {0}", myElevator.GetCurrentFloor()));

            myElevator.WaitDestination(firstDestinationAdded, waitTime);
            Console.WriteLine(String.Format("Arrived at floor {0}", myElevator.GetCurrentFloor()));

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }
    }
}
