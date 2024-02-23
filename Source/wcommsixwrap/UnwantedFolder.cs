using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace wcommsixwrap
{
    class UnwantedFolder
    {
        string FolderPath;

        public UnwantedFolder()
        {

            FolderPath = "";
        }

        public UnwantedFolder(XmlReader reader)
        {
            FolderPath = "";
            this.processXml(reader);

        }

        public void processXml(XmlReader reader)
        {
            while (!reader.Name.Equals("UnwantedFolder") || reader.IsStartElement())
            {
                reader.Read();
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "Folder":
                            reader.Read();
                            FolderPath = reader.Value;
                            break;
                    }

                }
            }
        }

        public string getFolderPath()
        {
            return Program.ResolveVariables(FolderPath);
        }


        public static void RecursiveDelete(DirectoryInfo baseDir)
        {
            if (!baseDir.Exists)
                return;

            foreach (var dir in baseDir.EnumerateDirectories())
            {
                RecursiveDelete(dir);
            }
            var files = baseDir.GetFiles();
            foreach (var file in files)
            {
                file.IsReadOnly = false;
                file.Delete();
            }
            baseDir.Delete();
        }


        public void Execute()
        {

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(getFolderPath());
                RecursiveDelete(dirInfo);
            }
            catch (System.IO.IOException e)
            {

            }
        }

    }
}
