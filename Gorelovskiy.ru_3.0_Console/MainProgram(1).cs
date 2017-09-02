using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorelovskiy.ru_3._0_Console
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            BasicFunction o_BasicFunction = new BasicFunction();
            o_BasicFunction.ReadScanAndDividing();
            o_BasicFunction.Read2DPicture();
            o_BasicFunction.Dividing2DPictureAndRemaking();
        }
    }
}
