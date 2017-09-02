using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorelovskiy.ru_3._0_Console.AddictionalFunctionsClass
{
    class AddictionalForWorkWithCoordinates : WorkWithRead2DCoordinates
    {
        //***********************************************************************************************
        //Глобальные переменные
        int AFWWC_NumberOfCoordinate;
        float AFWWC_New3DA;
        float AFWWC_New3DY;
        float AFWWC_New3DZ;
        float AFWWC_New3DX;
        //***********************************************************************************************
        //_______________________________________________________________________________________________
        //________________________________________функция нахождения угла________________________________
        //_______________________________________________________________________________________________
        public float PereschetAngle(float AFWWC_New2DX, int AFWWC_NuberOfCoordinate)
        {
            float Z1, Z0;
            float Angle;
            int AFWWC_NumberOfSectionX = this.ZonaNewCoordinate(AFWWC_New2DX);
            if (AFWWC_NuberOfCoordinate == 0)
            {
                Z1 = MorePrettyZ(AFWWC_New2DX, AFWWC_NuberOfCoordinate);//текущая координата зет
                AFWWC_NuberOfCoordinate++;
                Z0 = MorePrettyZ(AFWWC_New2DX, AFWWC_NuberOfCoordinate);//координата зет соответсвующая следущей координате игрек (для армяней икс)
                AFWWC_NuberOfCoordinate--;
            }
            else
            {
                Z0 = MorePrettyZ(AFWWC_New2DX, AFWWC_NuberOfCoordinate);//текущая координата зет
                AFWWC_NuberOfCoordinate--;
                Z1 = MorePrettyZ(AFWWC_New2DX, AFWWC_NuberOfCoordinate);//координата зет соответсвующая следущей координате игрек (для армяней икс)
                AFWWC_NuberOfCoordinate++;
            }
            Angle = Convert.ToSingle(Math.Atan((Z0 - Z1) / 0.01));
            return Angle;
        }

        //__________________________________________________________________________________________________________________________________
        //___________________________________________определяем значение зет, зависящее от координаты икс___________________________________
        //__________________________________________________________________________________________________________________________________
        public float MorePrettyZ(float AFWWC_New2DX, int AFWWC_NumberOfCoordinate)//для опредения угла наклона необходимо найти координаты зет соответствующие настоящей, следущей и предыдущей координатам игрек (для армяней икс), вот именно тогда и заходим сюда
        {
            float k;
            float b;
            float prettyZ;
            int AFWWC_NumberOfSectionX = this.ZonaNewCoordinate(AFWWC_New2DX);
            if (AFWWC_New2DX != newduga[AFWWC_NumberOfSectionX, 0, 0])
            {

                k = (newduga[AFWWC_NumberOfSectionX, 2, AFWWC_NumberOfCoordinate] - newduga[AFWWC_NumberOfSectionX - 1, 2, AFWWC_NumberOfCoordinate]) / ShagSkanaVdol;
                b = newduga[AFWWC_NumberOfSectionX, 2, AFWWC_NumberOfCoordinate] - k * newduga[AFWWC_NumberOfSectionX, 0, 0];
                prettyZ = k * AFWWC_New2DX + b;
            }
            else
            {
                prettyZ = newduga[AFWWC_NumberOfSectionX, 2, AFWWC_NumberOfCoordinate];
            }
            return prettyZ;
        }

        //__________________________________________________________________________________________________________________________________
        //___________________________________________определяем зону в которой лежит настоящая координата___________________________________
        //__________________________________________________________________________________________________________________________________
        public int ZonaNewCoordinate(float AFWWC_New2DX)
        {
            int AFWCC_NumberOfScan = 0;
            int AFWWC_NumberOfSectionX = 0;
            while (AFWCC_NumberOfScan < KolichestvoProhodov)//ищем в какой области находится считанная координата по оси икс (для армяней игрек)
            {
                if (newduga[AFWCC_NumberOfScan, 0, 0] - AFWWC_New2DX <= 0)
                {
                    AFWWC_NumberOfSectionX++;
                }
                AFWCC_NumberOfScan++;
            }
            if (AFWWC_NumberOfSectionX < 1)
            {
                AFWWC_NumberOfSectionX = 1;
            }
            else if (AFWWC_NumberOfSectionX >= KolichestvoProhodov - 1)
            {
                AFWWC_NumberOfSectionX = KolichestvoProhodov - 1;
            }
            return AFWWC_NumberOfSectionX;
        }

        //__________________________________________________________________________________________________________________________________
        //_____________________________определяем порядковый номер в массиве координат скана для двух мерной координаты_____________________
        //__________________________________________________________________________________________________________________________________
        public int NumberOfCoordinateInScanMassive(float AFWWC_New2DY)
        {
            int AFWWC_NuberOfCoordinate = 0;
            try
            {
                while (newduga[0, 3, AFWWC_NuberOfCoordinate] < AFWWC_New2DY)
                {
                    AFWWC_NuberOfCoordinate++;
                }
            }
            catch (IndexOutOfRangeException)
            {
                oBF_Exception.ExeptionBigWidthOfPicture();
            }
            return AFWWC_NuberOfCoordinate;
        }

        //__________________________________________________________________________________________________________________________________
        //_________________________________________Вычисляем новые трехмерные координаты____________________________________________________
        //_________________________________________если есть изменение двухмерного игрек____________________________________________________
        //__________________________________________________________________________________________________________________________________
        public void CalculationNew3DCoordinates(float AFWWC_2DX, float AFWWC_2DY, float AFWWC_2DGlubinaReza)
        {
            AFWWC_NumberOfCoordinate = this.NumberOfCoordinateInScanMassive(AFWWC_2DY);
            AFWWC_New3DA = (-1) * this.PereschetAngle(AFWWC_2DX, AFWWC_NumberOfCoordinate);
            AFWWC_New3DY = newduga[0, 1, AFWWC_NumberOfCoordinate] + (DlinaSpindelPlusDlinaFrezi + AFWWC_2DGlubinaReza) * Convert.ToSingle(Math.Sin(AFWWC_New3DA));
            AFWWC_New3DZ = this.MorePrettyZ(AFWWC_2DX, AFWWC_NumberOfCoordinate) + (DlinaSpindelPlusDlinaFrezi + AFWWC_2DGlubinaReza) * Convert.ToSingle(Math.Cos(AFWWC_New3DA));
            //записываем текущие значения в переменный для последущего сравнения с новыми пересчитанными координатами, составленными для новых считанных координат

            Write3DCoordinate.WriteOtherMoves(AFWWC_2DX, AFWWC_New3DY, AFWWC_New3DZ, AFWWC_New3DA, DlinaSpindelPlusDlinaFrezi);

            WWR2DC_Old3DA = AFWWC_New3DA;
            WWR2DC_Old3DX = AFWWC_2DX;
            WWR2DC_Old3DY = AFWWC_New3DY;
            WWR2DC_Old3DZ = AFWWC_New3DZ;
        }

        //__________________________________________________________________________________________________________________________________
        //_________________________________________Вычисляем новые трехмерные координаты____________________________________________________
        //_______________________________________если есть нет изменение двухмерного игрек__________________________________________________
        //_______________________________________и если икс принимает зонные значения_______________________________________________________
        //__________________________________________________________________________________________________________________________________
        public void CalculationNew3DCoordinatesIf2DYDoesntChangeWithZoneX(float AFWWC_2DGlubinaReza, int AFWWC_NumberOfSection, int AFWWC_NumberOfCoordinate)
        {
            AFWWC_New3DA = (-1) * ADDFunctions.PereschetAngle(newduga[AFWWC_NumberOfSection, 0, 0], AFWWC_NumberOfCoordinate);
            AFWWC_New3DY = newduga[0, 1, AFWWC_NumberOfCoordinate] + (DlinaSpindelPlusDlinaFrezi + AFWWC_2DGlubinaReza) * Convert.ToSingle(Math.Sin(AFWWC_New3DA));
            AFWWC_New3DZ = newduga[AFWWC_NumberOfSection, 2, AFWWC_NumberOfCoordinate] + (DlinaSpindelPlusDlinaFrezi + AFWWC_2DGlubinaReza) * Convert.ToSingle(Math.Cos(AFWWC_New3DA));
            AFWWC_New3DX = newduga[AFWWC_NumberOfSection, 0, 0];

            //записываем в файл новые координаты
            Write3DCoordinate.WriteOtherMoves(AFWWC_New3DX, AFWWC_New3DY, AFWWC_New3DZ, AFWWC_New3DA, DlinaSpindelPlusDlinaFrezi);
        }

        //__________________________________________________________________________________________________________________________________
        //_________________________________________Вычисляем новые трехмерные координаты____________________________________________________
        //_______________________________________если есть нет изменение двухмерного игрек__________________________________________________
        //_______________________________________и если икс принимает не зонные значения____________________________________________________
        //__________________________________________________________________________________________________________________________________
        public void CalculationNew3DCoordinatesIf2DYDoesntChangeWithNotZoneX(float AFWWC_2DX, float AFWWC_2DGlubinaReza, int AFWWC_NumberOfCoordinate)
        {
            AFWWC_New3DA = (-1) * ADDFunctions.PereschetAngle(AFWWC_2DX, AFWWC_NumberOfCoordinate);
            AFWWC_New3DY = newduga[0, 1, AFWWC_NumberOfCoordinate] + (DlinaSpindelPlusDlinaFrezi + AFWWC_2DGlubinaReza) * Convert.ToSingle(Math.Sin(AFWWC_New3DA));
            AFWWC_New3DZ = this.MorePrettyZ(AFWWC_2DX, AFWWC_NumberOfCoordinate) + (DlinaSpindelPlusDlinaFrezi + AFWWC_2DGlubinaReza) * Convert.ToSingle(Math.Cos(AFWWC_New3DA));
            AFWWC_New3DX = AFWWC_2DX;

            //записываем в файл новые координаты
            Write3DCoordinate.WriteOtherMoves(AFWWC_New3DX, AFWWC_New3DY, AFWWC_New3DZ, AFWWC_New3DA, DlinaSpindelPlusDlinaFrezi);

            //переписываем старые координаты в программе разбора считанных координат
            WWR2DC_Old3DX = AFWWC_New3DX;
            WWR2DC_Old3DY = AFWWC_New3DY;
            WWR2DC_Old3DZ = AFWWC_New3DZ;
            WWR2DC_Old3DA = AFWWC_New3DA;
        }
    }
}
