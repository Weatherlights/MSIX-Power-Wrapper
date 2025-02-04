using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace wcommsixwrap
{
    class RougeConfig
    {
        string FilePath;
        string AppDataFilePath;
        private bool RougeFileExisted;

        public RougeConfig()
        {
            FilePath = "";
        }

        public RougeConfig(XmlReader reader)
        {
            FilePath = "";
            this.processXml(reader);
        }

        public void processXml(XmlReader reader)
        {
            while (!reader.Name.Equals("RougeConfig") || reader.IsStartElement())
            {
                reader.Read();
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "File":
                            reader.Read();
                            FilePath = reader.Value;
                            string fileName = Path.GetFileName(this.getFilePath());
                            AppDataFilePath = Program.ResolveVariables("[WRAPPER_APPDATA]") + fileName;
                            break;
                    }
                }
            }
        }

        public string getFilePath()
        {
            return Program.ResolveVariables(FilePath);
        }

        public string getAppDataFilePath()
        {
            return Program.ResolveVariables(AppDataFilePath);
        }

        public void CleanUp()
        {
            try {
                if (File.Exists(getFilePath()) && RougeFileExisted == false)
                {
                    File.Copy(this.getFilePath(), getAppDataFilePath(), true);
                    File.Delete(this.getFilePath());
                }
            }
            catch (System.IO.IOException e)
            {
                // Do nothing
            }
        }

        public void Execute()
        {
            RougeFileExisted = false;
            try
            {
                if (File.Exists(getAppDataFilePath()) == true && File.Exists(getFilePath()) == false)
                {
                    File.Copy(getAppDataFilePath(),this.getFilePath(), true);
                } else if (File.Exists(getFilePath()) == true)
                {
                    RougeFileExisted = true;
                } else
                {
                    RougeFileExisted = false;
                }
            }
            catch (System.IO.IOException e)
            {
                // Do nothing
            }
        }

    }
}
