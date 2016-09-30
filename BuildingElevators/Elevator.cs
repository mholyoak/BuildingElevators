using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingElevators
{
    abstract public class ElevatorNotification
    {
        public ElevatorNotification()
        {

        }

        abstract public void OnElevatorArrived(int floor);
        abstract public void OnElevatorFloorChange(int floor);

        public static void OnElevatorArrived(ElevatorNotification obj, int floor)
        {
            obj.OnElevatorArrived(floor);
        }

        public static void OnElevatorFloorChange(ElevatorNotification obj, int floor)
        {
            obj.OnElevatorFloorChange(floor);
        }
    }

    public class Elevator
    {
        object mCriticalSection = new object();
        EventWaitHandle mArrivedDestination = new EventWaitHandle(false, EventResetMode.ManualReset);
        ElevatorNotification mArrivedNotifier;
        bool mRun = false;
        Task mTask = null;
        int mNumberFloors = 0;
        int mCurrentFloor = 0;
        SortedList<int, int> mDestinationFloors = new SortedList<int, int>();
        bool mDoorsOpen = false;
        enum ElevatorState { idleState, openDoorsState, openDoorsPauseState, closeDoorsState, transportingState };
        ElevatorState mElevatorState = ElevatorState.idleState;

        public Elevator(ElevatorNotification arrivedNotifier, int numberFloors)
        {
            mArrivedNotifier = arrivedNotifier;
            mRun = true;
            mNumberFloors = numberFloors;

            PowerOn();
        }

        ~Elevator()
        {
            PowerOff();
        }

        public void Request(int floor, bool upRequest)
        {
            //throw new ArgumentException("Invalid floor")
            lock (mCriticalSection)
            {
                mDestinationFloors.Add(floor, floor);
            }
        }

        public void AddDestination(int floor)
        {
            //throw new ArgumentException("Invalid floor")
            lock (mCriticalSection)
            {
                mDestinationFloors.Add(floor, floor);
            }
        }

        public bool AreDoorsOpen()
        {
            return mDoorsOpen;
        }

        public void CloseDoors()
        {
            lock (mCriticalSection)
            {
                mDoorsOpen = false;
            }
        }

        public bool WaitDestination(int floor, TimeSpan waitTime)
        {
            bool isAtFloor = false;

            mArrivedDestination.Reset();

            lock (mCriticalSection)
            {
                isAtFloor = ((mCurrentFloor == floor) & mDoorsOpen);
            }

            if (isAtFloor == false)
            {
                if (mArrivedDestination.WaitOne(waitTime))
                {
                    lock (mCriticalSection)
                    {
                        isAtFloor = ((mCurrentFloor == floor) & mDoorsOpen);
                    }
                }
            }

            return isAtFloor;
        }

        public int GetCurrentFloor()
        {
            return mCurrentFloor;
        }

        public bool OpenDoors()
        {
            throw new NotImplementedException();
        }

        public bool SetAlarm()
        {
            throw new NotImplementedException();
        }

        public bool IsIdle()
        {
            throw new NotImplementedException();
        }

        public void SetFloorElevatorServices(List<int> listServicingFloors)
        {
            throw new NotImplementedException();
        }

        public enum Direction { idleDirection, upDirection, downDirection};

        public Direction GetDirection()
        {
            throw new NotImplementedException();
        }

        void PowerOn()
        {
            lock (mCriticalSection)
            {
                mRun = true;

                mTask = new Task(RunElevator);

                mTask.Start();
            }
        }

        void PowerOff()
        {
            lock (mCriticalSection)
            {
                mRun = false;
            }
        }

        void RunElevator()
        {
            TimeSpan timeIdel = new TimeSpan(0, 0, 0, 0, 1);
            TimeSpan timeMoveOneFloor = new TimeSpan(0, 0, 0, 1);
            TimeSpan timeOpenDoors = new TimeSpan(0, 0, 0, 0, 500);
            TimeSpan timeCloseDoors = new TimeSpan(0, 0, 0, 0, 500);
            TimeSpan timeDoorsStayOpen = new TimeSpan(0, 0, 0, 0, 500);

            while (mRun)
            {
                switch (mElevatorState)
                {
                    case ElevatorState.idleState:
                        {
                            lock (mCriticalSection)
                            {
                                if (mDestinationFloors.Count() > 0)
                                {
                                    mElevatorState = ElevatorState.transportingState;
                                }
                            }

                            Thread.Sleep(timeIdel);
                        }
                        break;
                        
                    case ElevatorState.openDoorsState:
                        {
                            Thread.Sleep(timeOpenDoors);

                            lock (mCriticalSection)
                            {
                                mDoorsOpen = true;
                                mElevatorState = ElevatorState.openDoorsPauseState;
                                mArrivedDestination.Set();
                            }

                            ElevatorNotification.OnElevatorArrived(mArrivedNotifier, mCurrentFloor);
                        }
                        break;

                    case ElevatorState.openDoorsPauseState:
                        {
                            Thread.Sleep(timeDoorsStayOpen);

                            lock (mCriticalSection)
                            {
                                mElevatorState = ElevatorState.closeDoorsState;
                            }
                        }
                        break;

                    case ElevatorState.closeDoorsState:
                        {
                            Thread.Sleep(timeCloseDoors);

                            lock (mCriticalSection)
                            {
                                mDoorsOpen = false;

                                if (mDestinationFloors.Count() > 0)
                                {
                                    mElevatorState = ElevatorState.transportingState;
                                }
                                else
                                {
                                    mElevatorState = ElevatorState.idleState;
                                }
                            }
                        }
                        break;

                    case ElevatorState.transportingState:
                        {
                            Thread.Sleep(timeMoveOneFloor);

                            lock (mCriticalSection)
                            {
                                int index = mDestinationFloors.Count() - 1;
                                mCurrentFloor = mDestinationFloors.ElementAt(index).Value;
                                mDestinationFloors.RemoveAt(index);
                                mElevatorState = ElevatorState.openDoorsState;
                            }
                        }
                        break;
                }
            }
        }
    }
}
