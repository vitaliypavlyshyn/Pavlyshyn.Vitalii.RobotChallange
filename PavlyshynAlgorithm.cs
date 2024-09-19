using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Robot.Common;

namespace Pavlyshyn.Vitalii.RobotChallange
{
    public class PavlyshynAlgorithm : IRobotAlgorithm
    {
        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            Robot.Common.Robot movingRobot = robots[robotToMoveIndex];
            int countFreeStations = CountFreeStations(map.Stations, robots);
            IList<Robot.Common.Robot> myRobots = GetMyRobots(robots);
            //if ((movingRobot.Energy > 360) && (myRobots.Count < (CountFreeStations(map.Stations, robots))))
            if (RoundCount <= 40)
            {
                if ((movingRobot.Energy > 360) && (myRobots.Count < 80))
                {
                    return new CreateNewRobotCommand() { NewRobotEnergy = 150 };
                }
            }

            //Position nearestStationPosition = AlgorithmMoveToStation(movingRobot, map, robots);
            Position stationPosition = FindNearestFreeStation(robots[robotToMoveIndex], map, robots);
            if (stationPosition == null)
            {
                return null;
            }

            /*if (stationPosition == movingRobot.Position && movingRobot.Energy >= 200)
            {
                if (IsNewRobotNearest(movingRobot, robots))
                {
                    Position newPosition = new Position();
                    newPosition.X = stationPosition.X + 1;
                    newPosition.Y = stationPosition.Y + 1;
                    return new MoveCommand() { NewPosition = newPosition };
                }
                else
                {
                    return new CollectEnergyCommand();
                }
            }*/
            if (stationPosition == movingRobot.Position)
            {
                return new CollectEnergyCommand();
            }
            else if (DistanceHelper.FindDistance(stationPosition, movingRobot.Position) <= movingRobot.Energy)
            {
                return new MoveCommand() { NewPosition = stationPosition };
            }
            else
            {
                Position nearestStationPosition = AlgorithmMoveToStation(movingRobot, map, robots, stationPosition);
                return new MoveCommand() { NewPosition = nearestStationPosition };
            }
        }

        public string Author => "Pavlyshyn Vitalii";


        public static IList<Robot.Common.Robot> GetMyRobots(IList<Robot.Common.Robot> robots)
        {
            IList<Robot.Common.Robot> myRobots = robots
                .Where(robot => robot.OwnerName == "Pavlyshyn Vitalii")
                .ToList();
            return myRobots;
        }

        public static int CountFreeStations(
            IList<EnergyStation> stations,
            IList<Robot.Common.Robot> robots
        )
        {
            int freeStationsCount = 0;
            foreach (var station in stations)
            {
                bool isFree = true;
                foreach (var robot in robots)
                {
                    if (robot.Position.X == station.Position.X && robot.Position.Y == station.Position.Y)
                    {
                        isFree = false;
                        break;
                    }
                }
                if (isFree)
                {
                    freeStationsCount++;
                }
            }
            return freeStationsCount;
        }

        public Position AlgorithmMoveToStation(
            Robot.Common.Robot movingRobot,
            Map map,
            IList<Robot.Common.Robot> robots,
            Position nearestFreeStation)
        {
            //var nearestFreeStation = FindNearestFreeStation(movingRobot, map, robots);
            if (nearestFreeStation != null)
            {
                if (
                    DistanceHelper.FindDistance(
                        nearestFreeStation,
                        movingRobot.Position
                    ) > movingRobot.Energy
                )
                {
                    int minLength;
                    int deltaX = Math.Abs(nearestFreeStation.X - movingRobot.Position.X);
                    int deltaY = Math.Abs(nearestFreeStation.Y - movingRobot.Position.Y);
                    if (deltaX != 0 && deltaY != 0)
                    {
                        minLength = Math.Min(deltaX, deltaY);
                        if (minLength > 3) minLength = 3;
                        if (movingRobot.Energy <= 10) minLength = 1;
                        movingRobot.Position.X += minLength;
                        movingRobot.Position.Y += minLength;
                        return movingRobot.Position;
                    }
                    else if (deltaX == 0 && deltaY != 0)
                    {
                        minLength = deltaY;
                        if (deltaY > 3) minLength = 3;
                        if (movingRobot.Energy <= 10) minLength = 1;
                        movingRobot.Position.Y += minLength;
                        return movingRobot.Position;
                    }
                    else if (deltaX != 0 && deltaY == 0)
                    {
                        minLength = deltaX;
                        if (deltaX > 3) minLength = 3;
                        if (movingRobot.Energy <= 10) minLength = 1;
                        movingRobot.Position.X += minLength;
                        return movingRobot.Position;
                    }
                }
            }
            return movingRobot.Position;
        }

        public Position FindNearestFreeStation(Robot.Common.Robot movingRobot, Map map,
            IList<Robot.Common.Robot> robots)
        {
            EnergyStation nearest = null;
            int minDistance = int.MaxValue;
            foreach (var station in map.Stations)
            {
                if (IsStationFree(station, movingRobot, robots))
                {
                    int d = DistanceHelper.FindDistance(station.Position, movingRobot.Position);
                    if (d < minDistance)
                    {
                        minDistance = d;
                        nearest = station;
                    }
                }
            }
            return nearest == null ? null : nearest.Position;
        }
        public bool IsStationFree(EnergyStation station, Robot.Common.Robot movingRobot,
            IList<Robot.Common.Robot> robots)
        {
            return IsCellFree(station.Position, movingRobot, robots);
        }
        public bool IsCellFree(Position cell, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            foreach (var robot in robots)
            {
                if (robot != movingRobot)
                {
                    if (robot.Position == cell)
                        return false;
                }
            }
            return true;
        }

        public bool IsNewRobotNearest(
            Robot.Common.Robot movingRobot,
            IList<Robot.Common.Robot> robots)
        {
            foreach (var robot in robots)
            {
                if (robot != movingRobot)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (x != 0 || y != 0)
                            {
                                var newX = movingRobot.Position.X + x;
                                var newY = movingRobot.Position.Y + y;

                                if (robot.Position.X == newX && robot.Position.Y == newY)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static int CalculateEnergy(Robot.Common.Robot robot) => robot.Energy;
        public int RoundCount { get; set; }
        private void LogRound(object sender, LogRoundEventArgs e) => ++this.RoundCount;
    }
}

