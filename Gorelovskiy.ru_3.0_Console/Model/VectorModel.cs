using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorelovskiy.ru_3._0_Console.Model
{
    public class VectorModel
    {
        /// <summary>
        /// ккордината икс вектора
        /// </summary>
        private double _ax;
        /// <summary>
        /// ккордината игрек вектора
        /// </summary>
        private double _ay;
        /// <summary>
        /// ккордината зет вектора
        /// </summary>
        private double _az;
        /// <summary>
        /// точка начала вектора
        /// </summary>
        private PointModel _start_point;
        /// <summary>
        /// точка конца вектора
        /// </summary>
        private PointModel _end_point;
        /// <summary>
        /// конструктор модели вектора
        /// </summary>
        /// <param name="start_point">точка начало вектора</param>
        /// <param name="end_point">точка конца вектора</param>
        public VectorModel(PointModel start_point, PointModel end_point)
        {
            this._ax = end_point._x - start_point._x;
            this._ay = end_point._y - start_point._y;
            this._az = end_point._z - start_point._z;
            this._start_point = new PointModel(start_point);
            this._end_point = new PointModel(end_point);
        }
        /// <summary>
        /// Задание новой длины вектора
        /// </summary>
        /// <param name="new_length">новая длина вектора</param>
        public void SetLength(double new_length)
        {
            double old_length = this.length;

            if (old_length == 0)
            {
                this._ax = this._end_point._x - this._start_point._x;
                this._ay = this._end_point._y - this._start_point._y;
                this._az = this._end_point._z - this._start_point._z;

                old_length = this.length;
            }
            //пересчитываем длину вектора
            this._ax = this._ax * new_length / old_length;
            this._ay = this._ay * new_length / old_length;
            this._az = this._az * new_length / old_length;

            if (Math.Round(this.length,3)!= Math.Round(new_length,3))
                throw new Exception("Не удалось пересчитать вектор");
        }
        /// <summary>
        /// расчет длины ветктора при определенной координате
        /// </summary>
        /// <param name="y">коордана игрек новой точки конца вектора</param>
        /// <returns></returns>
        public double CalculateLength(double y)
        {
            var percent = (y - this._start_point._y) / this._ay;
            return this.length * percent;
        }
        /// <summary>
        /// длина вектора
        /// </summary>
        public double length
        {
            get
            {
                if (this._ax == 0 &&
                    this._ay == 0 &&
                    this._az == 0)
                    return 0;

                return Math.Sqrt(Math.Pow((double)this._ax, 2) +
                                        Math.Pow((double)this._ay, 2) +
                                        Math.Pow((double)this._az, 2));
            }
        }
        /// <summary>
        /// получение точки конца вектора
        /// </summary>
        /// <returns></returns>
        public PointModel GetEndPoint()
        {
            return new PointModel(this._ax + this._start_point._x, this._ay + this._start_point._y, this._az + this._start_point._z);
        }
    }
}
