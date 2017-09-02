using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorelovskiy.ru_3._0_Console.CoordinatesWork
{
    class MoveToXAndHigh:WorkWithRead2DCoordinates
    {
        //_______________________________________________________________________________________________________________________
        //___________________________________Смотрим в каких зонах сканирования лежат координаты_________________________________
        //_______________________________________________________________________________________________________________________
        public void ChangeXCoordinateAndHigh(float CXCAH_New2DX, float CXCAH_Old2DX, float CXCAH_Old2DY, float CXCAH_New2DGlubinaReza, float CXCAH_Old2DGlubinaReza)
        {
            int CXCAH_OldNumberOfSection = ADDFunctions.ZonaNewCoordinate(CXCAH_Old2DX);
            int CXCAH_NewNumberOfSection = ADDFunctions.ZonaNewCoordinate(CXCAH_New2DX);
            if (CXCAH_OldNumberOfSection != CXCAH_NewNumberOfSection)
            {
                this.NewXAndOldXLiesInDifferentZones(CXCAH_New2DX, CXCAH_Old2DX, CXCAH_Old2DY, CXCAH_New2DGlubinaReza,CXCAH_Old2DGlubinaReza);
            }
            else
            {
                this.NewXAndOldXLiesInOnceZones(CXCAH_New2DX, CXCAH_Old2DX, CXCAH_Old2DY, CXCAH_New2DGlubinaReza, CXCAH_Old2DGlubinaReza);
            }
        }

        //_______________________________________________________________________________________________________________________
        //___________________________________Старая и новые координаты лежат в разных зонх_______________________________________
        //_______________________________________________________________________________________________________________________
        public void NewXAndOldXLiesInDifferentZones(float CXCAH_New2DX, float CXCAH_Old2DX, float CXCAH_Old2DY, float CXCAH_New2DGlubinaReza, float CXCAH_Old2DGlubinaReza)
        {
            float MediumGlubinaReza;

            int CXCAH_OldNumberOfSection = ADDFunctions.ZonaNewCoordinate(CXCAH_Old2DX);//зона в которой лежит старая координата икс
            int CXCAH_NewNumberOfSection = ADDFunctions.ZonaNewCoordinate(CXCAH_New2DX);//зона в которой лежит новая координата икс
            int CXCAH_NumberOfCoordinate = ADDFunctions.NumberOfCoordinateInScanMassive(CXCAH_Old2DY);//порядковый номер координаты игрек(она остается постоянной, так как перемещение идет только по икс и зет)

            float k = (CXCAH_New2DGlubinaReza - CXCAH_Old2DGlubinaReza) / (CXCAH_New2DX - CXCAH_Old2DX);//тангенс угла наклона прямой z(x) в двухмерных координатах
            float b = CXCAH_New2DGlubinaReza - k * CXCAH_New2DX;//свободный член прямой z(x) в двухмерных координатах

            if (CXCAH_OldNumberOfSection > CXCAH_NewNumberOfSection)//старая координата лежит в зоне которая номер которой больше номера зоны в которой лежит новая координата
            {
                while (CXCAH_OldNumberOfSection > CXCAH_NewNumberOfSection)
                {
                    MediumGlubinaReza = this.MediumGlubinaReza(k, b, newduga[CXCAH_OldNumberOfSection, 0, 0]);//вычисляем промежуточную глубину реза
                    ADDFunctions.CalculationNew3DCoordinatesIf2DYDoesntChangeWithZoneX(MediumGlubinaReza, CXCAH_OldNumberOfSection, CXCAH_NumberOfCoordinate);
                    CXCAH_OldNumberOfSection--;
                }
                ADDFunctions.CalculationNew3DCoordinatesIf2DYDoesntChangeWithNotZoneX(CXCAH_New2DX, CXCAH_New2DGlubinaReza, CXCAH_NumberOfCoordinate);
            }
            else//старая координата лежит в зоне которая номер которой меньше номера зоны в которой лежит новая координата
            {
                while (CXCAH_OldNumberOfSection < CXCAH_NewNumberOfSection)
                {
                    MediumGlubinaReza = this.MediumGlubinaReza(k, b, newduga[CXCAH_OldNumberOfSection, 0, 0]);//вычисляем промежуточную глубину реза
                    ADDFunctions.CalculationNew3DCoordinatesIf2DYDoesntChangeWithZoneX(MediumGlubinaReza, CXCAH_OldNumberOfSection, CXCAH_NumberOfCoordinate);
                    CXCAH_OldNumberOfSection++;
                }
                ADDFunctions.CalculationNew3DCoordinatesIf2DYDoesntChangeWithNotZoneX(CXCAH_New2DX, CXCAH_New2DGlubinaReza, CXCAH_NumberOfCoordinate);
            }
        }

        //_______________________________________________________________________________________________________________________
        //___________________________________Старая и новая кординаты лежат в одной зоне_________________________________________
        //_______________________________________________________________________________________________________________________
        public void NewXAndOldXLiesInOnceZones(float CXCAH_New2DX, float CXCAH_Old2DX, float CXCAH_Old2DY, float CXCAH_New2DGlubinaReza, float CXCAH_Old2DGlubinaReza)
        {
            int CXCAH_NumberOfCoordinate = ADDFunctions.NumberOfCoordinateInScanMassive(CXCAH_Old2DY);//порядковый номер нашей координаты игрек  в массиве координат игрек

            ADDFunctions.CalculationNew3DCoordinatesIf2DYDoesntChangeWithNotZoneX(CXCAH_New2DX, CXCAH_New2DGlubinaReza, CXCAH_NumberOfCoordinate);
        }

        //_______________________________________________________________________________________________________________________
        //___________________________________Функция определения промежуточной глубины реза______________________________________
        //_______________________________________________________________________________________________________________________
        public float MediumGlubinaReza(float k, float b, float x)
        {
            float NewGlubinaReza = k * x + b;
            return NewGlubinaReza;
        }
    }
}
