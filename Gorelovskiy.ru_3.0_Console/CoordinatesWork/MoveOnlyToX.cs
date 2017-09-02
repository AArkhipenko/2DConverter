using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorelovskiy.ru_3._0_Console.CoordinatesWork
{
    class MoveOnlyToX:WorkWithRead2DCoordinates
    {
        //_____________________________________________________________________________________________________
        //______определяем зоны в которых лежат координаты икс и определяем дальнейшие действия________________
        //_____________________________________________________________________________________________________
        public void ChangeOnlyXCoordinate(float COXC_New2DX, float COXC_Old2DX, float COXC_Old2DY, float COXC_Old2DGlubinaReza)
        {
            int COXC_OldNumberOfSection = ADDFunctions.ZonaNewCoordinate(COXC_Old2DX);
            int COXC_NewNumberOfSection = ADDFunctions.ZonaNewCoordinate(COXC_New2DX);
            if (COXC_OldNumberOfSection != COXC_NewNumberOfSection)
            {
                this.NewXAndOldXLiesInDifferentZones(COXC_New2DX, COXC_Old2DX, COXC_Old2DY, COXC_Old2DGlubinaReza);
            }
            else
            {
                this.NewXAndOldXLiesInOnceZones(COXC_New2DX, COXC_Old2DX, COXC_Old2DY, COXC_Old2DGlubinaReza);
            }
        }

        //_____________________________________________________________________________________________________
        //_________если начальная и конечная точки лежат в разных областях тогда обращается сюда_______________
        //_____________________________________________________________________________________________________
        public void NewXAndOldXLiesInDifferentZones(float COXC_New2DX, float COXC_Old2DX, float COXC_Old2DY, float COXC_Old2DGlubinaReza)
        {
            int COXC_OldNumberOfSection = ADDFunctions.ZonaNewCoordinate(COXC_Old2DX);
            int COXC_NewNumberOfSection = ADDFunctions.ZonaNewCoordinate(COXC_New2DX);
            int COXC_NumberOfCoordinate = ADDFunctions.NumberOfCoordinateInScanMassive(COXC_Old2DY);

            if (COXC_OldNumberOfSection > COXC_NewNumberOfSection)
            {
                while (COXC_OldNumberOfSection > COXC_NewNumberOfSection)
                {
                    COXC_OldNumberOfSection--;

                    ADDFunctions.CalculationNew3DCoordinatesIf2DYDoesntChangeWithZoneX(COXC_Old2DGlubinaReza, COXC_OldNumberOfSection, COXC_NumberOfCoordinate);
                }
                ADDFunctions.CalculationNew3DCoordinatesIf2DYDoesntChangeWithNotZoneX(COXC_New2DX, COXC_Old2DGlubinaReza, COXC_NumberOfCoordinate);
            }
            else
            {
                while (COXC_OldNumberOfSection < COXC_NewNumberOfSection)
                {
                    ADDFunctions.CalculationNew3DCoordinatesIf2DYDoesntChangeWithZoneX(COXC_Old2DGlubinaReza, COXC_OldNumberOfSection, COXC_NumberOfCoordinate);

                    COXC_OldNumberOfSection++;
                }
                ADDFunctions.CalculationNew3DCoordinatesIf2DYDoesntChangeWithNotZoneX(COXC_New2DX, COXC_Old2DGlubinaReza, COXC_NumberOfCoordinate);
            }
        }

        //_____________________________________________________________________________________________________
        //________________________если начальный икс и конечный лежат в одной зоне_____________________________
        //_____________________________________________________________________________________________________
        public void NewXAndOldXLiesInOnceZones(float COXC_New2DX, float COXC_Old2DX, float COXC_Old2DY, float COXC_Old2DGlubinaReza)
        {
            int COXC_NumberOfCoordinate = ADDFunctions.NumberOfCoordinateInScanMassive(COXC_Old2DY);

            ADDFunctions.CalculationNew3DCoordinatesIf2DYDoesntChangeWithNotZoneX(COXC_New2DX, COXC_Old2DGlubinaReza, COXC_NumberOfCoordinate);
        }
    }
}
