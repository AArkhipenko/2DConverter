using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorelovskiy.ru_3._0_Console.Model
{
    public class PointModel
    {
        /// <summary>
        /// модель точки в трехмерном пространстве
        /// </summary>
        /// <param name="x">координата x</param>
        /// <param name="y">координата y</param>
        /// <param name="z">координата z</param>
        public PointModel(double x, double y, double z)
        {
            this._x = x;
            this._y = y;
            this._z = z;
        }
        /// <summary>
        /// модель точки в трехмерном пространстве
        /// </summary>
        /// <param name="x">координата x</param>
        /// <param name="y">координата y</param>
        /// <param name="z">координата z</param>
        /// <param name="a">угол склонения</param>
        public PointModel(double x, double y, double z, double a)
        {
            this._x = x;
            this._y = y;
            this._z = z;
            this._a = a;
            this._a_grad = a * 180 / Math.PI;
        }
        /// <summary>
        /// модель точки в трехмерном пространстве
        /// </summary>
        /// <param name="point">модель точки источника</param>
        public PointModel(PointModel point)
        {
            this._x = point._x;
            this._y = point._y;
            this._z = point._z;
            this._a = point._a;
            this._a_grad = point._a * 180 / Math.PI;
        }
        /// <summary>
        /// координата икс
        /// </summary>
        public double _x { get; private set; }
        /// <summary>
        /// координата игрек
        /// </summary>
        public double _y { get; private set; }
        /// <summary>
        /// координата зет
        /// </summary>
        public double _z { get; private set; }
        /// <summary>
        /// угол склонения в радинах
        /// </summary>
        public double _a { get; private set; }
        /// <summary>
        /// угол склонения в градусах
        /// </summary>
        public double _a_grad { get; private set; }

        /// <summary>
        /// сравнение двух точек и определение в каких параметрах произошло изменение
        /// </summary>
        /// <param name="other_point">вторая точка для сравнения</param>
        /// <returns></returns>
        public TransformCoordinates.Option TestDelta(PointModel other_point)
        {
            int change_x = Math.Abs(other_point._x - this._x) > 0 ? 7 : 1;
            int change_y = Math.Abs(other_point._y - this._y) > 0 ? 11 : 3;
            int change_z = Math.Abs(other_point._z - this._z) > 0 ? 13 : 5;

            int multiplication = change_x * change_y * change_z;
            return (TransformCoordinates.Option)multiplication;

        }
        /// <summary>
        /// сравнение двух точек
        /// </summary>
        /// <param name="other">вторая точка для сравнения</param>
        /// <returns></returns>
        public bool Equals(PointModel other)
        {
            if (other != null &&
                this.TestDelta(other) == TransformCoordinates.Option.NONE)
                return true;

            return false;
        }
    }
}
