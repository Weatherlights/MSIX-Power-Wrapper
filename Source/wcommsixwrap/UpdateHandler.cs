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
using Windows.Services.Store;
using Windows.UI.Popups;

namespace wcommsixwrap
{

    internal class UpdateHandler
    {
        private CultureInfo ci;
        private StoreContext context = null;
        private IReadOnlyList<StorePackageUpdate> updates;
        public string message;
        public string caption;
        public string messageFailRequired = "We can not launch the application without installing important updates. Please check the Microsoft Store for possible installation failures and retry.";
        public string captionFailRequired = "Updates are missing";
        public bool WaitForUpdateSearchToFinish = true;
        public bool mandatoryInstallationFailure = false;
        public bool hasMandatoryUpdates { get; set; }

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
                        case "RequiredUpdateFailureMessage":
                            string languageFailRequired = reader.GetAttribute("lang");

                            if (languageFailRequired.Equals(langcode))
                            {
                                captionFailRequired = reader.GetAttribute("caption");
                                reader.Read();
                                messageFailRequired = reader.Value;
                            }
                            break;
                        case "WaitForUpdateSearchToFinish":
                            reader.Read();
                            if ( reader.Value.Equals("false"))
                                WaitForUpdateSearchToFinish = false;
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

        private void configureForm()
        {
            updateHandlerForm.lblHeading.Text = this.caption;
            updateHandlerForm.Text = this.message;
            updateHandlerForm.lblHeading.Text = this.caption;

        }


        public async void InstallOnStart() {

            //   Program.CreateDirectoryRecursively(getLink());
            //formOperation.Wait();
            myLogWriter.LogWrite("Checking for updates");
            
            if (context == null)
            {
                context = StoreContext.GetDefault();
            }
            myLogWriter.LogWrite("Loaded store context.");
            
            
            //MessageBox.Show("Searching for Updates", "Searching", MessageBoxButton.OK, MessageBoxImage.Information);
            try
            {
                IAsyncOperation<IReadOnlyList<StorePackageUpdate>> searchOperation = context.GetAppAndOptionalStorePackageUpdatesAsync();
                searchOperation.AsTask().Wait();


                updates = await searchOperation.AsTask();
                
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
                            hasMandatoryUpdates = true;
                            myLogWriter.LogWrite("No mandatory updates found.");
                        }

                    }

                    executeUpdateProcedure = true;
                    if (executeUpdateProcedure)
                    {
                        

                        if (hasMandatoryUpdates == false)
                        {
                            if (!context.CanSilentlyDownloadStorePackageUpdates)
                            {
                                myLogWriter.LogWrite("Can not install updates silently at the moment.");
                                return;
                            }

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
                            }
                          

                        } else
                        {
                            try
                            {
                                myLogWriter.LogWrite("Mandatory updates need to be installed and will now be enforced.");
                                UpdateHandlerWindow updateHandlerWindow = new UpdateHandlerWindow(context, updates, caption, message, captionFailRequired, messageFailRequired);
                                myLogWriter.LogWrite("Window has been initialized.");
                                updateHandlerWindow.ShowDialog();
                                // await InstallUpdate(updates);
                                myLogWriter.LogWrite("InstallUpdate has finished execution.");
                            } catch (Exception ex) {
                                myLogWriter.LogWrite("Failure initializing the mandatory app installation." +
                                    "\n" + ex.Message
                                    +"\n" + ex.ToString(), 3);
                            }
                        }

                    }


                } else
                {
                    myLogWriter.LogWrite("No updates found.", 1);
                    hasMandatoryUpdates = false;
                }
            } catch (Exception ex)
            {
                myLogWriter.LogWrite("Failure running context.GetAppAndOptionalStorePackageUpdatesAsync() with " +ex.ToString(), 3);
                hasMandatoryUpdates = false;
            }
            myLogWriter.LogWrite("Exiting Update procedure.");

        }

        private async void InstallOnExit()
        {
            
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
            this.configureForm();
           
            // Start the silent installation of the packages. Because the packages have already
            // been downloaded in the previous method, the following line of code just installs
            // the downloaded packages.

            IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> installOperation;
            if ( hasMandatoryUpdates )
                installOperation = context.TrySilentDownloadAndInstallStorePackageUpdatesAsync(storePackageUpdates);
            else
                installOperation = context.TrySilentDownloadAndInstallStorePackageUpdatesAsync(storePackageUpdates);
            myLogWriter.LogWrite("Status: " + installOperation.Status.ToString());
            updateHandlerForm.ShowDialog();

            myLogWriter.LogWrite("Showing Update Dialog");
   
           installOperation.AsTask().Wait();
            
            StorePackageUpdateResult installResult = await installOperation.AsTask();
            updateHandlerForm.Close();
            myLogWriter.LogWrite("Closed Update dialog");
            //StorePackageUpdateResult downloadResult =
            //    await context.TrySilentDownloadAndInstallStorePackageUpdatesAsync(storePackageUpdates);
            bool success = true;
            switch (installResult.OverallState)
            {
                // If the user cancelled the installation or you can't perform the installation  
                // for some other reason, try again later. The RetryInstallLater method is not  
                // implemented in this example, you should implement it as needed for your own app.
                case StorePackageUpdateState.Canceled:
                    myLogWriter.LogWrite("Update installation failed. Process has been canceled.",2);
                    success = false;
                    installOnExit = true;
                    return;
                case StorePackageUpdateState.ErrorLowBattery:
                    myLogWriter.LogWrite("Update installation failed. Batterylevel is low.",2);
                    success = false;
                    installOnExit = true;
                    return;
                case StorePackageUpdateState.OtherError:
                    myLogWriter.LogWrite("Update installation failed. An unknown error occured.", 3);
                    success = false;
                    installOnExit = true;
                    return;
                default:
                    myLogWriter.LogWrite("Update installation was successfull.");
                    break;
            }
            if ( !success && hasMandatoryUpdates)
            {
                mandatoryInstallationFailure = true;
            }

           
        }


    }
}
