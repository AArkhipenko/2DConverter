using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorelovskiy.ru_3._0_Console.CoordinatesWork
{
    class MoveToXAndYAndHigh: WorkWithRead2DCoordinates
    {
        //__________________________________________________________________________________________________________
        //______________________________Метод в котором выбираем нужные действия____________________________________
        //__________________________________________________________________________________________________________
        public void ChooseOurActions_ChangeXAndYCoordinatesAndHigh(float CXAYCAH_New2DX, float CXAYCAH_Old2DX, float CXAYCAH_New2DY, float CXAYCAH_Old2DY, float CXAYCAH_New2DGlubinaReza, float CXAYCAH_Old2DGlubinaReza)
        {
            float DlinaPerezda = Convert.ToSingle(Math.Sqrt((CXAYCAH_New2DX - CXAYCAH_Old2DX) * (CXAYCAH_New2DX - CXAYCAH_Old2DX) + (CXAYCAH_New2DY - CXAYCAH_Old2DY) * (CXAYCAH_New2DY - CXAYCAH_Old2DY)));//определяем длину переезда в двухмерных координатах в плоскости Oxy, так как важна только длиня проекции в Oxy
            if (DlinaPerezda < 5)
            {
                this.NotGlobalMoveInXAndYCoordinates(CXAYCAH_New2DX, CXAYCAH_Old2DX, CXAYCAH_New2DY, CXAYCAH_Old2DY,CXAYCAH_New2DGlubinaReza, CXAYCAH_Old2DGlubinaReza);
            }
            else
            {
                this.GlobalMoveInXAndYCoordinates(CXAYCAH_New2DX, CXAYCAH_Old2DX, CXAYCAH_New2DY, CXAYCAH_Old2DY,CXAYCAH_New2DGlubinaReza, CXAYCAH_Old2DGlubinaReza);
            }
        }
        //__________________________________________________________________________________________________________
        //______________________________________________Небольшой переезд___________________________________________
        //__________________________________________________________________________________________________________
        public void NotGlobalMoveInXAndYCoordinates(float CXAYCAH_New2DX, float CXAYCAH_Old2DX, float CXAYCAH_New2DY, float CXAYCAH_Old2DY, float CXAYCAH_New2DGlubinaReza, float CXAYCAH_Old2DGlubinaReza)
        {
            ADDFunctions.CalculationNew3DCoordinates(CXAYCAH_New2DX, CXAYCAH_New2DY, CXAYCAH_New2DGlubinaReza);
        }
        //__________________________________________________________________________________________________________
        //________________________________________________Большой переезд___________________________________________
        //__________________________________________________________________________________________________________
        public void GlobalMoveInXAndYCoordinates(float CXAYCAH_New2DX, float CXAYCAH_Old2DX, float CXAYCAH_New2DY, float CXAYCAH_Old2DY, float CXAYCAH_New2DGlubinaReza, float CXAYCAH_Old2DGlubinaReza)
        {
            float kXY = (CXAYCAH_New2DX - CXAYCAH_Old2DX) / (CXAYCAH_New2DY - CXAYCAH_Old2DY);//тангенс угла наклона прямой x(y)
            float bXY = CXAYCAH_New2DX - kXY * CXAYCAH_New2DY;//свободный член прямой x(y)
            float DlinaXY = Convert.ToSingle(Math.Sqrt((CXAYCAH_New2DX - CXAYCAH_Old2DX) * (CXAYCAH_New2DX - CXAYCAH_Old2DX) + (CXAYCAH_New2DY - CXAYCAH_Old2DY) * (CXAYCAH_New2DY - CXAYCAH_Old2DY)));//длина отрезка в плоскости Oxy
            int CeloeChisloRaz = Convert.ToInt16( DlinaXY / 5);//целое количество раз которое 5мм помещяются в отрезке в плоскости Оху
            float deltaY = (DlinaXY / CeloeChisloRaz) * Convert.ToSingle(Math.Cos(Math.Atan(Math.Abs(kXY))));//вычисляем какое должно иметь приращение координата игрек, чтобы длина отрезочка была 5мм

            float kYZ = (CXAYCAH_New2DGlubinaReza - CXAYCAH_Old2DGlubinaReza) / (CXAYCAH_New2DY - CXAYCAH_Old2DY);//тангенс угла наклона прямой z(y)
            float bYZ = CXAYCAH_New2DGlubinaReza - kYZ * CXAYCAH_New2DY;//свободный член прямой z(y)

            if (CXAYCAH_New2DY > CXAYCAH_Old2DY)//новая координата игрек больше старой
            {
                while (Math.Round(CXAYCAH_Old2DY, 2) < Math.Round(CXAYCAH_New2DY, 2))
                {
                    CXAYCAH_Old2DY = CXAYCAH_Old2DY + deltaY;//промежуточное значение игрек
                    CXAYCAH_Old2DX = this.MediumX(kXY, bXY, CXAYCAH_Old2DY);//промежуточное значение икс
                    CXAYCAH_Old2DGlubinaReza = this.MediumHigh(kYZ, bYZ, CXAYCAH_Old2DY);//промежуточное значение глубины реза фрезы

                    ADDFunctions.CalculationNew3DCoordinates(CXAYCAH_Old2DX, CXAYCAH_Old2DY, CXAYCAH_Old2DGlubinaReza);
                }
            }
            else//старая координата игрек больше новой
            {
                while (Math.Round(CXAYCAH_Old2DY, 2) > Math.Round(CXAYCAH_New2DY, 2))
                {
                    CXAYCAH_Old2DY = CXAYCAH_Old2DY - deltaY;//промежуточное значение игрек
                    CXAYCAH_Old2DX = this.MediumX(kXY, bXY, CXAYCAH_Old2DY);//промежуточное значение икс
                    CXAYCAH_Old2DGlubinaReza = this.MediumHigh(kYZ, bYZ, CXAYCAH_Old2DY);//промежуточное значение глубины реза фрезы

                    ADDFunctions.CalculationNew3DCoordinates(CXAYCAH_Old2DX, CXAYCAH_Old2DY, CXAYCAH_Old2DGlubinaReza);
                }
            }
        }

        //______________________________________________________________________________________________________________
        //_______________________________________Функция определения промежуточного икса________________________________
        //______________________________________________________________________________________________________________
        public float MediumX(float k, float b, float y)
        {
            float MediumX = k*y+b;
            return MediumX;
        }

        //______________________________________________________________________________________________________________
        //_________________________________Функция определения промежуточной глубины реза_______________________________
        //______________________________________________________________________________________________________________
        public float MediumHigh(float k, float b, float y)
        {
            float MediumHigh= k * y + b;
            return MediumHigh;
        }
    }
}
