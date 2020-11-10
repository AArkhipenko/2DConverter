using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;


namespace Gorelovskiy.ru_3._0_Console
{
    class BasicFunction
    {
        public BasicFunction()
        {
            Console.WriteLine("\tНачато создание модели фасада на основе скана");
            this.CreateFasadModel();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\tСоздание модели фасада на основе скана успешно завершено");
            Console.ForegroundColor = ConsoleColor.White;


            Console.WriteLine("\tНачато наложение 2D рисунка на трехмерную модель фасада");
            this.TransformPicture();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\tНаложение 2D рисунка на трехмерную модель фасада успешно завершено");
            Console.ForegroundColor = ConsoleColor.White;
        }



        /// <summary>
        /// чтение скана из файла и генерация интерполированной модели
        /// </summary>
        private void CreateFasadModel()
        {
            Console.WriteLine("\t\tНачато чтения файла скана и создание модели скана");
            #region чтение файла со сканами
            Model.ScanModel scan_model = null;
            if (!File.Exists(Services._scan_file_path))
                throw new CustomException("Отсутствует файл скана");
            using (StreamReader reader = new StreamReader(Services._scan_file_path))
            {
                //читаем файл скана и заполняем модель
                int counter = 0;
                try
                {
                    while (reader.Peek() > 0)
                    {
                        var line = reader.ReadLine();
                        if (counter == 0) // количество сканов
                            scan_model = new Model.ScanModel(int.Parse(line));
                        else if (counter == 1) //начало сканирования по игрек
                            scan_model._model.ForEach(a => a._y = double.Parse(line.Replace('.', ',')));
                        else if (counter == 2) //шаг сканов
                        {
                            int p = 0;
                            double delta_y = double.Parse(line.Replace('.', ','));
                            scan_model._model.ForEach(a =>
                            {
                                a._y += p * delta_y;
                                p++;
                            });
                        }
                        else // координаты
                        {
                            string[] coordinates = line.Split('\t');
                            int p = 0;
                            scan_model._model.ForEach(a =>
                            {
                                a._xz.Add(new Model.ScanModel.Scan.Coordanates
                                {
                                    _x = double.Parse(coordinates[0]),
                                    _z = double.Parse(coordinates[p + 1])
                                });
                                p++;
                            });
                        }
                        counter++;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                        throw new CustomException(ex.Message, counter);
                    else
                        throw new CustomException(ex.Message, ex.InnerException, counter);
                }
            }
            #endregion
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\t\tЧтения файла скана и создание модели скана успешно завершено");
            Console.ForegroundColor = ConsoleColor.White;


            Console.WriteLine("\t\tНачато создание модели фасада на основе модели скана");
            #region Создание модели фасада с более детальной прорисовкой, углами и длинами дуг
            //создать новую модель фасада и заполнить ее данными
            Services._fasad_model = new Model.FasadModel();
            var spline = new AddictFuncs.Spline();
            try
            {
                double min_x = scan_model._model.Min(a => a._xz.Min(b => b._x));
                double max_x = scan_model._model.Max(a => a._xz.Max(b => b._x)) + 100;
                foreach (var obj in scan_model._model)
                {
                    Services._fasad_model._model.Add(new Model.FasadModel.Duga()
                    {
                        _y = obj._y
                    });
                    spline.BuildSpline(obj._xz.Select(a => a._x).ToArray(), obj._xz.Select(a => a._z).ToArray());
                    double new_x = min_x;
                    double length = 0;
                    while (new_x < max_x)
                    {
                        var new_z = spline.GetY(new_x);
                        var new_a = spline.GetA(new_x);
                        var lastModel = Services._fasad_model._model.Last()._xz.LastOrDefault();
                        length += lastModel != null ? Math.Sqrt(Math.Pow((new_x - lastModel._x), 2) + Math.Pow((new_z - lastModel._z), 2)) : 0;
                        Services._fasad_model._model.Last()._xz.Add(new Model.FasadModel.Duga.Coordanates
                        {
                            _x = new_x,
                            _z = new_z,
                            _a = new_a,
                            _length = length
                        });
                        new_x += 0.01;
                    }
                    new_x = min_x;
                    while (new_x > min_x - 100)
                    {
                        new_x -= 0.01;
                        var new_z = spline.GetY(new_x);
                        var new_a = spline.GetA(new_x);
                        var firstModel = Services._fasad_model._model.Last()._xz.First();
                        length += Math.Sqrt(Math.Pow((new_x - firstModel._x), 2) + Math.Pow((new_z - firstModel._z), 2));
                        Services._fasad_model._model.Last()._xz.Insert(0, new Model.FasadModel.Duga.Coordanates
                        {
                            _x = new_x,
                            _z = new_z,
                            _a = new_a,
                            _length = (-1) * length
                        });
                    }
                }
            }
            catch(Exception ex)
            {
                if (ex.InnerException == null)
                    throw new CustomException(ex.Message);
                else
                    throw new CustomException(ex.Message, ex.InnerException);
            }
            #endregion
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\t\tСоздание модели фасада на основе модели скана успешно завершено");
            Console.ForegroundColor = ConsoleColor.White;
#if DEBUG
            this.WriteCoordinatesInFile(scan_model);
            this.WriteCoordinatesInFile(Services._fasad_model);
#endif
        }



        /// <summary>
        /// преобразование двух мерного рисунка
        /// </summary>
        private void TransformPicture()
        {
            Console.WriteLine("\t\tНачато создание шаблонов поиска информации");
            #region Шаблоны поиска значений в тексте
            //\W? - любой не алфавитно-цифровой символ повторяется 0 или 1 раз
            //\d+ - любой цифровой символ повторяется 1 и более раз
            //\d* - любой цифровой символ повторяется 0 и более раз

            string patternG = @"[G]\d*";
            string patternX = null;
            string patternY = null;
            if (Services._is_right_screw)//правовинтовая система координат
            {
                patternX = @"[X]\W?\d+\W?\d*";
                patternY = @"[Y]\W?\d+\W?\d*";
            }
            else//левовинтоая система координат
            {
                patternX = @"[Y]\W?\d+\W?\d*";
                patternY = @"[X]\W?\d+\W?\d*";
            }

            string patternZ = @"[Z]\W?\d+\W?\d*";
            string patternF = @"[F]\d+";
            string patternM = @"[M]\d+";
            string patternS = @"[S]\d+";
            string patternT = @"[T]\d+";
            #endregion
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\t\tСоздание шаблонов поиска информации успешно завершено");
            Console.ForegroundColor = ConsoleColor.White;


            Console.WriteLine("\t\tНачато получение основной информации о станке и моделе фасада");
            #region Параметры станка
            //открываем файл в котором находится инфа о инструментах
            if (!File.Exists(Services._2D_picture_ref_path))
                throw new CustomException("Отсутствует файл с информацией о пути к файлу рисунка");
            string[] info = File.ReadAllLines(Services._2D_picture_ref_path, Encoding.GetEncoding(1251));
            if (!File.Exists(info[0]))
                throw new Exception("Отсутствует файл 2D рисунка");
            if (!File.Exists(Services._spindel_file_path))
                throw new Exception("Отсутствует файл c длиной шпинделя");

            double spindel_length = double.Parse(File.ReadAllText(Services._spindel_file_path).Replace('.', ','));
            Services._full_instrument_length = spindel_length;
            double[] cutter_lengths;
            if (!Services._is_auto_change)
            {
                if (!File.Exists(Services._cutter_file_path))
                    throw new Exception("Отсутствует файл c длиной фрезы");
                cutter_lengths = new double[1];
                cutter_lengths[0] = spindel_length + double.Parse(File.ReadAllText(Services._cutter_file_path).Replace('.', ','));
                Services._full_instrument_length = cutter_lengths[0];
            }
            else
            {

                cutter_lengths = new double[int.Parse(info[1])];
                //заполняем массив содержащий длины фрез
                for (int i = 0; i < cutter_lengths.Length; i++)
                {
                    cutter_lengths[i] = spindel_length + double.Parse(info[i + 2].Replace('.', ','));
                }
            }

            Model.GeneralInfoModel general_info = null;
            try
            {
                var start_scan_point = new Model.GeneralInfoModel.Point(Services._fasad_model._model[0]._xz.Where(a => a._length == 0).First()._x,
                                                                                                    Services._fasad_model._model[0]._y,
                                                                                                    Services._fasad_model._model[0]._xz.Where(a => a._length == 0).First()._x);

                var max_point = Services._fasad_model._model.Select(a => new
                {
                    x = a._xz.OrderBy(b => b._z).First()._x,
                    y = a._y,
                    z = a._xz.OrderBy(b => b._z).First()._z,
                }).OrderBy(a => a.z).First();

                var min_point = Services._fasad_model._model.Select(a => new
                {
                    x = a._xz.OrderByDescending(b => b._z).First()._x,
                    y = a._y,
                    z = a._xz.OrderByDescending(b => b._z).First()._z,
                }).OrderBy(a => a.z).First();

                general_info = new Model.GeneralInfoModel(spindel_length,
                                                                                new Model.GeneralInfoModel.Point(max_point.x, max_point.y, max_point.z),
                                                                                new Model.GeneralInfoModel.Point(min_point.x, min_point.y, min_point.z),
                                                                                start_scan_point);
            }
            catch(Exception ex)
            {
                throw new CustomException("Не удалось получить основную информацию");
            }
            #endregion
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\t\tПолучение основной информации о станке и моделе фасада успешно завершено");
            Console.ForegroundColor = ConsoleColor.White;


            Console.WriteLine("\t\tНачато чтение файла с рисунком и наложение его на трехмерный фасад");
            #region Работа с файлом (чтение двухмерного и запись в трехмерный)
            Services._writer = new WriteFile();
            TransformCoordinates transformator = new TransformCoordinates();
            //@@@Записать общую информацию в файл

            //@@@Читаем файл и делаем преобразование
            bool is_first = true;
            using (StreamReader reader = new StreamReader(info[0]))
            {
                int counter = 0;
                try
                {
                    Services._writer.Write(WriteFile.MatchValue.INFO, general_info);
                    while (reader.Peek() > 0)
                    {
                        string line_2d_file = reader.ReadLine();
                        int is_read_2d_x = 1;
                        int is_read_2d_y = 3;
                        int is_read_2d_z = 5;

                        double? x_2d = null;
                        double? y_2d = null;
                        double? z_2d = null;

                        foreach(Match match in Regex.Matches(line_2d_file, patternG))
                        {
                            int G = int.Parse(match.ToString().Substring(1));
                            if (G != 2 && G != 3)
                                Services._writer.Write(WriteFile.MatchValue.G, G);

                        }
                        foreach (Match match in Regex.Matches(line_2d_file, patternM))
                        {
                            int M = int.Parse(match.ToString().Substring(1));
                            if (M == 5)
                                break;
                            else
                                Services._writer.Write(WriteFile.MatchValue.M, M);

                        }
                        foreach (Match match in Regex.Matches(line_2d_file, patternT))
                        {
                            int T = int.Parse(match.ToString().Substring(1));

                            //@@@изменяем длину фрезы
                            Services._writer.Write(WriteFile.MatchValue.T, T);
                        }
                        foreach (Match match in Regex.Matches(line_2d_file, patternT))
                        {
                            int S = int.Parse(match.ToString().Substring(1));

                            Services._writer.Write(WriteFile.MatchValue.S, S);
                        }
                        foreach (Match match in Regex.Matches(line_2d_file, patternF))
                        {
                            int F = int.Parse(match.ToString().Substring(1));

                            Services._writer.Write(WriteFile.MatchValue.F, F);
                        }
                        if (Regex.IsMatch(line_2d_file, patternY))
                        {
                            is_read_2d_y = 11;
                            y_2d = double.Parse(Regex.Match(line_2d_file, patternY).ToString().Substring(1).Replace(".", ","));
                        }
                        if (Regex.IsMatch(line_2d_file, patternZ))
                        {
                            is_read_2d_z = 13;
                            z_2d = double.Parse(Regex.Match(line_2d_file, patternZ).ToString().Substring(1).Replace(".", ","));
                        }
                        if (Regex.IsMatch(line_2d_file, patternX))
                        {
                            is_read_2d_x = 7;
                            x_2d = double.Parse(Regex.Match(line_2d_file, patternX).ToString().Substring(1).Replace(".", ","));
                        }

                        int multiplication = is_read_2d_y * is_read_2d_z * is_read_2d_x;
                        if (multiplication == (int)TransformCoordinates.Option.X_Y_Z &&
                            is_first)
                        {
                            transformator.TransformData(TransformCoordinates.Option.FIRST, x_2d, y_2d, z_2d);
                            is_first = false;
                        }
                        else if(!is_first)
                            transformator.TransformData((TransformCoordinates.Option)(is_read_2d_y * is_read_2d_z * is_read_2d_x), x_2d, y_2d, z_2d);

                        Services._writer.Write(WriteFile.MatchValue.END);
                        counter++;
                    }
                }
                catch(Exception ex)
                {
                    Services._writer.Dispose();
                    if (ex.InnerException == null)
                        throw new CustomException(ex.Message, counter);
                    else
                        throw new CustomException(ex.Message, ex.InnerException, counter);
                }
            }
            Services._writer.Dispose();

            //@@@Закончить преобразование
            #endregion
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\t\tЧтение файла с рисунком и наложение его на трехмерный фасад успешно завершено");
            Console.ForegroundColor = ConsoleColor.White;
        }




#if DEBUG
        /// <summary>
        /// Запись моделей в файл
        /// </summary>
        /// <param name="model">модель для записи в файл</param>
        private void WriteCoordinatesInFile(object model)
        {
            if (model.GetType() == typeof(Model.ScanModel))
            {
                using (var sw = new StreamWriter("c:\\temp\\scan.txt"))
                {
                    var obj = (Model.ScanModel)model;
                    foreach (var el in obj._model)
                    {
                        foreach (var coor in el._xz)
                            sw.WriteLine(coor._x + "\t" + coor._z);
                        sw.Write("\r\n\r\n\r\n\r\n");
                    }
                }
            }
            if (model.GetType() == typeof(Model.FasadModel))
            {
                using (var sw = new StreamWriter("c:\\temp\\fasad.txt"))
                {
                    var obj = (Model.FasadModel)model;
                    foreach (var el in obj._model)
                    {
                        foreach (var coor in el._xz)
                            sw.WriteLine(coor._x + "\t" + coor._z);
                        sw.Write("\r\n\r\n\r\n\r\n");
                    }
                }
            }
        }
#endif
    }
}
