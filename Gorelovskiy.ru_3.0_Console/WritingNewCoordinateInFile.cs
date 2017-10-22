using System;
using System.Linq;
using System.Text;
using System.IO;

namespace Gorelovskiy.ru_3._0_Console
{
    class WritingNewCoordinateInFile : BasicFunction
    {
        StreamWriter StreamForWrite3DCoordinates;
        //___________________________________________________________________________
        //_____________________________Конструктор класса____________________________
        //___________________________________________________________________________
        /// <summary>
        /// Создание потока для записи в новый файл.
        /// </summary>
        public WritingNewCoordinateInFile()
        {
            try
            {
                StreamForWrite3DCoordinates = new StreamWriter(@"C:\Mach3\GCode\3DGcode.tap", false, Encoding.GetEncoding(1251));//3DGcode
            }
            catch (IOException ex)
            {
                oBF_Exception.Exception3DPicture();
            }
        }

        //___________________________________________________________________________
        //___________________Запись информационных строк в файл______________________
        //___________________________________________________________________________
        /// <summary>
        /// Записываем первые строки в файл.
        /// </summary>
        /// <param name="DlinaShpindel">Длина шпинделя (берется из файла)</param>
        /// <param name="yPriMinZ">(y;z) минимальная точка</param>
        /// <param name="MaxY">(y;z) максимальная точка</param>
        /// <param name="MaxZ">(y;z) максимальная точка</param>
        /// <param name="centrY">(y;z) середина фасада</param>
        /// <param name="centrZ">(y;z) середина фасада</param>
        /// <param name="HordaFasada"></param>
        public void WriteInformationStrings(float DlinaShpindel, float yPriMinZ, float MaxY, float MaxZ, float centrY, float centrZ)
        {
            StreamForWrite3DCoordinates.WriteLine("(  GORELOVSKIY.RU  )\r\n\r\n");
            StreamForWrite3DCoordinates.WriteLine("(DzRotory = {0})\r\n\r\n", DlinaShpindel); //здесь была высота фрезы
            StreamForWrite3DCoordinates.WriteLine("(Нижняя точка: Z=0   при   X={0})\r\n", yPriMinZ);
            StreamForWrite3DCoordinates.WriteLine("(Максимальная точка Z={1}  при X={0} )", MaxY, MaxZ);
            StreamForWrite3DCoordinates.WriteLine("(Координаты точки начала сканирования фасада:)" + "\r\n" + "(Z={1} при X={0} )", Math.Round(centrY, 2), Math.Round(centrZ, 2));
            StreamForWrite3DCoordinates.WriteLine(" \r\n\r\n");
            StreamForWrite3DCoordinates.WriteLine("G43");
            StreamForWrite3DCoordinates.WriteLine("G0    Z{0}    A0", MaxZ + 20);

        }

        //___________________________________________________________________________
        //___________________Записываем выход из фасада______________________________
        //___________________после полного выполенения УП____________________________
        //___________________________________________________________________________
        /// <summary>
        /// Запись последних строк в файл.
        /// </summary>
        public void EndStrings()
        {
            StreamForWrite3DCoordinates.WriteLine("M05" + "\r\n" + "G53 G0 Z0" + "\r\n" + "G0 A0" + "\r\n" + "M30");
        }

        //___________________________________________________________________________
        //_________________________________Записываем G______________________________
        //___________________________________________________________________________
        /// <summary>
        /// Записываем G в файл
        /// </summary>
        /// <param name="W_G">значение G</param>
        public void WriteG(float W_G)
        {
            if (W_G == 0)
            {
                StreamForWrite3DCoordinates.Write("G" + 1 + "\t" + "F" + 10000 + "\t");
            }
            else
            {
                StreamForWrite3DCoordinates.Write("G" + Math.Round(W_G, 2) + "\t");
            }
        }

        //___________________________________________________________________________
        //_________________________________Записываем M______________________________
        //___________________________________________________________________________
        /// <summary>
        /// Записываем M в файл
        /// </summary>
        /// <param name="W_M">значение M</param>
        public void WriteM(float W_M)
        {
            StreamForWrite3DCoordinates.Write("M" + Math.Round(W_M, 2) + "\t");
        }

        //___________________________________________________________________________
        //_________________________________Записываем T______________________________
        //___________________________________________________________________________
        /// <summary>
        /// Записываем T (только для рисунков с автоматической сменой инструмента)
        /// </summary>
        /// <param name="W_T">значение Т</param>
        public void WriteT(float W_T)
        {
            StreamForWrite3DCoordinates.Write("T" + Math.Round(W_T, 2) + "\t");
        }

        //___________________________________________________________________________
        //_________________________________Записываем S______________________________
        //___________________________________________________________________________
        /// <summary>
        /// Записываем S в файл
        /// </summary>
        /// <param name="W_S">значение S</param>
        public void WriteS(float W_S)
        {
            StreamForWrite3DCoordinates.Write("S" + Math.Round(W_S, 2) + "\t");
        }

        //___________________________________________________________________________
        //_________________________________Записываем F______________________________
        //___________________________________________________________________________
        /// <summary>
        /// Записываем F в файл
        /// </summary>
        /// <param name="W_F">значение F</param>
        public void WriteF(float W_F)
        {
            StreamForWrite3DCoordinates.Write("F" + W_F + " \r\n");
        }

        //___________________________________________________________________________
        //___________________________Записываем X, Y, Z, A___________________________
        //___________________________________________________________________________
        public void WriteX_Y_Z_A(float W_X, float W_Y, float W_Z, float W_A)
        {
            StreamForWrite3DCoordinates.Write(" ");
        }

        //___________________________________________________________________________
        //___________________________Записываем сброс строки_________________________
        //___________________________________________________________________________
        /// <summary>
        /// Записываем конец одной строки
        /// </summary>
        public void WriteEndOfOnceString()
        {
            StreamForWrite3DCoordinates.Write("\r\n");
        }

        //___________________________________________________________________________
        //___________________________Записываем сброс строки_________________________
        //___________________________________________________________________________
        /// <summary>
        /// Закрытие потока записи в файл
        /// </summary>
        public void CloseStream()
        {
            StreamForWrite3DCoordinates.Close();
        }

        //___________________________________________________________________________
        //________________________________Первое перемещение_________________________
        //___________________________________________________________________________
        /// <summary>
        /// Записываем первые встретившиеся координаты.
        /// </summary>
        /// <param name="W_X">трехмерная координата Х</param>
        /// <param name="W_Y">трехмерная координата Y</param>
        /// <param name="W_Z">трехмерная координата Z</param>
        /// <param name="W_A">угол поворота шпинделя А</param>
        /// <param name="DlinaShpindelPlusDlinaFrezi">Длина шпинделя с длиной фрезы</param>
        public void WriteFirstMoves(float W_X, float W_Y, float W_Z, float W_A, float DlinaShpindelPlusDlinaFrezi)
        {
            if (BasicFunction.customer == (int)BasicFunction.customers.ArmenGeorgevsk ||
                BasicFunction.customer == (int)BasicFunction.customers.ArmenOtradnoe)
            {
                StreamForWrite3DCoordinates.Write("Y" + Math.Round(W_X, 2) + "\r\n");
                StreamForWrite3DCoordinates.Write("X" + Math.Round(W_Y, 2) + "\t" + "A" + (-1) * Math.Round(180 * W_A / Math.PI, 2)+"\r\n");
                StreamForWrite3DCoordinates.Write("Z" + (Math.Round(W_Z - DlinaShpindelPlusDlinaFrezi, 2)) + "\r\n");
            }
            else if (BasicFunction.customer == (int)BasicFunction.customers.VerlinRostov)
            {
                StreamForWrite3DCoordinates.Write("Y" + Math.Round(W_Y, 2) + "\r\n");
                StreamForWrite3DCoordinates.Write("X" + Math.Round(W_X, 2) + "\t" + "A" + (-1) * Math.Round(180 * W_A / Math.PI, 2) + "\r\n");
                StreamForWrite3DCoordinates.Write("Z" + (Math.Round(W_Z - DlinaShpindelPlusDlinaFrezi, 2)) + "\r\n");
            }
            else if(BasicFunction.customer == (int)BasicFunction.customers.DiakovRostov)
            {
                StreamForWrite3DCoordinates.Write("Y" + Math.Round(W_Y, 2) + "\r\n");
                StreamForWrite3DCoordinates.Write("X" + Math.Round(W_X, 2) + "\t" + "A" + Math.Round(180 * W_A / Math.PI, 2) + "\r\n");
                StreamForWrite3DCoordinates.Write("Z" + (Math.Round(W_Z - DlinaShpindelPlusDlinaFrezi, 2)) + "\r\n");
            }
        }

        //___________________________________________________________________________
        //_____________________________Остальные перемещение_________________________
        //___________________________________________________________________________
        /// <summary>
        /// Записываем не перве координаты.
        /// </summary>
        /// <param name="W_X">трехмерная координата Х</param>
        /// <param name="W_Y">трехмерная координата Y</param>
        /// <param name="W_Z">трехмерная координата Z</param>
        /// <param name="W_A">угол поворота шпинделя А</param>
        /// <param name="DlinaShpindelPlusDlinaFrezi">Длина шпинделя с длиной фрезы</param>
        public void WriteOtherMoves(float W_X, float W_Y, float W_Z, float W_A, float DlinaShpindelPlusDlinaFrezi)
        {
            if (BasicFunction.customer == (int)BasicFunction.customers.ArmenGeorgevsk ||
                BasicFunction.customer == (int)BasicFunction.customers.ArmenOtradnoe)
            {
                StreamForWrite3DCoordinates.Write("Y" + Math.Round(W_X, 2) + " ");
                StreamForWrite3DCoordinates.Write("X" + Math.Round(W_Y, 2) + " ");
                StreamForWrite3DCoordinates.Write("Z" + (Math.Round(W_Z - DlinaShpindelPlusDlinaFrezi, 2)) + " ");
                StreamForWrite3DCoordinates.Write("A" + (-1) * Math.Round(180 * W_A / Math.PI, 2) + "\r\n");
            }
            else if (BasicFunction.customer == (int)BasicFunction.customers.VerlinRostov)
            {
                StreamForWrite3DCoordinates.Write("Y" + Math.Round(W_Y, 2) + " ");
                StreamForWrite3DCoordinates.Write("X" + Math.Round(W_X, 2) + " ");
                StreamForWrite3DCoordinates.Write("Z" + (Math.Round(W_Z - DlinaShpindelPlusDlinaFrezi, 2)) + " ");
                StreamForWrite3DCoordinates.Write("A" + (-1) * Math.Round(180 * W_A / Math.PI, 2) + "\r\n");
            }
            else if (BasicFunction.customer == (int)BasicFunction.customers.DiakovRostov)
            {
                StreamForWrite3DCoordinates.Write("Y" + Math.Round(W_Y, 2) + " ");
                StreamForWrite3DCoordinates.Write("X" + Math.Round(W_X, 2) + " ");
                StreamForWrite3DCoordinates.Write("Z" + (Math.Round(W_Z - DlinaShpindelPlusDlinaFrezi, 2)) + " ");
                StreamForWrite3DCoordinates.Write("A" + Math.Round(180 * W_A / Math.PI, 2) + "\r\n");
            }
        }
    }
}
