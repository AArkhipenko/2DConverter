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
        public void WriteInformationStrings(float DlinaShpindel, float yPriMinZ, float MaxY, float MaxZ, float centrY, float centrZ, float HordaFasada)
        {
            StreamForWrite3DCoordinates.WriteLine("(  GORELOVSKIY.RU  )\r\n\r\n");
            StreamForWrite3DCoordinates.WriteLine("(DzRotory = {0})\r\n\r\n", DlinaShpindel); //здесь была высота фрезы
            StreamForWrite3DCoordinates.WriteLine("(Нижняя точка: Z=0   при   Y={0})\r\n", yPriMinZ);
            StreamForWrite3DCoordinates.WriteLine("(Максимальная точка y={0} z={1})", MaxY, MaxZ);
            StreamForWrite3DCoordinates.WriteLine(" \r\n\r\n");
            StreamForWrite3DCoordinates.WriteLine("(Координаты точки начала сканирования фасада:)" + "\r\n" + "(Y={0} Z={1})", Math.Round(centrY, 2), Math.Round(centrZ, 2));
            StreamForWrite3DCoordinates.WriteLine(" \r\n\r\n");
            StreamForWrite3DCoordinates.WriteLine("G43");
            StreamForWrite3DCoordinates.WriteLine("G0    Z{0}    A0", MaxZ + 20);
            StreamForWrite3DCoordinates.WriteLine("G0    Y-50    ");
            StreamForWrite3DCoordinates.WriteLine("G0    X{0}    ", HordaFasada / 2);
        }

        //___________________________________________________________________________
        //___________________Записываем выход из фасада______________________________
        //___________________после полного выполенения УП____________________________
        //___________________________________________________________________________
        public void EndStrings()
        {
            StreamForWrite3DCoordinates.WriteLine("M05" + "\r\n" + "G53 G0 Z-50" + "\r\n" + "A0" + "\r\n" + "M30");
        }

        //___________________________________________________________________________
        //_________________________________Записываем G______________________________
        //___________________________________________________________________________
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
        public void WriteM(float W_M)
        {
            StreamForWrite3DCoordinates.Write("M" + Math.Round(W_M, 2) + "\t");
        }
        //___________________________________________________________________________
        //_________________________________Записываем T______________________________
        //___________________________________________________________________________
        public void WriteT(float W_T)
        {
            StreamForWrite3DCoordinates.Write("T" + Math.Round(W_T, 2) + "\t");
        }

        //___________________________________________________________________________
        //_________________________________Записываем S______________________________
        //___________________________________________________________________________
        public void WriteS(float W_S)
        {
            StreamForWrite3DCoordinates.Write("S" + Math.Round(W_S, 2) + "\t");
        }

        //___________________________________________________________________________
        //_________________________________Записываем F______________________________
        //___________________________________________________________________________
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
        public void WriteEndOfOnceString()
        {
            StreamForWrite3DCoordinates.Write("\r\n");
        }
        //___________________________________________________________________________
        //___________________________Записываем сброс строки_________________________
        //___________________________________________________________________________
        public void CloseStream()
        {
            StreamForWrite3DCoordinates.Close();
        }
        //___________________________________________________________________________
        //________________________________Первое перемещение_________________________
        //___________________________________________________________________________
        public void WriteFirstMoves(float W_X, float W_Y, float W_Z, float W_A, float DlinaShpindelPlusDlinaFrezi)
        {
            StreamForWrite3DCoordinates.Write("Y" + Math.Round(W_X, 2) + "\r\n" + "X" + Math.Round(W_Y, 2) + "\t" + "A" + (-1) * Math.Round(180 * W_A / Math.PI, 2) + "\r\n" + "Z" + (Math.Round(W_Z - DlinaShpindelPlusDlinaFrezi, 2)) + "\r\n");
        }
        //___________________________________________________________________________
        //_____________________________Остальные перемещение_________________________
        //___________________________________________________________________________
        public void WriteOtherMoves(float W_X, float W_Y, float W_Z, float W_A, float DlinaShpindelPlusDlinaFrezi)
        {
            StreamForWrite3DCoordinates.Write("Y" + Math.Round(W_X, 2) + "    " + "X" + Math.Round(W_Y, 2) + " " + "Z" + (Math.Round(W_Z - DlinaShpindelPlusDlinaFrezi, 2)) + "  " + "A" + (-1) * Math.Round(180 * W_A / Math.PI, 2) + "\r\n");
        }
    }
}
