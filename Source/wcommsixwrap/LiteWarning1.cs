using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace wcommsixwrap
{

    class LiteWarning
    {
        private CultureInfo ci;
        private string message;
        private string caption;


        public static string Element = "LiteWarning";

        public LiteWarning()
        {
            ci = CultureInfo.InstalledUICulture;
        }

        public void processXml(XmlReader reader)
        {
            string langcode = ci.TwoLetterISOLanguageName;
            while (!reader.Name.Equals(Element) || reader.IsStartElement())
            {
                reader.Read();
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "Message":
                            string language = reader.GetAttribute("lang");
                            
                            if (language.Equals(langcode))
                            {
                                caption = reader.GetAttribute("caption");
                                reader.Read();
                                message = reader.Value;
                            }
                            break;
                    }

                }
            }
        }

        public string getMessage()
        {
            return Program.ResolveVariables(message);
        }

        public string getCaption()
        {
            return Program.ResolveVariables(caption);
        }

        private bool isSMode()
        {
            bool result = false;
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\CI\\Policy"))
                {
                    if (key != null)
                    {
                        object o = key.GetValue("SKUPolicyRequired");
                        if (o != null)
                        {
                            int mode = int.Parse(o.ToString());
                            if ( mode > 0 ) 
                                result = true;  //"as" because it's REG_SZ...otherwise ToString() might be safe(r)
                                                                         //do what you like with version
                        }
                    }
                }
            }
            catch (Exception ex)  //just for demonstration...it's always best to handle specific exceptions
            {
                //react appropriately
            }
            return result;
        }

        public void Execute()
        {
            string stateFile = Program.ResolveVariables("[WRAPPER_APPDATA]\\MessageBoxShow.touch");

            if (File.Exists(stateFile) == false && this.isSMode() == false)
            {
                
                MessageBox.Show(getMessage(), getCaption(), MessageBoxButton.OK, MessageBoxImage.Information);
                File.Create(stateFile);
            }

        }
    }
}
