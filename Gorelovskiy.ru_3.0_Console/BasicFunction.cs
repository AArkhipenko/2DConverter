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
        }

        /// <summary>
        /// Публичный метод выполнения полного цикла обработки рисунка
        /// </summary>
        public void Start()
        {
            Services.Log("\tНачато создание модели фасада на основе скана");
            this.CreateFasadModel();
            Services.Log("\tСоздание модели фасада на основе скана успешно завершено", Services.LogType.SUCCESS);

            Services.Log("\tНачато наложение 2D рисунка на трехмерную модель фасада");
            this.TransformPicture();
            Services.Log("\tНаложение 2D рисунка на трехмерную модель фасада успешно завершено", Services.LogType.SUCCESS);
        }



        /// <summary>
        /// чтение скана из файла и генерация интерполированной модели
        /// </summary>
        private void CreateFasadModel()
        {
            Services.Log("\t\tНачато чтения файла скана и создание модели скана");
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
                        // количество сканов
                        if (counter == 0)
                            scan_model = new Model.ScanModel(int.Parse(line));
                        //начало сканирования по игрек
                        else if (counter == 1)
                        {
                            double startY = double.Parse(line.Replace('.', ','));
                            scan_model._model.ForEach(a => a._y = startY);
                        }
                        //шаг сканов
                        else if (counter == 2)
                        {
                            //в моделе сканирования задаем координату по игрек каждого скана
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
                            //строка с коориданатами имеет формат ({х}\t{z1}\t{z2}\t...)
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
            Services.Log("\t\tЧтения файла скана и создание модели скана успешно завершено", Services.LogType.SUCCESS);


            Services.Log("\t\tНачато создание модели фасада на основе модели скана");
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
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    throw new CustomException(ex.Message);
                else
                    throw new CustomException(ex.Message, ex.InnerException);
            }
            #endregion
            Services.Log("\t\tСоздание модели фасада на основе модели скана успешно завершено", Services.LogType.SUCCESS);

#if RESEARCH
            this.WriteCoordinatesInFile(scan_model);
            this.WriteCoordinatesInFile(Services._fasad_model);
#endif
        }



        /// <summary>
        /// преобразование двух мерного рисунка
        /// </summary>
        private void TransformPicture()
        {
            Services.Log("\t\tНачато создание шаблонов поиска информации");
            #region Шаблоны поиска значений в тексте
            //\W? - любой не алфавитно-цифровой символ повторяется 0 или 1 раз
            //\d+ - любой цифровой символ повторяется 1 и более раз
            //\d* - любой цифровой символ повторяется 0 и более раз

            string patternG = @"G\d+";
            string patternX = null;
            string patternY = null;
            if (Services._is_right_screw)//правовинтовая система координат
            {
                patternX = @"X\-?\d+([\,\.]\d+)?";
                patternY = @"Y\-?\d+([\,\.]\d+)?";
            }
            else//левовинтоая система координат
            {
                patternX = @"Y\-?\d+([\,\.]\d+)?";
                patternY = @"X\-?\d+([\,\.]\d+)?";
            }

            string patternZ = @"Z\-?\d+([\,\.]\d+)?";
            string patternF = @"F\d+";
            string patternM = @"M\d+";
            string patternS = @"S\d+";
            string patternT = @"T\d+";
            #endregion
            Services.Log("\t\tСоздание шаблонов поиска информации успешно завершено", Services.LogType.SUCCESS);


            Services.Log("\t\tНачато получение основной информации о станке и моделе фасада");
            #region Параметры станка
            //открываем файл в котором находится инфа о инструментах
            if (!File.Exists(Services._2D_picture_ref_path))
                throw new CustomException("Отсутствует файл с информацией о пути к файлу рисунка");

            string[] info = File.ReadAllLines(Services._2D_picture_ref_path, Encoding.GetEncoding(1251));

            if (!File.Exists(info[0]))
                throw new CustomException("Отсутствует файл 2D рисунка");
            if (!File.Exists(Services._spindel_file_path))
                throw new CustomException("Отсутствует файл c длиной шпинделя");

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
                                                                        Services._fasad_model._model[0]._xz.Where(a => a._length == 0).First()._z);

                var max_point = Services._fasad_model._model.Select(a => new
                {
                    x = a._xz.OrderByDescending(b => b._z).First()._x,
                    y = a._y,
                    z = a._xz.OrderByDescending(b => b._z).First()._z,
                }).OrderBy(a => a.z).First();

                var min_point = Services._fasad_model._model.Select(a => new
                {
                    x = a._xz.OrderBy(b => b._z).First()._x,
                    y = a._y,
                    z = a._xz.OrderBy(b => b._z).First()._z,
                }).OrderBy(a => a.z).First();

                general_info = new Model.GeneralInfoModel(spindel_length,
                                                            new Model.GeneralInfoModel.Point(max_point.x, max_point.y, max_point.z),
                                                            new Model.GeneralInfoModel.Point(min_point.x, min_point.y, min_point.z),
                                                            start_scan_point);
            }
            catch (Exception ex)
            {
                throw new CustomException("Не удалось получить основную информацию", ex);
            }
            #endregion
            Services.Log("\t\tПолучение основной информации о станке и моделе фасада успешно завершено", Services.LogType.SUCCESS);


            Services.Log("\t\tНачато чтение файла с рисунком и наложение его на трехмерный фасад");
            #region Работа с файлом (чтение двухмерного и запись в трехмерный)
            using (Services._writer = new WriteFile())
            {

                TransformCoordinates transformator = new TransformCoordinates();
                //Читаем файл и делаем преобразование
                bool is_first = true;
                using (StreamReader reader = new StreamReader(info[0]))
                {
                    int counter = 0;
                    try
                    {
                        Match match = null;
                        //запис основной информации в файл
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

                            //находим G
                            match = Regex.Match(line_2d_file, patternG);
                            if (match.Success)
                            {
                                int G = int.Parse(match.Value.Substring(1));
                                if (G != 2 && G != 3)
                                    Services._writer.Write(WriteFile.MatchValue.G, G);
                            }

                            //находим M
                            match = Regex.Match(line_2d_file, patternM);
                            if (match.Success)
                            {
                                int M = int.Parse(match.Value.Substring(1));
                                if (M == 5)
                                    break;
                                else
                                    Services._writer.Write(WriteFile.MatchValue.M, M);

                            }

                            //находим T
                            match = Regex.Match(line_2d_file, patternT);
                            if (match.Success)
                            {
                                int T = int.Parse(match.Value.Substring(1));

                                Services._writer.Write(WriteFile.MatchValue.T, T);
                                Services._full_instrument_length = cutter_lengths[T - 1];
                            }

                            //находим S
                            match = Regex.Match(line_2d_file, patternS);
                            if (match.Success)
                            {
                                int S = int.Parse(match.Value.Substring(1));

                                Services._writer.Write(WriteFile.MatchValue.S, S);
                            }

                            //находим F
                            match = Regex.Match(line_2d_file, patternF);
                            if (match.Success)
                            {
                                int F = int.Parse(match.Value.Substring(1));

                                Services._writer.Write(WriteFile.MatchValue.F, F);
                            }


                            //находим Y
                            match = Regex.Match(line_2d_file, patternY);
                            if (match.Success)
                            {
                                is_read_2d_y = 11;
                                y_2d = double.Parse(match.Value.Substring(1).Replace(".", ","));
                            }

                            //находим Z
                            match = Regex.Match(line_2d_file, patternZ);
                            if (match.Success)
                            {
                                is_read_2d_z = 13;
                                z_2d = double.Parse(match.Value.Substring(1).Replace(".", ","));
                            }

                            //находим X
                            match = Regex.Match(line_2d_file, patternX);
                            if (match.Success)
                            {
                                is_read_2d_x = 7;
                                x_2d = double.Parse(match.Value.Substring(1).Replace(".", ","));
                            }

                            if ((is_read_2d_x == 7 ||
                                is_read_2d_y == 11 ||
                                is_read_2d_z == 13) && is_first)
                            {
                                transformator.TransformData(TransformCoordinates.Option.FIRST, 0, 0, 100);
                                is_first = false;
                            }

                            int multiplication = is_read_2d_y * is_read_2d_z * is_read_2d_x;
                            transformator.TransformData((TransformCoordinates.Option)(is_read_2d_y * is_read_2d_z * is_read_2d_x), x_2d, y_2d, z_2d);

                            Services._writer.Write(WriteFile.MatchValue.END);
                            counter++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Services._writer.Dispose();
                        if (ex.InnerException == null)
                            throw new CustomException(ex.Message, counter);
                        else
                            throw new CustomException(ex.Message, ex.InnerException, counter);
                    }
                }
            }

            #endregion
            Services.Log("\t\tЧтение файла с рисунком и наложение его на трехмерный фасад успешно завершено", Services.LogType.SUCCESS);
        }




#if RESEARCH
        /// <summary>
        /// Запись моделей в файл
        /// </summary>
        /// <param name="model">модель для записи в файл</param>
        private void WriteCoordinatesInFile(object model)
        {
            if (model is Model.ScanModel)
            {
                using (var sw = new StreamWriter(@".\scan.txt"))
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
            else if (model is Model.FasadModel)
            {
                using (var sw = new StreamWriter(@".\fasad.txt"))
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
