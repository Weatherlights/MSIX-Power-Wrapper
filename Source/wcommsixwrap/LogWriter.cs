using System;
using System.IO;
using System.Threading;

namespace wcommsixwrap
{
    internal class LogWriter
    {  
        private string appDataPath = string.Empty;
        public string component = "";
        private string wrapperAppData;

        public LogWriter(string component) {
            this.component = component;
            this.wrapperAppData = Program.ResolveVariables("[WRAPPER_APPDATA]");
        }

        public void LogWrite(string logMessage)
        {
            this.LogWrite(logMessage, 1);
        }

        private void RotateLogs()
        {
            string dateformat = "MMddyyyy";
            string timeformat = "HHmmss";

            string logFilePath = wrapperAppData + "\\" + "Wrapper" + ".log";
            string archivedLogFilePath = wrapperAppData + "\\" + "Wrapper-" + DateTime.Now.ToString(dateformat) + "-" + DateTime.Now.ToString(timeformat) + ".log";

            FileInfo LogFile = new FileInfo(logFilePath);

            if (LogFile.Exists && (LogFile.Length) >= 10 * 1048576)
            {
                LogFile.MoveTo(archivedLogFilePath);
            }
        }

        public void LogWrite(string logMessage, int type)
        {
            string logFilePath = wrapperAppData + "\\" + "Wrapper" + ".log";
            RotateLogs();
            try
            {
                using (StreamWriter w = File.AppendText(logFilePath))
                {
                    Log(logMessage, type, w);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void Log(string logMessage, int type, TextWriter txtWriter)
        {
            string dateformat = "MM-dd-yyyy";
            string timeformat = "HH:mm:ss.fff-000";
      
            string logLine = "<![LOG[" + logMessage + "]LOG]!><time=\"" + DateTime.Now.ToString(timeformat) + "\" date=\"" + DateTime.Now.ToString(dateformat) + "\" component=\"" + component + "\" context=\"\" type=\""+type+"\" thread=\""+Thread.CurrentThread.ManagedThreadId+ "\" file=\"\">";
            try
            {
                txtWriter.WriteLine(logLine);
            }
            catch (Exception ex)
            {
            }
        }
    }
}

