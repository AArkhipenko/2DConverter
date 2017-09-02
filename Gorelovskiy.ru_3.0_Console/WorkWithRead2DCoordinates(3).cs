using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorelovskiy.ru_3._0_Console
{
    class WorkWithRead2DCoordinates : BasicFunction
    {
        //Глобальные переменные
        //********************************************************

        public static float WWR2DC_Old3DX { get; set; }
        public static float WWR2DC_Old3DY { get; set; }
        public static float WWR2DC_Old3DZ { get; set; }
        public static float WWR2DC_Old3DA { get; set; }
        static float WWR2DC_Old2DGlubinaReza { get; set; }
        static float WWR2DC_Old2DX { get; set; }
        static float WWR2DC_Old2DY { get; set; }

        //Глобальные объекты классов
        //1)используются другими классами
        public static AddictionalFunctionsClass.AddictionalForWorkWithCoordinates ADDFunctions = new AddictionalFunctionsClass.AddictionalForWorkWithCoordinates();
        //2) используются только в этом классе
        CoordinatesWork.MoveOnlyToX XMoves;
        CoordinatesWork.MoveToXAndHigh XAndHighMoves;
        CoordinatesWork.MoveOnlyToY YMoves;
        CoordinatesWork.MoveOnlyToHigh HighMoves;
        CoordinatesWork.MoveToYAndHigh YAndHighMoves;
        CoordinatesWork.FirstMovesToTheCoordinates FirstMoves;
        CoordinatesWork.MoveToXAndY XAndYMoves;
        CoordinatesWork.MoveToXAndYAndHigh XAndYAndHighMoves;
        //********************************************************


        //__________________________________________________________________________________________________________________
        //_________________________________________Рассматриваем какое перемещение нам попалось_____________________________
        //__________________________________________________________________________________________________________________
        public void WorkAllSituationsOfCoordinates(int WWR2DC_TypeOfSituation, float WWR2DC_New2DX, float WWR2DC_New2DY, float WWR2DC_New2DGlubinaReza)
        {
            //ADDFunctions ;
            
            if (WWR2DC_TypeOfSituation == 666)//впервые встретили в строке передвижение по координатам
            {
                FirstMoves = new CoordinatesWork.FirstMovesToTheCoordinates();
                FirstMoves.FirstMoves(WWR2DC_New2DX, WWR2DC_New2DY, WWR2DC_New2DGlubinaReza);
                WWR2DC_Old2DX = WWR2DC_New2DX;
                WWR2DC_Old2DY = WWR2DC_New2DY;
                WWR2DC_Old2DGlubinaReza = WWR2DC_New2DGlubinaReza;
            }
            if (WWR2DC_TypeOfSituation == 15)//в строке нет ни одной кординаты, нет перемещения, тогда ничего не делаем
            {
            }

            else if (WWR2DC_TypeOfSituation == 39)//в строке есть координата зет (глубина реза) только
            {
                //проверяем изменилось ли значение глубины реза
                if (WWR2DC_Old2DGlubinaReza != WWR2DC_New2DGlubinaReza)
                {
                    HighMoves = new CoordinatesWork.MoveOnlyToHigh();
                    HighMoves.ChangeOnlyGlubinaReza(WWR2DC_Old3DX, WWR2DC_Old3DY, WWR2DC_Old3DZ, WWR2DC_Old3DA, WWR2DC_New2DGlubinaReza, WWR2DC_Old2DGlubinaReza);
                    WWR2DC_Old2DGlubinaReza = WWR2DC_New2DGlubinaReza;
                }
            }

            else if (WWR2DC_TypeOfSituation == 55)// в строке есть координата игрек только
            {
                if (WWR2DC_Old2DY != WWR2DC_New2DY)
                {
                    YMoves = new CoordinatesWork.MoveOnlyToY();
                    YMoves.ChooseOurWay(WWR2DC_Old2DX, WWR2DC_New2DY, WWR2DC_Old2DY, WWR2DC_Old2DGlubinaReza);
                    WWR2DC_Old2DY = WWR2DC_New2DY;
                }
            }

            else if (WWR2DC_TypeOfSituation == 105)//в строке есть только координата икс
            {
                if (WWR2DC_Old2DX != WWR2DC_New2DX)
                {
                    XMoves = new CoordinatesWork.MoveOnlyToX();
                    XMoves.ChangeOnlyXCoordinate(WWR2DC_New2DX, WWR2DC_Old2DX, WWR2DC_Old2DY, WWR2DC_Old2DGlubinaReza);
                    WWR2DC_Old2DX = WWR2DC_New2DX;
                }
            }

            else if (WWR2DC_TypeOfSituation == 143)//в строке есть координаты игрек и зет (глубина реза)
            {
                //оба значения поменялись
                if (WWR2DC_Old2DY != WWR2DC_New2DY && WWR2DC_Old2DGlubinaReza != WWR2DC_New2DGlubinaReza)
                {
                    YAndHighMoves = new CoordinatesWork.MoveToYAndHigh();
                    YAndHighMoves.ChooseOurWay(WWR2DC_Old2DX, WWR2DC_New2DY, WWR2DC_Old2DY, WWR2DC_New2DGlubinaReza, WWR2DC_Old2DGlubinaReza);
                    WWR2DC_Old2DY = WWR2DC_New2DY;
                    WWR2DC_Old2DGlubinaReza = WWR2DC_New2DGlubinaReza;
                }
                    //поменялось только значение игрек
                else if(WWR2DC_Old2DY != WWR2DC_New2DY)
                {
                    YMoves = new CoordinatesWork.MoveOnlyToY();
                    YMoves.ChooseOurWay(WWR2DC_Old2DX, WWR2DC_New2DY, WWR2DC_Old2DY, WWR2DC_Old2DGlubinaReza);
                    WWR2DC_Old2DY = WWR2DC_New2DY;
                }
                    //поменялось только значение глубины реза
                else if (WWR2DC_Old2DGlubinaReza!=WWR2DC_New2DGlubinaReza)
                {
                    HighMoves = new CoordinatesWork.MoveOnlyToHigh();
                    HighMoves.ChangeOnlyGlubinaReza(WWR2DC_Old3DX, WWR2DC_Old3DY, WWR2DC_Old3DZ, WWR2DC_Old3DA, WWR2DC_New2DGlubinaReza, WWR2DC_Old2DGlubinaReza);
                    WWR2DC_Old2DGlubinaReza = WWR2DC_New2DGlubinaReza;
                }
            }

            else if (WWR2DC_TypeOfSituation == 273)//в строке есть координаты икс и зет (глубина реза)
            {
                if (WWR2DC_Old2DX != WWR2DC_New2DX && WWR2DC_Old2DGlubinaReza != WWR2DC_New2DGlubinaReza)
                {
                    XAndHighMoves = new CoordinatesWork.MoveToXAndHigh();
                    XAndHighMoves.ChangeXCoordinateAndHigh(WWR2DC_New2DX, WWR2DC_Old2DX, WWR2DC_Old2DY, WWR2DC_New2DGlubinaReza, WWR2DC_Old2DGlubinaReza);
                    WWR2DC_Old2DGlubinaReza = WWR2DC_New2DGlubinaReza;
                    WWR2DC_Old2DX = WWR2DC_New2DX;
                }
                    //поменялась только координата икс
                else if (WWR2DC_Old2DX != WWR2DC_New2DX)
                {
                    XMoves = new CoordinatesWork.MoveOnlyToX();
                    XMoves.ChangeOnlyXCoordinate(WWR2DC_New2DX, WWR2DC_Old2DX, WWR2DC_Old2DY, WWR2DC_Old2DGlubinaReza);
                    WWR2DC_Old2DX = WWR2DC_New2DX;
                }
                //поменялось только значение глубины реза
                else if (WWR2DC_Old2DGlubinaReza != WWR2DC_New2DGlubinaReza)
                {
                    HighMoves = new CoordinatesWork.MoveOnlyToHigh();
                    HighMoves.ChangeOnlyGlubinaReza(WWR2DC_Old3DX, WWR2DC_Old3DY, WWR2DC_Old3DZ, WWR2DC_Old3DA, WWR2DC_New2DGlubinaReza, WWR2DC_Old2DGlubinaReza);
                    WWR2DC_Old2DGlubinaReza = WWR2DC_New2DGlubinaReza;
                }
            }

            else if (WWR2DC_TypeOfSituation == 385)// в строке есть координаты икс и игрек
            {
                if (WWR2DC_Old2DX != WWR2DC_New2DX && WWR2DC_Old2DY != WWR2DC_New2DY)
                {
                    XAndYMoves = new CoordinatesWork.MoveToXAndY();
                    XAndYMoves.ChooseOurActions_ChangeXAndYCoordinates(WWR2DC_New2DX, WWR2DC_Old2DX, WWR2DC_New2DY, WWR2DC_Old2DY, WWR2DC_Old2DGlubinaReza);
                    WWR2DC_Old2DX = WWR2DC_New2DX;
                    WWR2DC_Old2DY = WWR2DC_New2DY;
                }
                else if (WWR2DC_Old2DX != WWR2DC_New2DX)
                {
                    XMoves = new CoordinatesWork.MoveOnlyToX();
                    XMoves.ChangeOnlyXCoordinate(WWR2DC_New2DX, WWR2DC_Old2DX, WWR2DC_Old2DY, WWR2DC_Old2DGlubinaReza);
                    WWR2DC_Old2DX = WWR2DC_New2DX;
                }
                else if (WWR2DC_Old2DY != WWR2DC_New2DY)
                {
                    YMoves = new CoordinatesWork.MoveOnlyToY();
                    YMoves.ChooseOurWay(WWR2DC_Old2DX, WWR2DC_New2DY, WWR2DC_Old2DY, WWR2DC_Old2DGlubinaReza);
                    WWR2DC_Old2DY = WWR2DC_New2DY;
                }
            }

            else if (WWR2DC_TypeOfSituation == 1001)// в строке есть все координаты
            {
                //все новые
                if (WWR2DC_Old2DY != WWR2DC_New2DY && WWR2DC_Old2DX != WWR2DC_New2DX && WWR2DC_Old2DGlubinaReza != WWR2DC_New2DGlubinaReza)
                {
                    XAndYAndHighMoves = new CoordinatesWork.MoveToXAndYAndHigh();
                    XAndYAndHighMoves.ChooseOurActions_ChangeXAndYCoordinatesAndHigh(WWR2DC_New2DX, WWR2DC_Old2DX, WWR2DC_New2DY, WWR2DC_Old2DY, WWR2DC_New2DGlubinaReza, WWR2DC_Old2DGlubinaReza);
                    WWR2DC_Old2DX = WWR2DC_New2DX;
                    WWR2DC_Old2DY = WWR2DC_New2DY;
                    WWR2DC_Old2DGlubinaReza = WWR2DC_New2DGlubinaReza;
                }
                    //новые икс и игрек
                else if (WWR2DC_Old2DY != WWR2DC_New2DY && WWR2DC_Old2DX != WWR2DC_New2DX)
                {
                    XAndYMoves = new CoordinatesWork.MoveToXAndY();
                    XAndYMoves.ChooseOurActions_ChangeXAndYCoordinates(WWR2DC_New2DX, WWR2DC_Old2DX, WWR2DC_New2DY, WWR2DC_Old2DY, WWR2DC_Old2DGlubinaReza);
                    WWR2DC_Old2DX = WWR2DC_New2DX;
                    WWR2DC_Old2DY = WWR2DC_New2DY;
                }
                    //поменялись икс и зет
                else if (WWR2DC_Old2DX != WWR2DC_New2DX && WWR2DC_Old2DGlubinaReza != WWR2DC_New2DGlubinaReza)
                {
                    XAndHighMoves = new CoordinatesWork.MoveToXAndHigh();
                    XAndHighMoves.ChangeXCoordinateAndHigh(WWR2DC_New2DX, WWR2DC_Old2DX, WWR2DC_Old2DY, WWR2DC_New2DGlubinaReza, WWR2DC_Old2DGlubinaReza);
                    WWR2DC_Old2DGlubinaReza = WWR2DC_New2DGlubinaReza;
                    WWR2DC_Old2DX = WWR2DC_New2DX;
                }
                    //поменялись игрек и зет
                else if (WWR2DC_Old2DY != WWR2DC_New2DY && WWR2DC_Old2DGlubinaReza != WWR2DC_New2DGlubinaReza) 
                {
                    YAndHighMoves = new CoordinatesWork.MoveToYAndHigh();
                    YAndHighMoves.ChooseOurWay(WWR2DC_Old2DX, WWR2DC_New2DY, WWR2DC_Old2DY, WWR2DC_New2DGlubinaReza, WWR2DC_Old2DGlubinaReza);
                    WWR2DC_Old2DY = WWR2DC_New2DY;
                    WWR2DC_Old2DGlubinaReza = WWR2DC_New2DGlubinaReza;
                }
                    //поменялся только икс
                else if (WWR2DC_Old2DX != WWR2DC_New2DX)
                {
                    XMoves = new CoordinatesWork.MoveOnlyToX();
                    XMoves.ChangeOnlyXCoordinate(WWR2DC_New2DX, WWR2DC_Old2DX, WWR2DC_Old2DY, WWR2DC_Old2DGlubinaReza);
                    WWR2DC_Old2DX = WWR2DC_New2DX;
                }
                    //поменялся только игрек
                else if (WWR2DC_Old2DY != WWR2DC_New2DY)
                {
                    YMoves = new CoordinatesWork.MoveOnlyToY();
                    YMoves.ChooseOurWay(WWR2DC_Old2DX, WWR2DC_New2DY, WWR2DC_Old2DY, WWR2DC_Old2DGlubinaReza);
                    WWR2DC_Old2DY = WWR2DC_New2DY;
                }
                //поменялось только значение глубины реза
                else if (WWR2DC_Old2DGlubinaReza != WWR2DC_New2DGlubinaReza)
                {
                    HighMoves = new CoordinatesWork.MoveOnlyToHigh();
                    HighMoves.ChangeOnlyGlubinaReza(WWR2DC_Old3DX, WWR2DC_Old3DY, WWR2DC_Old3DZ, WWR2DC_Old3DA, WWR2DC_New2DGlubinaReza, WWR2DC_Old2DGlubinaReza);
                    WWR2DC_Old2DGlubinaReza = WWR2DC_New2DGlubinaReza;
                }
            }
        }

    }
}
