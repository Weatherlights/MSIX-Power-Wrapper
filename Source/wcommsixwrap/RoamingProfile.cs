using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml;
using System.Diagnostics.Eventing.Reader;
using System.IO;

namespace wcommsixwrap
{
    internal class RoamingProfile
    {
        public static string Element = "RoamingProfile";
        private string _profileFilePath;
        private string _remoteStoragePath;
        private string _pathThatNeedsToExist;
        private string _enabled;
        private bool fileExisted = false;
        private LogWriter LogWriter = new LogWriter(Element);

        private DateTime AppStartTime;

        public RoamingProfile(XmlReader reader)
        {
            this.AppStartTime = DateTime.Now;
            this.processXml(reader);
        }

        private string profileFilePath
        {
            get
            {
                return Program.ResolveVariables(_profileFilePath);
            }
            set => _profileFilePath = value;
        }

        private string pathThatNeedsToExist
        {
            get
            {
                return Program.ResolveVariables(_pathThatNeedsToExist);
            }
            set => _pathThatNeedsToExist = value;
        }

        private string remoteStoragePath
        {
            get
            {
                return Program.ResolveVariables(_remoteStoragePath);
            }
            set => _remoteStoragePath = value;
        }
        
        private string remoteStorageFilePath
        {
            get
            {
                string targetFileName = Path.GetFileName(profileFilePath);
                return Path.Combine(remoteStoragePath, targetFileName);
            }
        }


        public bool Enabled
        {
            get
            {
                string enabledEvalValue = Program.ResolveVariables(_enabled);
                if (enabledEvalValue == "FALSE" || enabledEvalValue == "0" || enabledEvalValue == null)
                    return false;
                else
                    return true;
            }
            set
            {
                if ( value )
                    _enabled = "TRUE";
                else
                    _enabled = "FALSE";
            }
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
                        case "ProfileFilePath":
                            reader.Read();
                            profileFilePath = reader.Value;
                            break;
                        case "RemoteStoragePath":
                            reader.Read();
                            remoteStoragePath = reader.Value;
                            break;
                        case "Enabled":
                            reader.Read();
                            _enabled = reader.Value;
                            break;
                        case "PathThatNeedsToExist":
                            reader.Read();
                            pathThatNeedsToExist = reader.Value;
                            break;
                    }

                }
            }
        }

        public void CleanUp()
        {
            if (this.Enabled && Directory.Exists(pathThatNeedsToExist))
                try
                {
                    if (File.Exists(remoteStorageFilePath) && File.Exists(profileFilePath))
                    {
                        DateTime targetFileLastWriteTime = File.GetLastWriteTimeUtc(remoteStorageFilePath);
                        DateTime sourceFileLastWriteTime = File.GetLastWriteTimeUtc(profileFilePath);

                        if (sourceFileLastWriteTime > targetFileLastWriteTime)
                        {
                            File.Copy(profileFilePath, remoteStorageFilePath, true);
                            LogWriter.LogWrite("Coping roaming configuration file from " + profileFilePath + " to " + remoteStorageFilePath, 1);
                        } else
                        {
                            LogWriter.LogWrite("Will not write back app configuration since the roaming configuration was updated by another process or is unchanged.", 1);
                        }
                    } else if (File.Exists(profileFilePath))
                    {
                        if (!Directory.Exists(remoteStoragePath))
                        {
                            Program.CreateDirectoryRecursively(remoteStoragePath);
                        }
                        File.Copy(profileFilePath, remoteStorageFilePath, false);
                        LogWriter.LogWrite("Coping roaming configuration file from " + profileFilePath + " to " + remoteStorageFilePath, 1);
                    }
                    else
                    {
                        LogWriter.LogWrite("App configuration file " + profileFilePath + " does not exist yet.", 1);
                    }



                }
                catch (System.IO.IOException e)
                {
                    LogWriter.LogWrite("Exception raised: " + e.Message.ToString());
                }
        }

        public void Execute()
        {
            if ( this.Enabled )
                try
                {

                    if (File.Exists(remoteStorageFilePath)) {
                        File.Copy(remoteStorageFilePath, profileFilePath, true);
                        LogWriter.LogWrite("Coping roaming configuration file from " + remoteStorageFilePath + " to " + profileFilePath, 1);
                    } else
                    {
                        LogWriter.LogWrite("Roaming configuration file " + remoteStorageFilePath  + " does not exist yet.", 1);
                    }

                }
                catch (System.IO.IOException e)
                {
                    LogWriter.LogWrite("Exception raised: " + e.Message.ToString());
                }
            else
            {
                LogWriter.LogWrite("Skipping profile copy since it is not enabled. ", 1);
            }
        }
    }
}
