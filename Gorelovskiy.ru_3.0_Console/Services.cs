using System;
using System.Globalization;

namespace Gorelovskiy.ru_3._0_Console
{
    static class Services
    {
        /// <summary>
        /// список Заказчиков
        /// </summary>
        public enum customers : int { VerlinRostov = 1, DiakovRostov, ArmenOtradnoe, ArmenGeorgevsk }


        #region параметры для каждого станка свои
        /// <summary>
        /// флаг автоматической смены инструмента
        /// </summary>
        public const bool _is_auto_change = false;
        /// <summary>
        /// флаг правовинтовой системы <para/>
        /// true - поворот шпинделя происходит в проскости OXZ
        /// false - поворот шпинделя в плоскости OYZ
        /// </summary>
        public const bool _is_right_screw = true;
        /// <summary>
        /// направление отсчета угла наклона<para/>
        /// Как определить. Стоим в точке (0,0,0), смотрим в направлении оси вдоль которой нет поворота шпинделя<para/>
        /// Если при повороте по часовой стрелке шпинделя угол растет, тогда угол склонения по часовой стрелке
        /// </summary>
        public const bool _is_clock_angle = true;
        /// <summary>
        /// текущий Заказчик
        /// </summary>
        public const int customer = (int)customers.DiakovRostov;
        /// <summary>
        /// Задаем разделитель десятичных
        /// </summary>
        public static NumberFormatInfo _number_info
        {
            get
            {
                return new NumberFormatInfo() { NumberDecimalSeparator = "." };
            }
        }
        #endregion


        #region Пути к файлам
        /// <summary>
        /// путь к файлу скана
        /// </summary>
        public const string _scan_file_path = @"C:\Mach3\GCode\1Scan.txt";
        /// <summary>
        /// путь к файлу, в котором храниться ссылка на рисунок и возможно еще список фрез
        /// </summary>
        public const string _2D_picture_ref_path = _is_auto_change ? @"C:\Mach3\GCode\1_info.dat" : @"C:\Mach3\GCode\addresGCode.txt";
        /// <summary>
        /// путь к файл, в котором храниться расстояние от точки поворота до конца шпинделя
        /// </summary>
        public const string _spindel_file_path = @"C:\Mach3\GCode\DZRotory.txt";
        /// <summary>
        /// путь к файлу к файлу, в котором храниться длина фрезы
        /// </summary>
        public const string _cutter_file_path = @"C:\Mach3\GCode\dzr.txt";
        /// <summary>
        /// путь к файлу в который записывается трехмерный рисунок
        /// </summary>
        public const string _3d_file_path = @"C:\Mach3\GCode\3DGcode.tap";
        #endregion


        #region Параметры станка
        /// <summary>
        /// Длина шпинделя + длина фрезы
        /// </summary>
        public static double _full_instrument_length = 0;
        /// <summary>
        /// стандартное значение скорости вектора движения
        /// </summary>
        public const int _default_f = 10000;
        #endregion



        /// <summary>
        /// модель интерполированного фасада
        /// </summary>
        public static Model.FasadModel _fasad_model;
        /// <summary>
        /// Запись в файл данных
        /// </summary>
        public static WriteFile _writer;
    }
}
