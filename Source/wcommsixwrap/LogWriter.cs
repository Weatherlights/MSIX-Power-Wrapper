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
             
        public void LogWrite(string logMessage, int type)
        {
            try
            {
                using (StreamWriter w = File.AppendText(wrapperAppData + "\\" + "Wrapper" +".log"))
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
            string timetormat = "HH:mm:ss.fff-000";
      
            string logLine = "<![LOG[" + logMessage + "]LOG]!><time=\"" + DateTime.Now.ToString(timetormat) + "\" date=\"" + DateTime.Now.ToString(dateformat) + "\" component=\"" + component + "\" context=\"\" type=\""+type+"\" thread=\""+Thread.CurrentThread.ManagedThreadId+ "\" file=\"\">";
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

