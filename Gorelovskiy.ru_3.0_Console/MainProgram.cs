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
            Services.Log("Начато выполнения программы");
            try
            {
                BasicFunction basicFunction = new BasicFunction();
                basicFunction.Start();
                Services.Log("Выполнение программы успешно завершено", Services.LogType.SUCCESS);
            }
            catch(CustomException ex)
            {
                Services.Log("Выполнение программы завершено с ошибкой", Services.LogType.ERROR);
                Services.Log(ex.GetMessage(), Services.LogType.ERROR);
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Services.Log("Выполнение программы завершено с ошибкой", Services.LogType.ERROR);
                Services.Log(ex.Message, Services.LogType.ERROR);
                Console.ReadKey();
            }
        }
    }
}
