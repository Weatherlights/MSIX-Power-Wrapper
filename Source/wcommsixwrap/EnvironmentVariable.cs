using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace wcommsixwrap
{
    internal class EnvironmentVariable
    {
        private string _variableName;
        private string _variableValue;
        private string _variableTarget;
        private LogWriter myLogWriter;

        public static string Element = "EnvironmentVariable";
        public string variableName
        {
            get
            {
                return Program.ResolveVariables(_variableName);
            }
            set => _variableName = value;
        }

        public string variableValue
        {
            get
            {
                return Program.ResolveVariables(_variableValue);
            }
            set => _variableValue = value;
        }

        public string variableTarget
        {
            get
            {
                if (_variableTarget != null)
                {
                    return Program.ResolveVariables(_variableTarget);
                } else
                {
                    return "Process";
                }
            
            }
            set => _variableTarget = value;
        }

        public EnvironmentVariable()
        {
            this.myLogWriter = new LogWriter(Element);
        }

        public EnvironmentVariable(XmlReader reader)
        {
            this.myLogWriter = new LogWriter(Element);
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
                        case "Name":
                            reader.Read();
                            variableName = reader.Value;
                            break;
                        case "Value":
                            reader.Read();
                            variableValue = reader.Value;
                            break;
                        case "Target":
                            reader.Read();
                            variableValue = reader.Value;
                            break;
                    }

                }
            }
        }



        public void Execute()
        {
            System.EnvironmentVariableTarget target = EnvironmentVariableTarget.Process;
            if ( variableTarget == "Machine" )
            {
                target = EnvironmentVariableTarget.Machine;
            } else if ( variableTarget == "User" )
            {
                target = EnvironmentVariableTarget.User;
            }
            System.Environment.SetEnvironmentVariable(variableName, variableValue, target);


        }
    }
}
