using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using Windows.Foundation;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.UI.Popups;
using System.Windows.Documents;
using Windows.Networking.BackgroundTransfer;
using Windows.Services.Store;

namespace wcommsixwrap
{

    internal class AppInstallerUpdateHandler
    {
        private CultureInfo ci;
        private PackageManager packageManager = null;
        private PackageUpdateAvailabilityResult updateresult;
        //private IReadOnlyList<StorePackageUpdate> updates;
        public string message = "";
        public string caption = "";
        public string targetPackageFullName;
        public string messageFailRequired = "We can not launch the application without installing important updates. Please check the Microsoft Store for possible installation failures and retry.";
        public string captionFailRequired = "Updates are missing";
        public bool WaitForUpdateSearchToFinish = true;
        public bool mandatoryInstallationFailure = false;
        private bool restartOnMandatoryUpdate = true;
        private bool treatAvailableUpdateAsMandatory = false;
        public bool hasMandatoryUpdates { get; set; }
        public Uri AppInstallerUri { get; set; }

        LogWriter myLogWriter = null;

        private bool installOnExit { get; set; }

        public static string Element = "AppInstallerUpdateHandler";

        public AppInstallerUpdateHandler()
        {
            installOnExit = false;
            myLogWriter = new LogWriter(Element);

            ci = CultureInfo.InstalledUICulture;
        }

        public void processXml(XmlReader reader)
        {
            string langcode = ci.TwoLetterISOLanguageName;
            bool languageFoundForDialog = false;
            bool languageFoundForError = false;
            while (!reader.Name.Equals(Element) || reader.IsStartElement())
            {

                reader.Read();
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "Message":
                            string language = reader.GetAttribute("lang");

                            if (language.Equals(langcode) || (language.Equals("default") && !languageFoundForDialog))
                            {
                                caption = reader.GetAttribute("caption");
                                reader.Read();
                                message = reader.Value;
                                languageFoundForDialog = true;
                            }
                            break;
                        case "RequiredUpdateFailureMessage":
                            string languageFailRequired = reader.GetAttribute("lang");

                            if (languageFailRequired.Equals(langcode) || (languageFailRequired.Equals("default") && !languageFoundForError))
                            {
                                captionFailRequired = reader.GetAttribute("caption");
                                reader.Read();
                                messageFailRequired = reader.Value;
                                languageFoundForError = true;
                            }
                            break;
                        case "WaitForUpdateSearchToFinish":
                            reader.Read();
                            if (reader.Value.Equals("false"))
                                WaitForUpdateSearchToFinish = false;
                            break;
                        case "RestartOnMandatoryUpdate":
                            reader.Read();
                            if (reader.Value.Equals("false"))
                                restartOnMandatoryUpdate = false;
                            break;
                        case "TreatAvailableUpdateAsMandatory":
                            reader.Read();
                            if (reader.Value.Equals("true"))
                                treatAvailableUpdateAsMandatory = true;
                            break;
                        case "AppInstallerUri":
                            reader.Read();
                            AppInstallerUri = new Uri(reader.Value);
                            break;
                        case "TargetPackageFullName":
                            reader.Read();
                            targetPackageFullName = reader.Value;
                            break;

                    }

                }
            }
        }

        private void SetRestartOnUpdate()
        {
            if (restartOnMandatoryUpdate)
            {
                myLogWriter.LogWrite("Will now set application restart in case of mandatory update.");
                uint res = RelaunchHelper.RegisterApplicationRestart(null, RelaunchHelper.RestartFlags.RESTART_NO_HANG);
                if (res != 0)
                {
                    myLogWriter.LogWrite("Restart could not be set.");
                }
                else
                {
                    myLogWriter.LogWrite("Registered app in case of mandatory update.");
                }
            }
        }

        public void Execute()
        {

            if (packageManager != null)
            {
                myLogWriter.LogWrite("Will now see if we need to install something since we are about to exit.");
                Task updateTask = new Task(this.InstallOnExit);

                updateTask.Start();
                updateTask.Wait();
            }
            if (packageManager == null)
            {

                myLogWriter.LogWrite("Will now search for updates.");
                Thread updateThread = new Thread(this.InstallOnStart);
                updateThread.SetApartmentState(ApartmentState.STA);
                updateThread.Start();
                myLogWriter.LogWrite("Status: " + WaitForUpdateSearchToFinish);
                if (this.WaitForUpdateSearchToFinish == true)
                {
                    myLogWriter.LogWrite("Wait for update procedure to finish.");
                    updateThread.Join();
                }
                else
                {
                    myLogWriter.LogWrite("We will not wait for update procedure to finish.");
                }



                //Task searchTask = new Task(this.InstallOnStart);
                //searchTask.Start();
                //searchTask.Wait();
            }
            myLogWriter.LogWrite("Exiting UpdateHandler.");
        }



        public async void InstallOnStart()
        {
           
            //   Program.CreateDirectoryRecursively(getLink());
            //formOperation.Wait();
            myLogWriter.LogWrite("Checking for updates");

            if (packageManager == null)
            {
                myLogWriter.LogWrite("Package manager was not initialized.");
                packageManager = new PackageManager();
            }

            //MessageBox.Show("Searching for Updates", "Searching", MessageBoxButton.OK, MessageBoxImage.Information);
            try
            {
                myLogWriter.LogWrite("Searching updates for app " + Package.Current.Id.FullName, 1);
                Package package = packageManager.FindPackageForUser(string.Empty, Package.Current.Id.FullName);
                bool executeUpdateProcedure = false;

                if (AppInstallerUri == null)
                {
                    myLogWriter.LogWrite("No AppInstallerUri defined in configuration. Retriving configuration from package.", 2);
                    AppInstallerUri = package.GetAppInstallerInfo().Uri;
                    myLogWriter.LogWrite("Retrived uri: " + AppInstallerUri.AbsoluteUri, 1);
                }

                if (AppInstallerUri == null)
                {
                    myLogWriter.LogWrite("No AppInstallerUri defined in package. Can not continue", 3);
                    executeUpdateProcedure = true;
                }

                IAsyncOperation<PackageUpdateAvailabilityResult> searchOperation = package.CheckUpdateAvailabilityAsync();
                
                searchOperation.AsTask().Wait();
                
                updateresult = await searchOperation.AsTask();
                switch (updateresult.Availability)
                {
                    case PackageUpdateAvailability.Available:
                        myLogWriter.LogWrite("Found available updates.");
                        if (treatAvailableUpdateAsMandatory)
                        {
                            hasMandatoryUpdates = true;
                            myLogWriter.LogWrite("treatAvailableUpdateAsMandatory is set. Will treat available update as mandatory.");
                        } else
                        {
                            installOnExit = true;
                            myLogWriter.LogWrite("treatAvailableUpdateAsMandatory is not set. Will install update on exit.");
                        }
                        break;
                    case PackageUpdateAvailability.Required:
                        //Queue up the update and close the current instance
                            hasMandatoryUpdates = true;
                            myLogWriter.LogWrite("Found mandatory updates.");
                        break;
                    case PackageUpdateAvailability.NoUpdates:
                        // Close AppInstaller.
                        hasMandatoryUpdates = false;
                        myLogWriter.LogWrite("No updates found.");
                        break;
                    case PackageUpdateAvailability.Unknown:
                    default:
                        // Log and ignore error.
                        myLogWriter.LogWrite("Failed to process updates.",3);
                        break;
                }

                    executeUpdateProcedure = true;
                    if (executeUpdateProcedure)
                    {


                        if (hasMandatoryUpdates == false)
                        {

                            /* 
                            myLogWriter.LogWrite("Begin downloading optional updates.");
                            StorePackageUpdateResult downloadResult =
                                await context.TrySilentDownloadStorePackageUpdatesAsync(updates);

                            switch (downloadResult.OverallState)
                            {
                                case StorePackageUpdateState.Completed:
                                    myLogWriter.LogWrite("Download has been finished successfully.");
                                    installOnExit = true;
                                    myLogWriter.LogWrite("Mark update to be installed when the application closes.");
                                    break;
                                case StorePackageUpdateState.Canceled:
                                    myLogWriter.LogWrite("Update canceled: Canceled by the user.", 2);
                                    return;
                                case StorePackageUpdateState.ErrorLowBattery:
                                    myLogWriter.LogWrite("Update canceled: Battery level to low to download update.", 2);
                                    return;
                                case StorePackageUpdateState.ErrorWiFiRecommended:
                                    myLogWriter.LogWrite("Update canceled: The user is recommended to use a wifi to update.", 2);
                                    return;
                                case StorePackageUpdateState.ErrorWiFiRequired:
                                    myLogWriter.LogWrite("Update canceled: A wifi connection is required to perform the update.", 2);
                                    return;
                                case StorePackageUpdateState.OtherError:
                                    myLogWriter.LogWrite("Update canceled: Unknown error.", 3);
                                    return;
                                default:
                                    break;
                            }*/


                        }
                        else
                        {
                            this.SetRestartOnUpdate();
                            try
                            {

                                myLogWriter.LogWrite("Mandatory updates need to be installed and will now be enforced.");
                                UpdateHandlerWindow updateHandlerWindow = new UpdateHandlerWindow(packageManager, AppInstallerUri, caption, message, captionFailRequired, messageFailRequired);
                                myLogWriter.LogWrite("Window has been initialized.");
                                 updateHandlerWindow.ShowDialog();


                            myLogWriter.LogWrite("InstallUpdate has finished execution.");
                                if (!updateHandlerWindow.failure)
                                {
                                    myLogWriter.LogWrite("App has been scheduled for a restart.");
                                }
                                // await InstallUpdate(updates);

                            }
                            catch (Exception ex)
                            {
                                myLogWriter.LogWrite("Failure initializing the mandatory app installation." +
                                    "\n" + ex.Message
                                    + "\n" + ex.ToString(), 3);
                            }
                        }

                    }
                else
                {
                    myLogWriter.LogWrite("Will not update.", 1);
                    hasMandatoryUpdates = false;
                }
            }
            catch (Exception ex)
            {
                myLogWriter.LogWrite("Failure running package.CheckUpdateAvailabilityAsync with " + ex.ToString(), 3);
                hasMandatoryUpdates = false;
            }
            myLogWriter.LogWrite("Exiting Update procedure.");

        }

        private async void InstallOnExit()
        {

            if (installOnExit != false)
            {
                myLogWriter.LogWrite("Pending updates will now be installed.");
                await InstallUpdate();
                myLogWriter.LogWrite("InstallUpdate has finished execution.");
            }
            else
            {
                myLogWriter.LogWrite("Nothing to install on exit.");
            }
        }


        private async Task InstallUpdate()
        {

            // Start the silent installation of the packages. Because the packages have already
            // been downloaded in the previous method, the following line of code just installs
            // the downloaded packages.

            IAsyncOperationWithProgress<Windows.Management.Deployment.DeploymentResult, Windows.Management.Deployment.DeploymentProgress> installOperation = null;
            installOperation = packageManager.AddPackageByAppInstallerFileAsync(AppInstallerUri, AddPackageByAppInstallerOptions.ForceTargetAppShutdown, packageManager.GetDefaultPackageVolume());

            myLogWriter.LogWrite("Status: " + installOperation.Status.ToString());
            //updateHandlerForm.ShowDialog();

            

            

            var installResult = await installOperation;
            //updateHandlerForm.Close();
            myLogWriter.LogWrite("Finished update");
            //StorePackageUpdateResult downloadResult =
            //    await context.TrySilentDownloadAndInstallStorePackageUpdatesAsync(storePackageUpdates);
            bool success = true;

            var result = await installOperation;
            if (result.ExtendedErrorCode != null)
            {
                success = false;
                installOnExit = true;

                myLogWriter.LogWrite("Update failed with: result." + result.ExtendedErrorCode, 3);
            }
            if (!success && hasMandatoryUpdates)
            {
                mandatoryInstallationFailure = true;
            }


        }


    }
}
