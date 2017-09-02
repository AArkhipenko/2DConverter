using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorelovskiy.ru_3._0_Console.CoordinatesWork
{
    class MoveToYAndHigh:WorkWithRead2DCoordinates
    {
        //_______________________________________________________________________________________________________
        //_____________________________________Изменеие координаты игрек неглобальные____________________________
        //_______________________________________________________________________________________________________
        public void ChooseOurWay(float CYCAGR_New2DX, float CYCAGR_New2DY, float CYCAGR_Old2DY, float CYCAGR_New2DGlubinaReza, float CYCAGR_Old2DGlubinaReza)
        {
            if (Math.Abs(CYCAGR_New2DY - CYCAGR_Old2DY) < 5)
            {
                this.NotGlobalChangYCoordinateAndGlubinaReza(CYCAGR_New2DX, CYCAGR_New2DY, CYCAGR_Old2DY, CYCAGR_New2DGlubinaReza, CYCAGR_Old2DGlubinaReza);
            }
            else
            {
                this.GlobalChangYCoordinateAndGlubinaReza(CYCAGR_New2DX, CYCAGR_New2DY, CYCAGR_Old2DY, CYCAGR_New2DGlubinaReza, CYCAGR_Old2DGlubinaReza);
            }
        }
        //_______________________________________________________________________________________________________
        //_____________________________________Изменеие координаты игрек неглобальные____________________________
        //_______________________________________________________________________________________________________
        public void NotGlobalChangYCoordinateAndGlubinaReza(float CYCAGR_New2DX, float CYCAGR_New2DY, float CYCAGR_Old2DY, float CYCAGR_New2DGlubinaReza, float CYCAGR_Old2DGlubinaReza)
        {
            ADDFunctions.CalculationNew3DCoordinates(CYCAGR_New2DX, CYCAGR_New2DY, CYCAGR_New2DGlubinaReza);
        }

        //_______________________________________________________________________________________________________
        //_____________________________________Изменение координаты игрек глобальные_____________________________
        //_______________________________________________________________________________________________________
        public void GlobalChangYCoordinateAndGlubinaReza(float CYCAGR_New2DX, float CYCAGR_New2DY, float CYCAGR_Old2DY, float CYCAGR_New2DGlubinaReza, float CYCAGR_Old2DGlubinaReza)
        {
            //определяем основные значения
            int KolvoOtrezkovPoY = Convert.ToInt32(Math.Abs(CYCAGR_New2DY - CYCAGR_Old2DY) / 5);//разбиваем отрезок изменения игрек на отрезки по пять миллиметров и смотри сколько раз такой отрезок поместится в нашем отрезке
            float DlinaOtrezkaPoY=Math.Abs(CYCAGR_New2DY - CYCAGR_Old2DY)/KolvoOtrezkovPoY;//получаем делта игрек, то есть приращение координаты игрек на каждом шаге

            float k = (CYCAGR_New2DGlubinaReza - CYCAGR_Old2DGlubinaReza) / (CYCAGR_New2DY - CYCAGR_Old2DY);//определяем кооэффициенты в уравнении прямой
            float b = CYCAGR_New2DGlubinaReza - k * CYCAGR_New2DY;

            if (CYCAGR_Old2DY < CYCAGR_New2DY)//если старая координата меньше новой
            {
                while (CYCAGR_Old2DY < CYCAGR_New2DY)
                {
                    CYCAGR_Old2DY = CYCAGR_Old2DY + DlinaOtrezkaPoY;//следущая точка по игрек
                    float TemporalH = EquationOf2DLine(k, b, CYCAGR_Old2DY);//определяем промежуточную глубину реза из уравнения прямой
                    ADDFunctions.CalculationNew3DCoordinates(CYCAGR_New2DX, CYCAGR_Old2DY, TemporalH);
                }
            }
            else//если старая координата больше новой
            {
                while (CYCAGR_Old2DY > CYCAGR_New2DY)
                {
                    CYCAGR_Old2DY = CYCAGR_Old2DY - DlinaOtrezkaPoY;//следущая точка по игрек
                    float TemporalH = EquationOf2DLine(k, b, CYCAGR_Old2DY);//определяем промежуточную глубину реза из уравнения прямой
                    ADDFunctions.CalculationNew3DCoordinates(CYCAGR_New2DX, CYCAGR_Old2DY, TemporalH);
                }
            }
        }

        //_______________________________________________________________________________________________________
        //_____________________________________Определяем новую координату зет___________________________________
        //_______________________________________________________________________________________________________
        //рассматриваем двухмерный рисунок с изменением координаты игрек и зет, находим уравнение прямой, по 
        //которой идет такое изменение координат, и с помощью этого уравнения находим новую координату зет
        public float EquationOf2DLine(float k, float b, float y)
        {
            float NewGlubinaReza = k * y + b;
            return NewGlubinaReza;
        }
    }
}
