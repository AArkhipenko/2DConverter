using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorelovskiy.ru_3._0_Console
{
    public class TransformCoordinates
    {
        private double _min_length = 5;
        private Model.PointModel _old_2d_point;
        private Option _current_option;
        public enum Option : int
        {
            NONE = 1 * 3 * 5,
            ONLY_X = 7 * 3 * 5,
            ONLY_Y = 1 * 11 * 5,
            ONLY_Z = 1 * 3 * 13,
            X_Y = 7 * 11 * 5,
            X_Z = 7 * 3 * 13,
            Y_Z = 1 * 11 * 13,
            X_Y_Z = 7 * 11 * 13,
            FIRST = 666
        }
        public TransformCoordinates()
        {
        }

        /// <summary>
        /// преобразование считанных из файла координат
        /// </summary>
        /// <param name="option">вариант комбинации координат</param>
        /// <param name="x_2d">координата икс из файла</param>
        /// <param name="y_2d">координата игрек файла</param>
        /// <param name="z_2d">координата зет (глубина реза) файла</param>
        public void TransformData(Option option, double? x_2d, double? y_2d, double? z_2d)
        {
            Model.PointModel point_2d = null;
            this._current_option = option;
            if (option != Option.NONE)
            {
                point_2d = new Model.PointModel(x_2d == null ? this._old_2d_point._x : (double)x_2d,
                                                                y_2d == null ? this._old_2d_point._y : (double)y_2d,
                                                                z_2d == null ? this._old_2d_point._z : (double)z_2d);
                if (this._old_2d_point != null)
                    this._current_option = point_2d.TestDelta(this._old_2d_point);
            }
            switch (option)
            {
                case Option.FIRST:
                    {
                        Model.PointModel point_3d = this.Calculate3DPoint(point_2d);
                        this.FinalChange(point_3d, WriteFile.MatchValue.FIRST);
                        this.ReinitOldCoordinates(point_2d);
                        break;
                    }
                case Option.ONLY_X:
                case Option.ONLY_Y:
                case Option.X_Y:
                case Option.X_Z:
                case Option.Y_Z:
                case Option.X_Y_Z:
                    {
                        this.ChooseStrategy(point_2d);
                        break;
                    }
                case Option.ONLY_Z:
                    {
                        Model.PointModel point_3d = this.Calculate3DPoint(point_2d);
                        this.FinalChange(point_3d, WriteFile.MatchValue.COORDINATES);
                        this.ReinitOldCoordinates(point_2d);
                        break;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// расчет расстояния между точками и принятие решения о дальнейших действиях
        /// </summary>
        /// <param name="x_2d">координата икс из файла</param>
        /// <param name="y_2d">координата игрек из файла</param>
        /// <param name="z_2d">координата зет (глеюина реза) из файла</param>
        private void ChooseStrategy(Model.PointModel point_2d)
        {
            double delta = Math.Sqrt(Math.Pow(point_2d._x - this._old_2d_point._x, 2) +
                                                    Math.Pow(point_2d._y - this._old_2d_point._y, 2) +
                                                    Math.Pow(point_2d._z - this._old_2d_point._z, 2));

            if (delta == 0)
                return;

            if (delta > this._min_length)
                this.GlobalChange(point_2d, delta);
            else
                this.NotGlobalChange(point_2d);
        }

        /// <summary>
        /// Расстояние между точками больше 5 мм
        /// </summary>
        /// <param name="x_2d">координата икс из файла</param>
        /// <param name="y_2d">координата игрек из файла</param>
        /// <param name="z_2d">координата зет (глеюина реза) из файла</param>
        /// <param name="delta">расстояние между предыдущей и новой точкой</param>
        private void GlobalChange(Model.PointModel point_2d, double delta)
        {
            //только вдоль
            if (this._current_option == Option.ONLY_Y)
            {
                int new_area = this.GetAreaIndex(point_2d._y);
                int old_area = this.GetAreaIndex(this._old_2d_point._y);
                if (new_area != old_area)
                {
                    //y_2d < _old_y_2d
                    if (new_area < old_area)
                        while (old_area > new_area)
                        {
                            old_area--;
                            Model.PointModel intermediate_point = new Model.PointModel(point_2d._x, Services._fasad_model._model[old_area]._y, point_2d._z);
                            this.NotGlobalChange(intermediate_point);
                        }
                    //y_2d > _old_y_2d
                    else
                        while (old_area < new_area)
                        {
                            Model.PointModel intermediate_point = new Model.PointModel(point_2d._x, Services._fasad_model._model[old_area]._y, point_2d._z);
                            this.NotGlobalChange(intermediate_point);
                            old_area++;
                        }
                }
            }
            // остальные перемещения
            else
            {
                //получаем вектор прямой
                Model.VectorModel vector = new Model.VectorModel(this._old_2d_point, point_2d);

                double length = 0;
                var old_area = this.GetAreaIndex(this._old_2d_point._y);
                var new_area = this.GetAreaIndex(point_2d._y);
                //если вектор перескакивает из одной области в другую, тогда добавляем еще границу
                while (old_area != new_area)
                {
                    if (old_area > new_area)
                        old_area--;
                    double scan_length = vector.CalculateLength(Services._fasad_model._model[old_area]._y);
                    vector.SetLength(scan_length);
                    Model.PointModel scan_point = vector.GetEndPoint();
                    if (scan_point.Equals(point_2d))
                        break;
                    this.TransformData(this._current_option, scan_point._x, scan_point._y, scan_point._z);
                    if (old_area < new_area)
                        old_area++;

                    vector = new Model.VectorModel(this._old_2d_point, point_2d);
                    length += scan_length;
                }

                //изменяя длину вектора получаем точки
                while (length < delta)
                {
                    vector.SetLength(length);
                    Model.PointModel intermediate_point = vector.GetEndPoint();
                    this.NotGlobalChange(intermediate_point);
                    length += this._min_length;
                }
            }
            this.NotGlobalChange(point_2d);
        }

        /// <summary>
        /// Расстояние между точками такое, что можно сразу произвести преобразование
        /// </summary>
        /// <param name="x_2d">координата икс из файла</param>
        /// <param name="y_2d">координата игрек из файла</param>
        /// <param name="z_2d">координата зет (глеюина реза) из файла</param>
        private void NotGlobalChange(Model.PointModel point_2d)
        {
            Model.PointModel point_3d = this.Calculate3DPoint(point_2d);
            this.FinalChange(point_3d, WriteFile.MatchValue.COORDINATES);
            this.ReinitOldCoordinates(point_2d);
        }

        /// <summary>
        /// Расчет трехмерной координаты
        /// </summary>
        /// <param name="point_2d">двухмерная координата</param>
        private Model.PointModel Calculate3DPoint(Model.PointModel point_2d)
        {
            var length = point_2d._x;
            var deepth = point_2d._z;

            var point_3d_index = Services._fasad_model._model[0]._xz.Select((el, ind) => new
            {
                index = ind,
                delta = Math.Abs(el._length - length)
            }).OrderBy(a => a.delta).First().index;


            int end_area = this.GetAreaIndex(point_2d._y);
            //если точка находится раньше нулевого скана, тогда используем нолевой и первый сканы
            if (end_area == 0)
            {
                end_area = 1;
            }
            //если точка находится после последнего скана, тогда используем последний и предпоследний сканы
            else if (end_area == Services._fasad_model._model.Count)
            {
                end_area -= 1;
            }
            int start_area = end_area - 1;

            double y0 = Services._fasad_model._model[start_area]._y;
            double z0 = Services._fasad_model._model[start_area]._xz[point_3d_index]._z;
            double a0 = Services._fasad_model._model[start_area]._xz[point_3d_index]._a;
            double y1 = Services._fasad_model._model[end_area]._y;
            double z1 = Services._fasad_model._model[end_area]._xz[point_3d_index]._z;
            double a1 = Services._fasad_model._model[end_area]._xz[point_3d_index]._a;

            double a_3d = this.SolveLinaryFunction(y0, a0, y1, a1, point_2d._y);
            double x_3d = Services._fasad_model._model[0]._xz[point_3d_index]._x - Math.Sin(a_3d) * deepth;
            double y_3d = point_2d._y;
            double z_3d = this.SolveLinaryFunction(y0, z0, y1, z1, point_2d._y) + Math.Cos(a_3d) * deepth;

            Model.PointModel point_3d = new Model.PointModel(x_3d, y_3d, z_3d, a_3d);
            return point_3d;
        }

        /// <summary>
        /// финальное преобразование координат перед записью в файл
        /// (перевод угла склонения в градусы, изменение системы координат, вычитание длины шпинделя...)
        /// </summary>
        /// <param name="point_3d">точка расчитанная для фасада без учета длины шпинделя</param>
        /// <param name="option">режим записи координат</param>
        private void FinalChange(Model.PointModel point_3d, WriteFile.MatchValue option)
        {
            double a = (Services._is_clock_angle ? (-1) : 1) * point_3d._a;

            double x = point_3d._x - (Services._is_clock_angle ? (-1) : 1) * Math.Sin(a) * Services._full_instrument_length;
            double y = point_3d._y;
            double z = point_3d._z + (Math.Cos(a) - 1) * Services._full_instrument_length;

            Model.PointModel final_point = Services._is_right_screw ? new Model.PointModel(x, y, z, a) : new Model.PointModel(y, x, z, a);
            Services._writer.Write(option, final_point);
            Services._writer.Write(WriteFile.MatchValue.END);
        }



        /// <summary>
        /// составляем линейное уравнение и решаем его
        /// </summary>
        /// <param name="x0">абсцисса начала</param>
        /// <param name="y0">ордината начала</param>
        /// <param name="x1">асбцисса конца</param>
        /// <param name="y1">ордината конца</param>
        /// <param name="xn">абсцисса, для которой надо вычислить ординату</param>
        /// <returns>ордината для xn</returns>
        private double SolveLinaryFunction(double x0, double y0, double x1, double y1, double xn)
        {
            double k = (y1 - y0) / (x1 - x0);
            double b = y0 - x0 * k;

            double yn = k * xn + b;
            return yn;
        }

        /// <summary>
        /// после завершения преобразования перезаем старые значения координат
        /// </summary>
        /// <param name="x_2d">координата икс из файла</param>
        /// <param name="y_2d">координата игрек из файла</param>
        /// <param name="z_2d">координата зет (глеюина реза) из файла</param>
        private void ReinitOldCoordinates(Model.PointModel point_2d)
        {
            this._old_2d_point = new Model.PointModel(point_2d);
        }

        /// <summary>
        /// Получение идентификатора области в которой находится точка
        /// </summary>
        /// <param name="y_2d">координата игрек, нужна только она, так как области находятся вдоль Oy</param>
        /// <returns></returns>
        private int GetAreaIndex(double y_2d)
        {
            for (var i = 0; i < Services._fasad_model._model.Count; i++)
            {
                if (y_2d <= Services._fasad_model._model[i]._y)
                    return i;
            }

            return Services._fasad_model._model.Count;
        }
    }
}
