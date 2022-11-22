//********************************************************************************************
//        Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;

namespace Cliver
{
    public class Geometry
    {
        public class PointD
        {
            public double X;
            public double Y;
        }

        public static PointD GetIntersectionPoint(PointD a1, PointD a2, PointD b1, PointD b2)
        {
            double d = (a1.X - a2.X) * (b1.Y - b2.Y) - (a1.Y - a2.Y) * (b1.X - b2.X);
            //if (Approximately(d, epsilon))//the lines are parallel
            //return false;
            double pX = (a1.X * a2.Y - a1.Y * a2.X) * (b1.X - b2.X) - (a1.X - a2.X) * (b1.X * b2.Y - b1.Y * b2.X);
            double pY = (a1.X * a2.Y - a1.Y * a2.X) * (b1.Y - b2.Y) - (a1.Y - a2.Y) * (b1.X * b2.Y - b1.Y * b2.X);
            return new PointD { X = pX / d, Y = pY / d };
        }
    }
}