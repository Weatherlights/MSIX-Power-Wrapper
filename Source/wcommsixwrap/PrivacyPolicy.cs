using System.Globalization;
using System.IO;
using System.Xml;

namespace wcommsixwrap
{
    internal class PrivacyPolicy
    {
        private CultureInfo ci;
        private string url;
        private string donotshowagain;
        private string caption;
        private bool isProcessed = false;


        public static string Element = "PrivacyPolicy";

        public PrivacyPolicy()
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
                                this.parseXml(reader);
                            } else if (language.Equals("default") && !isProcessed)
                            {
                                this.parseXml(reader);
                            }
                            break;
                    }

                }
            }
        }

        private void parseXml (XmlReader reader)
        {
            caption = reader.GetAttribute("Caption");
            donotshowagain = reader.GetAttribute("DoNotShowAgainText");
            reader.Read();
            url = reader.Value;
            isProcessed = true;
        }

        public string getUrl()
        {
            return Program.ResolveVariables(url);
        }

        public string getCaption()
        {
            return Program.ResolveVariables(caption);
        }

        public string getDoNotShowAgainText()
        {
            return Program.ResolveVariables(donotshowagain);
        }


        public void Execute()
        {
            string stateFile = Program.ResolveVariables("[WRAPPER_APPDATA]\\PrivacyPolicyAccepted.touch");
            PrivacyPolicyForm myPrivacyPolicyForm = new PrivacyPolicyForm();
            myPrivacyPolicyForm.setDoNotShowAgain(this.getDoNotShowAgainText());
            myPrivacyPolicyForm.setPrivacyPolicyUrl(this.getUrl());
            myPrivacyPolicyForm.setCaption(this.getCaption());
            if (File.Exists(stateFile) == false)
            {
                myPrivacyPolicyForm.ShowDialog();
                if (myPrivacyPolicyForm.getDoNotShowAgainState()) {
                    File.Create(stateFile);
                }
            }

        }

    }
}