﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
//using Windows.ApplicationModel;

namespace wcommsixwrap
{
    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
           
                
            string location = System.Reflection.Assembly.GetEntryAssembly().Location;
            string configlocation = location + ".wrunconfig";
            string wrapperAppData = ResolveVariables("[WRAPPER_APPDATA]");

            CreateDirectoryRecursively(wrapperAppData);

 //           ConfigurationReader configReader = new ConfigurationReader();
 //           bool test = configReader.validateSignature("C:\\Users\\hauke\\GitHub\\Winget-AutoUpdate-Intune\\WinGet-AutoUpdate-Configurator\\wcommsixconfig.cat");

 //           bool test2 = configReader.validateFileAgainstHash("C:\\Users\\hauke\\GitHub\\Winget-AutoUpdate-Intune\\WinGet-AutoUpdate-Configurator\\wcommsixconfig.cat", "C:\\Users\\hauke\\GitHub\\Winget-AutoUpdate-Intune\\WinGet-AutoUpdate-Configurator\\Winget-AutoUpdate-x64.exe.wrunconfig");

            LogWriter myLogWriter = new LogWriter("Main");
            myLogWriter.LogWrite("MSIX Powerwrapper started.");

            myLogWriter.LogWrite("Configuration location is " + configlocation);

            List<VirtualFile> myFiles = new List<VirtualFile>();
            List<VirtualFolder> myVirtualFolders = new List<VirtualFolder>();
            List<RougeConfig> myRougeConfigs = new List<RougeConfig>();
            List<RegistryEntry> myRegistryEntries = new List<RegistryEntry>();
            List<SymbolicLink> mySymbolicLinks = new List<SymbolicLink>();
            List<Runtime> myRuntimes = new List<Runtime>();
            List<UnwantedFolder> myUnwantedFolders = new List<UnwantedFolder>();
            List<UnwantedFile> myUnwantedFiles = new List<UnwantedFile>();
            List<ServiceHandler> myServiceHandlers = new List<ServiceHandler>();
            List<CertificateInstall> myCertificateInstallers = new List<CertificateInstall>();
            List<EnvironmentVariable> myEnvironmentVariables = new List<EnvironmentVariable>();
            LiteWarning myLiteWarning = null;
            UpdateHandler myUpdateHandler = null;
            AppInstallerUpdateHandler myAppInstallerUpdateHandler = null;
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
                            case "UnwantedFolder":
                                myUnwantedFolders.Add(new UnwantedFolder(reader));
                                break;
                            case "UnwantedFile":
                                myUnwantedFiles.Add(new UnwantedFile(reader));
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
                            case "AppInstallerUpdateHandler":
                                myAppInstallerUpdateHandler = new AppInstallerUpdateHandler();
                                myAppInstallerUpdateHandler.processXml(reader);
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
                            case "EnvironmentVariable":
                                myEnvironmentVariables.Add(new EnvironmentVariable(reader));
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
                if (myUpdateHandler.hasMandatoryUpdates && myUpdateHandler.WaitForUpdateSearchToFinish)
                {
                    myLogWriter.LogWrite("Will exit the application to finish the installation of updates.");
                    return;
                }
            }

            if (myAppInstallerUpdateHandler != null)
            {
                myLogWriter.LogWrite("Executing myAppInstallerUpdateHandler.Execute();");

                myAppInstallerUpdateHandler.Execute();
                if (myAppInstallerUpdateHandler.hasMandatoryUpdates && myAppInstallerUpdateHandler.WaitForUpdateSearchToFinish)
                {
                    myLogWriter.LogWrite("Will exit the application to finish the installation of updates.");
                    return;
                }
            }


            foreach (EnvironmentVariable myEnvironmentVariable in myEnvironmentVariables)
            {
                myEnvironmentVariable.Execute();
                myLogWriter.LogWrite("Set environment variable  " + myEnvironmentVariable.variableName + " to " + myEnvironmentVariable.variableValue + " for " + myEnvironmentVariable.variableTarget);
            }

            foreach (RegistryEntry myRegistryEntry in myRegistryEntries)
            {
                myRegistryEntry.Execute();
            }

            foreach (UnwantedFolder myUnwantedFolder in myUnwantedFolders)
            {
                myUnwantedFolder.Execute();
                myLogWriter.LogWrite("Removing unwanted Folder " + myUnwantedFolder.getFolderPath());
            }

            foreach (UnwantedFile myUnwantedFile in myUnwantedFiles)
            {
                myUnwantedFile.Execute();
                myLogWriter.LogWrite("Removing unwanted File " + myUnwantedFile.getFilePath());
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

            if (myAppInstallerUpdateHandler != null)
            {
                myLogWriter.LogWrite("Executing myUpdateHandler.Execute();");

                myAppInstallerUpdateHandler.Execute();

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

        static public string getCommandLineArgument(string argumentName)
        {
            var args = Environment.GetCommandLineArgs();
          
            string value = "";
            try
            {
                for (int i = 1; i < args.Length-1; i++ )
                {
                    if (args[i].ToUpper().Equals("/" + argumentName.ToUpper()) == true || args[i].ToUpper().Equals("-" + argumentName.ToUpper()) == true)
                    {
                        value = args[i + 1];
                    }

                }
            }
            catch (Exception e)
            {

            }
            return value;
        }

        static public string ResolveVariables(string unresolvedString)
        {
            string resolvedString = unresolvedString;
            //  Regex rx = new Regex(@"\[(?>\[(?<LEVEL>)|\](?<-LEVEL>)|(?!\[|\]).)+(?(LEVEL)(?!))\]");
            Regex rx = new Regex(@"\[(?>\[(?<c>)|[^\[\]]+|\](?<-c>))*(?(c)(?!))\]");
            //Regex rx = new Regex(@"\[([^[]*?)\]");
            MatchCollection matches = rx.Matches(resolvedString);
            int totalMatches = matches.Count;
            int lostLength = 0;
            int gainedLength = 0;
            //while (totalMatches >= 0 )
                for (int i = 0; i < matches.Count; i++)
            {
              
                Match match = matches[i];

                GroupCollection groups = match.Groups;
                string variable = groups[0].Value;
                int index = groups[0].Index;
                int length = groups[0].Length;
                int actualIndex = index - lostLength + gainedLength;
                string data = getResolvedVariable(variable);
                if (data != null)
                {
                    resolvedString = resolvedString.Remove(actualIndex, length);
                    lostLength += length;
                    resolvedString = resolvedString.Insert(actualIndex, data);
                    gainedLength += data.Length;

                    //matches = rx.Matches(resolvedString);
                    //totalMatches = matches.Count;
                    
                } else
                {
                    
                }
            }
           
            return resolvedString;
        }

        static string getResolvedExeName()
        {
            return System.AppDomain.CurrentDomain.FriendlyName;
        }

        static string getResolvedAppDir()
        {
            string location = System.Reflection.Assembly.GetEntryAssembly().Location;
            location = location + "\\..";
            return location;
        }

        static string getResolvedArgs()
        {
            string commandLine = getCommandLineArgument();
            return commandLine;
        }

        static string getResolvedRESOLVED_ARGS()
        {
            string value = "";
            value = ResolveVariables(getCommandLineArgument());
            return value;
        }

        static string removeResolvedARGSSelector()
        {
            string value = "";
            return value;
        }

        static string getResolvedWRAPPER_APPDATA()
        {
            string value = "";
            value= ResolveVariables("[APPDATA]\\weatherlights.com\\[EXENAME]\\");
            return value;
        }

        static public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;

        }

        static public string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
            string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }



        static string[] prepareParameters(string variable)
        {
            string preparedVariable = variable.TrimStart('[').TrimEnd(']');
            Regex rx = new Regex(@"\[(?>\[(?<c>)|[^\[\]]+|\](?<-c>))*(?(c)(?!))\]");
            //Regex rx = new Regex(@"\[([^[]*?)\]");
            MatchCollection matches = rx.Matches(preparedVariable);
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                string otherVariable = groups[0].Value;
                preparedVariable = preparedVariable.Replace(otherVariable, "[" + EncodeTo64(otherVariable) + "]");
            }
            string[] splittedVariable = preparedVariable.Split('|');
            for (int i = 0; i < splittedVariable.Length; i++ )
            {
                MatchCollection b64m = rx.Matches(splittedVariable[i]);
                foreach(Match match in b64m)
                {
                    GroupCollection groups = match.Groups;
                    string encodedVariable = groups[0].Value;
                    string decodedVariable = DecodeFrom64(encodedVariable.TrimStart('[').TrimEnd(']'));
                    splittedVariable[i] = splittedVariable[i].Replace(encodedVariable, decodedVariable);
                }
            }


            return splittedVariable;


        }

        static string getResolvedVariable(string variable)
        {
            Console.Write("Resolving " + variable + " to ");
            string[] parameters = prepareParameters(variable);
            string value = "";
           /* if ( variable.Contains("|") )
           // {
           
                parameters = variable.Split('|');
                variable = parameters[0];
                parameters[parameters.Length - 1] = parameters[parameters.Length-1].TrimEnd(']');
            }*/
            switch ( parameters[0] )
            {
                case "EXENAME":
                    value = getResolvedExeName();
                    break;
                case "APPDIR":
                    value = getResolvedAppDir();
                    break;
//                case "[INSTALLDIR]":
//                    value = Package.Current.InstalledPath;
 //                   break;
                case "ARGS":
                    if (parameters.Length == 2)
                        value = getCommandLineArgument(parameters[1]);
                    else
                        value = getResolvedArgs();
                    break;
                case "RESOLVED_ARGS":
                    value = getResolvedRESOLVED_ARGS();
                    break;
                case "ARGSSELECTOR": // Remove the ArgsSelector.
                    value = removeResolvedARGSSelector();
                    break;
                case "CHANGEEXTENSION":
                    if (parameters.Length > 2)
                        value = Path.ChangeExtension(parameters[1], parameters[2]);
                    break;
                case "QUOTE":
                    if (parameters.Length > 1)
                        value = parameters[1];
                    break;
                case "ENV":
                    if (parameters.Length > 1)
                        value = System.Environment.GetEnvironmentVariable(parameters[1]);
                    break;
                case "SPECIALFOLDER":
                    if (parameters.Length > 1)
                        switch (parameters[1])
                        {
                            case "MYDOCUMENTS":
                                value = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                            break;
                        }
                    break;
                case "RETRIVEFROMREGISTRY": // Get data from the registry
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
                case "WRAPPER_APPDATA":
                    value = getResolvedWRAPPER_APPDATA();
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
