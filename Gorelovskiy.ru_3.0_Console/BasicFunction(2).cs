using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;


namespace Gorelovskiy.ru_3._0_Console
{
    class BasicFunction
    {
        //*******************************************

        //Глобальные переменные:
        //1)используются другими классами
        public enum customers : int { VerlinRostov = 1, DiakovRostov, ArmenOtradnoe, ArmenGeorgevsk }//список заказчиков
        public static int customer = (int)customers.DiakovRostov; //для какого типчика прога
        public static float[, ,] newduga;
        public static float DlinaSpindelPlusDlinaFrezi;
        public static float[] allInstruments;
        public static int KolichestvoProhodov;
        public static float ShagSkanaVdol;

        public static AddictionalFunctionsClass.Exceptions oBF_Exception = new AddictionalFunctionsClass.Exceptions();
        public static WritingNewCoordinateInFile Write3DCoordinate;
        public static WorkWithRead2DCoordinates WorkWithMoves;
        //2) не используются другими классами но используются в этом классе в нескольких функциях
        private string[] line2D;
        private float centrY, centrZ;
        private float HordaFasada;
        private float MaxY, MaxZ;// высшие координаты по игрек и зет
        private float yPriMinZ, MinZ;//низшая точка по зет

        //_____________________________________________________________________________________________
        //____________________________________чтение скана и разбиение дуги____________________________
        //_____________________________________________________________________________________________
        /// <summary>
        /// Метод чтение скана из файла и интерполяция его до глдакой поверхности.
        /// </summary>
        public void ReadScanAndDividing()
        {
            string[] line = File.ReadAllLines(@"C:\Mach3\GCode\1Scan.txt");//создаем массив типа стринг, в который записываем по строчно информацию содержащеюся в duga (фактически читаем файл построчно)
            KolichestvoProhodov = Convert.ToInt16(line[0]);//считали из файла количество проходов вдоль фасада
            float NachaloSkanirovaniya = Convert.ToSingle(line[1].Replace('.', ','));//считываем из файла точку начала сканирования вдоль фасада
            ShagSkanaVdol = Convert.ToSingle(line[2].Replace('.', ','));//считываем из файла шаг вдоль фасада, с которым идут сканы
            float[, ,] readcoor = new float[KolichestvoProhodov, 7, line.Length];//в массиве находятся: координата икс (для армяней это игрек)(0); координата игрек (для армяней это икс) (1); координата зет(2);центры окружностей игрек (для армяней икс)(3) и зет(4); радиус кривизны фасада(5); средний радиус кривизны(6)
            float[] newc = new float[line.Length];//массив игрек координат(нужен для определения максимального значения игрек)
            char Razdelitel = '\t';

            MaxY = -1000;
            MaxZ = -1000;
            yPriMinZ = 0;
            MinZ = 100000;

            int i = 0;
            int t = 3;
            int PokazatelX = 0;

            //________________________________________________________________________________________________________
            //_____________________записываем в массив координаты икс при которых стоят соотвествующие сканы__________
            //________________________________________________________________________________________________________
            while (PokazatelX < KolichestvoProhodov)
            {
                readcoor[PokazatelX, 0, 0] = NachaloSkanirovaniya + ShagSkanaVdol * PokazatelX;
                PokazatelX++;
            }
            //_____________________________открываем и читаем координаты дуги_________________________________________
            //________________________________________________________________________________________________________
            while (t < line.Length)
            {
                readcoor[0, 1, t - 3] = Convert.ToSingle((line[t].Substring(0, line[t].IndexOf(Razdelitel))).Replace('.', ','));
                line[t] = line[t].Substring(line[t].IndexOf(Razdelitel) + 1);
                readcoor[0, 2, t - 3] = Convert.ToSingle((line[t].Substring(0, line[t].IndexOf(Razdelitel))).Replace('.', ','));
                line[t] = line[t].Substring(line[t].IndexOf(Razdelitel) + 1);
                PokazatelX = 1;
                while (PokazatelX < KolichestvoProhodov)
                {
                    readcoor[PokazatelX, 1, t - 3] = readcoor[0, 1, t - 3];
                    readcoor[PokazatelX, 2, t - 3] = Convert.ToSingle((line[t].Substring(0, line[t].IndexOf(Razdelitel))).Replace('.', ','));
                    line[t] = line[t].Substring(line[t].IndexOf(Razdelitel) + 1);
                    PokazatelX++;
                }
                if (MinZ > readcoor[0, 2, t - 3])//нахождение низшей точки по зет, так же находится игрек соответствующая этому зет
                {
                    MinZ = readcoor[0, 2, t - 3];
                    yPriMinZ = readcoor[0, 1, t - 3];
                }
                if (MaxZ < readcoor[0, 2, t - 3])//нахождение высшей точки по зет, так же находится игрек соответствующая этому зет
                {
                    MaxZ = readcoor[0, 2, t - 3];
                    MaxY = readcoor[0, 1, t - 3];
                }
                t++;
            }
            t = 0;
            float yPerenosKoordinate = readcoor[0, 1, 0];//перенос координат по игрек
            //________________________________________________________________________________________________________
            //______________________________________________Перенос системы координат_________________________________
            //________________________________________________________________________________________________________
            while (t < line.Length - 3)  // Перенос системы координат
            {
                PokazatelX = 0;
                while (PokazatelX < KolichestvoProhodov)
                {
                    readcoor[PokazatelX, 1, t] = readcoor[PokazatelX, 1, t] - yPerenosKoordinate;
                    readcoor[PokazatelX, 2, t] = readcoor[PokazatelX, 2, t] - MinZ;
                    PokazatelX++;
                }
                newc[t] = readcoor[0, 1, t];
                t++;
            }
            MaxY = MaxY - yPerenosKoordinate;
            MaxZ = MaxZ - MinZ;
            yPriMinZ = yPriMinZ - yPerenosKoordinate;

            AddictionalFunctionsClass.AdditionalFunctionsForReadAndDividingFunction o_Spline = new AddictionalFunctionsClass.AdditionalFunctionsForReadAndDividingFunction();
            HordaFasada = newc.Max();//определяем максимальное значение по игрек
            newduga = new float[KolichestvoProhodov, 4, 10000 * line.Length];//в данном массиве находятся: координаты икс (для армяней игрек)(0);координата игрек (для армяней икс)(1);координата зет(2); длина дуги до определенной точки(3)
            double[] xInterpolation = new double[line.Length - 3];//массив координат икс, отправляется в функцию интерполяции
            double[] yInterpolation = new double[line.Length - 3];//массив координат игрек, отправляется в функцию интерполяции
            PokazatelX = 0;

            while (PokazatelX < KolichestvoProhodov)
            {
                t = 0;
                while (t < line.Length - 3)
                {
                    xInterpolation[t] = Convert.ToDouble(readcoor[PokazatelX, 1, t]);
                    yInterpolation[t] = Convert.ToDouble(readcoor[PokazatelX, 2, t]);
                    t++;
                }

                o_Spline.BuildSpline(xInterpolation, yInterpolation, line.Length - 3);//сплан вычисление значений коэффициентов

                newduga[PokazatelX, 2, 0] = readcoor[PokazatelX, 2, 0];//задаем первую координату зет
                newduga[PokazatelX, 1, 0] = readcoor[PokazatelX, 1, 0];//задаем первую координату игрек (для армяней икс)
                newduga[PokazatelX, 0, 0] = readcoor[PokazatelX, 0, 0];//записываем координату икс (для армяней игрек) которой соответствует скан
                //_____________________________________________________________________________________________________________________
                //____________________________Разбиваем фасад на более мелкие отрезки с учетом радиуса кривизны фасада_________________
                //_____________________________________________________________________________________________________________________
                //дробим координаты игрек
                t = 1;
                while (newduga[0, 1, t - 1] <= HordaFasada + 10)
                {
                    newduga[PokazatelX, 1, t] = newduga[PokazatelX, 1, t - 1] + Convert.ToSingle(0.01);//координаты игрек (для армяней икс) на дуге фасада
                    t++;
                }
                //находим координаты зет при соотвествующих игрек
                t = 0;
                while (newduga[0, 1, t] <= HordaFasada + 10)
                {
                    newduga[PokazatelX, 2, t] = Convert.ToSingle(o_Spline.Interpolate(Convert.ToDouble(newduga[PokazatelX, 1, t])));
                    t++;
                }
                PokazatelX++;
            }
            //________________________________________________________________________________________________________
            //____________________________________________Определяем длину дуги до выбранной точки____________________
            //________________________________________________________________________________________________________
            i = 0;
            while (newduga[0, 1, i + 1] <= HordaFasada)
            {
                float ndlina = Convert.ToSingle(Math.Sqrt((newduga[0, 1, i] - newduga[0, 1, i + 1]) * (newduga[0, 1, i] - newduga[0, 1, i + 1]) + (newduga[0, 2, i] - newduga[0, 2, i + 1]) * (newduga[0, 2, i] - newduga[0, 2, i + 1])));
                newduga[0, 3, i + 1] = newduga[0, 3, i] + ndlina;//ДЛИНА ДУГИ ДО ОПРЕДЕЛЕННОЙ ТОЧКИ
                i++;
            }
            i = 0;
            while (newduga[0, 1, i] <= HordaFasada / 2)
            {
                centrY = newduga[0, 1, i];
                centrZ = newduga[0, 2, i];
                i++;
            }

        }

        //____________________________________________________________________________________
        //__________________________________чтение координат двухмерного рисунка______________
        //____________________________________________________________________________________
        /// <summary>
        /// Метод чтения двухмерного рисунка в массив. Имеется два варианта: <para/>
        /// 1)Для рисунка без автоматической смены инструмента <para/>
        /// 2)Для рисунка с автоматической сменой инструмента (с Т*)
        /// </summary>
        public void Read2DPicture()
        {
            string[] addres;
            if (customer == (int)customers.ArmenOtradnoe ||
                customer == (int)customers.VerlinRostov ||
                customer == (int)customers.DiakovRostov)
            {
                addres = File.ReadAllLines(@"C:\Mach3\GCode\addresGCode.txt", Encoding.GetEncoding(1251));
            }
            else //в файле лежат адрес рисунки, количество инструментов и длины инструментов
            {
                addres = File.ReadAllLines(@"C:\Mach3\GCode\1_info.dat", Encoding.GetEncoding(1251));
            }

            try
            {
                line2D = File.ReadAllLines(addres[0]);//построчное считывание из переменной типа стринг в массив типа стринг
            }
            catch (ArgumentException ex)
            {
                oBF_Exception.Exception2DPicture();
            }
        }

        //____________________________________________________________________________________
        //______________________________разбор двухмерного рисунка____________________________
        //____________________________________________________________________________________
        /// <summary>
        /// Метод поиска в массиве рисунка координата с последующей передачей на преобразование.
        /// </summary>
        public void Dividing2DPictureAndRemaking()
        {
            //________________________________________________________________________________________________________
            //_________________________________условия выбоки из файла________________________________________________
            //________________________________________________________________________________________________________

            Write3DCoordinate = new WritingNewCoordinateInFile();
            WorkWithMoves = new WorkWithRead2DCoordinates();
            //длина шпиндиля
            string[] DZRotorymass = File.ReadAllLines(@"C:\Mach3\GCode\DZRotory.txt");
            string patternG = @"[G]\d*";
            string patternX = null;
            string patternY = null;

            if (customer == (int)customers.VerlinRostov ||
                customer == (int)customers.DiakovRostov)//левая система координат
            {
                patternX = @"[X]\W?\d*\W?\d*";
                patternY = @"[Y]\d*\W?\d*";
            }
            else//правая система координат
            {
                patternX = @"[Y]\W?\d*\W?\d*";
                patternY = @"[X]\d*\W?\d*";
            }

            string patternZ = @"[Z]\W?\d*\W?\d*";
            string patternF = @"[F]\d*";
            string patternM = @"[M]\d*";
            string patternS = @"[S]\d*";
            string patternT = @"[T]\d*";
            float GlubinaReza2D = 0;
            float Read2DY = 0;
            float Read2DX = 0;
            bool FlagIndicatorOfFirstMoves = false;
            int mmm = 0;//номер строки в массиве стринговских строк прочитанного файла, в котором записанны все координаты двухмерного рисунка
            float DlinaShpindel = float.Parse(DZRotorymass[0].Replace('.', ','));

            if (customer == (int)customers.ArmenOtradnoe ||
                customer == (int)customers.VerlinRostov ||
                customer == (int)customers.DiakovRostov)
            {
                string[] dzrmass = File.ReadAllLines(@"C:\Mach3\GCode\dzr.txt");
                float DlinaFrezi = Convert.ToSingle(dzrmass[0]);
                DlinaSpindelPlusDlinaFrezi = DlinaShpindel + DlinaFrezi;
            }
            else
            {
                //открываем файл в котором находится инфа о инструментах
                string[] info = File.ReadAllLines(@"C:\Mach3\GCode\1_info.dat", Encoding.GetEncoding(1251));

                //если всего один элемен в массиве, тогда сразу задаем длину фрезы + шпиндель
                if (int.Parse(info[1]) == 1)
                {
                    DlinaSpindelPlusDlinaFrezi = float.Parse(DZRotorymass[0].Replace('.', ',')) +
                                                      float.Parse(info[2].Replace('.', ','));
                }
                else
                {
                    //задаем размер массива
                    allInstruments = new float[int.Parse(info[1])];
                    //заполняем массив содержащий длины фрез
                    for (int i = 0; i < allInstruments.Length; i++)
                    {
                        allInstruments[i] = float.Parse(DZRotorymass[0].Replace('.', ',')) +
                                                             float.Parse(info[i + 2].Replace('.', ','));
                    }
                }
            }

            //___________________________________________________________________________
            //___________________Запись информационных строк в файл______________________
            //___________________________________________________________________________
            Write3DCoordinate.WriteInformationStrings(DlinaShpindel, yPriMinZ, MaxY, MaxZ, centrY, centrZ);
            //________________________________________________________________________________________________________
            //______________________________________чтение из файла интересующих данных_______________________________
            //________________________________________________________________________________________________________
            while (mmm < line2D.Length)//считывание осуществляется до тех пор пока не закончится файл
            {
                int IndicatorRead2DX = 1;
                int IndicatorRead2DY = 3;
                int IndicatorRead2DGlubinaReza = 5;
                //________________________________________________________________________________________________________
                //____________________________________находим в тесте значение G__________________________________________
                //________________________________________________________________________________________________________
                foreach (Match g in Regex.Matches(line2D[mmm], patternG))
                {
                    string g1 = Convert.ToString(g);
                    g1 = g1.Substring(1);
                    float G = Convert.ToSingle(g1);
                    if (G == 2 || G == 3)
                    {
                        oBF_Exception.ExeptionInvalidCommand(mmm, "G" + g1);
                    }
                    else
                    {
                        Write3DCoordinate.WriteG(G);
                    }
                }
                //________________________________________________________________________________________________________
                //________________________________________Находим в тексте значение М_____________________________________
                //________________________________________________________________________________________________________
                foreach (Match M in Regex.Matches(line2D[mmm], patternM))
                {

                    string M1 = Convert.ToString(M);

                    //проверяем на конец работы инструмента
                    if (M1 == "M5" || M1 == "M05")
                    {
                        if (customer == (int)customers.ArmenOtradnoe ||
                            customer == (int)customers.VerlinRostov ||
                            customer == (int)customers.DiakovRostov)
                        {
                            goto endRead;
                        }
                        else
                        {
                            //если mmm это последняя строка тогда заканчиваем считывание
                            if (mmm + 1 != line2D.Length)
                            {
                                //сследуем стледущющую строку на предмет нахождения М6
                                Match M6 = Regex.Match(line2D[mmm + 1], patternM);
                                //если не нашли, заканчиваем работу
                                if (M6 == null || line2D[mmm + 1] == "M30")
                                    goto endRead;
                                else
                                {
                                    M1 = M1.Substring(1);
                                    float M2 = Convert.ToSingle(M1);
                                    Write3DCoordinate.WriteM(M2);
                                }
                            }
                            else
                            {
                                goto endRead;
                            }
                        }
                    }
                    else
                    {
                        M1 = M1.Substring(1);
                        float M2 = Convert.ToSingle(M1);
                        Write3DCoordinate.WriteM(M2);
                    }
                }
                //________________________________________________________________________________________________________
                //________________________________________Находим в тексте значение T_____________________________________
                //________________________________________________________________________________________________________
                if (customer != (int)customers.ArmenOtradnoe &&
                    customer != (int)customers.VerlinRostov &&
                    customer != (int)customers.DiakovRostov)
                {
                    foreach (Match T in Regex.Matches(line2D[mmm], patternT))
                    {
                        string T1 = Convert.ToString(T);
                        T1 = T1.Substring(1);
                        float T2 = Convert.ToSingle(T1);

                        DlinaSpindelPlusDlinaFrezi = allInstruments[(int)T2 - 1];

                        FlagIndicatorOfFirstMoves = false;

                        Write3DCoordinate.WriteT(T2);
                    }
                }
                //________________________________________________________________________________________________________
                //________________________________________Находим в тексте значение S_____________________________________
                //________________________________________________________________________________________________________
                foreach (Match S in Regex.Matches(line2D[mmm], patternS))
                {
                    string S1 = Convert.ToString(S);
                    S1 = S1.Substring(1);
                    float S2 = Convert.ToSingle(S1);
                    Write3DCoordinate.WriteS(S2);
                }
                //________________________________________________________________________________________________________
                //________________________________________Находим в тексте значение F_____________________________________
                //________________________________________________________________________________________________________
                foreach (Match f in Regex.Matches(line2D[mmm], patternF))
                {
                    string f1 = Convert.ToString(f);
                    f1 = f1.Substring(1);
                    float F = Convert.ToSingle(f1);
                    Write3DCoordinate.WriteF(F);
                }
                //________________________________________________________________________________________________________
                //________________________________________Находим в тексте значение Y_____________________________________
                //________________________________________________________________________________________________________
                foreach (Match y in Regex.Matches(line2D[mmm], patternY))
                {
                    IndicatorRead2DY = 11;
                    string y1 = Convert.ToString(y);
                    y1 = y1.Replace(".", ",");
                    //фактически это расстояние от края фасада до нужной точки в плоском рисунке, поэтому для гнутого фасада это расстояние от его края до нужной точки-длина дуги гнутого фасада
                    Read2DY = Convert.ToSingle(Math.Round(Convert.ToDouble(y1.Substring(1)), 2));

                }
                //________________________________________________________________________________________________________
                //________________________________________Находим в тексте значение Z_____________________________________
                //____________________в плоском рисунке это высота фрезы над поверностью стола____________________________
                //________________________________________________________________________________________________________
                foreach (Match z in Regex.Matches(line2D[mmm], patternZ))
                {
                    IndicatorRead2DGlubinaReza = 13;
                    string z1 = Convert.ToString(z);
                    z1 = z1.Replace(".", ",");
                    GlubinaReza2D = Convert.ToSingle(z1.Substring(1));//глубина погружения фрезы
                }
                //________________________________________________________________________________________________________
                //________________________________________Находим в тексте значение X_____________________________________
                //___________________эти координаты записываются без изменения, так как нет изменений вдоль оси Ох________
                //________________________________________________________________________________________________________
                foreach (Match x in Regex.Matches(line2D[mmm], patternX))
                {
                    IndicatorRead2DX = 7;
                    string x1 = Convert.ToString(x);
                    x1 = x1.Replace(".", ",");
                    Read2DX = Convert.ToSingle(x1.Substring(1));
                }

                if (IndicatorRead2DGlubinaReza * IndicatorRead2DX * IndicatorRead2DY != 15)
                {
                    if (FlagIndicatorOfFirstMoves == false)
                    {
                        WorkWithMoves.WorkAllSituationsOfCoordinates(666, Read2DX, Read2DY, GlubinaReza2D);
                        FlagIndicatorOfFirstMoves = true;
                    }
                    else
                    {
                        WorkWithMoves.WorkAllSituationsOfCoordinates(IndicatorRead2DGlubinaReza * IndicatorRead2DX * IndicatorRead2DY, Read2DX, Read2DY, GlubinaReza2D);
                    }
                }

                Write3DCoordinate.WriteEndOfOnceString();
                mmm++;
            }
        endRead:
            Write3DCoordinate.EndStrings();
            //окончание цикла, файл полностью разобран!!!!!!!!!!!!
            Write3DCoordinate.CloseStream();
        }
    }
}
