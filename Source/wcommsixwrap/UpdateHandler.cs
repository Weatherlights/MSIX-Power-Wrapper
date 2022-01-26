using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Windows.Foundation;
using Windows.Services.Store;
using Windows.UI.Popups;

namespace wcommsixwrap
{
    internal class UpdateHandler
    {
        private CultureInfo ci;
        private StoreContext context = null;
        public string message;
        public string caption;
        public string mode;
        public bool hasMandatoryUpdates { get; set; }
        private IReadOnlyList<StorePackageUpdate> updates;
        LogWriter myLogWriter = null;

        private bool installOnExit { get; set; }
        UpdateHandlerForm updateHandlerForm;

        public static string Element = "UpdateHandler";

        public UpdateHandler()
        {
            installOnExit = false;
            myLogWriter = new LogWriter(Element);

            ci = CultureInfo.InstalledUICulture;
            updateHandlerForm = new UpdateHandlerForm();
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
                        case "Mode":
                            mode = reader.Value;
                            break;
                                
                    }

                }
            }
        }

        private async Task Sleepy()
        {
            myLogWriter.LogWrite("Sleepy Start.");
            Thread.Sleep(100);
            myLogWriter.LogWrite("Sleepy End.");

        }

        public void Execute()
        {
            if (context != null)
            {
                myLogWriter.LogWrite("Will now see if we need to install something since we are about to exit.");
                Task updateTask = new Task(this.InstallOnExit);
                updateTask.Start();
                updateTask.Wait();
            }
            if ( context == null )
            {
                myLogWriter.LogWrite("Will now search for updates.");
                Task searchTask = new Task(this.InstallOnStart);
                searchTask.Start();
                searchTask.Wait();
            }
            myLogWriter.LogWrite("Exiting UpdateHandler.");
        }


            public async void InstallOnStart() {
            //   Program.CreateDirectoryRecursively(getLink());

            myLogWriter.LogWrite("Checking for updates");
            
            if (context == null)
            {
                context = StoreContext.GetDefault();
            }
            myLogWriter.LogWrite("Loaded store context.");
            
            //MessageBox.Show("Searching for Updates", "Searching", MessageBoxButton.OK, MessageBoxImage.Information);
            try
            {
                await Sleepy();
                updates = await context.GetAppAndOptionalStorePackageUpdatesAsync();


                myLogWriter.LogWrite("Finished search for updates.");
                //MessageBox.Show("Update search finished.", "Searching", MessageBoxButton.OK, MessageBoxImage.Information);
                //MessageBox.Show("Found: " + updates.Count + "Updates", "Searching", MessageBoxButton.OK, MessageBoxImage.Information);
                myLogWriter.LogWrite("Found " + updates.Count + " updates.");
                if (updates.Count > 0)
                {

                    //MessageBox.Show("Updates found", "Searching", MessageBoxButton.OK, MessageBoxImage.Information);
                    bool executeUpdateProcedure = false;
                    foreach (StorePackageUpdate update in updates)
                    {
                        if (update.Mandatory == true)
                        {
                            hasMandatoryUpdates = true;
                            myLogWriter.LogWrite("Found mandatory updates.");
                        }
                        else
                        {
                            hasMandatoryUpdates = false;
                            myLogWriter.LogWrite("No mandatory updates found.");
                        }

                    }
                    /* if (hasMandatoryUpdates == false)
                    {
                        myLogWriter.LogWrite("Asking user to perform update.");
                        MessageBoxResult msgresult = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (msgresult.Equals(MessageBoxResult.Yes))
                        {
                            executeUpdateProcedure = true;
                            myLogWriter.LogWrite("User accepted update.");
                        }
                        else
                        {
                            myLogWriter.LogWrite("User rejected update.");
                        }

                    }
                    else */
                        executeUpdateProcedure = true;
                    //MessageBox.Show("Exceute Update is " + executeUpdateProcedure, "Update is running", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (executeUpdateProcedure)
                    {
                        

                        if (hasMandatoryUpdates == false)
                        {
                            myLogWriter.LogWrite("Begin downloading optional updates.");
                            StorePackageUpdateResult downloadResult =
                                await context.TrySilentDownloadStorePackageUpdatesAsync(updates);
                            
                            if (downloadResult.OverallState == StorePackageUpdateState.Completed)
                            {
                                myLogWriter.LogWrite("Download has been finished successfully.");
                                installOnExit = true;
                                myLogWriter.LogWrite("Mark update to be installed when the application closes.");
                            } else
                            {
                                myLogWriter.LogWrite("Download failed with status: " + downloadResult.OverallState.ToString(), 2);
                            }

                        } else
                        {
                            myLogWriter.LogWrite("Mandatory updates need to be installed and will now be enforced.");
                            await InstallUpdate(updates);
                            myLogWriter.LogWrite("InstallUpdate has finished execution.");
                        }

                       

                        // Download and install the updates.
                        //IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> downloadOperation =
                        //await context.RequestDownloadAndInstallStorePackageUpdatesAsync(updates);

                        //downloadOperation.Pro

                        /* updateHandlerForm.ShowDialog();
                        downloadOperation.Progress = async (asyncInfo, progress) =>
                        {
                        updateHandlerForm.prgProgress.Value = (int)progress.PackageDownloadProgress;

                        };
                        updateHandlerForm.Close();*/

                        //StorePackageUpdateResult result = await downloadOperation.AsTask();
                        //MessageBox.Show("Update has finished" , "Update is running", MessageBoxButton.OK, MessageBoxImage.Information);

                    }


                } else
                {
                    myLogWriter.LogWrite("No updates found.", 1);
                    hasMandatoryUpdates = false;
                }
            } catch (Exception ex)
            {
                myLogWriter.LogWrite("Failure running context.GetAppAndOptionalStorePackageUpdatesAsync() with " + ex.Message, 3);
                hasMandatoryUpdates = false;
            }
            myLogWriter.LogWrite("Exiting Update procedure.");

        }

        private async void InstallOnExit()
        {
            await Sleepy();
            if ( installOnExit != false )
            {
                myLogWriter.LogWrite("Pending updates will now be installed.");
                await InstallUpdate(updates);
                myLogWriter.LogWrite("InstallUpdate has finished execution.");
            } else
            {
                myLogWriter.LogWrite("Nothing to install on exit.");
            }
        }


        private async Task InstallUpdate(IReadOnlyList<StorePackageUpdate> storePackageUpdates)
        {
            // Start the silent installation of the packages. Because the packages have already
            // been downloaded in the previous method, the following line of code just installs
            // the downloaded packages.
            IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> installOperation = context.TrySilentDownloadAndInstallStorePackageUpdatesAsync(storePackageUpdates);
            myLogWriter.LogWrite("Status: " + installOperation.Status.ToString());
            updateHandlerForm.ShowDialog();
            myLogWriter.LogWrite("Showing Installation progress");
            
            Progress<StorePackageUpdateStatus> progress = new Progress<StorePackageUpdateStatus>(
            report => updateHandlerForm.prgProgress.Value = ((int)report.PackageDownloadProgress));

            
            installOperation.AsTask(progress).Wait();
            updateHandlerForm.Close();
            myLogWriter.LogWrite("Closing Installation progress");
            StorePackageUpdateResult installResult = await installOperation.AsTask();
            //StorePackageUpdateResult downloadResult =
            //    await context.TrySilentDownloadAndInstallStorePackageUpdatesAsync(storePackageUpdates);

            switch (installResult.OverallState)
            {
                // If the user cancelled the installation or you can't perform the installation  
                // for some other reason, try again later. The RetryInstallLater method is not  
                // implemented in this example, you should implement it as needed for your own app.
                case StorePackageUpdateState.Canceled:
                    myLogWriter.LogWrite("Update installation failed. Process has been canceled.",2);
                    installOnExit = true;
                    return;
                case StorePackageUpdateState.ErrorLowBattery:
                    myLogWriter.LogWrite("Update installation failed. Batterylevel is low.",2);
                    installOnExit = true;
                    return;
                case StorePackageUpdateState.OtherError:
                    myLogWriter.LogWrite("Update installation failed. An unknown error occured.", 3);
                    installOnExit = true;
                    return;
                default:
                    myLogWriter.LogWrite("Update installation was successfull.");
                    break;
            }
        }


    }
}
