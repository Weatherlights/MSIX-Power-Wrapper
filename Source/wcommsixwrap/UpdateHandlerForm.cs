using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Services.Store;

namespace wcommsixwrap
{

    public partial class UpdateHandlerForm : Form
    {
        public IReadOnlyList<StorePackageUpdate> StorePackages = null;
        public UpdateHandlerForm()
        {
            InitializeComponent();
        }



        private void UpdateHandlerForm_Load(object sender, EventArgs e)
        {
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Size.Width,
                                      workingArea.Bottom - Size.Height);
            
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

    }
}
