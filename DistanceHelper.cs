﻿using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pavlyshyn.Vitalii.RobotChallange
{
    class DistanceHelper
    {
        public static int FindDistance(Position a, Position b)
        {
            return (int)(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
    }
}
