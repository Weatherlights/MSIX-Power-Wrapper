using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace wcommsixwrap
{
    class UnwantedFile
    {
        string FilePath;

        public UnwantedFile()
        {

            FilePath = "";
        }

        public UnwantedFile(XmlReader reader)
        {
            FilePath = "";
            this.processXml(reader);

        }

        public void processXml(XmlReader reader)
        {
            while (!reader.Name.Equals("UnwantedFile") || reader.IsStartElement())
            {
                reader.Read();
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "File":
                            reader.Read();
                            FilePath = reader.Value;
                            break;
                    }

                }
            }
        }

        public string getFilePath()
        {
            return Program.ResolveVariables(FilePath);
        }


        public void Execute()
        {

            try
            {
                if (File.Exists(getFilePath()))
                {
                    File.Delete(getFilePath());
                }
            }
            catch (System.IO.IOException e)
            {

            }
        }

    }
}
