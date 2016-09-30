using Microsoft.VisualStudio.TestTools.UnitTesting;
using BuildingElevators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingElevators.Tests
{
    [TestClass()]
    public class BuildingTests
    {
        [TestMethod()]
        public void TestSingleElevatorRequestOnly()
        {
            Building building = new Building(10, 1);

            BuildingSingleRequestElevator(building, 4);
        }

        Elevator BuildingSingleRequestElevator(Building building, int requestFloor)
        {
            TimeSpan waitTime = new TimeSpan(0, 0, 0, 3);
            building.RequestElevator(requestFloor, true);
            
            Elevator myElevator = building.WaitForElevator(requestFloor, waitTime);
            Assert.AreEqual(requestFloor, myElevator.GetCurrentFloor());
            Assert.IsTrue(myElevator.AreDoorsOpen());

            return myElevator;
        }

        [TestMethod()]
        public void TestSingleElevatorSingleRequest()
        {
            Building myBuilding = new Building(10, 1);

            Elevator myElevator = BuildingSingleRequestElevator(myBuilding, 4);

            const int destinationFloor = 1;
            TimeSpan waitTime = new TimeSpan(0, 0, 0, 3);

            myElevator.AddDestination(destinationFloor);
            myElevator.WaitDestination(destinationFloor, waitTime);
            Assert.AreEqual(destinationFloor, myElevator.GetCurrentFloor());
            Assert.IsTrue(myElevator.AreDoorsOpen());
        }

        [TestMethod()]
        public void TestSingleElevatorMultipleDestinations()
        {
            Building myBuilding = new Building(20, 1);

            Elevator myElevator = BuildingSingleRequestElevator(myBuilding, 20);

            int firstDestinationAdded = 1;
            int secondDestinationAdded = 15;
            int thirdDestinationAdded = 9;
            TimeSpan waitTime = new TimeSpan(0, 0, 0, 3);

            // Add destinations in different order than the elevator should stop
            myElevator.AddDestination(firstDestinationAdded);
            myElevator.AddDestination(secondDestinationAdded);
            myElevator.AddDestination(thirdDestinationAdded);

            WaitAndVerifyDestination(myElevator, secondDestinationAdded, waitTime);

            myElevator.CloseDoors();
            Assert.IsFalse(myElevator.AreDoorsOpen());

            WaitAndVerifyDestination(myElevator, thirdDestinationAdded, waitTime);

            myElevator.CloseDoors();
            Assert.IsFalse(myElevator.AreDoorsOpen());

            WaitAndVerifyDestination(myElevator, firstDestinationAdded, waitTime);
        }

        void WaitAndVerifyDestination(Elevator myElevator, int destination, TimeSpan waitTime)
        {
            myElevator.WaitDestination(destination, waitTime);
            Assert.AreEqual(destination, myElevator.GetCurrentFloor());
            Assert.IsTrue(myElevator.AreDoorsOpen());
        }
    }
}