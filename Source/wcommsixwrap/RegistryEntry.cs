using Microsoft.Win32;
using System.Xml;

namespace wcommsixwrap
{
    internal class RegistryEntry
    {
        private string key;
        private string node;
        private string attribute;
        private string value;
        private string type;

        public RegistryEntry(XmlReader reader)
        {
            this.processXml(reader);
        }

        public void processXml(XmlReader reader)
        {
            while (!reader.Name.Equals("RegistryEntry") || reader.IsStartElement())
            {
                reader.Read();
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "Key":
                            reader.Read();
                            key = reader.Value;
                            break;
                        case "Attribute":
                            reader.Read();
                            attribute = reader.Value;
                            break;
                        case "Value":
                            reader.Read();
                            value = reader.Value;
                            break;
                        case "Node":
                            reader.Read();
                            node = reader.Value;
                            break;
                        case "Type":
                            reader.Read();
                            type = reader.Value;
                            break;
                    }

                }
            }
        }

        public string getNode()
        {
            return node;
        }

        public string getKey()
        {
            return Program.ResolveVariables(key);
        }

        public string getAttribute()
        {
            return Program.ResolveVariables(attribute);
        }

        public string getValue()
        {
            return Program.ResolveVariables(value);
        }

        public RegistryValueKind getType()
        {
            RegistryValueKind returnval = RegistryValueKind.String;
            switch (type)
            {
                case "DWord":
                    returnval = RegistryValueKind.DWord;
                    break;
                case "String":
                    returnval = RegistryValueKind.String;
                    break;
                case "Binary":
                    returnval = RegistryValueKind.Binary;
                    break;
            }

            return returnval;
        }



        public void Execute()
        {

            RegistryKey mkey = null;

            if (node == "USER")
                mkey = Registry.CurrentUser.OpenSubKey(key, true);
            else if (node == "MACHINE")
                mkey = Registry.LocalMachine.OpenSubKey(key, true);

            if (mkey == null)
            {
                if (node == "USER")
                    mkey = Registry.CurrentUser.CreateSubKey(key);
                else if (node == "MACHINE")
                    mkey = Registry.LocalMachine.CreateSubKey(key);
            }
            if ( mkey.GetValue(this.getAttribute()) == null )
                mkey.SetValue(this.getAttribute(), this.getValue(), this.getType());

            mkey.Close();
        }
    }
}