using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Foundation;
using Windows.UI.Core;
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
        
        public UpdateHandlerWindow(StoreContext context, IReadOnlyList<StorePackageUpdate> updates, String heading, String message)
        {
            myLogWriter = new LogWriter("UpdateHandlerForm");
            myLogWriter.LogWrite("Window is initializing.");
            this.context = context;
            this.updates = updates;
            myLogWriter.LogWrite("Calling InitializeComponent().");
            InitializeComponent();
            this.lblHeading.Content = heading;
            this.lblMessage.Content = message;
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
            try
            {
                IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> downloadOperation = context.TrySilentDownloadAndInstallStorePackageUpdatesAsync(updates);
                myLogWriter.LogWrite("Displaying Progress.");
                // The Progress async method is called one time for each step in the download
                // and installation process for each package in this request.
                downloadOperation.Progress = async (asyncInfo, progress) =>
                {
                    await this.Dispatcher.InvokeAsync(
                    () =>
                    {
                        prgProgress.Value = progress.PackageDownloadProgress;
                    });
                };

                StorePackageUpdateResult result = await downloadOperation.AsTask();
            }
            catch (Exception ex)
            {
                myLogWriter.LogWrite("Error: " + ex.ToString());
                
            }
            
        }
    }
}
