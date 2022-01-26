using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace wcommsixwrap
{
    class CertificateInstall
    {
        private string _filename;
        private string _storename;
        private string _storelocation;

        public CertificateInstall(XmlReader reader)
        {
            this.processXml(reader);
        }

        public void processXml(XmlReader reader)
        {
            while (!reader.Name.Equals("Certificate") || reader.IsStartElement())
            {
                reader.Read();
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "FileName":
                            reader.Read();
                            FileName = reader.Value;
                            break;
                        case "StoreName":
                            reader.Read();
                            StoreName = reader.Value;
                            break;
                        case "StoreLocation":
                            reader.Read();
                            StoreLocation = reader.Value;
                            break;
                    }

                }
            }
        }

        public string FileName
        {
            get { return Program.ResolveVariables(_filename); }
            set { _filename = value; }
        }

        public string StoreName
        {
            get { return Program.ResolveVariables(_storename); }
            set { _storename = value; }
        }

        public string StoreLocation
        {
            get { return Program.ResolveVariables(_storelocation); }
            set { _storelocation = value; }
        }


        public void Execute()
        {
            System.Security.Cryptography.X509Certificates.StoreLocation myStoreLocation;
            if (StoreLocation == "Machine")
                myStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine;
            else
                myStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;

            X509Certificate myCertificate = X509Certificate2.CreateFromCertFile(FileName);

            X509Store store = new X509Store(this.StoreName, myStoreLocation);
            store.Open(OpenFlags.ReadWrite);
            if ( !store.Certificates.Contains(myCertificate) )
            {
                store.Add(new X509Certificate2(myCertificate));
                
            }
            store.Close();
        }
    }
}
