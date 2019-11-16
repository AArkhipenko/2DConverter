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
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Начато выполнения программы");
            try
            {
                BasicFunction o_BasicFunction = new BasicFunction();
                Console.WriteLine("Выполнение программы успешно завершено");
            }
            catch(CustomException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.GetMessage());
                Console.WriteLine("Выполнение программы завершено с ошибкой");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.WriteLine("Выполнение программы завершено с ошибкой");
                Console.ReadKey();
            }
        }
    }
}
