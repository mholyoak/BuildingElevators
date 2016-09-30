using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingElevators
{
    public class Building : ElevatorNotification
    {
        List<Elevator> mElevator = new List<Elevator>();
        int mNumberFloors = 0;

        public Building(int numberFloors, int numberElevators)            
        {
            mNumberFloors = numberFloors;
            for (int i = 0; i < numberElevators; i++)
            {
                mElevator.Add(new BuildingElevators.Elevator(this, numberFloors));
            }
        }

        public void RequestElevator(int floorNumber, bool up)
        {
            Elevator useElevator = null;

            useElevator = mElevator[0];
            /*
            foreach (var elevator in mElevator)
            {
                if (elevator.IsIdle())
                {
                    useElevator = elevator;
                    break;
                }
            }
            */

            if (useElevator != null)
            {
                useElevator.Request(floorNumber, up);
            }
        }

        public Elevator WaitForElevator(int floor, TimeSpan waitTime)
        {
            Elevator availableElevator = null;
            Elevator useElevator = null;

            useElevator = mElevator[0];

            if (useElevator.WaitDestination(floor, waitTime))
            {
                availableElevator = useElevator;
            }

            return availableElevator;
        }

        override public void OnElevatorArrived(int floor)
        {
            // This method is intended to allow the building class to keep
            // track of what the elevators are doing are responded
            Console.WriteLine(String.Format("Elevator Arrived at floor {0}", floor));
        }

        override public void OnElevatorFloorChange(int floor)
        {
            // This method is intended to allow the building class to keep 
            // track of the elevators and do things like update display for which
            // floor and elevator is on.
            Console.WriteLine(String.Format("Elevator Located on floor {0}", floor));
        }
    }
}
