using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorelovskiy.ru_3._0_Console.AddictFuncs
{
    public class Spline
    {
        SplineTuple[] splines; // Сплайн
        // Структура, описывающая сплайн на каждом сегменте сетки
        private struct SplineTuple
        {
            public double a, b, c, d, x;
        }
        /// <summary>
        /// Создание интерполяционной модели по набору точек
        /// </summary>
        /// <param name="x">координаты икс</param>
        /// <param name="y">координаты игрек</param>
        /// <param name="n">количество точек</param>
        public void BuildSpline(double[] x, double[] y)
        {
            int n = x.Length;
            // Инициализация массива сплайнов
            splines = new SplineTuple[n];
            for (int i = 0; i < n; ++i)
            {
                splines[i].x = x[i];
                splines[i].a = y[i];
            }
            splines[0].c = splines[n - 1].c = 0.0;
            // Решение СЛАУ относительно коэффициентов сплайнов c[i] методом прогонки для трехдиагональных матриц
            // Вычисление прогоночных коэффициентов - прямой ход метода прогонки
            double[] alpha = new double[n - 1];
            double[] beta = new double[n - 1];
            alpha[0] = beta[0] = 0.0;
            for (int i = 1; i < n - 1; ++i)
            {
                double hi = x[i] - x[i - 1];
                double hi1 = x[i + 1] - x[i];
                double A = hi;
                double C = 2.0 * (hi + hi1);
                double B = hi1;
                double F = 6.0 * ((y[i + 1] - y[i]) / hi1 - (y[i] - y[i - 1]) / hi);
                double z = (A * alpha[i - 1] + C);
                alpha[i] = -B / z;
                beta[i] = (F - A * beta[i - 1]) / z;
            }
            // Нахождение решения - обратный ход метода прогонки
            for (int i = n - 2; i > 0; --i)
            {
                splines[i].c = alpha[i] * splines[i + 1].c + beta[i];
            }

            // По известным коэффициентам c[i] находим значения b[i] и d[i]
            for (int i = n - 1; i > 0; --i)
            {
                double hi = x[i] - x[i - 1];
                splines[i].d = (splines[i].c - splines[i - 1].c) / hi;
                splines[i].b = hi * (2.0 * splines[i].c + splines[i - 1].c) / 6.0 + (y[i] - y[i - 1]) / hi;
            }
        }
        //_________________________________________________________________________________________________
        //______________Вычисление значения интерполированной функции в произвольной точке_________________
        //_________________________________________________________________________________________________
        public double GetY(double x)
        {
            var s = splines[this.GetIndex(x)];
            double dx = x - s.x;
            return s.a + (s.b + (s.c / 2.0 + s.d * dx / 6.0) * dx) * dx;
        }

        public double GetA(double x)
        {
            var s = splines[this.GetIndex(x)];
            double dx = x - s.x;
            double tgx = s.b + (s.c + s.d * dx / 2.0) * dx;
            return Math.Atan(tgx);
        }

        private int GetIndex(double x)
        {
            if (splines == null || splines.Length == 0)
                throw new Exception("СПЛАЙН еще не вычислен");

            int n = splines.Length;

            if (x <= splines[0].x) // Если x меньше точки сетки x[0] - пользуемся первым эл-тов массива
            {
                return 1;
            }
            else if (x >= splines[n - 1].x) // Если x больше точки сетки x[n - 1] - пользуемся последним эл-том массива
            {
                return n - 1;
            }
            else // Иначе x лежит между граничными точками сетки - производим бинарный поиск нужного эл-та массива
            {
                int i = 0;
                int j = n - 1;
                while (i + 1 < j)
                {
                    int k = i + (j - i) / 2;
                    if (x <= splines[k].x)
                        j = k;
                    else
                        i = k;
                }
                return j;
            }
        }
    }
}
