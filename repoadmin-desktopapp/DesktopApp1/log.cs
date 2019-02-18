#define GITHUB_IO

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

#if GITHUB_IO
        const string m_logfilePath = @"C:\Users\eha\OneDrive\GitHub\nasjonal-arkitektur\nasjonal-arkitektur.github.io\log\logfile.txt";
#else
        const string m_logfilePath = @"C:\Users\eha\OneDrive\GitHub\Difi\nasjonal_arkitektur\log\logfile.txt";
#endif

        /*
        public Log(string logPath)
        {
            m_logfilePath = logPath + @"\" + "logfile.txt";
        }
*/
        static public void doLog(string txtToLog)
        {
            string logline = System.Environment.NewLine + DateTime.Now.ToString() + ": " + txtToLog;
            File.AppendAllText(m_logfilePath, logline);
            Console.WriteLine(logline);
        }
    }
}
