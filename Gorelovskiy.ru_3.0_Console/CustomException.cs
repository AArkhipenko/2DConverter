using System;
using System.Linq;
using System.Text;
using System.IO;

namespace Gorelovskiy.ru_3._0_Console
{
    class CustomException : Exception
    {
        private int? _row_counter;
        public CustomException():base()
        { }
        public CustomException(string message) : base(message)
        { }
        public CustomException(string message, int row_counter) : base(message)
        {
            this._row_counter = row_counter;
        }
        public CustomException(string message, Exception inner_exception) : base(message, inner_exception)
        { }
        public CustomException(string message, Exception inner_exception, int row_counter) : base(message, inner_exception)
        {
            this._row_counter = row_counter;
        }
        public string GetMessage()
        {
            string message = "";
            if (_row_counter != null)
                message += "Исключение возникло в при чтении файл в строке " + _row_counter + "\r\n";
            if (base.Message != null)
                message += "Сообщение основного исключения: " + base.Message+"\r\n";
            if (base.InnerException != null)
                message += "Сообщение внутреннего исключения: " + this.GetInnerMessage(base.InnerException, 0);
            return message;
        }

        private string GetInnerMessage(Exception ex, int level)
        {
            string message = "Исключение уровня " + level + ": " + ex.Message + "\r\n";
            if(ex.InnerException != null)
            message += this.GetInnerMessage(ex.InnerException, level + 1);
            return message;
        }
    }
}
