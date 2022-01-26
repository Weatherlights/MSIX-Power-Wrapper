using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Xml;
using System.Diagnostics;
using System.Timers;
using System.Runtime.Serialization;

namespace wcommsixwrap
{

    [Serializable]
    public class ProcessToServeClosedException : Exception
    {
        // Constructors
        public ProcessToServeClosedException(string message)
            : base(message)
        { }

        // Ensure Exception is Serializable
        protected ProcessToServeClosedException(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        { }
    }

    class ServiceHandler
    {
        private string _filename;
        private string _workingdirectory;
        private string _arguments;
        private string _argsselector = null;

        public static string Element = "Service";

        public class Service : System.ServiceProcess.ServiceBase
        {
            private Process ProcessToServe = null;

            public Service(string myFileName, string myWorkingDirectory, string myArguments)
            {

                ProcessToServe = new Process();

                ProcessToServe.StartInfo.FileName = myFileName;
                ProcessToServe.StartInfo.Arguments = myArguments;
                ProcessToServe.StartInfo.WorkingDirectory = myWorkingDirectory;
// ProcessToServe.

                ProcessToServe.Exited += new EventHandler(ProcessToServe_Exited);

            }

            private void ProcessToServe_Exited(object sender, System.EventArgs e)
            {
                //throw new ProcessToServeClosedException("The process "+ ProcessToServe.StartInfo.FileName + " " + ProcessToServe.StartInfo.Arguments + " did ")
                //ProcessToServe.Start();
            }

            protected override void OnStart(string[] args)
            {
                 ProcessToServe.Start();
            }

            protected override void OnStop()
            {
                ProcessToServe.Close();
            }

        }

        public ServiceHandler(XmlReader reader)
        {
            this.processXml(reader);
        }


        public void processXml(XmlReader reader)
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
                            _argsselector = reader.Value;
                            break;
                        case "Filename":
                            reader.Read();
                            _filename = reader.Value;
                            break;
                        case "WorkingDirectory":
                            reader.Read();
                            _workingdirectory = reader.Value;
                            break;
                        case "Arguments":
                            reader.Read();
                            _arguments = reader.Value;
                            break;
                    }

                }
            }
        }


        public string Filename
        {
            get { return Program.ResolveVariables(_filename); }
        }


        public string WorkingDirectory
        {
            get { return Program.ResolveVariables(_workingdirectory); }
        }

        public string Arguments
        {
            get { return Program.ResolveVariables(_arguments); }
        }

        private bool TestArgsSelector()
        {
            bool result = false;

            string args = Program.getCommandLineArgument();
            if (args.Contains("[ARGSSELECTOR|"))
                result = args.Contains("[ARGSSELECTOR|" + _argsselector + "]");
            else
                if (_argsselector is null)
                result = true;

            return result;
        }

        public void Execute()
        {
            if (this.TestArgsSelector())
            {
                ServiceBase[] ServicesToRun = new ServiceBase[] { new Service(Filename, WorkingDirectory, Arguments) };

                ServiceBase.Run(ServicesToRun);
            }

        }
    }
}
