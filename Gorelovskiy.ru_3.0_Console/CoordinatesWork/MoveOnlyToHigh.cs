using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorelovskiy.ru_3._0_Console.CoordinatesWork
{
    class MoveOnlyToHigh: WorkWithRead2DCoordinates
    {
        //**************************************************************************************
        //глобальные переменные

        //**************************************************************************************
        public void ChangeOnlyGlubinaReza(float MOTH_Old3DX, float MOTH_Old3DY, float MOTH_Old3DZ, float MOTH_Old3DA, float MOTH_New2DGlubinaReza, float MOTH_Old2DGlubinaReza)
        {
            float MOTH_New3DA = MOTH_Old3DA;
            float MOTH_New3DX = MOTH_Old3DX;
            float MOTH_New3DY = MOTH_Old3DY + (MOTH_New2DGlubinaReza - MOTH_Old2DGlubinaReza) * Convert.ToSingle(Math.Sin(MOTH_Old3DA));
            float MOTH_New3DZ = MOTH_Old3DZ + (MOTH_New2DGlubinaReza - MOTH_Old2DGlubinaReza) * Convert.ToSingle(Math.Cos(MOTH_Old3DA));
            Write3DCoordinate.WriteOtherMoves(MOTH_New3DX, MOTH_New3DY, MOTH_New3DZ, MOTH_New3DA, DlinaSpindelPlusDlinaFrezi);
            WWR2DC_Old3DA = MOTH_New3DA;
            WWR2DC_Old3DX = MOTH_New3DX;
            WWR2DC_Old3DY = MOTH_New3DY;
            WWR2DC_Old3DZ = MOTH_New3DZ;

        }
    }
}
