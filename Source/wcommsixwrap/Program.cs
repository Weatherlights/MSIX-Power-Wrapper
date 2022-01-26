using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Windows.ApplicationModel;

namespace wcommsixwrap
{
    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LogWriter myLogWriter = new LogWriter("Main");
            myLogWriter.LogWrite("WCOMMSIXWRAP started.");

            string location = System.Reflection.Assembly.GetEntryAssembly().Location;
            string configlocation = location + ".wrunconfig";
            string wrapperAppData = ResolveVariables("[WRAPPER_APPDATA]");

            myLogWriter.LogWrite("Configuration location is " + configlocation);

            List<VirtualFile> myFiles = new List<VirtualFile>();
            List<VirtualFolder> myVirtualFolders = new List<VirtualFolder>();
            List<RougeConfig> myRougeConfigs = new List<RougeConfig>();
            List<RegistryEntry> myRegistryEntries = new List<RegistryEntry>();
            List<SymbolicLink> mySymbolicLinks = new List<SymbolicLink>();
            List<Runtime> myRuntimes = new List<Runtime>();
            List<ServiceHandler> myServiceHandlers = new List<ServiceHandler>();
            List<CertificateInstall> myCertificateInstallers = new List<CertificateInstall>();
            LiteWarning myLiteWarning = null;
            UpdateHandler myUpdateHandler = null;
            PrivacyPolicy myPrivacyPolicy = null;

            myLogWriter.LogWrite("Initialized objects");

            using (XmlReader reader = XmlReader.Create(configlocation)) {
                while (reader.Read())
                if (reader.IsStartElement())
                {
                    switch ( reader.Name )
                        {

                            case "Process":
                                myRuntimes.Add(new Runtime(reader));
                                break;
                            case "Service":
                                myServiceHandlers.Add(new ServiceHandler(reader));
                                break;
                            case "RougeConfig":
                                myRougeConfigs.Add(new RougeConfig(reader));
                                break;
                            case "VirtualFile":
                                myFiles.Add(new VirtualFile(reader));
                                break;
                            case "VirtualFolder":
                                myVirtualFolders.Add(new VirtualFolder(reader));
                                break;
                            case "RegistryEntry":
                                myRegistryEntries.Add(new RegistryEntry(reader));
                                break;
                            case "UpdateHandler":
                                myUpdateHandler = new UpdateHandler();
                                myUpdateHandler.processXml(reader);
                                break;
                            case "LiteWarning":
                                myLiteWarning = new LiteWarning();
                                myLiteWarning.processXml(reader);
                                break;
                            case "PrivacyPolicy":
                                myPrivacyPolicy = new PrivacyPolicy();
                                myPrivacyPolicy.processXml(reader);
                             break;
                            case "SymbolicLink":
                                mySymbolicLinks.Add(new SymbolicLink(reader));
                                break;
                            case "Certificate":
                                myCertificateInstallers.Add(new CertificateInstall(reader));
                                break;
                        }
                }


             }

            myLogWriter.LogWrite("Finished loading configuration");

            CreateDirectoryRecursively(wrapperAppData);

            myLogWriter.LogWrite("Created Wrapper folder at " + wrapperAppData);
            // var DLL = Assembly.LoadFile(ResolveVariables("[APPDIR]") + "\\PsfRuntime64.dll");
            if (myPrivacyPolicy != null) {
                myLogWriter.LogWrite("Executing myPrivacyPolicy.Execute();");
                myPrivacyPolicy.Execute();
            }

            if (myUpdateHandler != null) {
                myLogWriter.LogWrite("Executing myUpdateHandler.Execute();");

               myUpdateHandler.Execute();


            }
            foreach (RegistryEntry myRegistryEntry in myRegistryEntries)
            {
                myRegistryEntry.Execute();
            }
            
            foreach ( VirtualFile myFile in myFiles)
            {
                myFile.Execute();
                myLogWriter.LogWrite("Copying " + myFile.getFile() + " to " + myFile.getTarget());
            }
            foreach (VirtualFolder myVirtualFolder in myVirtualFolders)
            {
                myVirtualFolder.Execute();
                myLogWriter.LogWrite("Copying " + myVirtualFolder.getFolder() + " to " + myVirtualFolder.getTarget());
            }
            foreach (SymbolicLink mySymbolicLink in mySymbolicLinks)
            {
                mySymbolicLink.Execute();
                myLogWriter.LogWrite("Creating Symblic Link " + mySymbolicLink.getLink() + " with destination " + mySymbolicLink.getTarget());
            }

            foreach (RougeConfig myRougeConfig in myRougeConfigs)
            {
                myRougeConfig.Execute();
                myLogWriter.LogWrite("Copying " + myRougeConfig.getFilePath() + " to " + myRougeConfig.getAppDataFilePath());
            }

            if ( myLiteWarning != null ) myLiteWarning.Execute();

            foreach (CertificateInstall myCertificateInstall in myCertificateInstallers)
            {
                myCertificateInstall.Execute();
            }

            foreach ( Runtime myRuntime in myRuntimes ) {
                myLogWriter.LogWrite("Executing myRuntime.Execute();");
                myRuntime.Execute();
            }

            foreach (ServiceHandler myServiceHandler in myServiceHandlers)
            {
                myLogWriter.LogWrite("Executing myServiceHandler.Execute();");
                myServiceHandler.Execute();
            }

            foreach (RougeConfig myRougeConfig in myRougeConfigs)
            {
                myRougeConfig.CleanUp();
                myLogWriter.LogWrite("Copying " + myRougeConfig.getAppDataFilePath() + " to " + myRougeConfig.getFilePath());
            }

            if (myUpdateHandler != null)
            {
                myLogWriter.LogWrite("Executing myUpdateHandler.Execute();");

                myUpdateHandler.Execute();

            }
            myLogWriter.LogWrite("Exiting wrapper.");
        }

        static public string getCommandLineArgument()
        {
            var exe = Environment.GetCommandLineArgs()[0]; // Command invocation part
            var rawCmd = Environment.CommandLine;          // Complete command
            string argsOnly = "";
            try // Improve me
            {
                argsOnly = rawCmd.Remove(rawCmd.IndexOf(exe), exe.Length).TrimStart('"').Substring(1);
            } catch (Exception e)
            {

            }
            return argsOnly;
        }

        static public string ResolveVariables(string unresolvedString)
        {
            string resolvedString = unresolvedString;
          //  Regex rx = new Regex(@"\[(?>\[(?<LEVEL>)|\](?<-LEVEL>)|(?!\[|\]).)+(?(LEVEL)(?!))\]");
           Regex rx = new Regex(@"\[([^[]*?)\]");
            MatchCollection matches = rx.Matches(resolvedString);
            while ( matches.Count > 0 )
            {
              
                Match match = matches[0];

                GroupCollection groups = match.Groups;
                string variable = groups[0].Value;
                int index = groups[0].Index;
                int length = groups[0].Length;
                string data = getResolvedVariable(groups[0].Value);
                resolvedString = resolvedString.Remove(index, length);
                resolvedString = resolvedString.Insert(index, data);

                matches = rx.Matches(resolvedString);
           
            }
            return resolvedString;
        }

        static string getResolvedVariable(string variable)
        {
            Console.Write("Resolving " + variable + " to ");
            string[] parameters = null;
            string value = "";
            if ( variable.Contains("|") )
            {
                parameters = variable.Split('|');
                variable = parameters[0];
                parameters[parameters.Length - 1] = parameters[parameters.Length-1].TrimEnd(']');
            }
            switch ( variable )
            {
                case "[EXENAME]":
                    value = System.AppDomain.CurrentDomain.FriendlyName;
                    break;
                case "[APPDIR]":
                    string location = System.Reflection.Assembly.GetEntryAssembly().Location;
                    value = location + "\\..";
                    break;
                case "[INSTALLDIR]":
                    value = Package.Current.InstalledPath;
                    break;
                case "[ARGS]":
                    value = getCommandLineArgument();
                    break;
                case "[RESOLVED_ARGS]":
                    value = ResolveVariables(getCommandLineArgument());
                    break;
                case "[ARGSSELECTOR": // Remove the ArgsSelector.
                    value = "";
                    break;
                case "[CHANGEEXTENSION":
                    if (parameters.Length > 2)
                        value = Path.ChangeExtension(parameters[1], parameters[2]);
                    break;
                case "[SPECIALFOLDER":
                    if (parameters.Length > 1)
                        switch (parameters[1])
                        {
                            case "MYDOCUMENTS":
                                value = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                            break;
                        }
                    break;
                case "[RETRIVEFROMREGISTRY": // Get data from the registry
                    if (parameters.Length > 4)
                        value = null;
                        try
                    {
                        value = RetriveFromRegistry(parameters[1], parameters[2], parameters[3]);
                    } catch (Exception e)
                    {
                        // do nothing
                    }
                    if ( value == null )
                        value = parameters[4];  // In case the value does not exist.
                    value = ResolveVariables(value);
                    break;
                case "[WRAPPER_APPDATA]":
                    value = ResolveVariables("[APPDATA]\\weatherlights.com\\[EXENAME]\\");
                break;
                default:
                    string envvariabel = variable.TrimStart('[').TrimEnd(']');
                    value = System.Environment.GetEnvironmentVariable(envvariabel);
                    break;

            }
            Console.WriteLine(value);
            return value;

        }

        public static string RetriveFromRegistry(string BaseKey, string SubKey, string ValueName)
        {
            RegistryKey myRegistryKey = null;
            if (BaseKey == "HKLM")
                myRegistryKey = Registry.LocalMachine.OpenSubKey(SubKey);
            else if (BaseKey == "HKCU")
                myRegistryKey = Registry.CurrentUser.OpenSubKey(SubKey);

            string value = (string)myRegistryKey.GetValue(ValueName);
            if ( myRegistryKey != null  )
                myRegistryKey.Close();
            return value;
        }



        public static void CreateDirectoryRecursively(string path)
        {
            string[] pathParts = path.Split('\\');
            for (var i = 0; i < pathParts.Length - 1; i++)
            {
                // Correct part for drive letters
                if (i == 0 && pathParts[i].Contains(":"))
                {
                    pathParts[i] = pathParts[i] + "\\";
                } // Do not try to create last part if it has a period (is probably the file name)
                else if (i == pathParts.Length - 1 && pathParts[i].Contains("."))
                {
                    return;
                }
                if (i > 0)
                {
                    pathParts[i] = Path.Combine(pathParts[i - 1], pathParts[i]);
                }
                if (!Directory.Exists(pathParts[i]))
                {
                    Directory.CreateDirectory(pathParts[i]);
                }
            }
        }
    }
}
