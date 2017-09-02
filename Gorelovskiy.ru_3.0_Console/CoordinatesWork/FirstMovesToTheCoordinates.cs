using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorelovskiy.ru_3._0_Console.CoordinatesWork
{
    class FirstMovesToTheCoordinates: WorkWithRead2DCoordinates
    {
        //*****************************************************************************************************
        //Глобальные переменные
        //*****************************************************************************************************

        //__________________________________________________________________________________________
        //_____________________________Обработка первого переменщения_______________________________
        //__________________________________________________________________________________________
        public void FirstMoves(float FMTTC_New2DX, float FMTTC_New2DY, float FMTTC_New2DGlubinaReza)
        {
            int FMTTC_NumberOfCoordinateInScanMassive = ADDFunctions.NumberOfCoordinateInScanMassive(FMTTC_New2DY);
            float FMTTC_New3DA = (-1) * ADDFunctions.PereschetAngle(FMTTC_New2DX, FMTTC_NumberOfCoordinateInScanMassive);
            float FMTTC_New3DY = newduga[0, 1, FMTTC_NumberOfCoordinateInScanMassive] + (DlinaSpindelPlusDlinaFrezi + FMTTC_New2DGlubinaReza) * Convert.ToSingle(Math.Sin(FMTTC_New3DA));
            float FMTTC_New3DZ = ADDFunctions.MorePrettyZ(FMTTC_New2DX, FMTTC_NumberOfCoordinateInScanMassive) + (DlinaSpindelPlusDlinaFrezi + FMTTC_New2DGlubinaReza) * Convert.ToSingle(Math.Cos(FMTTC_New3DA));
            Write3DCoordinate.WriteFirstMoves(FMTTC_New2DX, FMTTC_New3DY, FMTTC_New3DZ, FMTTC_New3DA, DlinaSpindelPlusDlinaFrezi);
            WWR2DC_Old3DX = FMTTC_New2DX;
            WWR2DC_Old3DY = FMTTC_New3DY;
            WWR2DC_Old3DZ = FMTTC_New3DZ;
            WWR2DC_Old3DA = FMTTC_New3DA;
        }
    }
}
