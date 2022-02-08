using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Foundation;

using Windows.Services.Store;


namespace wcommsixwrap
{
    /// <summary>
    /// Interaktionslogik für UpdateHandlerWindow.xaml
    /// </summary>
    public partial class UpdateHandlerWindow : Window
    {
        private LogWriter myLogWriter = null;
        private StoreContext context = null;
        private IReadOnlyList<StorePackageUpdate> updates;
        private string updateFailureCaption = "";
        private string updateFailureMessage = "";
        public bool failure = false;

        public UpdateHandlerWindow(StoreContext context, IReadOnlyList<StorePackageUpdate> updates, String heading, String message, String myUpdateFailureCaption, String myUpdateFailureMessage)
        {
        myLogWriter = new LogWriter("UpdateHandlerForm");
            myLogWriter.LogWrite("Window is initializing.");
            this.context = context;
            this.updates = updates;
            this.updateFailureCaption = myUpdateFailureCaption;
            this.updateFailureMessage = myUpdateFailureMessage;
            myLogWriter.LogWrite("Calling InitializeComponent().");
            InitializeComponent();
            this.lblHeading.Content = heading;
            this.lblMessage.Text = message;
            this.Title = heading;
            myLogWriter.LogWrite("Variables are now set.");
        }

         public UpdateHandlerWindow() {
            myLogWriter = new LogWriter("UpdateHandlerForm");
            InitializeComponent();
        }

        private void winUpdateHandler_Activated(object sender, EventArgs e)
        {
            
        }

        private void winUpdateHandler_Loaded(object sender, RoutedEventArgs e)
        {
            myLogWriter.LogWrite("Form has loaded. Will now begin download task.");
            
           DownloadAndInstallAllUpdatesAsync();
           

        }

        public async Task DownloadAndInstallAllUpdatesAsync()
        {
            myLogWriter.LogWrite("Installation will now start.");
            
            this.TaskbarItemInfo = new System.Windows.Shell.TaskbarItemInfo() { ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal };
            try
            {
                bool failure = false;
                IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> downloadOperation = context.TrySilentDownloadAndInstallStorePackageUpdatesAsync(updates);
                myLogWriter.LogWrite("Displaying Progress.");
                // The Progress async method is called one time for each step in the download
                // and installation process for each package in this request.
                downloadOperation.Progress = async (asyncInfo, progress) =>
                {
                    await this.Dispatcher.InvokeAsync(
                    () =>
                    {
                        TaskbarItemInfo.ProgressValue = progress.PackageDownloadProgress;
                        prgProgress.Value = progress.PackageDownloadProgress;
                    });
                };
                
                StorePackageUpdateResult result = await downloadOperation.AsTask();
                prgProgress.IsIndeterminate = true;

                
                switch (result.OverallState)
                {
                    // If the user cancelled the installation or you can't perform the installation  
                    // for some other reason, try again later. The RetryInstallLater method is not  
                    // implemented in this example, you should implement it as needed for your own app.
                    case StorePackageUpdateState.Canceled:
                        myLogWriter.LogWrite("Update installation failed. Process has been canceled.", 2);
                        failure = true;
                        break;
                    case StorePackageUpdateState.ErrorWiFiRequired:
                        myLogWriter.LogWrite("No wifi connection available.", 2);
                        failure = true;
                        break;
                    case StorePackageUpdateState.ErrorWiFiRecommended:
                        myLogWriter.LogWrite("A wifi connection is recommended.", 2);
                        failure = true;
                        break;
                    case StorePackageUpdateState.ErrorLowBattery:
                        myLogWriter.LogWrite("Update installation failed. Batterylevel is low.", 2);
                        failure = true;
                        break;
                    case StorePackageUpdateState.OtherError:
                        myLogWriter.LogWrite("Update installation failed. An unknown error occured.", 3);
                        failure = true;
                        break;
                    default:
                        myLogWriter.LogWrite("Update installation was successfull.");
                        break;
                }
                if (failure)
                    MessageBox.Show(updateFailureMessage, updateFailureCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                    
            }
            catch (Exception ex)
            {
                myLogWriter.LogWrite("Error: " + ex.ToString());
                
            }
            this.Close();
        }
    }
}
