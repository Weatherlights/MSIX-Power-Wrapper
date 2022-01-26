using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace wcommsixwrap
{
    class VirtualFolder
    {
        private string folder;
        private string target;

        public VirtualFolder(XmlReader reader)
        {
            this.processXml(reader);
        }

        public void processXml(XmlReader reader)
        {
            while (!reader.Name.Equals("VirtualFolder") || reader.IsStartElement())
            {
                reader.Read();
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "Folder":
                            reader.Read();
                            folder = reader.Value;
                            break;
                        case "Target":
                            reader.Read();
                            target = reader.Value;
                            break;
                    }

                }
            }
        }

        public string getFolder()
        {
            return Program.ResolveVariables(folder);
        }

        public string getTarget()
        {
            return Program.ResolveVariables(target);
        }



        public void Execute()
        {
            Program.CreateDirectoryRecursively(getTarget());
            try
            {
                DirectoryCopy(getFolder(), getTarget());
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine("Virtual Folder Copy failed");
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                //file.CopyTo(temppath, false);
                try
                {
                    File.Copy(file.FullName, temppath, false);
                } catch (System.IO.IOException e) {
                    Console.WriteLine("Virtual Folder Copy failed.");
                };
            }
            // Get the files in the directory and copy them to the new location.


            // If copying subdirectories, copy them and their contents to new location.
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }

        }
    }
}
