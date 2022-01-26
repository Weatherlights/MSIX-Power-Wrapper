using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace wcommsixwrap
{
    internal class SymbolicLink
    {
        private string link;
        private string target;
        private string type;

        public static string Element = "SymbolicLink";

        [DllImport("kernel32.dll")]
        static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);

        public SymbolicLink(XmlReader reader)
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
                        case "Link":
                            reader.Read();
                            link = reader.Value;
                            break;
                        case "Target":
                            reader.Read();
                            target = reader.Value;
                            break;
                        case "Type":
                            reader.Read();
                            type = reader.Value;
                            break;
                    }

                }
            }
        }

        public string getLink()
        {
            return Program.ResolveVariables(link);
        }

        public string getTarget()
        {
            return Program.ResolveVariables(target);
        }

        public int getType()
        {
            if (type == "File")
                return 0;
            else
                return 1;
        }



        public void Execute()
        {
         //   Program.CreateDirectoryRecursively(getLink());
            try
            {
                CreateSymbolicLink(this.getLink(), this.getTarget(), this.getType());
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine("Symbolic Link creation failed");
            }
        }



    }
}