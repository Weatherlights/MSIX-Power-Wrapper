using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace wcommsixwrap
{
    class Runtime
    {
        private string filename;
        private string workingdirectory;
        private string arguments;
        private string windowStyle;
        private bool waitForExit = true;
        public bool WaitForExit {
            get { return this.waitForExit; }
        }

        private string argsselector = null;
        private LogWriter myLogWriter;

        public static string Element = "Process";

        public Runtime(XmlReader reader) {
            this.processXml(reader);
            this.myLogWriter = new LogWriter(Element);
        }

        public void processXml (XmlReader reader)
        {
            while (!reader.Name.Equals(Element) || reader.IsStartElement())
            {
                reader.Read();
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "ArgsSelector":
                            reader.Read();
                            argsselector = reader.Value;
                            break;
                        case "Filename":
                            reader.Read();
                            filename = reader.Value;
                            break;
                        case "WorkingDirectory":
                            reader.Read();
                            workingdirectory = reader.Value;
                            break;
                        case "Arguments":
                            reader.Read();
                            arguments = reader.Value;
                            break;
                        case "WaitForExit":
                            reader.Read();
                            if (reader.Value == "false" || reader.Value == "0")
                                waitForExit = false;
                            break;
                        case "WindowStyle":
                            reader.Read();
                            windowStyle = reader.Value;
                            break;
                    }

                }
            }
        }


        public string getFilename()
        {
            return Program.ResolveVariables(filename);
        }


        public string getWorkingDirectory()
        {
            return Program.ResolveVariables(workingdirectory);
        }

        public string getArguments()
        {
            return Program.ResolveVariables(arguments);
        }

        public ProcessWindowStyle getWindowStyle()
        {
            ProcessWindowStyle outStyle = ProcessWindowStyle.Normal;
            switch (this.windowStyle)
            {
                case "Hidden":
                    outStyle = ProcessWindowStyle.Hidden;
                    break;
                case "Maximized":
                    outStyle = ProcessWindowStyle.Maximized;
                    break;
                case "Minimized":
                    outStyle = ProcessWindowStyle.Minimized;
                    break;
            }
            return outStyle;

        }

        private bool TestArgsSelector()
        {
            bool result = false;

            string args = Program.getCommandLineArgument();
            if (args.Contains("[ARGSSELECTOR|"))
                result = args.Contains("[ARGSSELECTOR|" + argsselector + "]");
            else
                if ( argsselector is null )
                    result = true;
               
            return result;
        }

        public void Execute()
        {
            myLogWriter.LogWrite("Trying to match ArgsSelector " + argsselector + " on commandline arguments" + Program.getCommandLineArgument());
            if (this.TestArgsSelector())
                using (Process myProcess = new Process())
                {
                    myLogWriter.LogWrite("TestArgsSelector() matched");
                    myProcess.StartInfo.FileName = this.getFilename();
                    myLogWriter.LogWrite("FileName will be " + myProcess.StartInfo.FileName);
                    myProcess.StartInfo.Arguments = this.getArguments();
                    myLogWriter.LogWrite("Arguments will be " + myProcess.StartInfo.Arguments);
                    myProcess.StartInfo.WindowStyle = this.getWindowStyle();
                    myLogWriter.LogWrite("WindowStyle will be " + myProcess.StartInfo.WindowStyle);
                    myProcess.StartInfo.WorkingDirectory = this.getWorkingDirectory();
                    myLogWriter.LogWrite("WorkingDirectory will be " + myProcess.StartInfo.WorkingDirectory);
                    myProcess.Start();
                    myLogWriter.LogWrite("Process has started");

                    if (this.waitForExit) {
                        myLogWriter.LogWrite("Waiting until process exits");
                        myProcess.WaitForExit();
                        myLogWriter.LogWrite("Process has exited");
                    } else
                    {
                        myLogWriter.LogWrite("Skip waiting for process to exit");
                    }         
                }
            else
                myLogWriter.LogWrite("ArgsSelector did not match any configuration.");
        }
    }
}
