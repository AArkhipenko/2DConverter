using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorelovskiy.ru_3._0_Console.Model
{
    public class GeneralInfoModel
    {
        public double _spindel_length { get; private set; }
        public Point _max_point { get; private set; }
        public Point _min_point { get; private set; }
        public Point _start_scan_point { get; private set; }
        public GeneralInfoModel(double spindel_length, Point max_point, Point min_point, Point start_point_scan)
        {
            this._spindel_length = spindel_length;
            this._max_point = new Point(max_point);
            this._min_point = new Point(min_point);
            this._start_scan_point = new Point(start_point_scan);
        }

        public class Point
        {
            public Point(double x, double y, double z)
            {
                this._x = x;
                this._y = y;
                this._z = z;
            }
            public Point(Point point)
            {
                this._x = point._x;
                this._y = point._y;
                this._z = point._z;
            }
            public double _x { get; private set; }
            public double _y { get; private set; }
            public double _z { get; private set; }
        }
    }
}
