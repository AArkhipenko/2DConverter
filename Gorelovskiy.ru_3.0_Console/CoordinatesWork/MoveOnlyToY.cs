using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorelovskiy.ru_3._0_Console.CoordinatesWork
{
    class MoveOnlyToY:WorkWithRead2DCoordinates
    {
        //_____________________________________________________________________________________
        //________________________________Определяем какие измененеия игрек____________________
        //_____________________________________________________________________________________
        public void ChooseOurWay(float COYC_New2DX, float COYC_New2DY, float COYC_Old2DY, float COYC_GlubinaReza)
        {
            if (Math.Abs(COYC_New2DY - COYC_Old2DY) < 5)
            {
                this.NotGlobalChangeOnlyYCoordinate(COYC_New2DX, COYC_New2DY, COYC_GlubinaReza);
            }
            else
            {
                this.GlobalChangeOnlyYCoordinate(COYC_New2DX, COYC_New2DY, COYC_Old2DY, COYC_GlubinaReza);
            }
        }

        //_____________________________________________________________________________________
        //__________незначительное изменение координаты игрек в рисунке двухмерном_____________
        //_____________________________________________________________________________________
        public void NotGlobalChangeOnlyYCoordinate(float COYC_New2DX, float COYC_New2DY, float COYC_GlubinaReza)
        {
            ADDFunctions.CalculationNew3DCoordinates(COYC_New2DX, COYC_New2DY, COYC_GlubinaReza);
        }

        //_____________________________________________________________________________________
        //________Значительное изменение координаты игрек в двух мерном рисунке________________
        //_____________________________________________________________________________________
        public void GlobalChangeOnlyYCoordinate(float COYC_New2DX, float COYC_New2DY, float COYC_Old2DY, float COYC_GlubinaReza)
        {
            if (COYC_Old2DY < COYC_New2DY)//старая координата меньше новой
            {
                while (COYC_New2DY - COYC_Old2DY >= 5)//к старой координате прибавляем 5мм пока она не сравняется с новой координатой
                {
                    COYC_Old2DY = COYC_Old2DY + 5;//перехали на 5мм по игрек
                    ADDFunctions.CalculationNew3DCoordinates(COYC_New2DX, COYC_Old2DY, COYC_GlubinaReza);
                }
                ADDFunctions.CalculationNew3DCoordinates(COYC_New2DX, COYC_New2DY, COYC_GlubinaReza);
            }
            else//старая координата больше новой
            {
                while (COYC_Old2DY - COYC_New2DY >= 5)//отнимаем от старой координаты 5мм пока та не сравняется с новой
                {
                    COYC_Old2DY = COYC_Old2DY - 5;//промежуточная координата
                    ADDFunctions.CalculationNew3DCoordinates(COYC_New2DX, COYC_Old2DY, COYC_GlubinaReza);
                }
                ADDFunctions.CalculationNew3DCoordinates(COYC_New2DX, COYC_New2DY, COYC_GlubinaReza);
            }
        }
    }
}
