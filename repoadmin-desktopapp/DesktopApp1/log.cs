using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace DesktopApp1
{
    class Log
    {

        string logfilePath = null; //@"C:\Users\eha\OneDrive\GitHub\Difi\nasjonal_arkitektur\log\logfile.txt";
        DateTime timestamp = DateTime.Now;

        public Log(string logPath)
        {
            logfilePath = logPath + @"\" + "logfile.txt";
        }

        public void doLog(string txtToLog)
        {
            string logline = System.Environment.NewLine + timestamp.ToString() + ": " + txtToLog;
            File.AppendAllText(logfilePath, logline);
            //Console.WriteLine(logline);
        }
    }
}
