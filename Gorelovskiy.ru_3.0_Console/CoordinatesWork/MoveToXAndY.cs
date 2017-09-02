using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorelovskiy.ru_3._0_Console.CoordinatesWork
{
    class MoveToXAndY : WorkWithRead2DCoordinates
    {
        //__________________________________________________________________________________________________________
        //______________________________Метод в котором выбираем нужные действия____________________________________
        //__________________________________________________________________________________________________________
        public void ChooseOurActions_ChangeXAndYCoordinates(float CXAYC_New2DX, float CXAYC_Old2DX, float CXAYC_New2DY, float CXAYC_Old2DY, float CXAYC_Old2DGlubinaReza)
        {
            float DlinaPerezda = Convert.ToSingle(Math.Sqrt((CXAYC_New2DX - CXAYC_Old2DX) * (CXAYC_New2DX - CXAYC_Old2DX) + (CXAYC_New2DY - CXAYC_Old2DY) * (CXAYC_New2DY - CXAYC_Old2DY)));
            if (DlinaPerezda < 5)
            {
                this.NotGlobalMoveInXAndYCoordinates(CXAYC_New2DX, CXAYC_Old2DX, CXAYC_New2DY, CXAYC_Old2DY, CXAYC_Old2DGlubinaReza);
            }
            else
            {
                this.GlobalMoveInXAndYCoordinates(CXAYC_New2DX, CXAYC_Old2DX, CXAYC_New2DY, CXAYC_Old2DY, CXAYC_Old2DGlubinaReza);
            }
        }
        //__________________________________________________________________________________________________________
        //______________________________________________Небольшой переезд___________________________________________
        //__________________________________________________________________________________________________________
        public void NotGlobalMoveInXAndYCoordinates(float CXAYC_New2DX, float CXAYC_Old2DX, float CXAYC_New2DY, float CXAYC_Old2DY, float CXAYC_Old2DGlubinaReza)
        {
            ADDFunctions.CalculationNew3DCoordinates(CXAYC_New2DX, CXAYC_New2DY, CXAYC_Old2DGlubinaReza);
        }
        //__________________________________________________________________________________________________________
        //________________________________________________Большой переезд___________________________________________
        //__________________________________________________________________________________________________________
        public void GlobalMoveInXAndYCoordinates(float CXAYC_New2DX, float CXAYC_Old2DX, float CXAYC_New2DY, float CXAYC_Old2DY, float CXAYC_Old2DGlubinaReza)
        {
            float k = (CXAYC_New2DX - CXAYC_Old2DX) / (CXAYC_New2DY - CXAYC_Old2DY);//тангенс угла наклона прямой x(y)
            float b = CXAYC_New2DX - k * CXAYC_New2DY;//свободный член прямой x(y)
            float deltaY = 5 / Convert.ToSingle(Math.Cos(Math.Atan(Math.Abs(k))));//вычисляем какое должно иметь приращение координата игрек, чтобы длина отрезочка была 5мм

            if (CXAYC_New2DY > CXAYC_Old2DY)//новая координата игрек больше старой
            {
                while (Math.Abs(CXAYC_Old2DY - CXAYC_New2DY)>deltaY)
                {
                    CXAYC_Old2DY = CXAYC_Old2DY + deltaY;//промежуточное значение игрек
                    CXAYC_Old2DX = this.MediumX(k, b, CXAYC_Old2DY);//промежуточное значение икс

                    ADDFunctions.CalculationNew3DCoordinates(CXAYC_Old2DX, CXAYC_Old2DY, CXAYC_Old2DGlubinaReza);
                }
                ADDFunctions.CalculationNew3DCoordinates(CXAYC_New2DX, CXAYC_New2DY, CXAYC_Old2DGlubinaReza);
            }
            else//старая координата игрек больше новой
            {
                while (Math.Abs(CXAYC_Old2DY - CXAYC_New2DY) > deltaY)
                {
                    CXAYC_Old2DY = CXAYC_Old2DY - deltaY;//промежуточное значение игрек
                    CXAYC_Old2DX = this.MediumX(k, b, CXAYC_Old2DY);//промежуточное значение икс

                    ADDFunctions.CalculationNew3DCoordinates(CXAYC_Old2DX, CXAYC_Old2DY, CXAYC_Old2DGlubinaReza);
                }
                ADDFunctions.CalculationNew3DCoordinates(CXAYC_New2DX, CXAYC_New2DY, CXAYC_Old2DGlubinaReza);
            }
        }

        //______________________________________________________________________________________________________________
        //_______________________________________Функция определения промежуточного икса________________________________
        //______________________________________________________________________________________________________________
        public float MediumX(float k, float b, float y)
        {
            float MediumX = k * y + b;
            return MediumX;
        }
    }
}