using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace wcommsixwrap {



    class VirtualFile
    {
        string[] vscontent;
        bool overwrite = false;
        public VirtualFile()
        {
            vscontent = new string[2];

        }

        public VirtualFile(XmlReader reader)
        {
            vscontent = new string[2];
            this.processXml(reader);

        }

        public void processXml(XmlReader reader)
        {
            while(!reader.Name.Equals("VirtualFile") || reader.IsStartElement())
            {
                reader.Read();
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "File":
                            reader.Read();
                            vscontent[0] = reader.Value;
                            break;
                        case "Target":
                            reader.Read();
                            vscontent[1] = reader.Value;
                            break;
                        case "Overwrite":
                            reader.Read();
                            if ( reader.Value == "TRUE" || reader.Value == "1")
                                overwrite = true;
                            break;
                    }
                }
            }
        }

        public string getFile()
        {
            return Program.ResolveVariables(vscontent[0]);
        }

        public string getTarget()
        {
            return Program.ResolveVariables(vscontent[1]);
        }



        public void Execute()
        {
            Program.CreateDirectoryRecursively(getTarget());
            try
            {
                File.Copy(getFile(), getTarget(), overwrite);
            }catch (System.IO.IOException e)
            {

            }
        }

    }
}
