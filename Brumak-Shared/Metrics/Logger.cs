using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_Shared.Metrics
{
    public class Logger(string logType, object loggedClass)
    {
        private string LoggedClassName { get; set; } = loggedClass.ToString() ?? "undefined";
        private string LogType { get; set; } = logType;
        private readonly Lock _lockObject = new();

        public void Log(string message)
        {
            lock (this._lockObject)
            {
                using var writter = File.AppendText($"./{LogType}.log");
                string strMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ({LoggedClassName}) {message}";
                Console.WriteLine(strMessage);
                writter.WriteLine(strMessage);
            }
        }
    }
}
